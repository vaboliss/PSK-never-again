using EducationSystem.Data;
using EducationSystem.Interfaces;
using EducationSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EducationSystem.Provider
{
    public class TeamService:ITeam
    {
        private readonly EducationSystemDbContext _edu;

        public TeamService(EducationSystemDbContext edu)
        {
            _edu = edu;
        }

        public Team GetTeamByManagerId(int id)
        {
            Team t = _edu.Teams.Include(m => m.Manager).FirstOrDefault(t => t.WorkerId == id);
            return t;
        }

        public int GetTeamSize()
        {
            throw new System.NotImplementedException();
        }
    }
}
