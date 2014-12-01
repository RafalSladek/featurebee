namespace FeatureBee.FeatureStates
{
    using System.Diagnostics;

    using FeatureBee.WireUp;

    public class IsReleasedEvalutor : IEvaluateFeatures
    {
        public bool CanEvalute(string name, FeatureDto feature)
        {
            return feature != null && feature.State == "Released";
        }

        public bool IsEnabled(string name, FeatureDto feature)
        {
            Logger.Log(TraceEventType.Verbose, "Feature {0} is 'Released'", feature.Name);
            return true;
        }
    }
}