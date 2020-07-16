﻿using System.Collections.Generic;
using CalculateFunding.Common.ApiClient.DataSets.Models;
using CalculateFunding.Common.ApiClient.Providers.Models.Search;
using CalculateFunding.Common.Models.Search;
using CalculateFunding.Frontend.ViewModels.Common;

namespace CalculateFunding.Frontend.ViewModels.Datasets
{
    public class PagedDatasetSearchResults
    {
		public int TotalCount { get; set; }
		public int StartItemNumber { get; set; }
		public int EndItemNumber { get; set; }
		public IEnumerable<DatasetVersions> Items { get; set; }
		public IEnumerable<Facet> Facets { get; set; }
        public PagerState PagerState { get; set; }
    }
}
