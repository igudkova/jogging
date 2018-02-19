using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Jogging.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : BaseController
    {
        [Route("")]
        public IHttpActionResult Get()
        {
            try
            {
                string loggedUserId = GetLoggedUserId();
                if (string.IsNullOrEmpty(loggedUserId) || 
                    !authRepository.IsInRole(loggedUserId, "Manager"))
                {
                    return Unauthorized();
                }
            }
            catch (Exception)
            {
                return Unauthorized();
            }

            IEnumerable<object> users = databaseContext.Users.AsEnumerable().Select(user => new
            {
                name = user.UserName,
                isManager = authRepository.IsInRole(user.Id, "Manager")
            });

            return Ok(users);
        }

        [Route("")]
        public IHttpActionResult Put(string name, [FromBody]string value)
        {
            try
            {
                string loggedUserId = GetLoggedUserId();
                if (string.IsNullOrEmpty(loggedUserId) ||
                    !authRepository.IsInRole(loggedUserId, "Manager"))
                {
                    return Unauthorized();
                }
            }
            catch (Exception)
            {
                return Unauthorized();
            }

            IdentityUser user = authRepository.FindByName(name);
            if (user == null)
            {
                return NotFound();
            }

            string[] param = value.Split('=');
            if(param[0] != "isManager")
            {
                return BadRequest("Wrong parameter name");
            }

            bool paramValue;
            if(!bool.TryParse(param[1], out paramValue))
            {
                return BadRequest("Wrong parameter type; expected boolean");
            }

            try
            {
                if (paramValue != authRepository.IsInRole(user.Id, "Manager"))
                {
                    if (paramValue)
                    {
                        authRepository.RemoveFromRole(user.Id, "User");
                        authRepository.AddToRole(user.Id, "Manager");
                    }
                    else
                    {
                        authRepository.RemoveFromRole(user.Id, "Manager");
                        authRepository.AddToRole(user.Id, "User");
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return Ok();
        }

        [Route("")]
        public IHttpActionResult Delete(string name)
        {
            try
            {
                string loggedUserId = GetLoggedUserId();
                if (string.IsNullOrEmpty(loggedUserId) ||
                    !authRepository.IsInRole(loggedUserId, "Manager"))
                {
                    return Unauthorized();
                }
            }
            catch (Exception)
            {
                return Unauthorized();
            }

            IdentityUser user = authRepository.FindByName(name);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                authRepository.Delete(user);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return Ok();
        }
    }
}