namespace FeatureBee
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FeatureBee.FeatureStates;
    using FeatureBee.WireUp;

    public class FeatureEvaluator
    {
        public bool IsEnabled(string featureName)
        {
            if (FeatureBeeBuilder.Context == null)
            {
                throw new InvalidOperationException("FeatureBeeBuilder.For[Web|WindowsService].Use[Config]() needs to be called first!");
            }

            var featureEvaluators = new List<IEvaluateFeatures> { new GodModeEvaluator(), new NoSuchFeatureEvaluator(), new InDevelopmentEvaluator(), new IsReleasedEvalutor(), new ConditionBasedEvaluator() };
            
            var features = FeatureBeeBuilder.Context.FeatureRepository.GetFeatures();
            var feature = features.FirstOrDefault(x => string.Equals(x.Name, featureName));
            
            var featureEvaluator = featureEvaluators.FirstOrDefault(_ => _.CanEvalute(featureName, feature));
            return featureEvaluator != null && featureEvaluator.IsEnabled(featureName, feature);
        }
    }
}
