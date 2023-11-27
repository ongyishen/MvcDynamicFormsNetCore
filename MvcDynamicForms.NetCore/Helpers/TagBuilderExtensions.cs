using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MvcDynamicForms.NetCore.Helpers
{
    public static class TagBuilderExtensions
    {
        public static string ToRawHtmlString(this IHtmlContent htmlContent)
        {
            // Render the HTML content to a string
            var writer = new System.IO.StringWriter();
            htmlContent.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);

            // Get the raw HTML string
            string rawHtml = writer.ToString();

            return rawHtml;
        }
        public static string ToRawHtmlString(this TagBuilder tag)
        {
            // Render the HTML content to a string
            var writer = new System.IO.StringWriter();
            tag.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);

            // Get the raw HTML string
            string rawHtml = writer.ToString();

            return rawHtml;
        }
    }
}
