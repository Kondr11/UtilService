using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Http;

namespace Scan_servise
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             UriTemplate = "api/task/{id}")]
        Task GetTask(string id);

        [OperationContract]
        [WebInvoke(Method = "POST",
             ResponseFormat = WebMessageFormat.Json,
             UriTemplate = "api/task")]
        Task AddTask(Task task);

        [OperationContract]
        [WebInvoke(Method = "PUT",
             ResponseFormat = WebMessageFormat.Json,
             UriTemplate = "api/task")]
        Task StartScan(Task task);
    }

}
