namespace FeatureBee.Acceptance.Units.EnabledEvaluators
{
    using System.Collections.Generic;
    using System.Web;

    using FeatureBee.Conditions;
    using FeatureBee.FeatureStates;
    using FeatureBee.WireUp;

    using FluentAssertions;

    using Machine.Fakes;
    using Machine.Specifications;

    using Moq;

    using It = Machine.Specifications.It;

    public class ConditionBasedEvaluatorSpecs : WithSubject<ConditionBasedEvaluator>
    {
        private static readonly HttpContextBase HttpContextMock = HttpFakes.FakeHttpContext();

        public class When_Has_A_Feature_That_is_released_under_conditions
        {
            Because of = () => canEvaluate = Subject.CanEvalute("feature", new FeatureDto { State = "Under Test" });

            It should_say_it_can_evaluate_this_feature = () => canEvaluate.Should().BeTrue();
        }

        public class When_Has_A_Feature_that_is_not_released_under_conditions
        {
            Because of = () => canEvaluate = Subject.CanEvalute("feature", new FeatureDto { State = "Not Released" });

            It should_say_it_can_evaluate_this_feature = () => canEvaluate.Should().BeFalse();
        }

        public class When_Has_No_Feature
        {
            Because of = () => canEvaluate = Subject.CanEvalute("feature", null);

            It should_say_it_cannot_evaluate_this_feature = () => canEvaluate.Should().BeFalse();
        }

        public class When_Has_A_Feature_That_is_released_under_conditions_And_Asked_If_Enabled
        {
            Establish context = () =>
            {
                conditionEvaluator = An<IConditionEvaluator>();
                conditionEvaluator.WhenToldTo(_ => _.Name).Return("condition");
                featureRepository = An<IFeatureRepository>();
            };

            Because of = () => isEnabled = Subject.IsEnabled("feature", new FeatureDto { State = "Under Test", Conditions = new List<ConditionDto> { new ConditionDto { Type = "condition" }}});
            
            public class When_The_Condition_Is_Fullfilled
            {
                Establish context =
                    () =>
                    {
                        conditionEvaluator.WhenToldTo(_ => _.IsFulfilled(Param<string[]>.IsAnything)).Return(true);
                        FeatureBeeBuilder.ForWebApp(() => HttpContextMock)
                            .Use(featureRepository, new List<IConditionEvaluator> { conditionEvaluator })
                            .Build();
                    };

                It should_return_false = () => isEnabled.Should().BeTrue();
            }

            public class When_The_Condition_Is_Not_Fullfilled
            {
                Establish context =
                    () =>
                    {
                        conditionEvaluator.WhenToldTo(_ => _.IsFulfilled(Param<string[]>.IsAnything)).Return(false);
                        FeatureBeeBuilder.ForWebApp(() => HttpContextMock)
                            .Use(featureRepository, new List<IConditionEvaluator> { conditionEvaluator })
                            .Build();
                    };
                It should_return_false = () => isEnabled.Should().BeFalse();
            }

            static IConditionEvaluator conditionEvaluator;

            static IFeatureRepository featureRepository;
        }

        private static bool isEnabled;
        private static bool canEvaluate;
    }
}
