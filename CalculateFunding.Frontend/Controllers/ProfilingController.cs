﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Profiling;
using CalculateFunding.Common.ApiClient.Profiling.Models;
using CalculateFunding.Common.ApiClient.Publishing;
using CalculateFunding.Common.ApiClient.Publishing.Models;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.Utility;
using CalculateFunding.Frontend.ViewModels.Profiles;
using CalculateFunding.Frontend.ViewModels.Publish;
using Microsoft.AspNetCore.Mvc;

namespace CalculateFunding.Frontend.Controllers
{
	public class ProfilingController : Controller
    {
	    private readonly IProfilingApiClient _profilingApiClient;
        private readonly IPublishingApiClient _publishingApiClient;

	    public ProfilingController(IProfilingApiClient profilingApiClient,
            IPublishingApiClient publishingApiClient)
	    {
		    Guard.ArgumentNotNull(profilingApiClient, nameof(profilingApiClient));
            Guard.ArgumentNotNull(publishingApiClient, nameof(publishingApiClient));
            
		    _profilingApiClient = profilingApiClient;
            _publishingApiClient = publishingApiClient;
        }

        [HttpPost("api/profiling/preview")]
        public async Task<IActionResult> PreviewProfileChange([FromBody] ProfilePreviewRequestViewModel requestViewModel)
        {
            Guard.ArgumentNotNull(requestViewModel, nameof(requestViewModel));

            ApiResponse<IEnumerable<ProfileTotal>> profilePreview = await _publishingApiClient.PreviewProfileChange(new ProfilePreviewRequest
            {
                ConfigurationType = requestViewModel.ConfigurationType.AsMatchingEnum<ProfileConfigurationType>(),
                ProviderId = requestViewModel.ProviderId,
                SpecificationId = requestViewModel.SpecificationId,
                FundingLineCode = requestViewModel.FundingLineCode,
                FundingPeriodId = requestViewModel.FundingPeriodId,
                FundingStreamId = requestViewModel.FundingStreamId,
                ProfilePatternKey = requestViewModel.ProfilePatternKey
            });
            
            IActionResult errorResult = profilePreview.IsSuccessOrReturnFailureResult(nameof(FundingStreamPeriodProfilePattern));
            
            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok(profilePreview.Content);
        }

	    [HttpGet]
	    [Route("api/profiling/patterns/fundingStream/{fundingStreamId}/fundingPeriod/{fundingPeriodId}")]
	    public async Task<IActionResult> GetFutureInstallments(
		    string fundingStreamId,
		    string fundingPeriodId)
	    {
		    Guard.ArgumentNotNull(fundingStreamId, nameof(fundingStreamId));
		    Guard.ArgumentNotNull(fundingPeriodId, nameof(fundingPeriodId));

		    ApiResponse<IEnumerable<FundingStreamPeriodProfilePattern>> apiResponse =
			    await _profilingApiClient.GetProfilePatternsForFundingStreamAndFundingPeriod(fundingStreamId,
				    fundingPeriodId);

		    IActionResult errorResult = apiResponse.IsSuccessOrReturnFailureResult(nameof(FundingStreamPeriodProfilePattern));
		    if (errorResult != null)
		    {
			    return errorResult;
		    }

		    IEnumerable<FundingStreamPeriodProfilePattern> fundingStreamPeriodProfilePatterns =
			    apiResponse.Content;

		    var profilePatterns =
			    fundingStreamPeriodProfilePatterns.SelectMany(p => p.ProfilePattern).ToList();

		    var futureProfilePatterns = profilePatterns.Where(f =>
				    f.PeriodYear > DateTime.Now.Year ||
				    f.PeriodYear == DateTime.Now.Year &&
				    f.PeriodStartDate.Month > DateTime.Now.Month)
			    .ToList();

		    return Ok(!futureProfilePatterns.Any()
			    ? new List<ProfilingInstallment>()
			    : MapToFutureInstallmentModel(futureProfilePatterns));
	    }

	    private static List<ProfilingInstallment> MapToFutureInstallmentModel(
	        IEnumerable<ProfilePeriodPattern> profilePatterns) =>
	        profilePatterns
		        .Select(profilingTotal =>
			        new ProfilingInstallment(
				        profilingTotal.PeriodYear,
				        profilingTotal.Period,
				        profilingTotal.Occurrence, 0))
		        .ToList();
    }
}