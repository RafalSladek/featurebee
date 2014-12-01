namespace FeatureBee
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using FeatureBee.WireUp;

    public class Feature
    {
        private static Func<string, bool> evaluator = new FeatureEvaluator().IsEnabled;

        public string Name { get; set; }
        public string Team { get; set; }
        public string State { get; set; }
        public bool Enabled { get; set; }

        [Obsolete("Use Replace Evaluator instead", false)]
        public static void InjectEvaluator(Func<string, bool> isEnabled)
        {
            ReplaceEvaluator(isEnabled);
        }

        public static void ReplaceEvaluator(Func<string, bool> isEnabled)
        {
            evaluator = featureName =>
            {
                if (!isEnabled(featureName))
                {
                    Logger.Log(TraceEventType.Verbose, "injected evaluator is false");
                    return false;
                }

                Logger.Log(TraceEventType.Verbose, "injected evaluator is true");
                return true;
            };
        }

        public static void InsertEvaluatorBeforeDefault(Func<string, bool> isEnabled)
        {
            evaluator = featureName =>
            {
                if (!isEnabled(featureName))
                {
                    Logger.Log(TraceEventType.Verbose, "injected evaluator is false");
                    return new FeatureEvaluator().IsEnabled(featureName);
                }

                Logger.Log(TraceEventType.Verbose, "injected evaluator is true");
                return true;
            };
        }

        public static void RestoreDefaultEvaluator()
        {
            evaluator = new FeatureEvaluator().IsEnabled;
        }

        public static bool IsEnabled(string featureName)
        {
            return evaluator(featureName);
        }

        public static IEnumerable<Feature> AllFeatures()
        {
            var features = FeatureBeeBuilder.Context.FeatureRepository.GetFeatures();
            return features.Select(feature => new Feature
                                              {
                                                  Name = feature.Name,
                                                  State = feature.State,
                                                  Team = feature.Team,
                                                  Enabled = IsEnabled(feature.Name)
                                              });
        }
    }
}