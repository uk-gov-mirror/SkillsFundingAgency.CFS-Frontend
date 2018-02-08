﻿namespace CalculateFunding.Frontend.ViewModels.Common
{
    using System.Collections.Generic;

    public class SearchRequestViewModel
    {
        public int? PageNumber { get; set; }

        public string SearchTerm { get; set; }

        public bool IncludeFacets { get; set; }

        public IDictionary<string, string[]> Filters { get; set; }
    }
}
