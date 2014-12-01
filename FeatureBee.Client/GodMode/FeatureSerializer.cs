namespace FeatureBee.GodMode
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Web;

    using FeatureBee.WireUp;

    static public class FeatureSerializer
    {
        public static void SaveInCookie(HttpContextBase context, GodModeFeatureCollection featureCollection)
        {
            if (context == null || context.Request == null || context.Request.QueryString == null || context.Response == null || context.Request.Url == null)
            {
                return;
            }

            const string persistParamName = "FB_persist";
            const string featureBeeCookieName = "featureBee";

            var shouldPersistParam = context.Request.QueryString[persistParamName];

            if (shouldPersistParam != null 
                && shouldPersistParam.ToLower(CultureInfo.InvariantCulture) == bool.TrueString.ToLower())
            {
                var builder = new StringBuilder();

                builder.Append("#");
                foreach (var feature in featureCollection)
                {
                    builder.Append(feature.Key + "=" + feature.Value.ToString().ToLower() + "#");
                }

                var host = context.Request.Url.Host;
                host = host.Contains(".") ? host.Substring(host.IndexOf(".", StringComparison.Ordinal)) : host;

                context.Response.Cookies.Set(
                    new HttpCookie(featureBeeCookieName)
                    {
                        Value = HttpUtility.UrlEncode(builder.ToString()),
                        Path = "/",
                        HttpOnly = false,
                        Expires = DateTime.Now.AddDays(30),
                        Domain = host
                    }
                );
            }
        }
    }
}
