using System.Collections.Generic;
using GraphQLExample.GraphQL;
using GraphQLExample.Models;
using Xunit;

namespace Tests
{
    public class CursorTests
    {
        private readonly List<Item> collection = new()
        {
            new()
            {
                Id = 1,
                Name = "1"
            },
            new()
            {
                Id = 2,
                Name = "2"
            },
            new()
            {
                Id = 3,
                Name = "3"
            },
            new()
            {
                Id = 4,
                Name = "4"
            },
            new()
            {
                Id = 5,
                Name = "5"
            },
            new()
            {
                Id = 6,
                Name = "6"
            },
            new()
            {
                Id = 7,
                Name = "7"
            },
            new()
            {
                Id = 8,
                Name = "8"
            },
            new()
            {
                Id = 9,
                Name = "9"
            },
            new()
            {
                Id = 10,
                Name = "10"
            }
        };

        [Fact]
        public void NoParameters_ShouldReturnNoPages()
        {
            (bool hasNextPage, bool hasPreviousPage) = Cursor.GetPageStatus(this.collection, null, null, null, null);
            Assert.False(hasNextPage);
            Assert.False(hasPreviousPage);
        }


        [Fact]
        public void GivenFirstParamterAndLongCollection_ShouldReturnHasNextPageAndNoPreviosPage()
        {
            (bool hasNextPage, bool hasPreviousPage) = Cursor.GetPageStatus(this.collection, 3, null, null, null);
            Assert.True(hasNextPage);
            Assert.False(hasPreviousPage);
        }

        [Fact]
        public void GivenFirstParamterAndShortCollection_ShouldReturnNoNextPageAndNoPreviosPage()
        {
            (bool hasNextPage, bool hasPreviousPage) = Cursor.GetPageStatus(this.collection, 11, null, null, null);
            Assert.False(hasNextPage);
            Assert.False(hasPreviousPage);
        }

        [Fact]
        public void GivenFirstParamterAndAfterCursor_ShouldReturnHasNextPageAndHasPreviosPage()
        {
            (bool hasNextPage, bool hasPreviousPage) = Cursor.GetPageStatus(this.collection, 3, null, 3, null);
            Assert.True(hasNextPage);
            Assert.True(hasPreviousPage);
        }

        [Fact]
        public void GivenFirstParamterAndAfterCursorAtTheBoundary_ShouldReturnNoNextPageAndHasPreviosPage()
        {
            (bool hasNextPage, bool hasPreviousPage) = Cursor.GetPageStatus(this.collection, 3, null, 6, null);
            Assert.True(hasNextPage);
            Assert.True(hasPreviousPage);
        }

        [Fact]
        public void GivenFirstParamterAndAfterCursorAtTheBoundary2_ShouldReturnNoNextPageAndHasPreviosPage()
        {
            (bool hasNextPage, bool hasPreviousPage) = Cursor.GetPageStatus(this.collection, 3, null, 9, null);
            Assert.False(hasNextPage);
            Assert.True(hasPreviousPage);
        }

        [Fact]
        public void GivenFirstParamterAndAfterCursorAndShortCollection_ShouldReturnNoNextPageAndNoPreviosPage()
        {
            (bool hasNextPage, bool hasPreviousPage) = Cursor.GetPageStatus(this.collection, 10, null, 3, null);
            Assert.False(hasNextPage);
            Assert.True(hasPreviousPage);
        }

        [Fact]
        public void GivenFirstParamterAndAfterCursorAtTheEndOfCollection_ShouldReturnNoNextPageAndHasPreviosPage()
        {
            (bool hasNextPage, bool hasPreviousPage) = Cursor.GetPageStatus(this.collection, 3, null, 9, null);
            Assert.False(hasNextPage);
            Assert.True(hasPreviousPage);
        }


        [Fact]
        public void GivenLastParamterAndLongCollection_ShouldReturnNoNextPageAndHasPreviosPage()
        {
            (bool hasNextPage, bool hasPreviousPage) = Cursor.GetPageStatus(this.collection, null, 3, null, null);
            Assert.False(hasNextPage);
            Assert.True(hasPreviousPage);
        }

        [Fact]
        public void GivenLastParamterAndShortCollection_ShouldReturnNoNextPageAndNoPreviosPage()
        {
            (bool hasNextPage, bool hasPreviousPage) = Cursor.GetPageStatus(this.collection, null, 11, null, null);
            Assert.False(hasNextPage);
            Assert.False(hasPreviousPage);
        }

        [Fact]
        public void GivenLastParamterAndBeforeCursor_ShouldReturnHasNextPageAndHasPreviosPage()
        {
            (bool hasNextPage, bool hasPreviousPage) = Cursor.GetPageStatus(this.collection, null, 3, null, 7);
            Assert.True(hasNextPage);
            Assert.True(hasPreviousPage);
        }

        [Fact]
        public void GivenLastParamterAndBeforeCursorAtTheBoundary_ShouldReturnHasNextPageAndNoPreviosPage()
        {
            (bool hasNextPage, bool hasPreviousPage) = Cursor.GetPageStatus(this.collection, null, 3, null, 4);
            Assert.True(hasNextPage);
            Assert.False(hasPreviousPage);
        }

        [Fact]
        public void GivenLastParamterAndBeforeCursorAtTheBoundary2_ShouldReturnHasNextPageAndNoPreviosPage()
        {
            (bool hasNextPage, bool hasPreviousPage) = Cursor.GetPageStatus(this.collection, null, 4, null, 9);
            Assert.True(hasNextPage);
            Assert.True(hasPreviousPage);
        }

        [Fact]
        public void GivenLastParamterAndBeforeCursorAndShortCollection_ShouldReturnNoNextPageAndNoPreviosPage()
        {
            (bool hasNextPage, bool hasPreviousPage) = Cursor.GetPageStatus(this.collection, null, 10, null, 3);
            Assert.True(hasNextPage);
            Assert.False(hasPreviousPage);
        }

        [Fact]
        public void GivenLastParamterAndBeforeCursorAtTheStartOfCollection_ShouldReturnHasNextPageAndNoPreviosPage()
        {
            (bool hasNextPage, bool hasPreviousPage) = Cursor.GetPageStatus(this.collection, null, 3, null, 1);
            Assert.True(hasNextPage);
            Assert.False(hasPreviousPage);
        }

        public class Item : IEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
