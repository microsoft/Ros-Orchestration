// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RobotOrchestrator.OrderManager
{

    public class DispatcherClient : IDispatcherClient
    {
        private static readonly HttpClient httpClient = new HttpClient();

        private readonly string dispatcherUri;

        public DispatcherClient(string dispatcherUri)
        {
            this.dispatcherUri = dispatcherUri;
        }

        public async Task<bool> SendJobAsync(Job job)
        {
            var jobJson = JsonConvert.SerializeObject(job);
            var content = new StringContent(jobJson, Encoding.UTF8, "application/json");

            var httpResponseMessage = await httpClient.PostAsync(dispatcherUri, content);

            return httpResponseMessage.IsSuccessStatusCode;
        }
    }
}
