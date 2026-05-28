using SendSmsApi.Infrastructure;
using SendSmsApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// OpenAPI/Swagger
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info.Title = "SendSms API";
        document.Info.Description = "REST API za slanje SMS poruka preko SDP ParlayX servisa";
        document.Info.Version = "v1";
        return Task.CompletedTask;
    });
});

// SdpSettings options
builder.Services.Configure<SdpSettings>(
    builder.Configuration.GetSection("SdpSettings"));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration
            .GetSection("JwtSettings")
            .Get<JwtSettings>() ?? throw new InvalidOperationException("JwtSettings nisu konfigurisana u appsettings.json");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Typed HttpClient za SDP SOAP endpoint
builder.Services.AddHttpClient<SdpSoapClient>(client =>
{
    var sdpUrl = builder.Configuration["SdpSettings:Url"]
        ?? throw new InvalidOperationException("SdpSettings:Url nije konfigurisan u appsettings.json");

    client.BaseAddress = new Uri(sdpUrl);
    client.Timeout = TimeSpan.FromSeconds(
        builder.Configuration.GetValue<int>("SdpSettings:TimeoutSeconds", 30));
});

// Servis
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

app.MapGet("/", () => "Hello, all!!!");

// Swagger UI dostupan samo u Development modu
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}
// OpenAPI spec dostupan uvek (potrebno za M365 Copilot agent)
app.MapOpenApi("/openapi/v1.json");

// Swagger UI na /swagger
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "SendSms API v1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();