using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
  //  [ServiceContract]
    public interface ITest : IService
    {
   //     [OperationContract]
        Task<byte[]> GetBlob(int size);

 //       [OperationContract]
        Task<string> GetMessage();
    }
}
