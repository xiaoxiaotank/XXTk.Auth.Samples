using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace XXTk.Auth.Samples.Cookies.Web
{
    public class DistributedCacheTicketStore : ITicketStore
    {
        private const string KeyPrefix = "auth-session-store:";
        private readonly IDistributedCache _cache;
        private readonly TimeSpan _defaultExpireTimeSpan;
        private readonly TicketSerializer _serializer;

        public DistributedCacheTicketStore(IDistributedCache cache, TimeSpan defaultExpireTimeSpan, TicketSerializer serializer = null)
        {
            _cache = cache;
            _defaultExpireTimeSpan = defaultExpireTimeSpan;
            _serializer = serializer ?? TicketSerializer.Default;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var guid = Guid.NewGuid();
            var key = KeyPrefix + guid.ToString("N");
            await RenewAsync(key, ticket);
            return key;
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var options = new DistributedCacheEntryOptions();
            var expiresUtc = ticket.Properties.ExpiresUtc;
            if (expiresUtc.HasValue)
            {
                options.SetAbsoluteExpiration(expiresUtc.Value);
            }
            else
            {
                options.SetSlidingExpiration(_defaultExpireTimeSpan);
            }

            await _cache.SetAsync(key, _serializer.Serialize(ticket), options);
        }

        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var ticketByte = await _cache.GetAsync(key);
            if (ticketByte is null)
            {
                return null;
            }

            return _serializer.Deserialize(ticketByte);
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
