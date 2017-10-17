using System.Collections.Generic;
using System.Threading.Tasks;

namespace Compass.Domain.DataStore.Couchbase
{
    public interface ICouchbaseClient
    {
        Task<IEnumerable<T>> QueryAsync<T>(string query);
    }
}
