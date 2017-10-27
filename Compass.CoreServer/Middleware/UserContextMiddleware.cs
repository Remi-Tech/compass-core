using System.IO;
using System.Threading.Tasks;
using Compass.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;

namespace Compass.CoreServer.Middleware
{
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;

        public UserContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //// Enable request rewind so we can read the body
            //// multiple times.
            //context.Request.EnableRewind();

            //var jsonReader = new JsonTextReader(new StreamReader(context.Request.Body));
            //var compassRequest = new JsonSerializer().Deserialize<CompassEvent>(jsonReader);

            //// TODO: Do something with this wonderful information...

            //// Rewind the body since we've read it once 
            //// already. This will allow the rest of the
            //// application to use it, for example, to
            //// read models sent in the body as json.
            //context.Request.Body.Position = 0;

            await _next(context);
        }
    }
}
