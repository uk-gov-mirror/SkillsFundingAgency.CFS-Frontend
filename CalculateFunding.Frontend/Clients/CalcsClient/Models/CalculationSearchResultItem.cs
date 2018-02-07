﻿using CalculateFunding.Frontend.Clients.CommonModels;

namespace CalculateFunding.Frontend.Clients.CalcsClient.Models
{
    public class CalculationSearchResultItem : Reference
    {
        public string SpecificationName { get; set; }

        public string PeriodName { get; set; }

        public string Status { get; set; }
    }
}