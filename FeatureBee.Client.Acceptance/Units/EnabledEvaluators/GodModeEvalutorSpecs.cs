namespace FeatureBee.Acceptance.Units.EnabledEvaluators
{
    using FeatureBee.FeatureStates;
    using FeatureBee.WireUp;

    using FluentAssertions;

    using Machine.Fakes;
    using Machine.Specifications;

    public class GodModeEvalutorSpecs : WithSubject<GodModeEvaluator>
    {
        public class When_Has_A_Feature_In_God_Mode
        {
            Establish context = () => Subject.AddGodModeFeatures(new GodModeFeatureCollection { { Feature.Name, true } });

            Because of = () => canEvaluate = Subject.CanEvalute(Feature.Name, Feature);

            It should_say_it_can_evaluate_this_feature = () => canEvaluate.Should().BeTrue();
        }

        public class When_Has_No_Feature_In_God_Mode
        {
            Because of = () => canEvaluate = Subject.CanEvalute(Feature.Name, Feature);

            It should_say_it_can_evaluate_this_feature = () => canEvaluate.Should().BeFalse();

        }

        public class When_Has_A_Feature_In_God_Mode_And_Asked_If_Enabled
        {
            public class When_The_GodMode_Says_Its_On
            {
                Establish context = () => Subject.AddGodModeFeatures(new GodModeFeatureCollection { { Feature.Name, true } });
                
                Because of = () => isEnabled = Subject.IsEnabled(Feature.Name, Feature);

                It should_return_true = () => isEnabled.Should().BeTrue();
            }

            public class When_The_GodMode_Says_Its_Off
            {
                Establish context = () => Subject.AddGodModeFeatures(new GodModeFeatureCollection { { Feature.Name, false } });
                Because of = () => isEnabled = Subject.IsEnabled(Feature.Name, Feature);

                It should_return_false = () => isEnabled.Should().BeFalse();
            }

        }

        private static bool isEnabled;
        private static bool canEvaluate;
        private static readonly FeatureDto Feature = new FeatureDto { Name = "feature" };
    }
}
