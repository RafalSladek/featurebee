using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using FeatureBee.GodMode;


namespace FeatureBee.WireUp
{
    using FeatureBee.Conditions;

    internal class WebApplicationContext : IFeatureBeeContext
    {
        private readonly Func<HttpContextBase> _httpContextFunc;

        public WebApplicationContext(Func<HttpContextBase> httpContextFunc)
        {
            _httpContextFunc = httpContextFunc;
        }

        public List<IConditionEvaluator> Evaluators { get; set; }
        public IFeatureRepository FeatureRepository { get; set; }
        public bool ShowTrayIconOnPages { get; set; }
        public string TrafficDistributionCookie { get; set; }

        public bool IsDebugMode
        {
            get { return _httpContextFunc.Invoke().IsDebuggingEnabled; }
        }

        public GodModeFeatureCollection GodModeFeatures
        {
            get
            {
                var httpContext = _httpContextFunc.Invoke();
                var request = httpContext.Request;
                var response = httpContext.Response;
                var parser = new GodModeFeatureStateEvaluator();
                var strategies = new List<ICanGetGodModeStates>() { 
                    new GetGodModeFeaturesFromHttpHeader(parser), 
                    new GetGodModeFeaturesFromCookies(parser),
                    new GetGodModeFeaturesFromQueryString(parser)
                };

                var collection = new GodModeFeatureCollection();
                foreach (var strategy in strategies)
                {
                    collection.Combine(strategy.GetGodModeFeatures(request));
                }

                FeatureSerializer.SaveInCookie(httpContext, collection);

                return collection;
            }
        }
    }
}