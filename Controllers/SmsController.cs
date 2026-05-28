using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SendSmsApi.Models;
using SendSmsApi.Services;

namespace SendSmsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SmsController : ControllerBase
{
    private readonly ISmsService _smsService;

    public SmsController(ISmsService smsService)
    {
        _smsService = smsService;
    }

    /// <summary>Šalje SMS poruku preko SDP ParlayX servisa</summary>
    [HttpPost("send")]
    [ProducesResponseType(typeof(SendSmsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendSms(
        [FromBody] SendSmsRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _smsService.SendSmsAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>Slanje SMS Logo — nije implementirano</summary>
    [HttpPost("send-logo")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult SendSmsLogo() =>
        StatusCode(StatusCodes.Status501NotImplemented);

    /// <summary>Slanje SMS Ringtone — nije implementirano</summary>
    [HttpPost("send-ringtone")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult SendSmsRingtone() =>
        StatusCode(StatusCodes.Status501NotImplemented);

    /// <summary>Dohvatanje statusa dostave — nije implementirano</summary>
    [HttpGet("delivery-status/{requestIdentifier}")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult GetDeliveryStatus(string requestIdentifier) =>
        StatusCode(StatusCodes.Status501NotImplemented);
}