﻿using Newtonsoft.Json;

namespace CalculateFunding.Frontend.Clients.SpecsClient.Models
{
    public class CreateSpecificationModel
    {
        [JsonProperty("academicYearId")]
        public string AcademicYearId { get; set; }

        [JsonProperty("fundingStreamId")]
        public string FundingStreamId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
