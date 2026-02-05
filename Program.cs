using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using REPS_backend.Data; 
using REPS_backend.Repositories;
using REPS_backend.Services;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// 1. BASE DE DATOS
// -----------------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, 
        new MySqlServerVersion(new Version(8, 0, 45)), 
        mySqlOptions => mySqlOptions.EnableRetryOnFailure())
);

// -----------------------------------------------------------------------------
// 2. INYECCIÓN DE DEPENDENCIAS (Repositorios y Servicios)
// -----------------------------------------------------------------------------

// Usuarios y Auth
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Ejercicios
builder.Services.AddScoped<IEjercicioRepository, EjercicioRepository>();
builder.Services.AddScoped<IEjercicioService, EjercicioService>();

// Rutinas
builder.Services.AddScoped<IRutinaRepository, RutinaRepository>();
builder.Services.AddScoped<IRutinaService, RutinaService>();

// Rutina-Ejercicios (FALTABAN ESTOS, necesarios para RutinaEjerciciosController)
builder.Services.AddScoped<IRutinaEjercicioRepository, RutinaEjercicioRepository>();
builder.Services.AddScoped<IRutinaEjercicioService, RutinaEjercicioService>();



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Esto permite que los Enums se vean como texto ("Pecho") en lugar de números (0)
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; 
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        
        ValidIssuer = builder.Configuration["Jwt:Issuer"]!,
        ValidAudience = builder.Configuration["Jwt:Audience"]!,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)) 
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "REPS API", Version = "v1" });

    // Configuración del candadito para meter el Token en Swagger
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http, 
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    };

    c.AddSecurityRequirement(securityRequirement);
});

// CORS (Para que el Frontend pueda llamar al Backend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();


// 1. Inicializador de Base de Datos (EL SEEDER AUTOMÁTICO) 🆕
// Esto se ejecuta cada vez que arranca la app para comprobar si hay ejercicios
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Llamamos a la clase estática DbInitializer
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al sembrar la base de datos.");
    }
}

// 2. Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 3. Seguridad y Controladores
app.UseCors("PermitirTodo"); 

app.UseAuthentication(); 
app.UseAuthorization();  

app.MapControllers();

app.Run();