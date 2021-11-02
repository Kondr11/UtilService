using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Scan_service
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
