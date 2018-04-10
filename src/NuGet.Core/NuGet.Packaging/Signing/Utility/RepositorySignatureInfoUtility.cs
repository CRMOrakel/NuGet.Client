// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using NuGet.Common;
using NuGet.Packaging.Core;

namespace NuGet.Packaging.Signing
{
    public static class RepositorySignatureInfoUtility
    {
        /// <summary>
        /// Gets SignedPackageVerifierSettings from a given RepositorySignatureInfo. 
        /// </summary>
        /// <param name="repoSignatureInfo">RepositorySignatureInfo to be used.</param>
        /// <param name="fallbackSettings">SignedPackageVerifierSettings to be used if RepositorySignatureInfo is unavailable.</param>
        /// <returns>SignedPackageVerifierSettings based on the RepositorySignatureInfo and SignedPackageVerifierSettings.</returns>
        public static SignedPackageVerifierSettings GetSignedPackageVerifierSettings(
            RepositorySignatureInfo repoSignatureInfo,
            SignedPackageVerifierSettings fallbackSettings)
        {
            if (fallbackSettings == null)
            {
                throw new ArgumentNullException(nameof(fallbackSettings));
            }

            if (repoSignatureInfo == null)
            {
                return fallbackSettings;
            }
            else
            {
                var repositoryAllowList = GetRepositoryAllowList(repoSignatureInfo.RepositoryCertificateInfos);

                // Allow unsigned only if the common settings allow it and repository does not have all packages signed
                var allowUnsigned = fallbackSettings.AllowUnsigned && !repoSignatureInfo.AllRepositorySigned;

                // Allow an empty repository certificate list only if the repository does not have all packages signed
                var allowNoRepositoryCertificateList = !repoSignatureInfo.AllRepositorySigned;

                return new SignedPackageVerifierSettings(
                    allowUnsigned,
                    fallbackSettings.AllowIllegal,
                    fallbackSettings.AllowUntrusted,
                    fallbackSettings.AllowIgnoreTimestamp,
                    fallbackSettings.AllowMultipleTimestamps,
                    fallbackSettings.AllowNoTimestamp,
                    fallbackSettings.AllowUnknownRevocation,
                    allowNoRepositoryCertificateList,
                    fallbackSettings.AllowNoClientCertificateList,
                    fallbackSettings.AlwaysVerifyCountersignature,
                    repositoryAllowList?.AsReadOnly(),
                    fallbackSettings.ClientCertificateList);
            }
        }

        private static List<CertificateHashAllowListEntry> GetRepositoryAllowList(IEnumerable<IRepositoryCertificateInfo> repositoryCertificateInfos)
        {
            List<CertificateHashAllowListEntry> repositoryAllowList = null;

            if (repositoryCertificateInfos != null)
            {
                repositoryAllowList = new List<CertificateHashAllowListEntry>();

                foreach (var certInfo in repositoryCertificateInfos)
                {
                    var verificationTarget = VerificationTarget.Repository;
                    var signaturePlacement = SignaturePlacement.PrimarySignature | SignaturePlacement.Countersignature;

                    AddCertificateFingerprintIntoAllowList(verificationTarget, signaturePlacement, HashAlgorithmName.SHA256, certInfo, repositoryAllowList);
                    AddCertificateFingerprintIntoAllowList(verificationTarget, signaturePlacement, HashAlgorithmName.SHA384, certInfo, repositoryAllowList);
                    AddCertificateFingerprintIntoAllowList(verificationTarget, signaturePlacement, HashAlgorithmName.SHA512, certInfo, repositoryAllowList);
                }
            }

            return repositoryAllowList;
        }

        private static void AddCertificateFingerprintIntoAllowList(
            VerificationTarget target,
            SignaturePlacement placement,
            HashAlgorithmName algorithm,
            IRepositoryCertificateInfo certInfo,
            List<CertificateHashAllowListEntry> allowList)
        {
            var fingerprint = certInfo.Fingerprints[algorithm.ConvertToOidString()];

            if (!string.IsNullOrEmpty(fingerprint))
            {
                allowList.Add(new CertificateHashAllowListEntry(target, placement, fingerprint, algorithm));
            }
        }
    }
}
