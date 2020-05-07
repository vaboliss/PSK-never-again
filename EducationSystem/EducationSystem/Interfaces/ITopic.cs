using EducationSystem.Models;
using System.Collections.Generic;

namespace EducationSystem.Interfaces
{
    public interface ITopic
    {
        Topic GetTopicById(int id);

        List<Worker> GetWorkersByTopic(int topicId);
    }
}
