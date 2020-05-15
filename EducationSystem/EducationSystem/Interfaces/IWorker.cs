using System.Collections.Generic;
using EducationSystem.Models;

namespace EducationSystem.Interfaces
{
    public interface IWorker
    {
        int CreateWorkerRId(Worker worker);
        void RemoveWorkerById(int id);
    }

}
