using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Momentum.Analytics.Core.PageViews.Models;
using Momentum.Analytics.Lambda.Api.Cookies;
using Momentum.Analytics.Lambda.Api.PageViews.ViewModels;

namespace Momentum.Analytics.Lambda.Api.PageViews
{
    public static class PageViewExtensions
    {
        public static PageView ToDomain(this PageViewViewModel viewModel, Cookie cookie, string requestId)
        {
            return new PageView()
            {
                RequestId = requestId,
                CookieId = cookie.Id,
                Referer = viewModel.Referer,
                UtmParameters = viewModel.UtmParameters?
                    .Select(x => 
                    {
                        UrchinTrackingParameter result = null;
                        if(PageViewConstants.UTM_PARAMETERS.TryGetValue(x.Key, out UrchinParameterEnum utmParam))
                        {
                            result = new UrchinTrackingParameter()
                            {
                                Parameter = utmParam,
                                Value = x.Value
                            };
                        } // end if
                        
                        return result;                    
                    }).Where(x => x != null),
                Domain = viewModel.Domain,
                Path = viewModel.Path,
                FunnelStep = viewModel.FunnelStep                
            };
        } // end method
    } // end class
} // end namespace