using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EducationSystem.Middleware
{
    public class ConsoleLoggerMiddleware
    {
        private readonly RequestDelegate _next;

        public ConsoleLoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                Debug.WriteLine(
                    $" Username:  {context.User.Identity.Name} \n" +
                    $" Role: " + (context.User.IsInRole("Worker") ? "Worker" : (context.User.IsInRole("Manager") ? "Manager" : "Anonymous")) + "\n" +
                    $" Request Type : {context.Request.Method}\n" +
                    $" Request Url : {context.Request.Path}\n" +
                    $" Request Date : {DateTime.Now}");

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
