namespace TechChallenge.Games.Core.Extensions
{
    public static class FunctionalExtensions
    {
        public static TResult Map<TSource, TResult>(
            this TSource @this,
            Func<TSource, TResult> function) => function(@this);

        public static T Tee<T>(
            this T @this,
            Action<T> action)
        {
            action(@this);
            return @this;
        }
    }
}
