using System;
using System.Collections.Generic;
using System.Linq;
using DynamicMapper.Tests.Models;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace DynamicMapper.Tests
{
    public class DataParserTests
    {
        [Fact]
        public void CanParseDataTest()
        {
            var parser = new DataParser(new TestableDataProvider(), new ReflectionMapper());
            var questions = parser.ProcessData(typeof(Question)).Cast<Question>().ToList();

            questions.Should().NotBeNullOrEmpty();
            questions.Should().HaveCount(5, "First row contains headers");

            // First item
            var item = questions.First();
            item.Title.Should().Be("Test 1");
            item.GoodDescription.Should().Be("Good 1");
            item.BadDescription.Should().Be("Bad 1");

            // Second item
            item = questions.Skip(1).First();
            item.Title.Should().Be("Test 2");
            item.GoodDescription.Should().Be("Good 2");
            item.BadDescription.Should().BeNull();

            // Third item
            item = questions.Skip(2).First();
            item.Title.Should().Be("Test 3");
            item.GoodDescription.Should().BeNull();
            item.BadDescription.Should().Be("Bad 3");

            // Fourth item
            item = questions.Skip(3).First();
            item.Title.Should().BeNull();
            item.GoodDescription.Should().Be("Good 4");
            item.BadDescription.Should().Be("Bad 4");

            // Fifth item
            item = questions.Skip(4).First();
            item.Title.Should().BeNull();
            item.GoodDescription.Should().BeNull();
            item.BadDescription.Should().Be("Bad 5");
        }

        [Fact]
        public void CanParseDynamicDataTest()
        {
            var newModel = JsonConvert.DeserializeObject<JObject>(_jsonObject);
            var builder = new RuntimeTypeBuilder(nameof(_jsonObject));
            
            foreach (var property in newModel.Properties())
            {
                builder.Properties.Add(property.Name, GetType(property.Type));
            }
            
            var modelType = builder.Compile();

            var parser = new DataParser(new TestableDataProvider(), new ReflectionMapper());
            var questions = parser.ProcessData(modelType).ToList();

            questions.Should().NotBeNullOrEmpty();
            questions.Should().HaveCount(5, "First row contains headers");

            var resultString = JsonConvert.SerializeObject(questions);

            resultString.Should().Be(_jsonResult);
        }

        private Type GetType(JTokenType jType)
        {
            switch (jType)
            {
                case JTokenType.Array:
                    return typeof(Array);
                case JTokenType.Integer:
                    return typeof(int);
                case JTokenType.Float:
                    return typeof(float);
                case JTokenType.Boolean:
                    return typeof(bool);
                case JTokenType.Date:
                    return typeof(DateTime);
                case JTokenType.TimeSpan:
                    return typeof(TimeSpan);
                default:
                    return typeof(string);
            }
        }

        private const string _jsonObject = "{ \"Title\": \"\", \"GoodDescription\": \"\"}";
        private const string _jsonResult = "[{\"Title\":\"Test 1\",\"GoodDescription\":\"Good 1\"},{\"Title\":\"Test 2\",\"GoodDescription\":\"Good 2\"},{\"Title\":\"Test 3\",\"GoodDescription\":null},{\"Title\":null,\"GoodDescription\":\"Good 4\"},{\"Title\":null,\"GoodDescription\":null}]";

    }
}
