using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.Storage;

namespace Triceratops.Dashboard.Views
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent ConditionalRouteLink(this IHtmlHelper helper, bool showLink, string label, string routeName, string elementClass, object routeValues = null)
        {
            if (!showLink)
            {
                return new HtmlString($"<span class=\"button disabled-button\">{label}</span>");
            }

            return helper.RouteLink(label, routeName, routeValues, new Dictionary<string, string>
            {
                ["class"] = elementClass
            });
        }

        public static IHtmlContent CreateDirectoryTree(this IHtmlHelper helper, string slug, VolumeDirectory directory, bool show = false, int indent = 1)
        {
            var htmlBuilder = new HtmlContentBuilder();
            var identifier = Guid.NewGuid();
            var collapseId = $"collapse_{identifier}";
            var topLevelClass = show ? "collapse show" : "collapse";
            var leftMargin = indent * 8;

            if (directory.IsEmpty)
            {
                htmlBuilder
                    .AppendHtml("<h4 class=\"h5 border-bottom border-dark text-muted\">")
                    .AppendHtml($"<i class=\"far fa-folder\"></i> {directory.Name}")
                    .AppendHtml("</h4>");

                return htmlBuilder;
            }

            htmlBuilder
                .AppendHtml("<h4 class=\"h5 border-bottom border-dark\">")
                .AppendHtml($"<i class=\"far fa-folder\"></i> <a class=\"text-dark text-decoration-none\" data-toggle=\"collapse\" href=\"#{collapseId}\" role=\"button\" aria-expanded=\"false\" aria-controls=\"{collapseId}\">")
                .Append(directory.Name)
                .AppendHtml("</a></h4>")
                .AppendLine()
                .AppendHtmlLine($"<div class=\"{topLevelClass}\" id=\"{collapseId}\">")
                .AppendHtmlLine($"<div style=\"margin-left: {leftMargin}px;\">");

            if (directory.HasDirectories)
            {
                foreach (var childDirectory in directory.Directories)
                {
                    htmlBuilder.AppendHtml(helper.CreateDirectoryTree(slug, childDirectory, false, indent + 1));
                }
            }

            if (directory.HasFiles)
            {
                foreach (var file in directory.Files)
                {
                    if (FileHelper.CanEdit(file.Name))
                    {                        
                        htmlBuilder
                            .AppendHtml("<div>")
                            .AppendHtml("<i class=\"far fa-file\"></i> ")
                            .AppendHtml(helper.RouteLink(file.Name, "EditServerFile", new { slug, fileHash = HashHelper.CreateHash(file.RelativePath) }))
                            .AppendHtml("</div>");
                    }
                    else
                    {
                        htmlBuilder.AppendHtml($"<div class=\"text-muted\"><i class=\"far fa-file\"></i> {file.Name}</div>");
                    }
                }
            }

            htmlBuilder
                .AppendHtmlLine("</div>")
                .AppendHtmlLine("</div>");

            return htmlBuilder;
        }
    }
}
