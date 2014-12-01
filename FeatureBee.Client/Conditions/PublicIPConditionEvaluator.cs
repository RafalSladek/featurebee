namespace FeatureBee.Conditions
{
    using System;
    using System.Linq;
    using System.Web;

    using FeatureBee.WireUp;

    internal class PublicIPConditionEvaluator : IConditionEvaluator<WebApplicationContext>
    {
        public Func<HttpContextBase> CurrentContext = () => new HttpContextWrapper(HttpContext.Current);

        public string Name
        {
            get { return "public-ip"; }
        }

        public bool IsFulfilled(string[] values)
        {
            var ip = CurrentContext().Request.UserHostAddress;
            return values.Any(x => x.Equals(ip, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}