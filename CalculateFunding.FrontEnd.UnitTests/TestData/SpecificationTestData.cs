﻿// <copyright file="SpecificationTestData.cs" company="Department for Education">
// Copyright (c) Department for Education. All rights reserved.
// </copyright>

namespace CalculateFunding.Frontend.TestData
{
    using System.Collections.Generic;
    using CalculateFunding.Frontend.Clients.CommonModels;
    using CalculateFunding.Frontend.Clients.SpecsClient.Models;

    public static class SpecificationTestData
    {
        public static IEnumerable<Specification> Data()
        {
            return new[]
            {
                new Specification
                {
                    AcademicYear = new Reference(),
                    FundingStream = new Reference(),
                    Description = "test",
                    Policies = new List<Policy>()
                },
                new Specification
                {
                    AcademicYear = new Reference(),
                    FundingStream = new Reference(),
                    Description = "test",
                    Policies = new List<Policy>()
                },
                new Specification
                {
                    AcademicYear = new Reference(),
                    FundingStream = new Reference(),
                    Description = "test",
                    Policies = new List<Policy>()
                }
            };
        }
    }
}