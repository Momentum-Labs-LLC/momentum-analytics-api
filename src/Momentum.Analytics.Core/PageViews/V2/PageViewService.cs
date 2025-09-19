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
            
            var queryStringPieces = urlSplit[1].Split('&');

            var queryStringDict = queryStringPieces
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToDictionary(
                    x => x.Split('=')[0], 
                    x => {
                        var pieces = x.Split('=');
                        if(pieces.Length == 2)
                        {
                            return pieces[1];
                        }
                        return null;
                    }, StringComparer.OrdinalIgnoreCase);

            if(queryStringDict == null || queryStringDict.Count == 0)
            {
                return null;
            } // end if

            if(queryStringDict.TryGetValue("yourAppointmentId", out string? yourAppointmentId)
                && !string.IsNullOrWhiteSpace(yourAppointmentId))
            {
                return yourAppointmentId;
            }
            else if(queryStringDict.TryGetValue("appointmentId", out string? appointmentId)
                && !string.IsNullOrWhiteSpace(appointmentId))
            {
                return appointmentId;
            } // end if

            return null;
        } // end method
    } // end class
} // end namespace