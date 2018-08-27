using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iCoursework.Models
{
    public class Like
    {
        public string Id { get; set; }
        public virtual Comment Comment { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
