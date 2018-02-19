using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace Jogging.Data.Entities
{
    public class User : IdentityUser
    {
        public User()
        {
            Runs = new List<Run>();
        }

        public virtual ICollection<Run> Runs { get; set; }
    }
}