using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace iCoursework.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public bool IsBlocked { get; set; } = false;
        public virtual IList<Instruction> Instructions { get; set; } = new List<Instruction>();
        public virtual IList<Like> Likes { get; set; } = new List<Like>();
    }
}
