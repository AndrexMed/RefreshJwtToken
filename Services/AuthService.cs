using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RefreshJwtToken.Models;
using RefreshJwtToken.Models.Custom;
using RefreshJwtToken.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RefreshJwtToken.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthJwtContext _jwtContext;
        private readonly IConfiguration _configuration;

        public AuthService(AuthJwtContext jwtContext, IConfiguration configuration)
        {
            _jwtContext = jwtContext;
            _configuration = configuration;
        }

        public async Task<AutorizacionResponse> ReturnToken(AutorizacionRequest auth)
        {
            var userToFound = _jwtContext.Users.FirstOrDefault(p => p.UserName == auth.UserName &&
                                                                    p.Password == auth.Password);
            if (userToFound == null)
            {
                return await Task.FromResult<AutorizacionResponse>(null);
            }

            string tokenCreated = GenerateToken(userToFound.IdUser.ToString());

            string refreshTokenCreated = GenerateRefreshToken();

            //return new AutorizacionResponse() { Token = tokenCreated, Resultado = true, Mensaje = "Ok"};
            return await SaveRefreshTokenHistory(userToFound.IdUser, tokenCreated, refreshTokenCreated);
        }

        private string GenerateToken(string idUsuario)
        {
            var key = _configuration.GetValue<string>("JwtSettings:Key");
            var keyBytes = Encoding.ASCII.GetBytes(key);

            var credentialsToken = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature);

            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, idUsuario));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = credentialsToken
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            string tokenCreated = tokenHandler.WriteToken(tokenConfig);

            return tokenCreated;
        }

        public async Task<AutorizacionResponse> ReturnRefreshToken(RefreshTokenRequest refreshTokenRequest,
                                                                    int idUser)
        {
            var refreshTokenFounded = await _jwtContext
                .HistorialRefreshTokens
                .FirstOrDefaultAsync(p => p.Token == refreshTokenRequest.TokenExp &&
                                          p.RefreshToken == refreshTokenRequest.RefreshToken &&
                                          p.IdUser == idUser);

            if (refreshTokenFounded == null)
            {
                return new AutorizacionResponse { Resultado = false, Mensaje = "Refresh Token Not Found" };
            }

            var refreshTokenCrated = GenerateRefreshToken();
            var tokenCreated = GenerateToken(idUser.ToString());

            return await SaveRefreshTokenHistory(idUser, tokenCreated, refreshTokenCrated);
        }

        private string GenerateRefreshToken()
        {
            var byteArray = new byte[64];
            var refreshToken = "";

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(byteArray);
                refreshToken = Convert.ToBase64String(byteArray);
            }
            return refreshToken;
        }

        private async Task<AutorizacionResponse> SaveRefreshTokenHistory(int idUser, string token, string refreshToken)
        {
            var newData = new HistorialRefreshToken
            {
                IdUser = idUser,
                Token = token,
                RefreshToken = refreshToken,
                DateCreation = DateTime.UtcNow,
                DateExpiration = DateTime.UtcNow.AddMinutes(2),
            };

            await _jwtContext.HistorialRefreshTokens.AddAsync(newData);
            await _jwtContext.SaveChangesAsync();

            return new AutorizacionResponse { Token = token, RefreshToken = refreshToken, Mensaje = "Ok" };
        }
    }
}
