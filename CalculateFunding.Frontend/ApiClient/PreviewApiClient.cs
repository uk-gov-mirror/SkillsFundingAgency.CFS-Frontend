﻿using System.Threading.Tasks;
using CalculateFunding.Frontend.ApiClient.Models;
using CalculateFunding.Frontend.Interfaces.ApiClient;
using CalculateFunding.Frontend.Interfaces.Core;
using CalculateFunding.Frontend.Interfaces.Core.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CalculateFunding.Frontend.ApiClient
{
    public class PreviewApiClient : AbstractApiClient, IPreviewApiClient
    {
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented, ContractResolver = new CamelCasePropertyNamesContractResolver() };

        public PreviewApiClient(IOptionsSnapshot<ApiOptions> options, IHttpClient httpClient, ILoggingService logs)
             : base(options, httpClient, logs)
        { }


        public async Task<ApiResponse<PreviewResponse>> PostPreview(PreviewRequest request)
        {

            return (await PostAsync<PreviewResponse, PreviewRequest>("api/v1/engine/preview", request).ConfigureAwait(false));
        }
    }
}

