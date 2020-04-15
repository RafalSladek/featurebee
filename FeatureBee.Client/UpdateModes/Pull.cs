namespace FeatureBee.UpdateModes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Runtime.Caching;
    using System.Web.Script.Serialization;

    using FeatureBee.UpdateModes.FeatureCache;
    using FeatureBee.WireUp;

    internal class Pull : IFeatureRepository
    {
        private static readonly TimeLimitedInMemoryFeatureBeeCache MemCache = new TimeLimitedInMemoryFeatureBeeCache(TimeSpan.FromMinutes(1));
        private readonly Uri featuresUri;
        private readonly HttpClient httpClient;

        private static bool disposing = false;

        public Pull(string url, bool withRefresh = true)
        {
            httpClient = new HttpClient();
            //featuresUri = new Uri(string.Format("{0}/api/features", url));
            featuresUri = new Uri(url);

            if (withRefresh)
            {
                RefreshCache();
            }
        }

        public List<FeatureDto> GetFeatures()
        {
            List<FeatureDto> featureDtos;
            if (MemCache.TryGetValue("FeatureBee.Features", out featureDtos))
            {
                return featureDtos;
            }

            this.RefreshCache();
            if (!MemCache.TryGetValue("FeatureBee.Features", out featureDtos))
            {
                throw new Exception("Features could not be pulled!");
            }

            return featureDtos;
        }

        private void RefreshCache()
        {
            var features = PullFeatures();

            if (!disposing)
            {
                MemCache.SetValue("FeatureBee.Features", features, this.RefreshCache);
            }
        }

        public List<FeatureDto> PullFeatures()
        {
            var features = new List<FeatureDto>();
            try
            {
                Logger.Log(TraceEventType.Verbose, "Pull features...");

                var task = httpClient.GetStringAsync(featuresUri);
                task.Wait(10000);

                if (task.IsCompleted)
                {
                    var result = task.Result;
                    features = Deserialize(result);

                    Logger.Log(TraceEventType.Verbose, "Pulled features: " + result);
                }
                else
                {
                    Logger.Log(TraceEventType.Error, "Pull features did not complete. Status: {0}, Exception: {1}", task.Status, task.Exception);
                }
            }
            catch (Exception exception)
            {
                Logger.Log(TraceEventType.Error, "Failed to load features from url {0}. Exception: {1}", featuresUri, exception);
            }

            return features;
        }

        private List<FeatureDto> Deserialize(string input)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<List<FeatureDto>>(input);
        }

        public void Dispose()
        {
            disposing = true;
            MemCache.Dispose();
        }
    }
}