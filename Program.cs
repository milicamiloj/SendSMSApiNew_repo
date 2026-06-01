using Microsoft.AspNetCore.Authentication;
using SendSmsApi.Infrastructure;
using SendSmsApi.Services;

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

// ✅ NOVO: Registruj Bearer Token autentifikaciju
builder.Services.AddAuthentication("BearerToken")
    .AddScheme<AuthenticationSchemeOptions, BearerTokenAuthHandler>("BearerToken", null);

builder.Services.AddAuthorization();

var app = builder.Build();

app.MapGet("/", () => "Hello, all!!!");

app.MapOpenApi("/openapi/v1.json");

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "SendSms API v1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

// ✅ NOVO: mora biti pre MapControllers()
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();