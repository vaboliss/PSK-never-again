using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EducationSystem.Middleware
{
    public class FileLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private static object locker = new object();
        public FileLoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string path = Environment.CurrentDirectory + "\\logs\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                if (File.Exists(path))
                {
                    await WriteToFileAsync(context, path);

                }
                else
                {
                    File.Create(path).Dispose();
                    await WriteToFileAsync(context, path);
                }
                await _next(context);

            }
            catch (Exception e)
            {
                if (File.Exists(path))
                {
                    await WriteToFileAsync(context, path, $"\n the following error happened: {e.Message}\n");

                }
                else
                {
                    File.Create(path).Dispose();
                    await WriteToFileAsync(context, path);
                }
                throw;
            }
        }

        private async Task WriteToFileAsync(HttpContext context, string path, string error = "")
        {
            lock (locker)
            {
                using (TextWriter tw = new StreamWriter(path, true))
                {
                    tw.WriteLineAsync($" Username:  {context.User.Identity.Name} \n" +
                        $" Role: " + (context.User.IsInRole("Worker") ? "Worker" : (context.User.IsInRole("Manager") ? "Manager" : "Anonymous")) + "\n" +
                        $" Request Type : {context.Request.Method}\n" +
                        $" Request Url : {context.Request.Path}\n" +
                        $" Request Date : {DateTime.Now}\n" +
                        $"{error}");
                }
            }
        }
    }
}
