namespace FeatureBee.Acceptance.Units.EnabledEvaluators
{
    using FeatureBee.FeatureStates;
    using FeatureBee.WireUp;

    using FluentAssertions;

    using Machine.Fakes;
    using Machine.Specifications;

    public class IsReleasedEvalutorSpecs : WithSubject<IsReleasedEvalutor>
    {
        public class When_Has_A_Relased_Feature
        {
            Because of = () => canEvaluate = Subject.CanEvalute("feature", new FeatureDto { State = "Released" });

            It should_say_it_can_evaluate_this_feature = () => canEvaluate.Should().BeTrue();
        }

        public class When_Has_A_Feature_that_is_not_released
        {
            Because of = () => canEvaluate = Subject.CanEvalute("feature", new FeatureDto { State = "Not Released" });

            It should_say_it_can_evaluate_this_feature = () => canEvaluate.Should().BeFalse();
        }

        public class When_Has_No_Feature
        {
            Because of = () => canEvaluate = Subject.CanEvalute("feature", null);

            It should_say_it_cannot_evaluate_this_feature = () => canEvaluate.Should().BeFalse();
        }

        public class When_Has_A_Relased_Feature_And_Asked_If_Enabled
        {
            Because of = () => isEnabled = Subject.IsEnabled("feature", new FeatureDto { State = "Released" });

            It should_always_return_true = () => isEnabled.Should().BeTrue();
        }

        private static bool isEnabled;
        private static bool canEvaluate;
    }
}
