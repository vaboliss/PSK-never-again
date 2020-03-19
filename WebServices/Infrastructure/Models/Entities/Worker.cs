using System.Collections.Generic;

namespace Infrastructure.Models.Entities
{
    public class Worker
    {
        public int Worker_Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Restriction Restriction { get; set; }
        public List<Worker> Subordinates { get; set; }
    }
}