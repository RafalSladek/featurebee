namespace FeatureBee.FeatureStates
{
    using System.Diagnostics;
    using System.Linq;

    using FeatureBee.WireUp;

    public class ConditionBasedEvaluator : IEvaluateFeatures
    {
        public bool CanEvalute(string name, FeatureDto feature)
        {
            return feature != null && feature.State == "Under Test";
        }

        public bool IsEnabled(string name, FeatureDto feature)
        {
            var evaluators = FeatureBeeBuilder.Context.Evaluators;
            var isFulfilled = false;
            foreach (var condition in feature.Conditions)
            {
                var evaluator = evaluators.FirstOrDefault(x => string.Equals(x.Name, condition.Type));
                if (evaluator == null)
                {
                    return false;
                }

                isFulfilled = evaluator.IsFulfilled(condition.Values.ToArray());
                if (isFulfilled)
                {
                    continue;
                }

                Logger.Log(TraceEventType.Verbose, "Feature {0} does not fulfill condition {1} of type {2}", feature.Name, condition.Values, condition.Type);
                return false;
            }

            return isFulfilled;
        }
    }
}