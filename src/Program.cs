using System.Text.Json.Serialization;
using FlowCenter.Api.Middlewares;
using FlowCenter.Application.Interfaces;
using FlowCenter.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers & JSON ──────────────────────────────────────────────────────
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        // Serializa enums (Status, Prioridade) como texto, conforme os contratos do SDD.
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// ── Persistência (RNF03, RNF04) ──────────────────────────────────────────────
builder.Services.AddDbContext<FlowCenterDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITarefaRepository, TarefaRepository>();

// ── Swagger / OpenAPI (RF11, RNF07) ──────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FlowCenter API",
        Version = "v1",
        Description = "API REST para gerenciamento centralizado de tarefas."
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// ── Pipeline HTTP ────────────────────────────────────────────────────────────
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "FlowCenter API v1");
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>Classe parcial exposta para permitir testes de integração com WebApplicationFactory.</summary>
public partial class Program { }
