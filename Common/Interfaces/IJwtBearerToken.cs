using Common.Models.Token;

namespace Common.Interfaces
{
    public interface IJwtBearerToken
    {
        public JwtToken GenerateToken();
    }
}
