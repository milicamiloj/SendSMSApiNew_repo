using Microsoft.Extensions.Options;
using SendSmsApi.Infrastructure;
using SendSmsApi.Models;

namespace SendSmsApi.Services;

public class SmsService : ISmsService
{
    private readonly SdpSoapClient _sdpClient;
    public readonly SdpSettings _sdpSettings;
    private readonly ILogger<SmsService> _logger;

    public SmsService(SdpSoapClient sdpClient, ILogger<SmsService> logger, IOptions<SdpSettings> sdpOptions)
    {
        _sdpClient = sdpClient;
        _logger = logger;
        _sdpSettings = sdpOptions.Value;
    }

    public async Task<SendSmsResponse> SendSmsAsync(
        SendSmsRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string[] addresses = [$"tel:{request.SessionMSISDN}"];

            string requestId = await _sdpClient.SendSmsAsync(
                addresses,
                request.ShortNumber,
                request.Message,
                _sdpSettings.Username,
                _sdpSettings.Password,
                cancellationToken);

            _logger.LogInformation(
                "SMS uspješno poslat na {MSISDN}, requestId: {RequestId}",
                request.SessionMSISDN, requestId);

            return new SendSmsResponse { Success = true, RequestId = requestId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri slanju SMS na {MSISDN}", request.SessionMSISDN);
            return new SendSmsResponse { Success = false, ErrorMessage = ex.Message };
        }
    }
}