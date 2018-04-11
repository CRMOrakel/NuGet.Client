// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NuGet.Versioning;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Xunit;

using SdkResolverContextBase = Microsoft.Build.Framework.SdkResolverContext;

namespace Microsoft.Build.NuGetSdkResolver.Test
{
    public class NuGetSdkResolver_Tests
    {
        [Fact]
        public void TryGetNuGetVersionForSdkGetsVersionFromGlobalJson()
        {
            var expectedVersions = new Dictionary<string, string>
            {
                {"foo", "5.11.77"},
                {"bar", "2.0.0"}
            };

            using (var testEnvironment = TestEnvironment.Create())
            {
                var projectWithFiles = testEnvironment.CreateTestProjectWithFiles("", relativePathFromRootToProject: @"a\b\c");

                GlobalJsonReaderTests.WriteGlobalJson(projectWithFiles.TestRoot, expectedVersions);

                var context = new MockSdkResolverContext(projectWithFiles.ProjectFile);

                VerifyTryGetNuGetVersionForSdk(
                    version: null,
                    expectedVersion: NuGetVersion.Parse(expectedVersions["foo"]),
                    context: context);
            }
        }

        [Fact]
        public void TryGetNuGetVersionForSdkGetsVersionFromState()
        {
            var context = new MockSdkResolverContext("foo.proj")
            {
                State = new Dictionary<string, string>
                {
                    {"foo", "1.2.3"}
                }
            };

            VerifyTryGetNuGetVersionForSdk(
                version: null,
                expectedVersion: NuGetVersion.Parse("1.2.3"),
                context: context);
        }

        [Fact]
        public void TryGetNuGetVersionForSdkInvalidVersion()
        {
            VerifyTryGetNuGetVersionForSdk(
                version: "abc",
                expectedVersion: null);
        }

        [Fact]
        public void TryGetNuGetVersionForSdkInvalidVersionInGlobalJson()
        {
            var context = new MockSdkResolverContext("foo.proj")
            {
                State = new Dictionary<string, string>
                {
                    {"foo", "abc"}
                }
            };

            VerifyTryGetNuGetVersionForSdk(
                version: "abc",
                expectedVersion: null,
                context: context);
        }

        [Fact]
        public void TryGetNuGetVersionForSdkSucceeds()
        {
            VerifyTryGetNuGetVersionForSdk(
                version: "3.2.1",
                expectedVersion: NuGetVersion.Parse("3.2.1"));
        }

        [Fact]
        public void TryGetNuGetVersionNoVersionSpecified()
        {
            var context = new MockSdkResolverContext("foo.proj")
            {
                State = new Dictionary<string, string>()
            };

            VerifyTryGetNuGetVersionForSdk(
                version: null,
                expectedVersion: null,
                context: context);
        }

        private void VerifyTryGetNuGetVersionForSdk(string version, NuGetVersion expectedVersion, SdkResolverContextBase context = null)
        {
            var result = NuGetSdkResolver.TryGetNuGetVersionForSdk("foo", version, context, out var parsedVersion);

            if (expectedVersion != null)
            {
                result.Should().BeTrue();

                parsedVersion.Should().NotBeNull();

                parsedVersion.Should().Be(expectedVersion);
            }
            else
            {
                result.Should().BeFalse();

                parsedVersion.Should().BeNull();
            }
        }
    }
}
