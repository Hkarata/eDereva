using System.Text;
using eDereva.Api.Exceptions;
using eDereva.Api.Extensions;
using eDereva.Core.Interfaces;
using eDereva.Core.Services;
using eDereva.Core.ValueObjects;
using eDereva.Infrastructure.Data;
using eDereva.Infrastructure.Repositories;
using eDereva.Infrastructure.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!)),
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = configuration["Jwt:Issuer"]!,
            ValidAudience = configuration["Jwt:Audience"]!
        };
    });

builder.Services.AddAuthorization();

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.Configure<SmsConfiguration>(configuration.GetSection("SmsService"));

builder.Services.AddHttpClient();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Services.AddHybridCache(options =>
{
    options.MaximumPayloadBytes = 1024 * 1024;
    options.MaximumKeyLength = 1024;
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(5),
        LocalCacheExpiration = TimeSpan.FromMinutes(5)
    };
});
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<INIDAService, NIDAService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<ISmsService, SmsService>();

builder.Services.AddFastEndpoints()
    .AddResponseCaching();

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});

builder.Services.AddExceptionHandler<ProblemExceptionHandler>();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddDocumentTransformer<ApiKeySecuritySchemeTransformer>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//    app.MapScalarApiReference(options =>
//    {
//        options.Title = "eDereva API";
//        options.Theme = ScalarTheme.Saturn;
//        options.WithPreferredScheme("Bearer");
//        options.WithApiKeyAuthentication(keyOptions =>
//        {
//            keyOptions.Token = "Token";
//        });
//    });
//}

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "eDereva API";
    options.Theme = ScalarTheme.Saturn;
    options.WithPreferredScheme("Bearer");
    options.WithApiKeyAuthentication(keyOptions =>
    {
        keyOptions.Token = "Token";
    });
});

app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});


app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseMiddleware<JwtRefreshMiddleware>();
app.UseMiddleware<ApiKeyAuthenticationMiddleware>();
app.UseAuthorization();

app.UseResponseCaching()
    .UseFastEndpoints(options =>
    {
        options.Versioning.Prefix = "v";
        options.Versioning.DefaultVersion = 1;
        options.Versioning.PrependToRoute = true;
    });

app.Run();