using System.Collections.Generic;

namespace Compass.Domain.DataStore.Couchbase
{
    public class CouchbaseQueryResult<T>
    {
        public string RequestId { get; set; }
        public IEnumerable<T> Results { get; set; }
        public IEnumerable<CouchbaseError> Errors { get; set; }
        public string Status { get; set; }
    }
}
