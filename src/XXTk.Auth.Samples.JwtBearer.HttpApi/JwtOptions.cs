using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Cryptography;
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

        public string Algorithms { get; set; } = DefaultAlgorithms;

        public Encoding Encoding { get; set; } = DefaultEncoding;

        public string SymmetricSecurityKeyString { get; set; }

        public SymmetricSecurityKey SymmetricSecurityKey => new(Encoding.GetBytes(SymmetricSecurityKeyString));

        public string RsaSecurityPrivateKeyString { get; set; }

        public string RsaSecurityPublicKeyString { get; set; }

        public RsaSecurityKey RsaSecurityPrivateKey => RsaSecurityPrivateKeyString is null ? null : new(JsonConvert.DeserializeObject<RSAParameters>(RsaSecurityPrivateKeyString));

        public RsaSecurityKey RsaSecurityPublicKey => RsaSecurityPublicKeyString is null ? null : new(JsonConvert.DeserializeObject<RSAParameters>(RsaSecurityPublicKeyString));
    }
}
