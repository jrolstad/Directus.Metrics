using System;

namespace Directus.Metrics.Features.Services
{
    /// <summary>
    /// Service for tracking who subscribes to a given feature
    /// </summary>
    public interface IFeatureSubscriptionService
    {
        /// <summary>
        /// Notifies subscription to a given feature.  If it fails for any reason, logs the error and raises the exception.
        /// </summary>
        /// <param name="applicationName">Name of the application the feature is in</param>
        /// <param name="featureName">Name of the feature</param>
        /// <param name="subscriber">Who subscribed to the feature</param>
        /// <param name="subscribedAt">When it was subscribed at.  If null, defaults to the current date/time</param>
        void NotifyFeatureSubscription(string applicationName, string featureName, string subscriber, DateTime? subscribedAt = null);

        /// <summary>
        /// Notifies subscription to a given feature.  If it fails for any reason, logs the error but does not raise the exception.
        /// </summary>
        /// <param name="applicationName">Name of the application the feature is in</param>
        /// <param name="featureName">Name of the feature</param>
        /// <param name="subscriber">Who subscribed to the feature</param>
        /// <param name="subscribedAt">When it was subscribed at.  If null, defaults to the current date/time</param>
        void NotifyFeatureSubscriptionSafe(string applicationName, string featureName, string subscriber, DateTime? subscribedAt = null);

        /// <summary>
        /// Notifies subscription to a given feature asynchronously.  If it fails for any reason, logs the error but does not raise the exception.
        /// </summary>
        /// <param name="applicationName">Name of the application the feature is in</param>
        /// <param name="featureName">Name of the feature</param>
        /// <param name="subscriber">Who subscribed to the feature</param>
        /// <param name="subscribedAt">When it was subscribed at.  If null, defaults to the current date/time</param>
        void NotifyFeatureSubscriptionAsync(string applicationName, string featureName, string subscriber, DateTime? subscribedAt = null);
    }
}