﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Specifications;
using CalculateFunding.Common.ApiClient.Specifications.Models;
using CalculateFunding.Common.Identity.Authorization.Models;
using CalculateFunding.Common.Utility;
using CalculateFunding.Frontend.Extensions;
using CalculateFunding.Frontend.Helpers;
using CalculateFunding.Frontend.ViewModels.Common;
using CalculateFunding.Frontend.ViewModels.Specs;
using Microsoft.AspNetCore.Mvc;

namespace CalculateFunding.Frontend.Controllers
{
    public class SpecificationController : Controller
    {
        private readonly ISpecsApiClient _specsClient;
        private readonly ISpecificationsApiClient _specificationsApiClient;
        private readonly IAuthorizationHelper _authorizationHelper;

        public SpecificationController(ISpecsApiClient specsApiClient, ISpecificationsApiClient specificationsApiClient, IAuthorizationHelper authorizationHelper)
        {
            Guard.ArgumentNotNull(specsApiClient, nameof(specsApiClient));
            Guard.ArgumentNotNull(authorizationHelper, nameof(authorizationHelper));

            _specsClient = specsApiClient;
            _specificationsApiClient = specificationsApiClient;
            _authorizationHelper = authorizationHelper;
        }

        [Route("api/specifications-by-period/{fundingPeriodId}")]
        public async Task<IActionResult> GetSpecificationsByFundingPeriod(string fundingPeriodId)
        {
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            ApiResponse<IEnumerable<SpecificationSummary>> apiResponse = await _specsClient.GetSpecifications(fundingPeriodId);
            if (apiResponse == null)
            {
                return new StatusCodeResult(500);
            }

            if (apiResponse.StatusCode != HttpStatusCode.OK)
            {
                return new StatusCodeResult((int)apiResponse.StatusCode);
            }

            if (apiResponse.Content == null)
            {
                return new ObjectResult("List of Specifications was null")
                {
                    StatusCode = 500,
                };
            }

            List<ReferenceViewModel> result = new List<ReferenceViewModel>();

            foreach (SpecificationSummary specification in apiResponse.Content.OrderBy(o => o.Name))
            {
                result.Add(new ReferenceViewModel(specification.Id, specification.Name));
            }

            return Ok(result);
        }

        [Route("api/specs/specifications-selected-for-funding")]
        public async Task<IActionResult> GetSpecificationsSelectedForFunding()
        {
            ApiResponse<IEnumerable<SpecificationSummary>> apiResponse = await _specsClient.GetSpecificationsSelectedForFunding();

            if (apiResponse.StatusCode == HttpStatusCode.OK)
            {
                return Ok(apiResponse.Content.OrderBy(c => c.Name));
            }

            if (apiResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                return new BadRequestResult();
            }

            return new StatusCodeResult(500);
        }

        [Route("api/specs/specifications-selected-for-funding-by-period/{fundingPeriodId}")]
        public async Task<IActionResult> GetSpecificationsForFundingByPeriod(string fundingPeriodId)
        {
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            ApiResponse<IEnumerable<SpecificationSummary>> apiResponse = await _specsClient.GetSpecificationsSelectedForFundingByPeriod(fundingPeriodId);

            if (apiResponse.StatusCode == HttpStatusCode.OK)
            {
                return Ok(apiResponse.Content.OrderBy(c => c.Name));
            }

            if (apiResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                return new BadRequestResult();
            }

            return new StatusCodeResult(500);
        }

        [Route("api/specs/specifications-by-fundingperiod-and-fundingstream/{fundingPeriodId}/{fundingStreamId}")]
        public async Task<IActionResult> GetApprovedSpecifications(string fundingPeriodId, string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            ApiResponse<IEnumerable<SpecificationSummary>> apiResponse = await _specsClient.GetApprovedSpecifications(fundingPeriodId, fundingStreamId);

            if (apiResponse.StatusCode == HttpStatusCode.OK)
            {
                return Ok(apiResponse.Content.OrderBy(c => c.Name));
            }

            if (apiResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                return new BadRequestResult();
            }

            return new StatusCodeResult(500);
        }

        [Route("api/specs/specifications-by-fundingperiod-and-fundingstream-trimmed/{fundingPeriodId}/{fundingStreamId}")]
        public async Task<IActionResult> GetApprovedSpecificationsTrimmed(string fundingPeriodId, string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            ApiResponse<IEnumerable<SpecificationSummary>> apiResponse = await _specsClient.GetApprovedSpecifications(fundingPeriodId, fundingStreamId);

            if (apiResponse.StatusCode == HttpStatusCode.OK)
            {
                IEnumerable<SpecificationSummary> specificationSummaries = apiResponse.Content;
                IEnumerable<SpecificationSummary> specificationSummariesTrimmed = await _authorizationHelper.SecurityTrimList(User, specificationSummaries, SpecificationActionTypes.CanChooseFunding);

                return Ok(specificationSummaries.OrderBy(c => c.Name).Select(_ => (_, specificationSummariesTrimmed.Any(trimmedSpec => trimmedSpec.Id == _.Id) == true ? true : false)));
            }

            if (apiResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                return new BadRequestResult();
            }

            return new StatusCodeResult(500);
        }

        [Route("api/specs/specification-summary-by-id/{specificationId}")]
        [HttpGet]
        public async Task<IActionResult> GetSpecificationById(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            ApiResponse<SpecificationSummary> apiResponse = await _specificationsApiClient.GetSpecificationSummaryById(specificationId);

            if (apiResponse.StatusCode == HttpStatusCode.OK)
            {
                return Ok(apiResponse.Content);
            }

            if (apiResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                return new BadRequestResult();
            }

            return new StatusCodeResult(500);
        }

        [HttpPost]
        [Route("api/specs/create")]
        public async Task<IActionResult> CreateSpecification([FromBody]CreateSpecificationViewModel viewModel)
        {
            //TODO: Could do with some validation here

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            //var viewModel = JsonConvert.DeserializeObject<CreateSpecificationViewModel>(data);

            var fundingStreamIds = new List<string> { viewModel.FundingStreamId };

            CreateSpecificationModel specification = new CreateSpecificationModel
            {
                Description = viewModel.Description,
                Name = viewModel.Name,
                FundingPeriodId = viewModel.FundingPeriodId,
                FundingStreamIds = fundingStreamIds,
                ProviderVersionId = viewModel.ProviderVersionId
            };


            if (!await _authorizationHelper.DoesUserHavePermission(User, specification.FundingStreamIds, FundingStreamActionTypes.CanCreateSpecification))
            {
                return new ForbidResult();
            }

            ValidatedApiResponse<Specification> result = await _specsClient.CreateSpecification(specification);

            if (result.StatusCode.IsSuccess())
            {
                return new OkObjectResult(result.Content);
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                result.AddValidationResultErrors(ModelState);

                return new BadRequestObjectResult(result.ModelState);
            }
            else
            {
                return new InternalServerErrorResult($"Unable to create specification - result '{result.StatusCode}'");
            }
        }
    }
}
