using E_Commerce.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Identity.Services
{
    public class TokenService : ITokenService
    {
      
        public string CreateToken(string userId, string email, string userName, IEnumerable<string> roles)
        {
            // TODO: Implement token creation logic
            throw new NotImplementedException();
        }
    }
}
