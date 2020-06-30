using Consumer.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Consumer.Contracts.Interfaces
{
    public interface IDataClient
    {
        Task<IEnumerable<Owner>> GetOwners();
    }
}
