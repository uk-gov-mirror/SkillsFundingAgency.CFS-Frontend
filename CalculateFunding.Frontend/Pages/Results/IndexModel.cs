﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CalculateFunding.Frontend.ApiClient;
using CalculateFunding.Frontend.ApiClient.Models.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CalculateFunding.Frontend.Interfaces.APiClient;

namespace CalculateFunding.Frontend.Pages.Results
{
    public class IndexModel : PageModel
    {
        readonly IBudgetApiClient _apiClient;

        public IEnumerable<BudgetSummary> Budgets;

        public IndexModel(IBudgetApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var results = await _apiClient.GetBudgetResultsAsync(HttpContext.RequestAborted).ConfigureAwait(false);

            Budgets = results.Content;

            return Page();
        }
    }

}
