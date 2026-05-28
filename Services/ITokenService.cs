namespace SendSmsApi.Services;

public interface ITokenService
{
    string GenerateToken(string username);
}
