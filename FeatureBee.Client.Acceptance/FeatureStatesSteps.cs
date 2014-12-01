namespace FeatureBee.Acceptance
{
    using System.Collections.Generic;
    using System.Web;

    using FeatureBee.Conditions;
    using FeatureBee.WireUp;

    using Moq;

    using NUnit.Framework;

    using TechTalk.SpecFlow;

    [Binding]
    public class FeatureStatesSteps
    {
        private readonly Mock<IConditionEvaluator> _conditionEvaluatorsMock = new Mock<IConditionEvaluator>();
        private readonly Mock<IConditionEvaluator> _conditionEvaluatorsMock2 = new Mock<IConditionEvaluator>();
        private readonly Mock<IFeatureRepository> _featureRepositoryMock = new Mock<IFeatureRepository>();
        private readonly HttpContextBase _httpContextMock = HttpFakes.FakeHttpContext();
        private bool _featureIsEnabled;

        [Given(@"I have feature with a condition evaluator that is always fullfilled")]
        public void GivenIHaveFeatureWithAConditionEvaluatorThatIsAlwaysFullfilled()
        {
            _conditionEvaluatorsMock.Setup(x => x.Name).Returns("FakeEvaluator");
            _conditionEvaluatorsMock.Setup(x => x.IsFulfilled(It.IsAny<string[]>())).Returns(true);

            FeatureBeeBuilder
                .ForWebApp(() => _httpContextMock)
                .Use(_featureRepositoryMock.Object, new List<IConditionEvaluator> {_conditionEvaluatorsMock.Object})
                .Build();
        }

        [Given(@"I have feature with a condition evaluator that is never fullfilled")]
        public void GivenIHaveFeatureWithAConditionEvaluatorThatIsNeverFullfilled()
        {
            _conditionEvaluatorsMock.Setup(x => x.Name).Returns("FakeEvaluator");
            _conditionEvaluatorsMock.Setup(x => x.IsFulfilled(It.IsAny<string[]>())).Returns(false);

            FeatureBeeBuilder
                .ForWebApp(() => _httpContextMock)
                .Use(_featureRepositoryMock.Object, new List<IConditionEvaluator> {_conditionEvaluatorsMock.Object})
                .Build();
        }

        [Given(@"I have feature under test with a condition evaluator")]
        public void GivenIHaveFeatureUnderTestWithAConditionEvaluator()
        {
            var conditions = new List<ConditionDto> {new ConditionDto {Type = "FakeEvaluator"}};
            _featureRepositoryMock.Setup(x => x.GetFeatures())
                .Returns(new List<FeatureDto> {new FeatureDto {Name = "SampleFeature", State = "Under Test", Conditions = conditions}});
            _conditionEvaluatorsMock.Setup(x => x.Name).Returns("FakeEvaluator");

            FeatureBeeBuilder
                .ForWebApp(() => _httpContextMock)
                .Use(_featureRepositoryMock.Object, new List<IConditionEvaluator> {_conditionEvaluatorsMock.Object})
                .Build();
        }

        [Given(@"the condition is (.*)")]
        public void GivenTheConditionIs(bool condition)
        {
            _conditionEvaluatorsMock.Setup(x => x.IsFulfilled(It.IsAny<string[]>())).Returns(condition);
        }

        [Given(@"I have a feature in state (.*)")]
        public void GivenIHaveAFeatureInState(string featureState)
        {
            var conditions = new List<ConditionDto> {new ConditionDto {Type = "FakeEvaluator"}};
            _featureRepositoryMock.Setup(x => x.GetFeatures())
                .Returns(new List<FeatureDto> {new FeatureDto {Name = "SampleFeature", State = featureState, Conditions = conditions}});
        }

        [Given(@"the feature has a second condition evaluator")]
        public void GivenTheFeatureHasASecondConditionEvaluator()
        {
            var conditions = new List<ConditionDto>
            {
                new ConditionDto {Type = "FakeEvaluator"},
                new ConditionDto {Type = "FakeEvaluator2"}
            };
            _featureRepositoryMock.Setup(x => x.GetFeatures())
                .Returns(new List<FeatureDto>
                {
                    new FeatureDto {Name = "SampleFeature", State = "Under Test", Conditions = conditions}
                });
            _conditionEvaluatorsMock2.Setup(x => x.Name).Returns("FakeEvaluator2");

            FeatureBeeBuilder
                .ForWebApp(() => _httpContextMock)
                .Use(_featureRepositoryMock.Object, new List<IConditionEvaluator> {_conditionEvaluatorsMock.Object, _conditionEvaluatorsMock2.Object})
                .Build();
        }

        [Given(@"the first condition evaluator returns (.*)")]
        public void GivenTheFirstConditionEvaluatorReturns(bool firstEvaluator)
        {
            _conditionEvaluatorsMock.Setup(x => x.IsFulfilled(It.IsAny<string[]>())).Returns(firstEvaluator);
        }

        [Given(@"the second condition evaluator returns (.*)")]
        public void GivenTheSecondConditionEvaluatorReturns(bool secondEvaluator)
        {
            _conditionEvaluatorsMock2.Setup(x => x.IsFulfilled(It.IsAny<string[]>())).Returns(secondEvaluator);
        }

        [Given(@"I have (.*) the GodMode")]
        public void GivenIHaveEnabledTheGodMode(string mode)
        {
            var godModeCookie = new HttpCookie("FeatureBee", mode == "enabled" ? "#SampleFeature=true#" : "#SampleFeature=false#");
            var request = Mock.Get(_httpContextMock.Request);
            request.SetupGet(r => r.Cookies).Returns(new HttpCookieCollection());
            _httpContextMock.Request.Cookies.Add(godModeCookie);
        }

        [Given(@"I have a (.*) configuration")]
        public void GivenIHaveAReleaseConfiguration(string configuration)
        {
            var mock = Mock.Get(_httpContextMock);
            mock.SetupGet(x => x.IsDebuggingEnabled).Returns(configuration == "debug");
        }


        [When(@"evaluating the feature state")]
        public void WhenEvaluatingTheFeatureState()
        {
            _featureIsEnabled = Feature.IsEnabled("SampleFeature");
        }

        [Then(@"the feature is (.*)")]
        public void ThenTheFeatureIsEnabled(string featureIsEnabled)
        {
            Assert.AreEqual(featureIsEnabled == "enabled", this._featureIsEnabled);
        }

        [Then(@"(.*) to evaluate the state")]
        public void ThenToEvaluateTheState(bool conditionsWhereUsed)
        {
            if (conditionsWhereUsed)
                _conditionEvaluatorsMock.Verify(x => x.IsFulfilled(It.IsAny<string[]>()), Times.Once);
            else
                _conditionEvaluatorsMock.Verify(x => x.IsFulfilled(It.IsAny<string[]>()), Times.Never);
        }
    }
}