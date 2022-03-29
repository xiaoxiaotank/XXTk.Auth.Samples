using Microsoft.AspNetCore.Authorization;

namespace XXTk.Auth.Samples.PolicyBased.HttpApi.Authorizations
{
    public class MinimumAgeAuthorizeAttribute : AuthorizeAttribute
    {
        public const string PolicyPrefix = "MinimumAge";

        public MinimumAgeAuthorizeAttribute(int minimumAge) =>
            MinimumAge = minimumAge;

        public int MinimumAge
        {
            get
            {
                if (int.TryParse(Policy[PolicyPrefix.Length..], out var age))
                {
                    return age;
                }

                return default;
            }
            set
            {
                // 生成动态的Policy
                Policy = $"{PolicyPrefix}{value}";
            }
        }
    }
}
