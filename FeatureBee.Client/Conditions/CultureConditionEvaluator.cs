namespace FeatureBee.Conditions
{
    using System;
    using System.Linq;
    using System.Threading;

    internal class CultureConditionEvaluator : IConditionEvaluator
    {
        public string Name
        {
            get { return "culture"; }
        }

        public bool IsFulfilled(string[] values)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture.Name;
            return values.Any(x => x.Equals(currentCulture, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}