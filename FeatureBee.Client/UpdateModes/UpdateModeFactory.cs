namespace FeatureBee.UpdateModes
{
    using System;

    using FeatureBee.WireUp;

    static class UpdateModeFactory
    {
        public static IFeatureRepository Get(UpdateMode updateMode, string url)
        {
            switch (updateMode)
            {
                case UpdateMode.Pull:
                    return new Pull(url);
               // case UpdateMode.Push:
               //     return new Push(url);
            }

            throw new Exception("Invalid update mode specified");
        }
    }
}
