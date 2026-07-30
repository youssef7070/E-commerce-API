using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts
{
    public interface ICasheService
    {

        Task<string?> GetAsync(string Cashekey, CancellationToken ct = default);

        Task SetAsync(string Cashekey,object CasheValue, TimeSpan TimeToLive, CancellationToken ct = default);


    }
}
