using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;
using PhlatQL.Core.Types;
using PhlatQL.Core.Validation;
using RestExample.Models;

namespace RestExample.Requests
{
    public class BookInput<T> : UpdatePhlatType<T> where T : Book
    {
        public BookInput(List<Operation<T>> operations, IContractResolver contractResolver)
            : base(operations, contractResolver)
        {
        }

        protected override void Configure()
        {
            this.Field(x => x.AuthorId, new ValidationRuleBuilder().MinValue(1).MaxValue(int.MaxValue).Build());
            this.Field(x => x.UserId, new ValidationRuleBuilder().MinValue(1).MaxValue(int.MaxValue).Build());
            this.Field(x => x.Name, new ValidationRuleBuilder().Regex("^[a-zA-Z ]+$").Required().Build());
        }
    }
}
