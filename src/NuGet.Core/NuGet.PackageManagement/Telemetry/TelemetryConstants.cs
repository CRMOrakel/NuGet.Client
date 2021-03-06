// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace NuGet.PackageManagement
{
    /// <summary>
    /// This class contains telemetry events name and properties name.
    /// </summary>
    public static class TelemetryConstants
    {
        // nuget action step event data
        public static readonly string PreviewBuildIntegratedStepName = "Preview build integrated action for project {0} time";
        public static readonly string GatherDependencyStepName = "Gather dependency action for project {0} time";
        public static readonly string ResolveDependencyStepName = "Resolve dependency action for project {0} time";
        public static readonly string ResolvedActionsStepName = "Resolved nuget actions for project {0} time";
        public static readonly string ExecuteActionStepName = "Executing nuget actions for project {0} time";
    }
}
