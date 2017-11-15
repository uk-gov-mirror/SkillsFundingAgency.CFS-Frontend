﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Allocations.Web.ApiClient.Models;
using Allocations.Web.Pages;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Allocations.Web.ApiClient
{
    public class AllocationApiOptions
    {
        public string AllocationsApiKey { get; set; }
        public string AllocationsApiEndpoint { get; set; }
        public string ResultsPath { get; set; }
        public string SpecsPath { get; set; }
    }
    public class AllocationsApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings{Formatting = Formatting.Indented, ContractResolver = new CamelCasePropertyNamesContractResolver()};
        private readonly string _resultsPath;
        private string _specsPath;

        public AllocationsApiClient(IOptions<AllocationApiOptions> options)
        {
            _httpClient = new HttpClient(){BaseAddress = new Uri(options.Value.AllocationsApiEndpoint)};
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", options.Value.AllocationsApiKey);
            _resultsPath = options.Value.ResultsPath ?? "/api/v1/results";
            _specsPath = options.Value.SpecsPath ?? "/api/v1/specs";
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<T>(response.StatusCode, JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync(), _serializerSettings));
            }
            return new ApiResponse<T>(response.StatusCode);
        }

        public async Task<ApiResponse<RootObject[]>> GetBudgetResults()
        {
            return await GetAsync<RootObject[]>($"{_resultsPath}/budgets");
        }

        public async Task<ApiResponse<Budget>> GetBudget(string id)
        {
            return await GetAsync<Budget>($"{_specsPath}/budgets?budgetId={id}");
        }

        public async Task<ApiResponse<Budget[]>> GetBudgets()
        {
            return await GetAsync<Budget[]>($"{_specsPath}/budgets");
        }

        public async Task<ApiResponse<PreviewResponse>> PostPreview(PreviewRequest request)
        {

            return (await PostAsync<PreviewResponse, PreviewRequest>("api/v1/engine/preview", request));
        }


        private async Task<ApiResponse<TResponse>> PostAsync<TResponse, TRequest>(string url, TRequest request)
        {
            string json = JsonConvert.SerializeObject(request, _serializerSettings);
            var response = await _httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<TResponse>(response.StatusCode, JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync(), _serializerSettings));
            }
            return new ApiResponse<TResponse>(response.StatusCode);
        }

        private async Task<HttpStatusCode> PostAsync<TRequest>(string url, TRequest request)
        {
            string json = JsonConvert.SerializeObject(request, _serializerSettings);
            var response = await _httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> PostBudget(Budget budget)
        {
            return await PostAsync($"{_specsPath}/budgets", budget);
        }
    }
}

