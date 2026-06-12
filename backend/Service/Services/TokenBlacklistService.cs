using Microsoft.Extensions.Caching.Memory;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class TokenBlacklistService : ITokenBlacklist
    {
        private readonly IMemoryCache _cache;

        public TokenBlacklistService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void RevokeToken(string token, DateTime expiry)
        {
            _cache.Set(token, true, expiry - DateTime.UtcNow);
        }

        public bool IsRevoked(string token) =>
            _cache.TryGetValue(token, out _);
    }
}

