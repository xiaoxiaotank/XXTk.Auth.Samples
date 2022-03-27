using Microsoft.AspNetCore.Authorization;

namespace XXTk.Auth.Samples.PolicyBased.HttpApi.Authorizations
{
    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
        public MinimumAgeRequirement(int minimumAge) =>
           MinimumAge = minimumAge;

        public int MinimumAge { get; }
    }
}
