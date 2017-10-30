using Couchbase.Core;
using Couchbase.Linq;

namespace Compass.Domain.DataStore.Couchbase
{
    public interface ICouchbaseFactory
    {
        IBucket GetBucket();
    }
}
