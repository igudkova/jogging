using Jogging.Authentication;
using Jogging.Data;
using System.Security.Claims;
using System.Web.Http;

namespace Jogging.Controllers
{
    public class BaseController : ApiController
    {
        protected DatabaseContext databaseContext = new DatabaseContext();
        protected AuthRepository authRepository = new AuthRepository();

        protected string GetLoggedUserId()
        {
            string userId = string.Empty;

            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;

            userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (!authRepository.IsLogged(userId))
            {
                userId = string.Empty;
            }

            return userId;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                databaseContext.Dispose();
                authRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}