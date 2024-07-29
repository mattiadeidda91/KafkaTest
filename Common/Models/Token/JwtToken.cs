namespace Common.Models.Token
{
    public class JwtToken
    {
        public string? Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
