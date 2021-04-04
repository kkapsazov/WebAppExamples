namespace RestExample.Responses.Base
{
    public interface IFieldResolver
    {
        object Resolve(IResolveFieldContext context);
    }
}
