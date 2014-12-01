namespace FeatureBee.Conditions
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;

    using FeatureBee.WireUp;

    /// <summary>
    ///     Percentage based distribution of traffic based on cookie (Values in form [0-10] or [50-90])";
    /// </summary>
    internal class TrafficDistributionConditionEvaluator : IConditionEvaluator
    {
        private static readonly Regex FormatForRange = new Regex("\\[(?<from>\\d.*?)\\-(?<to>\\d.*?)]", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        public string Name
        {
            get { return "trafficDistribution"; }
        }

        public bool IsFulfilled(string[] values)
        {
            var currentContext = HttpContext.Current;
            if (currentContext == null)
            {
                return false;
            }

            var uniqueGuid = this.ReadExistingUniqueGuid(currentContext) ?? this.SetAndReturnUniqueGuid(currentContext);

            if (uniqueGuid == null)
            {
                return false;
            }

            var distribution = this.ConvertDistributionValue(uniqueGuid.Value);
            return values.Any(value => this.IsInRange(distribution, value));
        }

        private Guid? SetAndReturnUniqueGuid(HttpContext currentContext)
        {
            var uniqueGuid = Guid.NewGuid();
            currentContext.Response.AppendCookie(new HttpCookie(FeatureBeeBuilder.Context.TrafficDistributionCookie, uniqueGuid.ToString("N")));
            return uniqueGuid;
        }

        private Guid? ReadExistingUniqueGuid(HttpContext currentContext)
        {
            var userGuid = currentContext.Request.Cookies[FeatureBeeBuilder.Context.TrafficDistributionCookie];

            Guid result;
            return (userGuid != null && Guid.TryParse(userGuid.Value, out result))
                ? result
                : (Guid?) null;
        }

        private bool IsInRange(int distribution, string value)
        {
            var splittedValue = value.Split('-');
            var from = int.Parse(splittedValue[0]);
            var to = int.Parse(splittedValue[1]);

            return from <= distribution && distribution <= to;
        }

        private int ConvertDistributionValue(Guid guid)
        {
            return Math.Abs(Convert.ToInt32(BitConverter.ToInt64(guid.ToByteArray(), 0)%100)) + 1;
        }
    }
}