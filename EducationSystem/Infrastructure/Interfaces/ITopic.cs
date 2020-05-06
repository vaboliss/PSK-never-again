using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Models.Entities;

namespace Infrastructure.Interfaces
{
    public interface ITopic
    {
        Topic GetTopicById(int id);
    }
}
