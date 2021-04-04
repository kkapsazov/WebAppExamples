using System.Threading.Tasks;
using GraphQL;
using GraphQL.Authorization;
using GraphQL.Language.AST;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Validation;

namespace GraphQLExample.GraphQL
{
    public class MaskAuthValidationRule : IValidationRule
    {
        private readonly IAuthorizationEvaluator _evaluator;

        public MaskAuthValidationRule(IAuthorizationEvaluator evaluator)
        {
            this._evaluator = evaluator;
        }

        public Task<INodeVisitor> ValidateAsync(ValidationContext context)
        {
            IProvideClaimsPrincipal userContext = context.UserContext as IProvideClaimsPrincipal;

            return Task.FromResult((INodeVisitor)new EnterLeaveListener(_ =>
            {
                OperationType operationType = OperationType.Query;

                _.Match<Operation>(astType =>
                {
                    operationType = astType.OperationType;

                    IGraphType type = context.TypeInfo.GetLastType();
                    this.CheckAuth(astType, type, userContext, context, operationType);
                });

                _.Match<ObjectField>(objectFieldAst =>
                {
                    if (context.TypeInfo.GetArgument()?.ResolvedType.GetNamedType() is IComplexGraphType argumentType)
                    {
                        FieldType fieldType = argumentType.GetField(objectFieldAst.Name);
                        this.CheckAuth(objectFieldAst, fieldType, userContext, context, operationType);
                    }
                });

                _.Match<Field>(fieldAst =>
                {
                    FieldType fieldDef = context.TypeInfo.GetFieldDef();

                    if (fieldDef == null)
                    {
                        return;
                    }

                    this.CheckAuth(fieldAst, fieldDef, userContext, context, operationType);
                    this.CheckAuth(fieldAst, fieldDef.ResolvedType.GetNamedType(), userContext, context, operationType);
                });
            }));
        }

        private void CheckAuth(INode node, IProvideMetadata type, IProvideClaimsPrincipal userContext, ValidationContext context, OperationType operationType)
        {
            if (type == null || !type.RequiresAuthorization())
            {
                return;
            }

            AuthorizationResult result = type
                .Authorize(userContext?.User, context.UserContext, context.Inputs, this._evaluator)
                .GetAwaiter()
                .GetResult();

            if (result.Succeeded)
            {
                return;
            }

            //TODO: handle mask via "Mask" policy
            FieldType fieldType = (FieldType)type;
            fieldType.Resolver = new ExpressionFieldResolver<object, string>(c => "***");
            fieldType.ResolvedType = new StringGraphType();
        }
    }
}
