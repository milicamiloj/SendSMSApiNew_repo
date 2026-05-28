using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SendSmsApi.Models;

public class SendSmsRequest
{
    [Required]
    [Description("Broj telefona primaoca u formatu 381641234567")]
    public required string SessionMSISDN { get; init; }

    [Required]
    [Description("Tekst SMS poruke koja se šalje")]
    public required string Message { get; init; }

    [Required]
    [Description("Kratki broj pošiljaoca (npr. 12345)")]
    public required string ShortNumber { get; init; }

    //[Required]
    //[Description("Korisničko ime za autentifikaciju na SDP")]
    //public required string UserName { get; init; }

    //[Required]
    //[Description("Lozinka za autentifikaciju na SDP")]
    //public required string Password { get; init; }
}