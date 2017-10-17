using System;
using System.Collections.Generic;
using Compass.Shared;
using Couchbase.Core;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Linq;

namespace Compass.Domain.DataStore.Couchbase
{
    public class CouchbaseFactory : ICouchbaseFactory
    {
        private readonly ICompassEnvironment _compassEnvironment;
        private readonly ICluster _cluster;

        public CouchbaseFactory(ICompassEnvironment compassEnvironment)
        {
            _compassEnvironment = compassEnvironment;

            _cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> { _compassEnvironment.GetCouchbaseUri() }
            });

        }

        public IBucket GetBucket()
        {
            return _cluster.OpenBucket(_compassEnvironment.GetCouchbaseBucketName());
        }

        public IBucketContext GetBucketContext()
        {
            return new BucketContext(GetBucket());
        }
    }
}
