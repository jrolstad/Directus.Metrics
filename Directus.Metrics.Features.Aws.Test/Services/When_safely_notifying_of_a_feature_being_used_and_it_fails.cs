using System;
using System.Collections.Generic;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Directus.Metrics.Features.Aws.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Directus.Metrics.Features.Aws.Test.Services
{
    [TestFixture]
    public class When_safely_notifying_of_a_feature_being_used_and_it_fails
    {
        private AmazonSimpleDB _simpleDbClient;
        private string _domainName;
        private readonly List<Exception> _exceptions = new List<Exception>();
        private SimpleDbFeatureUsageService _service;
        private string _applicationName;
        private string _featureName;
        private string _usageDetails;
        private string _usedBy;
        private Exception _exceptionToThrow;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            GivenAFeatureUsageService();

            WhenNotifyingAFeatureHasBeenUsed();
        }

        [Test]
        public void Then_the_thrown_exception_is_handled()
        {
            // Assert
            Assert.That(_exceptions,Has.Member(_exceptionToThrow));
        }

        [Test]
        public void Then_the_thrown_exception_is_handled_only_once()
        {
            // Assert
            Assert.That(_exceptions, Has.Count.EqualTo(1));
        }

        private void WhenNotifyingAFeatureHasBeenUsed()
        {
            _applicationName = Guid.NewGuid().ToString();
            _featureName = Guid.NewGuid().ToString();
            _applicationName = Guid.NewGuid().ToString();
            _usageDetails = Guid.NewGuid().ToString();
            _usedBy= Guid.NewGuid().ToString();

            _service.NotifyFeatureUsageSafe(_applicationName, _featureName, _usageDetails, _usedBy);
        }

        private void GivenAFeatureUsageService()
        {
            _simpleDbClient = MockRepository.GenerateStub<AmazonSimpleDB>();
            _simpleDbClient
                .Stub(s => s.PutAttributes(Arg<PutAttributesRequest>.Is.Anything))
                .Return(null);

            _exceptionToThrow = new Exception("Something really bad happened");
            _simpleDbClient
                .Stub(s => s.CreateDomain(Arg<CreateDomainRequest>.Is.Anything))
                .Throw(_exceptionToThrow);
            
            _domainName = "My Domain";
            Action<Exception> exceptionAction = exception => _exceptions.Add(exception);

            _service = new SimpleDbFeatureUsageService(exceptionAction, _domainName, _simpleDbClient);
        }
    }
}