using EducationSystem.Models;

namespace EducationSystem.Interfaces
{
    public interface ITeam
    {
        Team GetTeamByManagerId(int id);
    }
}
