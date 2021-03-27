using System.Collections.Generic;
using System.Linq;
using GraphQL.Builders;
using GraphQL.Types.Relay.DataObjects;
using GraphQLExample.Models;

namespace GraphQLExample.GraphQL
{
    public static class ConnectionResolver
    {
        public static Connection<T> GetPaged<T>(this IResolveConnectionContext<object> context, IQueryable<T> collection) where T : IEntity
        {
            int? first = context.First;
            int? afterCursor = Cursor.FromCursor<int?>(context.After);
            int? last = context.Last;
            int? beforeCursor = Cursor.FromCursor<int?>(context.Before);

            int totalCount = collection.Count();

            IEnumerable<T> edges;
            if (first.HasValue)
            {
                edges = collection
                    .If(afterCursor.HasValue, x => x.Where(y => y.Id > afterCursor.Value))
                    .Take(first.Value)
                    .ToList();
            }
            else
            {
                edges = collection
                    .If(beforeCursor.HasValue, x => x.Where(y => y.Id < beforeCursor.Value))
                    .If(last.HasValue, x => x.TakeLast(last.Value))
                    .ToList();
            }

            (bool hasNextPage, bool hasPreviousPage) = Cursor.GetPageStatus(collection, first, last, afterCursor, beforeCursor);
            (string startCursor, string endCursor) = Cursor.GetFirstAndLastCursor(edges, x => x.Id);

            return new Connection<T>
            {
                Edges = edges
                    .Select(x =>
                        new Edge<T>
                        {
                            Cursor = Cursor.ToCursor(x.Id),
                            Node = x
                        })
                    .ToList(),
                PageInfo = new PageInfo
                {
                    HasNextPage = hasNextPage,
                    HasPreviousPage = hasPreviousPage,
                    StartCursor = startCursor,
                    EndCursor = endCursor
                },
                TotalCount = totalCount
            };
        }
    }
}
