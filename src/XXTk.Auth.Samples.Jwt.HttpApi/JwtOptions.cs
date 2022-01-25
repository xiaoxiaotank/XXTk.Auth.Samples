using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace XXTk.Auth.Samples.JwtBearer.HttpApi
{
    public class JwtOptions
    {
        public const string Name = "Jwt";
        public const string DefaultAlgorithms = SecurityAlgorithms.HmacSha256Signature;
        public readonly static Encoding DefaultEncoding = Encoding.UTF8;

        public string Audience { get; set; }

        public string Issuer { get; set; }

        public string SymmetricSecurityKeyString { get; set; }

        public string Algorithms { get; set; } = DefaultAlgorithms;

        public Encoding Encoding { get; set; } = DefaultEncoding;

        public SymmetricSecurityKey SymmetricSecurityKey => new(Encoding.GetBytes(SymmetricSecurityKeyString));
    }
}
