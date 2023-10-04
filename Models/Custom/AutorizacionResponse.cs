namespace RefreshJwtToken.Models.Custom
{
    public class AutorizacionResponse
    {
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public bool Resultado { get; set; }
        public string Mensaje { get; set; } = null!;
    }
}
