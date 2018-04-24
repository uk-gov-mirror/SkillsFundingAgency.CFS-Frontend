﻿
namespace CalculateFunding.Frontend.Clients.ScenariosClient.Models
{
    using CalculateFunding.Frontend.Clients.CommonModels;
    using CalculateFunding.Frontend.Clients.ResultsClient.Models;
    using Newtonsoft.Json;
    using System;
    public class Scenario : Reference
    {
        [JsonProperty("description")]
        public string ScenarioDescription { get; set; }

        [JsonProperty("publishStatus")]
        public string Status { get; set; }

        [JsonProperty("Gherkin ")]
        public string GherkinCode { get; set; }

        [JsonProperty("date")]
        public DateTime LastModified { get; set; }

        [JsonProperty("author")]
        public Reference LastModifiedBy { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("current")]
        public CurrentScenarioVersion CurrentVersion { get; set; }

        [JsonProperty("specification")]
        public SpecificationSummary Specification { get; set; }
    }
}
