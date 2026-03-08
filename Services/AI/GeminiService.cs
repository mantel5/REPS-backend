using REPS_backend.Data;
using REPS_backend.DTOs.Rutinas;
using REPS_backend.Models;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace REPS_backend.Services.AI
{
    public class GeminiService : IAIService
    {
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent";
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;

        public GeminiService(IConfiguration configuration, ApplicationDbContext context)
        {
            _apiKey = configuration["Gemini:ApiKey"] ?? string.Empty;
            _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
            _context = context;
        }

        public async Task<string> AnalyzeWorkoutAsync(Sesion sesion)
        {
            var prompt = $"Actúa como un entrenador experto. Analiza la siguiente sesión y dame un resumen y consejos en ESPAÑOL (máximo 3 frases). " +
                         $"Highlight any achievements or suggest improvements based on volume/intensity if possible.\n\n" +
                         $"Workout: {sesion.NombreRutinaSnapshot}\n" +
                         $"Duration: {sesion.DuracionRealMinutos} mins\n" +
                         $"Exercises:\n";

            if (sesion.SeriesRealizadas != null)
            {
                foreach (var serie in sesion.SeriesRealizadas)
                {
                    prompt += $"- Exercise ID {serie.EjercicioId}: {serie.RepsRealizadas} reps @ {serie.PesoUsado}kg (Success: {serie.Completada})\n";
                }
            }

            return await CallGeminiApiAsync(prompt);
        }

        public async Task<RutinaDetalleDto> GenerateRoutineAsync(RutinaIARequestDto dto)
        {
            var ejerciciosDb = await _context.Ejercicios.Select(e => new { e.Id, e.Nombre }).ToListAsync();
            var ejerciciosLista = string.Join(", ", ejerciciosDb.Select(e => $"{e.Id}:{e.Nombre}"));

            var prompt = $@"
Act as a professional personal trainer. Create a workout routine based on the following criteria:
Goal: {dto.Goal}
Level: {dto.Level}
Duration: {dto.Duration} 
Focus Areas: {string.Join(", ", dto.Muscles ?? new List<string>())}
Equipment: {string.Join(", ", dto.Equipment ?? new List<string>())}

IMPORTANT:
1. Use ONLY exercises from this list if possible (ID:Name): [{ejerciciosLista}]. If you MUST use others, use ID 0.
2. Return a JSON object strictly following this structure:
{{
  ""Nombre"": ""Routine Name"",
  ""Nivel"": ""Principiante o Intermedio o Avanzado"",
  ""DuracionMinutos"": 60,
  ""Ejercicios"": [
    {{
      ""EjercicioId"": 123, 
      ""NombreEjercicio"": ""Exercise Name"",
      ""Series"": 3,
      ""Repeticiones"": ""12""
    }}
  ]
}}
Do NOT wrap the JSON in markdown code blocks. Just return the raw JSON string.
";

            var jsonResponse = await CallGeminiApiAsync(prompt);
            jsonResponse = jsonResponse.Replace("```json", "").Replace("```", "").Trim();

            if (string.IsNullOrEmpty(jsonResponse)) throw new Exception("AI returned empty response.");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            try
            {
                Console.WriteLine($"[Gemini JSON Payload]:\n{jsonResponse}");
                var rutina = JsonSerializer.Deserialize<RutinaDetalleDto>(jsonResponse, options);
                return rutina ?? new RutinaDetalleDto();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[Gemini Parsing ERROR]: {ex.Message}\n[Payload]: {jsonResponse}");
                throw new Exception($"Failed to parse AI response. Error: {ex.Message}");
            }
        }

        private async Task<string> CallGeminiApiAsync(string textPrompt)
        {
            if (string.IsNullOrEmpty(_apiKey)) return "Error: Gemini API key is missing.";

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = textPrompt } } }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);

            int maxRetries = 5;
            int delay = 5000;

            for (int i = 0; i <= maxRetries; i++)
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}?key={_apiKey}", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(responseString);

                    if (doc.RootElement.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
                    {
                        var firstCandidate = candidates[0];
                        if (firstCandidate.TryGetProperty("content", out var contentElem) &&
                            contentElem.TryGetProperty("parts", out var parts) &&
                            parts.GetArrayLength() > 0)
                        {
                            return parts[0].GetProperty("text").GetString() ?? "";
                        }
                    }
                    return "";
                }

                if ((int)response.StatusCode == 429 || (int)response.StatusCode == 503)
                {
                    if (i == maxRetries)
                    {
                        var errorMsg = await response.Content.ReadAsStringAsync();
                        throw new Exception($"Gemini API Error (Rate Limit/Unavailable after retries): {errorMsg}");
                    }

                    await Task.Delay(delay);
                    delay *= 2;
                    continue;
                }

                var otherErrorMsg = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API Error ({response.StatusCode}): {otherErrorMsg}");
            }

            return "";
        }
    }
}
