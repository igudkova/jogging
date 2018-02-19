using Jogging.Authentication;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace Jogging.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private AuthRepository authRepository = new AuthRepository();

        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [HttpGet]
        [Route("login")]
        public IHttpActionResult StartLogin()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new GoogleChallengeResult(this);
            }

            var queryParams = Request.GetQueryNameValuePairs();
            string redirectUri = queryParams.FirstOrDefault(keyValue => string.Compare(keyValue.Key, "redirect_uri", true) == 0).Value;
            if (string.IsNullOrWhiteSpace(redirectUri))
            {
                return BadRequest("redirect_uri is required");
            }

            ClaimsIdentity identity = User.Identity as ClaimsIdentity;
            if(identity == null)
            {
                return InternalServerError();
            }

            Claim googleClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (googleClaim == null || googleClaim.Issuer != GoogleAuthProvider.Name || string.IsNullOrEmpty(googleClaim.Value))
            {
                return InternalServerError();
            }

            Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            redirectUri = string.Format("{0}#externalAccessToken={1}&userName={2}", redirectUri, identity.FindFirstValue("ExternalAccessToken"), identity.FindFirstValue(ClaimTypes.Email));

            return Redirect(redirectUri);
        }

        [HttpGet]
        [Route("token")]
        public async Task<IHttpActionResult> GetLocalToken(string userName, string externalAccessToken)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest("username is required");
            }
            if (string.IsNullOrWhiteSpace(externalAccessToken))
            {
                return BadRequest("external access token is required");
            }

            dynamic verifiedAccessToken = await VerifyExternalAccessToken(externalAccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("invalid external access token");
            }

            Tuple<IdentityUser, IdentityResult> registration = await RegisterOrFindUser(userName, new UserLoginInfo(GoogleAuthProvider.Name, verifiedAccessToken.UserId));
            IdentityUser user = registration.Item1;
            IdentityResult result = registration.Item2;

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    return BadRequest(ModelState);
                }
                return BadRequest();
            }

            var accessTokenResponse = await GenerateLocalAccessTokenResponse(user);
            return Ok(accessTokenResponse);
        }

        private async Task<object> VerifyExternalAccessToken(string accessToken)
        {
            string tokenVerifierUri = ConfigurationManager.AppSettings["GoogleTokenVerifier"] + accessToken;

            var client = new HttpClient();
            var response = await client.GetAsync(tokenVerifierUri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                dynamic jsonObject = (JObject) JsonConvert.DeserializeObject(content);

                var token = new {
                    UserId = (string) jsonObject["user_id"],
                    AppId = (string) jsonObject["audience"]
                };

                if (string.Equals(Startup.GoogleOAuth2AuthenticationOptions.ClientId, token.AppId, StringComparison.OrdinalIgnoreCase))
                {
                    return token;
                }
            }

            return null;
        }

        private async Task<Tuple<IdentityUser, IdentityResult>> RegisterOrFindUser(string userName, UserLoginInfo loginInfo)
        {
            IdentityResult result = IdentityResult.Success;

            IdentityUser user = await authRepository.FindAsync(loginInfo);
            if (user == null)
            {
                user = new IdentityUser()
                {
                    UserName = userName
                };

                result = await authRepository.CreateAsync(user);
                if (result.Succeeded)
                {
                    if (userName == ConfigurationManager.AppSettings["DefaultManager"])
                    {
                        await authRepository.AddToRoleAsync(user.Id, "Manager");
                    }
                    else
                    {
                        await authRepository.AddToRoleAsync(user.Id, "User");
                    }

                    result = await authRepository.AddLoginAsync(user.Id, loginInfo);
                }
            }

            return new Tuple<IdentityUser, IdentityResult> (user, result);
        }

        private async Task<JObject> GenerateLocalAccessTokenResponse(IdentityUser user)
        {
            var tokenValidityPeriod = TimeSpan.FromDays(1);

            var userRoles = string.Join(",", await authRepository.GetRolesAsync(user.Id));

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, userRoles));

            var authenticationProperties = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenValidityPeriod),
            };

            var ticket = new AuthenticationTicket(identity, authenticationProperties);

            string accessToken = Startup.OAuthBearerAuthenticationOptions.AccessTokenFormat.Protect(ticket);

            return new JObject(new JProperty("userName", user.UserName),
                               new JProperty("isManager", authRepository.IsInRole(user.Id, "Manager")),
                               new JProperty("accessToken", accessToken),
                               new JProperty("tokenType", "bearer"),
                               new JProperty("expiresIn", tokenValidityPeriod.TotalSeconds.ToString()),
                               new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                               new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString()));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                authRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}