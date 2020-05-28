using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EducationSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace EducationSystem.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                Debug.WriteLine(
                    $" Username:  {context.User.Identity.Name} \n" +
                    $" Role: "+ (context.User.IsInRole("Worker") ? "Worker" : (context.User.IsInRole("Manager") ? "Manager": "Anonymous")) +"\n" + 
                    $" Request Type : {context.Request.Method}\n" +
                    $" Request Url : {context.Request.Path}\n");
                await _next(context);

            }
            catch (Exception e)
            {
                Debug.WriteLine($"The following error happened: {e.Message}");
                throw;
            }
        }
    }
}
