using System;
using System.Linq.Expressions;

namespace RestExample.Responses.Base
{
    public class ExpressionFieldResolver<TSourceType, TProperty> : IFieldResolver
    {
        private readonly Func<TSourceType, TProperty> _property;

        public ExpressionFieldResolver(Expression<Func<TSourceType, TProperty>> property)
        {
            this._property = property.Compile();
        }

        public object Resolve(IResolveFieldContext context)
        {
            return this._property((TSourceType)context.Source);
        }
    }
}
