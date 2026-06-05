/// <summary>
/// AWS Lambda Entry Point.
/// Ovaj fajl je potreban da bi ASP.NET Core aplikacija radila kao Lambda funkcija
/// koristeći Amazon.Lambda.AspNetCoreServer.Hosting paket.
///
/// Program.cs ostaje nepromenjen — Lambda samo "wrappuje" isti WebApplication.
/// </summary>
public class LambdaEntryPoint : Amazon.Lambda.AspNetCoreServer.ApplicationLoadBalancerFunction
{
    // Prazna klasa — ApplicationLoadBalancerFunction automatski pronalazi
    // Program.cs (minimal API bootstrap) i pokrce ga kao Lambda handler.
    //
    // Ako koristis API Gateway (REST API), promeni baznu klasu u:
    //   Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    //
    // Ako koristis API Gateway HTTP API (v2 payload), koristi:
    //   Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction
}
