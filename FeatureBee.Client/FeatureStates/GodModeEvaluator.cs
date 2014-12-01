namespace FeatureBee.FeatureStates
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using FeatureBee.WireUp;

    public class GodModeEvaluator : IEvaluateFeatures
    {
        private readonly GodModeFeatureCollection godModeFeatures;

        public GodModeEvaluator()
            : this(FeatureBeeBuilder.Context.GodModeFeatures)
        {
        }

        public GodModeEvaluator(GodModeFeatureCollection godModeFeatures)
        {
            this.godModeFeatures = godModeFeatures ?? new GodModeFeatureCollection();
        }

        public void AddGodModeFeatures(GodModeFeatureCollection godModeFeatureCollection)
        {
            this.godModeFeatures.Combine(godModeFeatureCollection);
        }

        public bool CanEvalute(string featureName, FeatureDto feature)
        {
            return this.godModeFeatures.Any(x => x.Key.Equals(featureName, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool IsEnabled(string featureName, FeatureDto feature)
        {
            var godMode = this.godModeFeatures.First(x => x.Key.Equals(featureName, StringComparison.InvariantCultureIgnoreCase));
            Logger.Log(TraceEventType.Verbose, "Feature {0} overwritten by GodMode. Value is {1}", featureName, godMode.Value);

            return godMode.Value;
        }
    }
}