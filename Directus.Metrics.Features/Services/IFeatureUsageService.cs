using System;

namespace Directus.Metrics.Features.Services
{
    public interface IFeatureUsageService
    {
        /// <summary>
        /// Logs usage of a given feature.  If it fails for any reason, logs the error and raises the exception.
        /// </summary>
        /// <param name="applicationName">Name of the application the feature is in</param>
        /// <param name="featureName">Name of the feature</param>
        /// <param name="usedBy">Who used the feature</param>
        /// <param name="usedAt">When it was used at</param>
        /// <param name="usageDetails">Details of the usage</param>
        void NotifyFeatureUsage(string applicationName, string featureName, string usageDetails = null, string usedBy = null, DateTime? usedAt = null);

        /// <summary>
        /// Logs usage of a given feature.  If it fails for any reason, logs the error but does not raise the exception.
        /// </summary>
        /// <param name="applicationName">Name of the application the feature is in</param>
        /// <param name="featureName">Name of the feature</param>
        /// <param name="usedBy">Who used the feature</param>
        /// <param name="usedAt">When it was used at</param>
        /// <param name="usageDetails">Details of the usage</param>
        void NotifyFeatureUsageSafe(string applicationName, string featureName, string usageDetails = null, string usedBy = null, DateTime? usedAt = null);

        /// <summary>
        /// Logs usage of a given feature asynchronously.  If it fails for any reason, logs the error but does not raise the exception.
        /// </summary>
        /// <param name="applicationName">Name of the application the feature is in</param>
        /// <param name="featureName">Name of the feature</param>
        /// <param name="usedBy">Who used the feature</param>
        /// <param name="usedAt">When it was used at</param>
        /// <param name="usageDetails">Details of the usage</param>
        void NotifyFeatureUsageAsync(string applicationName, string featureName, string usageDetails = null, string usedBy = null, DateTime? usedAt = null);
    }
}