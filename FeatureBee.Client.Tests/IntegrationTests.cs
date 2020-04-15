using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureBee.Client.Tests
{
    using System.Diagnostics;

    using FeatureBee.ConfigSection;
    using FeatureBee.WireUp;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class IntegrationTests
    {
        [SetUp]
        public void Setup()
        {
            FeatureBeeBuilder
                .ForWindowsService()
                .UseConfig()
                .LogTo(Logger())
                .Build();
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void When_checking_if_test_featurebee_is_deployed_to_s3_bucket()
        {
            Assert.IsTrue(Feature.IsEnabled("TEST-SIMPLE-FETUREBEE"));
        }


        private static Action<TraceEventType, string> Logger()
        {
            return (eventType, message) => Trace.WriteLine(eventType + ": " + message);
        }
    }
}
