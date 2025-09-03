using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace OrderFlow.Web.TagHelpers
{
    [HtmlTargetElement("a", Attributes = ActiveClassAttributeName + ",asp-controller")]
    [HtmlTargetElement("a", Attributes = ActiveClassAttributeName + ",asp-page")]
    public sealed class ActiveRouteTagHelper : TagHelper
    {
        private const string ActiveClassAttributeName = "active-class";

        [HtmlAttributeName(ActiveClassAttributeName)]
        public string ActiveClass { get; set; } = "active";

        [HtmlAttributeName("asp-area")]
        public string? Area { get; set; }

        [HtmlAttributeName("asp-controller")]
        public string? Controller { get; set; }

        [HtmlAttributeName("asp-action")]
        public string? Action { get; set; }

        [HtmlAttributeName("asp-page")]
        public string? Page { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = default!;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (output == null) throw new ArgumentNullException(nameof(output));

            var currentArea = ViewContext.RouteData.Values["area"]?.ToString();
            var currentController = ViewContext.RouteData.Values["controller"]?.ToString();
            var currentAction = ViewContext.RouteData.Values["action"]?.ToString();
            var currentPage = ViewContext.RouteData.Values["page"]?.ToString();

            bool isActive = false;

            if (!string.IsNullOrEmpty(Page))
            {
                isActive = string.Equals(currentPage, Page, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                if (!string.IsNullOrEmpty(Area) &&
                    !string.Equals(currentArea, Area, StringComparison.OrdinalIgnoreCase))
                {
                    isActive = false;
                }
                else if (!string.IsNullOrEmpty(Controller) &&
                         !string.Equals(currentController, Controller, StringComparison.OrdinalIgnoreCase))
                {
                    isActive = false;
                }
                else if (!string.IsNullOrEmpty(Action))
                {
                    isActive = string.Equals(currentAction, Action, StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    // If only controller (and/or area) provided, consider it active
                    isActive = !string.IsNullOrEmpty(Controller) || !string.IsNullOrEmpty(Area);
                }
            }

            if (isActive)
            {
                var existing = output.Attributes.ContainsName("class")
                    ? output.Attributes["class"].Value?.ToString()
                    : null;

                var merged = string.IsNullOrWhiteSpace(existing)
                    ? ActiveClass
                    : $"{existing} {ActiveClass}";

                output.Attributes.SetAttribute("class", merged);
            }
        }
    }
}


