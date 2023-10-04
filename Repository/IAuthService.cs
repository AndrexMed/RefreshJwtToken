using RefreshJwtToken.Models.Custom;

namespace RefreshJwtToken.Repository
{
    public interface IAuthService
    {
        Task<AutorizacionResponse> ReturnToken(AutorizacionRequest autorizacionRequest);
        Task<AutorizacionResponse> ReturnRefreshToken(RefreshTokenRequest refreshTokenRequest);
    }
}
