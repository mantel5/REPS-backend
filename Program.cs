var builder = WebApplication.CreateBuilder(args);

// 1. SERVICIOS
// Agregamos los controladores (para que funcionen tus endpoints futuros)
builder.Services.AddControllers();

// Agregamos Swagger (La interfaz visual)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- ZONA DE BASE DE DATOS (COMENTADA POR AHORA) ---
// Cuando crees el AppDbContext, descomentas esto:
// builder.Services.AddDbContext<AppDbContext>(options => ... );
// ---------------------------------------------------

var app = builder.Build();

// 2. CONFIGURACIÓN
// Si estamos desarrollando, mostramos Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Activar carpeta de imágenes (wwwroot) por si metes los avatares luego
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthorization();

// Mapear los controladores
app.MapControllers();

// ¡ARRANCAR! 🚀
app.Run();