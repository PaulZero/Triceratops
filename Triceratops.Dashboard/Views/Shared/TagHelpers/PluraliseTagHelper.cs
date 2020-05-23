using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Triceratops.Libraries.Helpers;

namespace Triceratops.Dashboard.Views.Shared.TagHelpers
{
    [HtmlTargetElement("pluralise", TagStructure = TagStructure.WithoutEndTag)]
    public class PluraliseTagHelper : TagHelper
    {
        public string Name { get; set; }

        public int? Count { get; set; }

        public IEnumerable<object> Values { get; set; }

        public bool ShowCount { get; set; } = true;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;

            if ((Count == null && Values == null) || (Count != null && Values != null))
            {
                throw new Exception("Pluralise requires either a count or values attribute");                
            }

            if (Count == null)
            {
                Count = Values.Count();
            }

            var pluralisedName = TextHelper.Pluralise(Name, (int)Count);
            
            if (!ShowCount)
            {
                output.Content.SetHtmlContent(pluralisedName);

                return;
            }

            output.Content.SetHtmlContent($"{Count} {pluralisedName}");
        }
    }
}
