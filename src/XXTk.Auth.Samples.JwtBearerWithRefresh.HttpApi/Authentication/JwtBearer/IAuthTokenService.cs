using System.Threading.Tasks;
using XXTk.Auth.Samples.JwtBearerWithRefresh.HttpApi.Dtos;

namespace XXTk.Auth.Samples.JwtBearerWithRefresh.HttpApi.Authentication.JwtBearer
{
    public interface IAuthTokenService
    {
        Task<AuthTokenDto> CreateAuthTokenAsync(UserDto user);

        Task<AuthTokenDto> RefreshAuthTokenAsync(AuthTokenDto token);
    }
}
