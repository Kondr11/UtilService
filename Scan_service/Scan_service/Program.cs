using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Scan_service
{
    class Program
    {
        static void Main(string[] args)
        {
            WebServiceHost hostWeb = new WebServiceHost(typeof(Service));
            ServiceEndpoint ep = hostWeb.AddServiceEndpoint(typeof(IService), new WebHttpBinding(), "");
            ServiceDebugBehavior stp = hostWeb.Description.Behaviors.Find<ServiceDebugBehavior>();
            stp.HttpHelpPageEnabled = false;
            try
            {
                hostWeb.Open();
                Database.SetInitializer(new TaskDbInitializer());
                Console.WriteLine("Scan service was started.\r\nPress <Enter> to exit...");
                Console.Read();
            }
            catch (Exception)
            {
                Console.WriteLine("Run as admin");
            }
        }
    }
}
