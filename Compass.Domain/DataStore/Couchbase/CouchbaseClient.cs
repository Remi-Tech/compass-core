using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Compass.Shared;
using Newtonsoft.Json;

namespace Compass.Domain.DataStore.Couchbase
{
    public class CouchbaseClient : ICouchbaseClient
    {
        private readonly ICompassEnvironment _compassEnvironment;

        public CouchbaseClient(ICompassEnvironment compassEnvironment)
        {
            _compassEnvironment = compassEnvironment;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query)
        {
            var queryContent = new StringContent(JsonConvert.SerializeObject(new { Statement = query }), Encoding.UTF8, "application/json");
            var response = await GetHttpClient().PostAsync(GetCouchbaseUri(), queryContent);
            var resultString = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(resultString)) return new List<T>();

            var couchbaseResult = JsonConvert.DeserializeObject<CouchbaseQueryResult<T>>(resultString);
            ValidateResult(couchbaseResult);
            return couchbaseResult.Results;
        }

        private static void ValidateResult<T>(CouchbaseQueryResult<T> result)
        {
            if (string.CompareOrdinal(result.Status, "success") == 0) return;
            throw new Exception(string.Join(',', result.Errors.Select(e => e.Msg)));
        }
        
        private static HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        private Uri GetCouchbaseUri()
        {
            var uriBuilder =
                new UriBuilder(_compassEnvironment.GetCouchbaseUri())
                {
                    Port = _compassEnvironment.GetCouchbaseQueryPort(),
                    Path = "query/service"
                };
            return uriBuilder.Uri;
        }
    }

}
