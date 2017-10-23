using System.Collections.Generic;

namespace Compass.Domain.DataStore.Couchbase
{
    internal class CouchbaseClientModel
    {
        internal string Statement { get; set; }
        internal Dictionary<string, string> Args { get; set; } = new Dictionary<string, string>();
    }
}
