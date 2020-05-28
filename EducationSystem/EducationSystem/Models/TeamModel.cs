using System;
using System.Collections.Generic;
using System.Text;

namespace EducationSystem.Models
{
    public class TeamModel
    {
        public Team team;
        public Worker manager;
        public List<Worker> workers = new List<Worker>();
        public int teamSize;
    }
}
