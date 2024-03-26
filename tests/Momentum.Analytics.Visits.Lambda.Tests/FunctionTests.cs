using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Processing.DynamoDb.Visits.Interfaces;
using Momentum.Analytics.Processing.Visits.Interfaces;

namespace Momentum.Analytics.Visits.Lambda.Tests
{
    public class FunctionTests
    {
        private TestFunction _function;
        [Fact]
        public void DI_VisitTimeRangeProvider()
        {
            _function = new TestFunction();
            var timeRangeProvider = _function.GetTimeRangeProvider();

            Assert.NotNull(timeRangeProvider);
            Assert.IsAssignableFrom<IVisitTimeRangeProvider>(timeRangeProvider);
        } // end method

        [Fact]
        public void DI_Writer()
        {
            _function = new TestFunction();
            var writer = _function.GetWriter();

            Assert.NotNull(writer);
            Assert.IsAssignableFrom<IIdentifiedVisitWriter>(writer);
        } // end method


        // [Fact]
        // public void DI_VisitProcessor()
        // {
        //     _function = new TestFunction();
        //     var visitProcessor = _function.GetVisitProcessor();

        //     Assert.NotNull(visitProcessor);
        //     Assert.IsAssignableFrom<IDynamoDbIdentifiedVisitProcessor>(visitProcessor);
        // } // end method
    } // end class

    public class TestFunction : Function
    {
        public TestFunction() : base()
        {
        } // end method

        public TestFunction(
                IServiceProvider serviceProvider, 
                ILogger<TestFunction> logger) 
            : base(serviceProvider, logger)
        {
        } // end method

        public IVisitTimeRangeProvider GetTimeRangeProvider()
        {
            return _serviceProvider.GetRequiredService<IVisitTimeRangeProvider>();
        } // end method

        public IIdentifiedVisitWriter GetWriter()
        {
            return _serviceProvider.GetRequiredService<IIdentifiedVisitWriter>();
        }

        public IDynamoDbIdentifiedVisitProcessor GetVisitProcessor()
        {
            return _serviceProvider.GetRequiredService<IDynamoDbIdentifiedVisitProcessor>();
        } // end method
    } // end class
} // end namespace