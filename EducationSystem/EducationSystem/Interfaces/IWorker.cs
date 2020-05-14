using System.Collections.Generic;
using EducationSystem.Models;

namespace EducationSystem.Interfaces
{
    public interface IWorker
    {
        bool AssignGoal(Worker worker, int topicId);
    }

}
