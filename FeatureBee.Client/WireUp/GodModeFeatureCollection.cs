namespace FeatureBee.WireUp
{
    using System.Collections.Generic;
    using System.Linq;

    public class GodModeFeatureCollection : Dictionary<string, bool>
    {
        public GodModeFeatureCollection Combine(GodModeFeatureCollection with)
        {
            foreach (var key in with.Keys.Where(key => !this.ContainsKey(key)))
            {
                this.Add(key, with[key]);
            }

            return this;
        }
    }
}