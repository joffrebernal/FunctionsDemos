using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;

namespace TaxAndDiscountApi
{
    public static class Calculate
    {
        [FunctionName("Calculate")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string name = null;
            try
            {
                // parse query parameter
                //string name = req.GetQueryNameValuePairs()
                //    .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                //    .Value;

                // Get request body
                var data = await req.Content.ReadAsAsync<Item>();

                log.Info(data.ToString());

                DocumentDbRepo<Item>.Initialize();

                await DocumentDbRepo<Item>.CreateItemAsync(data);

                // Set name to query string or body data
                name = data.Name;
            }
            catch (Exception ex)
            {
                log.Info("Something failed: " + ex.Message + " " + ex.StackTrace);
            }

            return name == null
                    ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a valid item in the request body")
                    : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
        }
    }
}
