using System;
using System.Threading.Tasks;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Directus.Metrics.Features.Services;

namespace Directus.Metrics.Features.Aws.Services
{
    public class SimpleDbFeatureSubscriptionService:IFeatureSubscriptionService
    {
        private readonly Action<Exception> _onFailure;
        private readonly string _domainName;
        private readonly AmazonSimpleDB _simpleDbClient;
        private bool _isDomainCreated;

        /// <summary>
        /// Constructor with arguments
        /// </summary>
        /// <param name="onFailure">What to do in the catch clause when an exception occurs in the NotifyFeatureUsageSafe method</param>
        /// <param name="domainName">Domain to log feature usage to</param>
        /// <param name="simpleDbClient">SimpleDb client to use for persistence</param>
        public SimpleDbFeatureSubscriptionService(Action<Exception> onFailure, string domainName, AmazonSimpleDB simpleDbClient)
        {
            _onFailure = onFailure;
            _domainName = domainName;
            _simpleDbClient = simpleDbClient;
        }

        /// <summary>
        /// Notifies subscription to a given feature.  If it fails for any reason, logs the error and raises the exception.
        /// </summary>
        /// <param name="applicationName">Name of the application the feature is in</param>
        /// <param name="featureName">Name of the feature</param>
        /// <param name="subscriber">Who subscribed to the feature</param>
        /// <param name="subscribedAt">When it was subscribed at.  If null, defaults to the current date/time</param>
        public void NotifyFeatureSubscription( string applicationName, string featureName, string subscriber, DateTime? subscribedAt = new DateTime?() )
        {
            // Make sure the domain exists
            CreateDomain(_domainName);

            // Create the request
            var request = new PutAttributesRequest()
                .WithDomainName(_domainName)
                .WithItemName(Guid.NewGuid().ToString())
                .WithAttribute(new[]
                                   {
                                       new ReplaceableAttribute().WithName("application").WithValue(applicationName ?? ""),
                                       new ReplaceableAttribute().WithName("feature").WithValue(featureName ?? ""),
                                       new ReplaceableAttribute().WithName("subscriber").WithValue(subscriber ?? ""),
                                       new ReplaceableAttribute().WithName("subscribedAt").WithValue((subscribedAt ?? DateTime.Now).ToString())
                                   });

            // Send the request
            _simpleDbClient.PutAttributes(request);
        }

        /// <summary>
        /// Notifies subscription to a given feature.  If it fails for any reason, logs the error but does not raise the exception.
        /// </summary>
        /// <param name="applicationName">Name of the application the feature is in</param>
        /// <param name="featureName">Name of the feature</param>
        /// <param name="subscriber">Who subscribed to the feature</param>
        /// <param name="subscribedAt">When it was subscribed at.  If null, defaults to the current date/time</param>
        public void NotifyFeatureSubscriptionSafe( string applicationName, string featureName, string subscriber, DateTime? subscribedAt = new DateTime?() )
        {
            try
            {
                NotifyFeatureSubscription(applicationName, featureName, subscriber, subscribedAt);
            }
            catch (Exception exception)
            {
                _onFailure(exception);
            }
        }

        /// <summary>
        /// Notifies subscription to a given feature asynchronously.  If it fails for any reason, logs the error but does not raise the exception.
        /// </summary>
        /// <param name="applicationName">Name of the application the feature is in</param>
        /// <param name="featureName">Name of the feature</param>
        /// <param name="subscriber">Who subscribed to the feature</param>
        /// <param name="subscribedAt">When it was subscribed at.  If null, defaults to the current date/time</param>
        public void NotifyFeatureSubscriptionAsync( string applicationName, string featureName, string subscriber, DateTime? subscribedAt = new DateTime?() )
        {
            var task = new Task(() => this.NotifyFeatureSubscriptionSafe(applicationName, featureName, subscriber, subscribedAt));
            task.Start();
        }

        /// <summary>
        /// Creates the given domain
        /// </summary>
        /// <param name="domain"></param>
        private void CreateDomain(string domain)
        {
            if (!_isDomainCreated)
            {
                var request = new CreateDomainRequest()
                    .WithDomainName(domain);
                _simpleDbClient.CreateDomain(request);

                _isDomainCreated = true;
            }
        }
    }
}