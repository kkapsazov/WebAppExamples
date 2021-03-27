using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TP.CoreWebRequests.Responses
{
    public class BaseResponse<TSourceType>
    {
        private readonly Dictionary<string, Func<TSourceType, object>> items = new();

        public void Field(Expression<Func<TSourceType, object>> expression)
        {
            string name = expression.NameOf().ToLowerFirst();

            this.items.Add(name, expression.Compile());
        }

        public Dictionary<string, object> Build(TSourceType context)
        {
            Dictionary<string, object> dict = new();
            if (context is null)
            {
                return dict;
            }

            foreach (KeyValuePair<string, Func<TSourceType, object>> item in this.items)
            {
                dict.Add(item.Key, item.Value(context));
            }

            return dict;
        }

        public List<Dictionary<string, object>> Build(IEnumerable<TSourceType> context)
        {
            List<Dictionary<string, object>> list = new();
            if (context is null || !context.Any())
            {
                return list;
            }

            foreach (TSourceType ctx in context)
            {
                list.Add(this.Build(ctx));
            }

            return list;
        }
    }
}
