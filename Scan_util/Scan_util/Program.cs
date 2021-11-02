using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Scan_util
{
    class Program
    {
        static HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            URIInit();
            try
            {
                if (args.Length > 1)
                    switch (args[0])
                    {
                        case "scan":
                            Task task = CreateDirectory(args[1]);
                            if (task != null)
                            {
                                int id = Int32.Parse(AddTaskAsync(task).GetAwaiter().GetResult());
                                Console.WriteLine("Scan task was created with ID: " + id.ToString());
                                task.Id = id;
                                StartTaskAsync(task).GetAwaiter();
                            }
                            else
                                Console.WriteLine("Directory specified incorrectly");
                            break;
                        case "status":
                            Console.WriteLine(CheckStatusAsync(args[1]).GetAwaiter().GetResult());
                            break;
                        default:
                            Console.WriteLine("There is no such command");
                            break;
                    }
                else if (args.Length > 0)
                    Console.WriteLine("Enter directory name or task id");
                else
                    Console.WriteLine("Enter the command and its parameters");
            }
            catch (Exception)
            {
                Console.WriteLine("Server is not available");
            }
        }
        static void URIInit()
        {
            client.BaseAddress = new Uri("http://localhost:44304/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static Task CreateDirectory(string directoryName)
        {
            DirectoryInfo directory = new DirectoryInfo(directoryName);
            if (directory.Exists)
            {
                return new Task() { DirectoryName = directoryName, Status = "Scan task in progress, please wait" };
            }
            else
                return null;

        }
        static async System.Threading.Tasks.Task<string> AddTaskAsync(Task task)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
               "api/task", task);
            if (response.IsSuccessStatusCode)
            {
                task = await response.Content.ReadAsAsync<Task>();
                return task.Id.ToString();
            }
            else
                return "Failed to create task";
        }

        static async System.Threading.Tasks.Task<string> StartTaskAsync(Task task)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync($"api/task", task);
            if (response.IsSuccessStatusCode)
            {
                task = await response.Content.ReadAsAsync<Task>();
                return task.Id.ToString();
            }
            else
                return "Failed to start task";
        }
        static async System.Threading.Tasks.Task<string> CheckStatusAsync(string id)
        {
            int a = 0;
            if (Int32.TryParse(id, out a))
            {
                Task task = null;
                id = "api/task/" + id;
                HttpResponseMessage response = await client.GetAsync(id);
                if (response.IsSuccessStatusCode)
                {
                    task = await response.Content.ReadAsAsync<Task>();
                    return task.Status;
                }
                else
                    return "There is no such task";
            }
            else
                return "Incorrect id entered";

        }
    }
}
