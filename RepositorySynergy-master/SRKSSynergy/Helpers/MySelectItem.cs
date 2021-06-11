using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Web.Routing;

namespace SRKSSynergy.Helpers
{
        public static class MyHelpers
        {
            public class MySelectItem : SelectListItem
            {
                public string Class { get; set; }
                public string Disabled { get; set; }
            }

            public static MvcHtmlString MyDropdownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<MySelectItem> list, string optionLabel, object htmlAttributes)
            {
                return MyDropdownList(htmlHelper, ExpressionHelper.GetExpressionText(expression), list, optionLabel, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            }

            public static MvcHtmlString MyDropdownList(this HtmlHelper htmlHelper, string name, IEnumerable<MySelectItem> list, string optionLabel, IDictionary<string, object> htmlAttributes)
            {
                TagBuilder dropdown = new TagBuilder("select");
                dropdown.Attributes.Add("name", name);
                StringBuilder options = new StringBuilder();

                // Make optionLabel the first item that gets rendered.
                if (optionLabel != null)
                    options = options.Append("<option value='" + String.Empty + "'>" + optionLabel + "</option>");

                foreach (var item in list)
                {
                    if (item.Disabled == "disabled")
                        if(item.Selected)
                        options = options.Append("<option value='" + item.Value + "' class='" + item.Class + "' disabled='" + item.Disabled + "' selected = '" + item.Selected + "'>" + item.Text + "</option>");
                        else
                            options = options.Append("<option value='" + item.Value + "' class='" + item.Class + "' disabled='" + item.Disabled + "' >" + item.Text + "</option>");
                    else
                        if (item.Selected)
                            options = options.Append("<option value='" + item.Value + "' class='" + item.Class + "' selected = '" + item.Selected + "' >" + item.Text + "</option>");
                        else
                            options = options.Append("<option value='" + item.Value + "' class='" + item.Class + "' >" + item.Text + "</option>");
                }
                dropdown.InnerHtml = options.ToString();
                dropdown.MergeAttributes(new RouteValueDictionary(htmlAttributes));
                return MvcHtmlString.Create(dropdown.ToString(TagRenderMode.Normal));
            }
        }
}