using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Models;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.Processing.Visits;
using Momentum.Analytics.Processing.Visits.Interfaces;
using Moq;
using Xunit.Sdk;

namespace Momentum.Analytics.Processing.Tests.Visits
{
    public class IdentifiedVisitProcessorTests
    {
        private Mock<IVisitService<int, ISearchResponse<Visit>>> _visitService;
        private Mock<IIdentifiedVisitWriter> _identifiedVisitWriter;
        private Mock<ILogger<TestIdentifiedVisitProcessor>> _logger;
        private TestIdentifiedVisitProcessor _processor;

        public IdentifiedVisitProcessorTests()
        {
            _visitService = new Mock<IVisitService<int, ISearchResponse<Visit>>>();
            _identifiedVisitWriter = new Mock<IIdentifiedVisitWriter>();
            _logger = new Mock<ILogger<TestIdentifiedVisitProcessor>>();

            _processor = new TestIdentifiedVisitProcessor(
                _visitService.Object,
                _identifiedVisitWriter.Object,
                _logger.Object);
        } // end method

        [Fact]
        public async Task ReportAsync_None()
        {            
            var timeRange = new Mock<ITimeRange>();
            _visitService.Setup(x => x.GetIdentifiedAsync(timeRange.Object, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResponse<Visit>());

            await _processor.ReportAsync(timeRange.Object).ConfigureAwait(false);

            _visitService.Verify(x => x.GetIdentifiedAsync(timeRange.Object, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            _identifiedVisitWriter.Verify(x => x.WriteAsync(It.IsAny<ITimeRange>(), It.IsAny<IEnumerable<Visit>>(), It.IsAny<CancellationToken>()), Times.Never);
        } // end method

        [Fact]
        public async Task ReportAsync_OnePage()
        {
            var timeRange = new Mock<ITimeRange>();
            _visitService.Setup(x => x.GetIdentifiedAsync(timeRange.Object, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResponse<Visit>()
                {
                    Data = new List<Visit>()
                    {
                        new Visit(),
                        new Visit()
                    }
                });
            _identifiedVisitWriter.Setup(x => x.WriteAsync(timeRange.Object, It.IsAny<IEnumerable<Visit>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _processor.ReportAsync(timeRange.Object).ConfigureAwait(false);

            _visitService.Verify(x => x.GetIdentifiedAsync(timeRange.Object, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            _identifiedVisitWriter.Verify(x => x.WriteAsync(It.IsAny<ITimeRange>(), It.IsAny<IEnumerable<Visit>>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method

        [Fact]
        public async Task ReportAsync_MultiplePages()
        {
            var timeRange = new Mock<ITimeRange>();
            _visitService.SetupSequence(x => x.GetIdentifiedAsync(timeRange.Object, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResponse<Visit>()
                {
                    NextPage = 2,
                    HasMore = true,
                    Data = new List<Visit>()
                    {
                        new Visit(),
                        new Visit()
                    }
                })
                .ReturnsAsync(new SearchResponse<Visit>()
                {
                    HasMore = false,
                    Data = new List<Visit>()
                    {
                        new Visit(),
                        new Visit()
                    }
                });
            _identifiedVisitWriter.Setup(x => x.WriteAsync(timeRange.Object, It.IsAny<IEnumerable<Visit>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _processor.ReportAsync(timeRange.Object).ConfigureAwait(false);

            _visitService.Verify(x => x.GetIdentifiedAsync(timeRange.Object, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _identifiedVisitWriter.Verify(x => x.WriteAsync(It.IsAny<ITimeRange>(), It.IsAny<IEnumerable<Visit>>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method
    } // end class

    public class TestIdentifiedVisitProcessor : IdentifiedVisitProcessor<int, ISearchResponse<Visit>, IVisitService<int, ISearchResponse<Visit>>>
    {
        public TestIdentifiedVisitProcessor(
                IVisitService<int, ISearchResponse<Visit>> visitService, 
                IIdentifiedVisitWriter writer, 
                ILogger<TestIdentifiedVisitProcessor> logger) 
            : base(visitService, writer, logger)
        {
        } // end method
    } // end method
} // end namespace