namespace RefreshJwtToken.Models.Custom
{
    public class RefreshTokenRequest
    {
        public string TokenExp { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
