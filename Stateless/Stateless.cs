using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using ClassLibrary;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Client;
using System.Diagnostics;

namespace Stateless
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class Stateless : StatelessService
    {
        public Stateless(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            int iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                int sizeKB = ++iterations * 256;
                Task<int> task = Work(sizeKB * 1024);
                if (!task.Wait(30000))
                {
                    ServiceEventSource.Current.ServiceMessage(this, "CALL TIMED OUT for {0} KB", sizeKB);
                }
                else
                {
                    int blobLength = task.Result;
                    ServiceEventSource.Current.ServiceMessage(this, "Blob Length-{0}", blobLength);
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        private async Task<int> Work(int size)
        {
            try
            {
                Uri uri = new Uri("fabric:/BlobTest/Stateful");
                ITest blobService = ServiceProxy.Create<ITest>(uri, new ServicePartitionKey(0));

                string message = await blobService.GetMessage();

                byte[] blob = await blobService.GetBlob(size);
                return blob.Length;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception caught.");
                Debug.WriteLine(ex.Message);
                return -1;
            }
        }
    }
}
