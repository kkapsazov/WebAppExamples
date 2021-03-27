using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using GraphQLExample.Models;

namespace GraphQLExample.GraphQL
{
    public static class Cursor
    {
        public static T FromCursor<T>(string cursor)
        {
            if (string.IsNullOrEmpty(cursor))
            {
                return default;
            }

            string decodedValue;
            try
            {
                decodedValue = Base64Decode(cursor);
            }
            catch (FormatException)
            {
                return default;
            }

            Type type = typeof(T);
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(DateTimeOffset))
            {
                return (T)(object)DateTimeOffset.ParseExact(decodedValue, "o", CultureInfo.InvariantCulture);
            }

            return (T)Convert.ChangeType(decodedValue, type, CultureInfo.InvariantCulture);
        }

        public static (string firstCursor, string lastCursor) GetFirstAndLastCursor<TItem, TCursor>(
            IEnumerable<TItem> enumerable,
            Func<TItem, TCursor> getCursorProperty)
        {
            if (getCursorProperty is null)
            {
                throw new ArgumentNullException(nameof(getCursorProperty));
            }

            if (enumerable is null || !enumerable.Any())
            {
                return (null, null);
            }

            string firstCursor = ToCursor(getCursorProperty(enumerable.First()));
            string lastCursor = ToCursor(getCursorProperty(enumerable.Last()));

            return (firstCursor, lastCursor);
        }

        public static (bool hasNextPage, bool hasPreviousPage) GetPageStatus<TItem>(
            IEnumerable<TItem> enumerable,
            int? first, int? last,
            int? afterCursor, int? beforeCursor) where TItem : IEntity
        {
            if (!first.HasValue && !last.HasValue &&
                !afterCursor.HasValue && !beforeCursor.HasValue)
            {
                return (false, false);
            }

            if (!first.HasValue && !last.HasValue)
            {
                return (true, true);
            }

            bool isMovingForwards = first.HasValue;
            if (isMovingForwards && !afterCursor.HasValue)
            {
                if (first >= enumerable.Count())
                {
                    return (false, false);
                }

                return (true, false);
            }

            bool isMovingBackwards = last.HasValue;
            if (isMovingBackwards && !beforeCursor.HasValue)
            {
                if (last >= enumerable.Count())
                {
                    return (false, false);
                }

                return (false, true);
            }

            int offset = isMovingForwards ? first.Value : last.Value;
            int cursor = isMovingForwards ? afterCursor.Value : beforeCursor.Value;

            bool hasNextPage = enumerable
                .Count(y => y.Id > cursor) > (isMovingForwards ? offset : 0);

            bool hasPreviousPage = enumerable
                .Count(y => y.Id < cursor) > (isMovingBackwards ? offset : 0);

            return (hasNextPage, hasPreviousPage);
        }

        public static string ToCursor<T>(T value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value is DateTimeOffset dateTimeOffset)
            {
                return Base64Encode(dateTimeOffset.ToString("o", CultureInfo.InvariantCulture));
            }

            return Base64Encode(value.ToString());
        }

        private static string Base64Decode(string value)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }

        private static string Base64Encode(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }
    }
}
