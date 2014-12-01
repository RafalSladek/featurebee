namespace FeatureBee.Acceptance.Units.EnabledEvaluators
{
    using FeatureBee.FeatureStates;
    using FeatureBee.WireUp;

    using FluentAssertions;

    using Machine.Fakes;
    using Machine.Specifications;

    public class NoSuchFeatureEvalutorSpecs : WithSubject<NoSuchFeatureEvaluator>
    {
        public class When_Has_A_Feature
        {
            Because of = () => canEvaluate = Subject.CanEvalute("feature", new FeatureDto());

            It should_not_say_it_can_evaluate_this_feature = () => canEvaluate.Should().BeFalse();
        }

        public class When_Has_No_Feature
        {
            Because of = () => canEvaluate = Subject.CanEvalute("feature", null);

            It should_say_it_can_evaluate_this_feature = () => canEvaluate.Should().BeTrue();
        }

        public class When_Has_No_Feature_And_Asked_If_Enabled
        {
            Because of = () => isEnabled = Subject.IsEnabled("feature", null);

            It should_always_return_false = () => isEnabled.Should().BeFalse();
        }

        private static bool isEnabled;
        private static bool canEvaluate;
    }
}
