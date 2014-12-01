namespace FeatureBee.FeatureStates
{
    using FeatureBee.WireUp;

    internal interface IEvaluateFeatures
    {
        bool CanEvalute(string name, FeatureDto feature);

        bool IsEnabled(string name, FeatureDto feature);
    }
}