using System.Collections.Generic;

namespace RestExample.Responses.Base
{
    public interface IResolveFieldContext
    {
        object Source { get; set; }

        Dictionary<string, object> Build();
    }
}
