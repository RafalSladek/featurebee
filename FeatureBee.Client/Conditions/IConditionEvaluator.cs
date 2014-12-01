namespace FeatureBee.Conditions
{
    public interface IConditionEvaluator<T> : IConditionEvaluator
    {
        
    }

    public interface IConditionEvaluator
    {
        string Name { get; }
        bool IsFulfilled(string[] values);
    }
}
