namespace FeatureBee.WireUp
{
    using System.Collections.Generic;

    using FeatureBee.Conditions;

    internal interface IFeatureBeeContext
    {
        List<IConditionEvaluator> Evaluators { get; set; }
        IFeatureRepository FeatureRepository { get; set; }
        GodModeFeatureCollection GodModeFeatures { get; }
        bool IsDebugMode { get; }
        bool ShowTrayIconOnPages { get; set; }
        string TrafficDistributionCookie { get; set; }
    }
}