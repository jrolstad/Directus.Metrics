using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Directus.Metrics.Features.Aws.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Directus.Metrics.Features.Aws.Test.Services
{
    [TestFixture]
    public class When_notifying_of_a_feature_being_used
    {
        private AmazonSimpleDB _simpleDbClient;
        private string _domainName;
        private readonly List<Exception> _exceptions = new List<Exception>();
        private readonly List<PutAttributesRequest> _putRequestsSent = new List<PutAttributesRequest>();
        private readonly List<CreateDomainRequest> _createDomainRequestsSent = new List<CreateDomainRequest>();
        private SimpleDbFeatureUsageService _service;
        private string _applicationName;
        private string _featureName;
        private string _usageDetails;
        private string _usedBy;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            GivenAFeatureUsageService();

            WhenNotifyingAFeatureHasBeenUsed();
        }

        [Test]
        public void Then_the_domain_is_created()
        {
            // Assert
            Assert.That(_createDomainRequestsSent.Count,Is.EqualTo(1));
            Assert.That(_createDomainRequestsSent[0].DomainName,Is.EqualTo(_domainName));
        }

        [Test]
        public void Then_only_one_request_is_sent()
        {
            // Assert
            Assert.That(_putRequestsSent.Count, Is.EqualTo(1));
        }

        [Test]
        public void Then_the_item_has_a_unique_id()
        {
            // Assert
            Assert.That(_putRequestsSent[0].ItemName, Is.Not.Null);
        }

        [Test]
        public void Then_the_item_is_for_the_application()
        {
            // Assert
            Assert.That(_putRequestsSent[0].Attribute.Single(a=>a.Name == "application").Value, Is.EqualTo(_applicationName));
        }

        [Test]
        public void Then_the_item_is_for_the_feature()
        {
            // Assert
            Assert.That(_putRequestsSent[0].Attribute.Single(a => a.Name == "feature").Value, Is.EqualTo(_featureName));
        }

        [Test]
        public void Then_the_item_contains_the_usage_details()
        {
            // Assert
            Assert.That(_putRequestsSent[0].Attribute.Single(a => a.Name == "detail").Value, Is.EqualTo(_usageDetails));
        }

        [Test]
        public void Then_the_item_contains_the_used_by()
        {
            // Assert
            Assert.That(_putRequestsSent[0].Attribute.Single(a => a.Name == "usedBy").Value, Is.EqualTo(_usedBy));
        }

        [Test]
        public void Then_the_item_contains_the_used_at()
        {
            // Assert
            Assert.That(_putRequestsSent[0].Attribute.Single(a => a.Name == "usedAt").Value, Is.Not.Null);
        }

        private void WhenNotifyingAFeatureHasBeenUsed()
        {
            _applicationName = Guid.NewGuid().ToString();
            _featureName = Guid.NewGuid().ToString();
            _applicationName = Guid.NewGuid().ToString();
            _usageDetails = Guid.NewGuid().ToString();
            _usedBy= Guid.NewGuid().ToString();

            _service.NotifyFeatureUsage(_applicationName, _featureName, _usageDetails, _usedBy);
        }

        private void GivenAFeatureUsageService()
        {
            _simpleDbClient = MockRepository.GenerateStub<AmazonSimpleDB>();
            _simpleDbClient
                .Stub(s => s.PutAttributes(Arg<PutAttributesRequest>.Is.Anything))
                .WhenCalled(( a ) => _putRequestsSent.Add(a.Arguments[0] as PutAttributesRequest))
                .Return(null);
            _simpleDbClient
                .Stub(s => s.CreateDomain(Arg<CreateDomainRequest>.Is.Anything))
                .WhenCalled((a) => _createDomainRequestsSent.Add(a.Arguments[0] as CreateDomainRequest))
                .Return(null);
            
            _domainName = "My Domain";
            Action<Exception> exceptionAction = ( exception ) => _exceptions.Add(exception);

            _service = new SimpleDbFeatureUsageService(exceptionAction, _domainName, _simpleDbClient);
        }
    }
}