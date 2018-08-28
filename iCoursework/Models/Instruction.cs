using System;
using System.Collections.Generic;

namespace iCoursework.Models
{
    public class Instruction
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public double Rating { get; set; }
        public int RatesCount { get; set; }
        public string Description { get; set; }
        //public virtual IList<Tag> Tags { get; set; } = new List<Tag>();
        public virtual Category Category { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public virtual IList<Step> Steps { get; set; } = new List<Step>();                                                  
        public virtual IList<Comment> Comments { get; set; } = new List<Comment>();                      
        

    }
}
