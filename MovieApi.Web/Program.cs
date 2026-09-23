using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MovieApi.Core;
using MovieApi.Domain.Entities;
using MovieApi.Infrastructure.Minio;
using MovieApi.Infrastructure.Postgres;
using MovieApi.Infrastructure.Postgres.Persistence;
using MovieApi.Web;
using MovieApi.Web.Authorization;
using MovieApi.Web.Configuration;
using MovieApi.Web.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddMovieApiLogging(); //подключаем логгирование 

builder.Services.AddMinioInfrastructure(builder.Configuration);
builder.Services.AddJwtOptions();
builder.Services.AddJwtBearer(builder.Configuration);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        AuthorizationPolicies.ManageCatalog,
        policy => policy.RequireRole(UserRole.Admin.ToString()));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Вставь только JWT без префикса Bearer."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }
        ] = Array.Empty<string>()
    });
});
builder.Services.AddCore(); // подключаем сервисы из Core
builder.Services.AddPostgresInfrastructure(builder.Configuration.GetConnectionString("Postgres")!); // подкючаем БД
builder.Services.AddHandlers(); // подключаем хэндлеры
builder.Services.AddCurrentUser();

var app = builder.Build();

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("TraceId", httpContext.TraceIdentifier);
    };
});

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<MovieDbContext>();

    await dbContext.Database.MigrateAsync();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>(); // регистрируем обработку ошибок
app.UseAuthentication();
app.UseAuthorization();

app.AddMapEndpoints(); // подключение endpoints

app.Run();

public partial class Program
{

}