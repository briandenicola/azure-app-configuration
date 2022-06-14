using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement.FeatureFilters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppConfigFeatureFilteringDemo.TestFeatureFlags;

public class BetaTargetingContextAccessor : ITargetingContextAccessor
{
    private const string TargetingContextLookup = "BetaTargetingContextAccessor.TargetingContext";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BetaTargetingContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public ValueTask<TargetingContext> GetContextAsync()
    {
        HttpContext httpContext = _httpContextAccessor.HttpContext;
        if (httpContext.Items.TryGetValue(TargetingContextLookup, out object value))
        {
            return new ValueTask<TargetingContext>((TargetingContext)value);
        }

        TargetingContext targetingContext = new TargetingContext
        {
            UserId = httpContext.User.Identity.Name,
        };
        
        httpContext.Items[TargetingContextLookup] = targetingContext;
        return new ValueTask<TargetingContext>(targetingContext);
    }
}