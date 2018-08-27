using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iCoursework.Models
{
    public class Comment
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public virtual Instruction Instruction { get; set; }
        public virtual IList<Like> Likes { get; set; } = new List<Like>();
    }
}
