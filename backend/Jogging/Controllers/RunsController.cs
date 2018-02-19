using Jogging.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Jogging.Controllers
{
    [RoutePrefix("api/runs")]
    public class RunsController : BaseController
    {
        public IHttpActionResult GetTest()
        {
            IEnumerable<Run> runs = databaseContext.Runs.
                    Select(run => new Run()
                    {
                        Id = run.Id,
                        Date = run.Date,
                        Distance = run.Distance,
                        Duration = run.Duration
                    });

            return Ok(runs);
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            string loggedUserId = "";
            try
            {
                loggedUserId = GetLoggedUserId();
                if (string.IsNullOrEmpty(loggedUserId))
                {
                    return Unauthorized();
                }
            }
            catch (Exception)
            {
                return Unauthorized();
            }

            IEnumerable<object> runs = databaseContext.Runs.
                    Where(r => r.UserId == loggedUserId).
                    Select(run => new Run(){
                        Id = run.Id,
                        Date = run.Date,
                        Distance = run.Distance,
                        Duration = run.Duration
                    });

            return Ok(runs);
        }

        [Route("")]
        public IHttpActionResult Post(Run run)
        {
            string loggedUserId = "";
            try
            {
                loggedUserId = GetLoggedUserId();
                if (string.IsNullOrEmpty(loggedUserId))
                {
                    return Unauthorized();
                }
            }
            catch (Exception)
            {
                return Unauthorized();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    run.Id = Guid.NewGuid().ToString();
                    run.UserId = loggedUserId;

                    databaseContext.Runs.Add(run);
                    databaseContext.SaveChanges();

                    return Ok(new
                    {
                        id = run.Id,
                        date = run.Date,
                        distance = run.Distance,
                        duration = run.Duration
                    });
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("")]
        public IHttpActionResult Put(string id, [FromBody]Run value)
        {
            string loggedUserId = "";
            try
            {
                loggedUserId = GetLoggedUserId();
                if (string.IsNullOrEmpty(loggedUserId))
                {
                    return Unauthorized();
                }
            }
            catch (Exception)
            {
                return Unauthorized();
            }

            Run run = databaseContext.Runs.FirstOrDefault(r => r.Id == id);
            if (run == null)
            {
                return NotFound();
            }
            if (run.UserId != loggedUserId)
            {
                return Unauthorized();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    run.Date = value.Date;
                    run.Distance = value.Distance;
                    run.Duration = value.Duration;

                    databaseContext.SaveChanges();
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return Ok();
        }

        [Route("")]
        public IHttpActionResult Delete(string id)
        {
            string loggedUserId = "";
            try
            {
                loggedUserId = GetLoggedUserId();
                if (string.IsNullOrEmpty(loggedUserId))
                {
                    return Unauthorized();
                }
            }
            catch (Exception)
            {
                return Unauthorized();
            }

            Run run = databaseContext.Runs.FirstOrDefault(r => r.Id == id);
            if (run == null)
            {
                return NotFound();
            }
            if (run.UserId != loggedUserId)
            {
                return Unauthorized();
            }

            try
            {
                run = databaseContext.Runs.Remove(run);
                databaseContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return Ok();
        }
    }
}