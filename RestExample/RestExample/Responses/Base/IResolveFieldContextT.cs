namespace RestExample.Responses.Base
{
    public interface IResolveFieldContextT<TSource> : IResolveFieldContext
    {
        TSource Source { get; }
    }
}
