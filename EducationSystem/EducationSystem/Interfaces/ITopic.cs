using EducationSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EducationSystem.Interfaces
{
    public interface ITopic
    {
        Topic GetTopicById(int id);
        Task<List<Topic>> GetAllTopics();
        List<Worker> GetWorkersByTopic(int topicId);
    }
}
