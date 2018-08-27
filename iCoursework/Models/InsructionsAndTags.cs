using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iCoursework.Models
{
    public class InsructionsAndTags
    {
        public virtual IList<Instruction> Instructions { get; set; } = new List<Instruction>();
        public virtual IList<Tag> Tags { get; set; } = new List<Tag>();
    }
}
