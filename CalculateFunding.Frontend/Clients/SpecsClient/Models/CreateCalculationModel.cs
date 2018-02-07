﻿using Newtonsoft.Json;

namespace CalculateFunding.Frontend.Clients.SpecsClient.Models
{
    public class CreateCalculationModel
    {
        [JsonProperty("specificationId")]
        public string SpecificationId { get; set; }

        [JsonProperty("allocationLineId")]
        public string AllocationLineId { get; set; }

        [JsonProperty("policyId")]
        public string PolicyId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
