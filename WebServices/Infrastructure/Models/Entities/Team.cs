using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Models.Entities
{
    public class Team
    {
        public int Team_Id { get; set; }
        public string TeamName { get; set; }
        public Worker Manager { get; set; }
    }
}
