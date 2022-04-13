namespace XXTk.Auth.Samples.JwtBearerWithRefresh.HttpApi
{
    public class JwtOptions
    {
        public const string Name = "Jwt";
        public readonly static double DefaultAccessTokenExpiresMinutes = 30d;
        public readonly static double DefaultRefreshTokenExpiresDays = 7d;

        public string Audience { get; set; }

        public string Issuer { get; set; }

        public double AccessTokenExpiresMinutes { get; set; } = DefaultAccessTokenExpiresMinutes;

        public double RefreshTokenExpiresDays { get; set; } = DefaultRefreshTokenExpiresDays;
    }
}
