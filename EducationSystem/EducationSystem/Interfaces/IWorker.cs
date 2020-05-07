using System.Collections.Generic;
using EducationSystem.Models;

namespace EducationSystem.Interfaces
{
    public interface IWorker
    {
        List<Worker> GetWorkersByTopic(int topicId);
    }

}
