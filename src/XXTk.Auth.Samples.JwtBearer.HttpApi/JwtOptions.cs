using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace XXTk.Auth.Samples.JwtBearer.HttpApi
{
    public class JwtOptions
    {
        public const string Name = "Jwt";
        public readonly static Encoding DefaultEncoding = Encoding.UTF8;
        public readonly static double DefaultExpiresMinutes = 30d;

        public string Audience { get; set; }

        public string Issuer { get; set; }

        public double ExpiresMinutes { get; set; } = DefaultExpiresMinutes;

        public Encoding Encoding { get; set; } = DefaultEncoding;

        public string SymmetricSecurityKeyString { get; set; }

        public SymmetricSecurityKey SymmetricSecurityKey => new(Encoding.GetBytes(SymmetricSecurityKeyString));
    }
}
