using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicMapper.Tests
{
    public class TestableDataProvider : IParsableDataProvider
    {
        public IEnumerable<IEnumerable<object>> Get()
        {
            return new List<List<object>>
            {
                new List<object>
                {
                    { "Title" },
                    { "GoodDescription" },
                    { "BadDescription" },
                },
                new List<object>
                {
                    { "Test 1" },
                    { "Good 1" },
                    { "Bad 1" }
                },
                new List<object>
                {
                    { "Test 2" },
                    { "Good 2" }
                },
                new List<object>
                {
                    { "Test 3" },
                    { "" },
                    { "Bad 3" }
                },
                new List<object>
                {
                    { "" },
                    { "Good 4" },
                    { "Bad 4" }
                },
                new List<object>
                {
                    { "" },
                    { "" },
                    { "Bad 5" }
                }
            };
        }
    }
}
