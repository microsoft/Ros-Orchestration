using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RobotOrchestrator.Tests
{
    public class QueryHelperTests
    {
        private readonly Mock<IDocumentClient> mockClient;
        private readonly Uri testUri;

        public QueryHelperTests()
        {
            mockClient = new Mock<IDocumentClient>();
            testUri = UriFactory.CreateDocumentCollectionUri("testDb", "testCollection");

        }

        [Fact]
        public void QueryHelperByExpression_ExpressionGreaterThan_CorrectResult()
        {
            var testList = new List<int>() { 1, 2, 3, 0 };
            var queryable = testList.AsQueryable().OrderBy(i => i);

            mockClient.Setup(m => m.CreateDocumentQuery<int>(It.IsAny<Uri>(), It.IsAny<FeedOptions>())).Returns(queryable);

            var query = new QueryHelper<int>(mockClient.Object, testUri);

            var result = query.ByExpression(i => i > 2).Query.AsEnumerable();

            var resultExpected = new List<int> { 3 };

            // verify that expression was added correctly
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void QueryHelperOrderDescending_RobotIdProperty_CorrectOrder()
        {
            var testList = new List<RobotTelemetry>()
            {
                new RobotTelemetry { CreatedDateTime = new DateTime(2018, 9, 20) },
                new RobotTelemetry { CreatedDateTime = new DateTime(2018, 9, 17) },
                new RobotTelemetry { CreatedDateTime = new DateTime(2018, 9, 22) },
            };

            var queryable = testList.AsQueryable().OrderBy(t => t.Id);

            mockClient.Setup(m => m.CreateDocumentQuery<RobotTelemetry>(It.IsAny<Uri>(), It.IsAny<FeedOptions>())).Returns(queryable);

            var query = new QueryHelper<RobotTelemetry>(mockClient.Object, testUri);

            var result = query.OrderDescending(t => t.CreatedDateTime).Query.ToList();

            // verify that ordering was applied correctly
            Assert.Equal(22, result[0].CreatedDateTime.Day);
            Assert.Equal(20, result[1].CreatedDateTime.Day);
            Assert.Equal(17, result[2].CreatedDateTime.Day);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(10)]
        public void QueryHelperGetQueryTakeNItems_TakesAtMostNItems(int n)
        {
            var testList = new List<int>() { 1, 2, 3, 0 };

            var queryable = testList.AsQueryable().OrderBy(i => i);

            mockClient.Setup(m => m.CreateDocumentQuery<int>(It.IsAny<Uri>(), It.IsAny<FeedOptions>())).Returns(queryable);

            var query = new QueryHelper<int>(mockClient.Object, testUri);

            var result = query.Take(n).Query.ToList();

            // verify that n items were taken, or all items if there were > n items to begin with
            Assert.True(n == result.Count || (testList.Count < n && result.Count == testList.Count));
        }

        [Fact]
        public void QueryHelperTestCascade_AllQueryTypes_ExpectedResult()
        {
            const int n = 2;
            const string testId = "robot1";

            var testList = new List<RobotTelemetry>()
            {
                new RobotTelemetry { CreatedDateTime = new DateTime(2018, 9, 20), RobotId = "robot0" },
                new RobotTelemetry { CreatedDateTime = new DateTime(2018, 9, 17), RobotId = testId },
                new RobotTelemetry { CreatedDateTime = new DateTime(2018, 9, 22), RobotId = testId },
                new RobotTelemetry { CreatedDateTime = new DateTime(2018, 9, 18), RobotId = testId },
            };

            var queryable = testList.AsQueryable().OrderBy(t => t.Id);

            mockClient.Setup(m => m.CreateDocumentQuery<RobotTelemetry>(It.IsAny<Uri>(), It.IsAny<FeedOptions>())).Returns(queryable);

            var query = new QueryHelper<RobotTelemetry>(mockClient.Object, testUri);

            // get only robots for id "robot1", order by date descending
            var result = query.ByExpression(t => t.RobotId == testId).OrderDescending(t => t.CreatedDateTime).Take(n).Query.ToList();

            Assert.Equal(n, result.Count);
            Assert.Equal(22, result[0].CreatedDateTime.Day);
            Assert.Equal(testId, result[0].RobotId);
        }
    }
}
