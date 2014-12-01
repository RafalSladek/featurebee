namespace FeatureBee.FeatureStates
{
    using System.Diagnostics;

    using FeatureBee.WireUp;

    public class InDevelopmentEvaluator : IEvaluateFeatures
    {
        public bool CanEvalute(string name, FeatureDto feature)
        {
            return feature != null && feature.State == "In Development" && !FeatureBeeBuilder.Context.IsDebugMode;
        }

        public bool IsEnabled(string name, FeatureDto feature)
        {
            Logger.Log(TraceEventType.Verbose, "Feature {0} is 'In Development' and debug mode is false", feature.Name);
            return false;
        }
    }
}