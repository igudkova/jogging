using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Jogging.Authentication
{
    public class GoogleChallengeResult : IHttpActionResult
    {        
        public HttpRequestMessage Request { get; set; }

        public GoogleChallengeResult(ApiController controller)
        {
            Request = controller.Request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            Request.GetOwinContext().Authentication.Challenge(GoogleAuthProvider.Name);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.RequestMessage = Request;
            return Task.FromResult(response);
        }
    }
}