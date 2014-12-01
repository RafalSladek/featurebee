namespace FeatureBee.Acceptance.Units.HttpHandlerRouting.GodMode
{
    using System;
    using System.Collections.Specialized;
    using System.Web;

    using FeatureBee.GodMode;
    using FeatureBee.WireUp;

    using Machine.Fakes;
    using Machine.Specifications;

    using FluentAssertions;

    class FeatureSerializerSpecs
    {
        public class When_uses_requeires_persistence_of_passed_features : WithFakes
        {
            private static HttpContextBase httpContextBase;

            private static readonly GodModeFeatureCollection featureCollection = new GodModeFeatureCollection()
            {
                {"featureA", true}, {"featureB", false}, 
            };

            Establish that = () =>
            {
                httpContextBase = An<HttpContextBase>();
                var httpRequestBase = An<HttpRequestBase>();
                var httpResponseBase = An<HttpResponseBase>();

                httpRequestBase.WhenToldTo(_ => _.Url).Return(new Uri("http://www.somedomain.com"));
                httpRequestBase.WhenToldTo(_ => _.QueryString).Return(new NameValueCollection{{"FB_persist", "true"}});

                httpResponseBase.WhenToldTo(_ => _.Cookies).Return(new HttpCookieCollection());

                httpContextBase.WhenToldTo(_ => _.Request).Return(httpRequestBase);
                httpContextBase.WhenToldTo(_ => _.Response).Return(httpResponseBase);
            };

            Because of = () => FeatureSerializer.SaveInCookie(httpContextBase, featureCollection);

            private It Should_set_the_features_in_cookie_response = () =>
            {
                var cookie = httpContextBase.Response.Cookies["featureBee"];
                cookie.Should().NotBeNull();

                cookie.Value.Should().Be("%23featureA%3dtrue%23featureB%3dfalse%23");
            };
        }
    }
}
