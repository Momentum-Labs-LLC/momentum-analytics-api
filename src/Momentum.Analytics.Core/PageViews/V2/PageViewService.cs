using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.PageViews.V2.Interfaces;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;

namespace Momentum.Analytics.Core.PageViews.V2
{
    public interface IPageViewV2Service
    {
        Task RecordAsync(PageView pageView, CancellationToken token = default);
    }
    public class PageViewService : IPageViewV2Service
    {
        protected readonly IPageViewV2Storage _storage;
        protected readonly IPiiService _piiService;
        protected readonly ILogger _logger;

        public PageViewService(
            IPageViewV2Storage storage,
            IPiiService piiService,
            ILogger<PageViewService> logger)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _piiService = piiService ?? throw new ArgumentNullException(nameof(piiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public virtual async Task RecordAsync(PageView pageView, CancellationToken token = default)
        {
            var pageViewTask = _storage.InsertAsync(pageView, token);
            var piiTask = Task.CompletedTask;

            var appointmentId = GetAppointmentId(pageView);
            if(!string.IsNullOrWhiteSpace(appointmentId))
            {
                var collectedPii = new CollectedPii()
                {
                    CookieId = pageView.CookieId,
                    VisitId = pageView.VisitId,
                    UtcTimestamp = pageView.UtcTimestamp,
                    Pii = new PiiValue() { Value = appointmentId, PiiType = PiiTypeEnum.AppointmentId },
                };

                piiTask = _piiService.RecordAsync(collectedPii, token);
            }

            await Task.WhenAll(pageViewTask, piiTask).ConfigureAwait(false);
        } // end method

        protected virtual string? GetAppointmentId(PageView pageView)
        {           
            var urlSplit = pageView.Url.Split('?', 2);
            if(urlSplit.Length == 1)
            {
                return null;
            } // end if

            return urlSplit[1]
                .Split('&')
                .Where(x => !string.IsNullOrWhiteSpace(x)
                    && (x.StartsWith("appointmentId=", StringComparison.OrdinalIgnoreCase) 
                        || x.StartsWith("yourAppointmentId=", StringComparison.OrdinalIgnoreCase))
                    && !x.EndsWith("="))
                .Select(x => x.Split('=')[1])
                .FirstOrDefault();
        } // end method
    } // end class
} // end namespace