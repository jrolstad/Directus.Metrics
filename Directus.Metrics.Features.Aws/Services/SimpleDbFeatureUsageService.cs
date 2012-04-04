using System;
using System.Threading.Tasks;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Directus.Metrics.Features.Services;

namespace Directus.Metrics.Features.Aws.Services
{
    public class SimpleDbFeatureUsageService:IFeatureUsageService
    {
        private readonly Action<Exception> _onFailure;
        private readonly string _domainName;
        private bool _isDomainCreated = false;
        private readonly AmazonSimpleDB _simpleDbClient;

        /// <summary>
        /// Constructor with arguments
        /// </summary>
        /// <param name="onFailure">What to do in the catch clause when an exception occurs in the NotifyFeatureUsageSafe method</param>
        /// <param name="domainName">Domain to log feature usage to</param>
        /// <param name="simpleDbClient">SimpleDb Client to do the work</param>
        public SimpleDbFeatureUsageService(Action<Exception> onFailure, string domainName, AmazonSimpleDB simpleDbClient)
        {
            _onFailure = onFailure;
            _domainName = domainName;
            _simpleDbClient = simpleDbClient;
        }

        /// <summary>
        /// Logs usage of a given feature.  If it fails for any reason, logs the error and raises the exception.
        /// </summary>
        /// <param name="applicationName">Name of the application the feature is in</param>
        /// <param name="featureName">Name of the feature</param>
        /// <param name="usedBy">Who used the feature</param>
        /// <param name="usedAt">When it was used at</param>
        /// <param name="usageDetails">Details of the usage</param>
        public void NotifyFeatureUsage( string applicationName, string featureName, string usageDetails = null, string usedBy = null, DateTime? usedAt = new DateTime?() )
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
                                       new ReplaceableAttribute().WithName("detail").WithValue(usageDetails ?? ""),
                                       new ReplaceableAttribute().WithName("usedBy").WithValue(usedBy ?? ""),
                                       new ReplaceableAttribute().WithName("usedAt").WithValue((usedAt ?? DateTime.Now).ToString())
                                   });

            // Send the request
            _simpleDbClient.PutAttributes(request);
        }

        /// <summary>
        /// Logs usage of a given feature.  If it fails for any reason, logs the error but does not raise the exception.
        /// </summary>
        /// <param name="applicationName">Name of the application the feature is in</param>
        /// <param name="featureName">Name of the feature</param>
        /// <param name="usedBy">Who used the feature</param>
        /// <param name="usedAt">When it was used at</param>
        /// <param name="usageDetails">Details of the usage</param>
        public void NotifyFeatureUsageSafe( string applicationName, string featureName, string usageDetails = null, string usedBy = null, DateTime? usedAt = new DateTime?() )
        {
            try
            {
                NotifyFeatureUsage(applicationName, featureName, usageDetails, usedBy, usedAt);
            }
            catch(Exception exception)
            {
                _onFailure(exception);
            }
        }

        /// <summary>
        /// Logs usage of a given feature asynchronously.  If it fails for any reason, logs the error but does not raise the exception.
        /// </summary>
        /// <param name="applicationName">Name of the application the feature is in</param>
        /// <param name="featureName">Name of the feature</param>
        /// <param name="usedBy">Who used the feature</param>
        /// <param name="usedAt">When it was used at</param>
        /// <param name="usageDetails">Details of the usage</param>
        public void NotifyFeatureUsageAsync( string applicationName, string featureName, string usageDetails = null, string usedBy = null, DateTime? usedAt = new DateTime?() )
        {
            var task = new Task(() => this.NotifyFeatureUsageSafe(applicationName, featureName, usageDetails, usedBy, usedAt));
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