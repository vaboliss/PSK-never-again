using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Models.Entities
{
    public class Restriction
    {
        public int Restrictions_Id { get; set; }
        public int DayCap { get; set; }
        public DateTime Expires { get; set; }
    }
}
