using EducationSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EducationSystem.Interfaces
{
    public interface ITeam
    {
        Team GetTeamByManagerId(int id);
        int GetTeamSize();
    }
}
