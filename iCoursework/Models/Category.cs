using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iCoursework.Models
{
    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual IList<Instruction> Instructions { get; set; } = new List<Instruction>();
    }
}
