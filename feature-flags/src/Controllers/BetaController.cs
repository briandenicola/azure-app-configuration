using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using AppConfigFeatureFilteringDemo.Data;

namespace AppConfigFeatureFilteringDemo.Controllers
{
    public class BetaController: Controller
    {
        private readonly IFeatureManager _featureManager;

        public BetaController(IFeatureManagerSnapshot featureManager) =>
            _featureManager = featureManager;

        [FeatureGate(AppFeatureFlags.Beta)]
        public IActionResult Index() => View();
    }
}