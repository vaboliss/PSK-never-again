using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Models.Entities
{
    public class Goal
    {
        public int Goal_Id { get; set; }
        public Topic Topic { get; set; }
        public Worker Worker { get; set; }
    }
}
