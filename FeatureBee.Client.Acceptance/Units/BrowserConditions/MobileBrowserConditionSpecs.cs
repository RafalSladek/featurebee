namespace FeatureBee.Acceptance.Units.BrowserConditions
{
    using System.Web;

    using FeatureBee.Conditions;

    using FluentAssertions;

    using Machine.Fakes;
    using Machine.Specifications;

    public class MobileBrowserConditionSpecs : WithSubject<BrowserConditionEvaluator>
    {
        public class When_Visiting_A_Feature_With_A_Mobile_Browser_And_The_Feature_Should_Be_Shown
        {
            private Establish context = () => Subject.CurrentContext = () =>
            {
                var httpContextBase = An<HttpContextBase>();
                var httpRequestBase = An<HttpRequestBase>();
                var httpBrowserCapabilitiesBase = An<HttpBrowserCapabilitiesBase>();
                httpBrowserCapabilitiesBase.WhenToldTo(_ => _.Browser).Return("Mozilla/5.0 (Linux; U; Android-4.0.3; en-us; Galaxy Nexus Build/IML74K) AppleWebKit/535.7 (KHTML, like Gecko) CrMo/16.0.912.75 Mobile Safari/535.7");
                httpRequestBase.WhenToldTo(_ => _.Browser).Return(httpBrowserCapabilitiesBase);
                httpContextBase.WhenToldTo(_ => _.Request).Return(httpRequestBase);
                return httpContextBase;
            };

            Because of = () => result = Subject.IsFulfilled(new[] { "Mobile" });

            It should_return_feature_enabled = () => result.Should().Be(true);
        }

        public class When_Visiting_A_Feature_With_A_Mobile_Browser_And_The_Feature_Should_Be_Hidden
        {
            private Establish context = () => Subject.CurrentContext = () =>
            {
                var httpContextBase = An<HttpContextBase>();
                var httpRequestBase = An<HttpRequestBase>();
                var httpBrowserCapabilitiesBase = An<HttpBrowserCapabilitiesBase>();
                httpBrowserCapabilitiesBase.WhenToldTo(_ => _.Browser).Return("Mozilla/5.0 (Linux; U; Android-4.0.3; en-us; Galaxy Nexus Build/IML74K) AppleWebKit/535.7 (KHTML, like Gecko) CrMo/16.0.912.75 Mobile Safari/535.7");
                httpRequestBase.WhenToldTo(_ => _.Browser).Return(httpBrowserCapabilitiesBase);
                httpContextBase.WhenToldTo(_ => _.Request).Return(httpRequestBase);
                return httpContextBase;
            };

            Because of = () => result = Subject.IsFulfilled(new[] { "NOT:Mobile" });

            It should_return_feature_enabled = () => result.Should().Be(false);
        }

        private static bool result = false;
    }
}
