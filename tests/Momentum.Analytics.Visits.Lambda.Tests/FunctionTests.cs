using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.DynamoDb.Visits.Interfaces;
using Momentum.Analytics.Processing.DynamoDb.Visits.Interfaces;
using Momentum.Analytics.Processing.Visits.Interfaces;
using Momentum.Analytics.Visits.Lambda.IdentifiedVisits.Interfaces;

namespace Momentum.Analytics.Visits.Lambda.Tests
{
    public class FunctionTests
    {
        private TestFunction? _function;
        [Fact]
        public void DI_VisitTimeRangeProvider()
        {
            _function = new TestFunction();
            var timeRangeProvider = _function.GetTimeRangeProvider();

            Assert.NotNull(timeRangeProvider);
            Assert.IsAssignableFrom<IIdentifiedVisitTimeRangeProvider>(timeRangeProvider);
        } // end method

        [Fact]
        public void DI_Writer()
        {
            _function = new TestFunction();
            var writer = _function.GetWriter();

            Assert.NotNull(writer);
            Assert.IsAssignableFrom<IIdentifiedVisitWriter>(writer);
        } // end method

        [Fact]
        public void DI_DynamoDbVisitService()
        {
            _function = new TestFunction();
            var visitService = _function.GetVisitService();

            Assert.NotNull(visitService);
            Assert.IsAssignableFrom<IDynamoDbVisitService>(visitService);
        } // end method

        [Fact]
        public void DI_VisitProcessor()
        {
            _function = new TestFunction();
            var visitProcessor = _function.GetVisitProcessor();

            Assert.NotNull(visitProcessor);
            Assert.IsAssignableFrom<IDynamoDbIdentifiedVisitProcessor>(visitProcessor);
        } // end method

        [Fact]
        public void DI_UnidentifiedVisitProcessor()
        {
            _function = new TestFunction();
            var visitProcessor = _function.GetUnidentifiedVisitProcessor();

            Assert.NotNull(visitProcessor);
            Assert.IsAssignableFrom<IDynamoDbUnidentifiedVisitProcessor>(visitProcessor);
        } // end method
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

        public IIdentifiedVisitTimeRangeProvider GetTimeRangeProvider()
        {
            return _serviceProvider.GetRequiredService<IIdentifiedVisitTimeRangeProvider>();
        } // end method

        public IIdentifiedVisitWriter GetWriter()
        {
            return _serviceProvider.GetRequiredService<IIdentifiedVisitWriter>();
        } // end method

        public IDynamoDbVisitService GetVisitService()
        {
            return _serviceProvider.GetRequiredService<IDynamoDbVisitService>();
        } // end method

        public IIdentifiedVisitProcessor GetVisitProcessor()
        {
            return _serviceProvider.GetRequiredService<IIdentifiedVisitProcessor>();
        } // end method

        public IUnidentifiedVisitProcessor GetUnidentifiedVisitProcessor()
        {
            return _serviceProvider.GetRequiredService<IUnidentifiedVisitProcessor>();
        } // end method
    } // end class
} // end namespace