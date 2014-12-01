namespace FeatureBee.FeatureStates
{
    using System.Diagnostics;

    using FeatureBee.WireUp;

    public class NoSuchFeatureEvaluator : IEvaluateFeatures
    {
        public bool CanEvalute(string name, FeatureDto feature)
        {
            return feature == null;
        }

        public bool IsEnabled(string name, FeatureDto feature)
        {
            Logger.Log(TraceEventType.Verbose, "Feature {0} does not exist", name);
            return false;
        }
    }
}