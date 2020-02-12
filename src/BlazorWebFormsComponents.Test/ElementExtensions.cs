using AngleSharp.Dom;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents.Test
{
    public static class ElementExtensions
    {

        public static IElement ShouldLinkTo(this IElement element, string linkTarget)
        {

            element.ShouldBeAnchor();

            var attr = element.GetAttribute("href");
            attr.ShouldNotBeNull();
            attr.ShouldBe(linkTarget, StringCompareShould.IgnoreCase);
            return element;

        }

        public static IElement ShouldBeAnchor(this IElement element)
        {

          element.TagName.ShouldBe("a", StringCompareShould.IgnoreCase);
					return element;

        }
    }
}
