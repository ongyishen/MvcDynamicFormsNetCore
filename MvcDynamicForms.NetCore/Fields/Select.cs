using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcDynamicForms.NetCore.Fields.Abstract;
using MvcDynamicForms.NetCore.Helpers;
using System;
using System.Text;

namespace MvcDynamicForms.NetCore.Fields
{
    /// <summary>
    /// Represents an html select element.
    /// </summary>
    [Serializable]
    public class Select : ListField
    {
        public Select()
        {
            this.FieldType = "Select";
        }
        /// <summary>
        /// The number of options to display at a time.
        /// </summary>
        public int Size
        {
            get
            {
                string size;
                return this._inputHtmlAttributes.TryGetValue("size", out size) ? int.Parse(size) : 1;
            }
            set { this._inputHtmlAttributes["size"] = value.ToString(); }
        }

        /// <summary>
        /// Determines whether the select element will accept multiple selections.
        /// </summary>
        public bool MultipleSelection
        {
            get
            {
                string multiple;
                if (this._inputHtmlAttributes.TryGetValue("multiple", out multiple))
                {
                    return multiple.ToLower() == "multiple";
                }
                return false;
            }
            set { this._inputHtmlAttributes["multiple"] = value.ToString(); }
        }

        /// <summary>
        /// The text to be rendered as the first option in the select list when ShowEmptyOption is set to true.
        /// </summary>
        public string EmptyOption { get; set; }

        /// <summary>
        /// Determines whether a valueless option is rendered as the first option in the list.
        /// </summary>
        public bool ShowEmptyOption { get; set; }

        public override string RenderHtml()
        {
            var html = new StringBuilder(this.Template);
            var inputName = this.GetHtmlId();

            // prompt
            var prompt = new TagBuilder("label");
            prompt.AddCssClass(this._promptClass);
            prompt.Attributes.Add("for", inputName);
            prompt.InnerHtml.SetContent(this.GetPrompt());
            html.Replace(PlaceHolders.Prompt, prompt.ToRawHtmlString());

            // error label
            if (!this.ErrorIsClear)
            {
                var error = new TagBuilder("label");
                error.AddCssClass(this._errorClass);
                error.Attributes.Add("for", inputName);
                error.InnerHtml.SetContent(this.Error);
                html.Replace(PlaceHolders.Error, error.ToRawHtmlString());
            }

            // open select element
            var input = new StringBuilder();
            var select = new TagBuilder("select");
            select.Attributes.Add("id", inputName);
            select.Attributes.Add("name", inputName);
            select.MergeAttributes(this._inputHtmlAttributes);
            input.Append(select.RenderStartTag().ToRawHtmlString());

            // initial empty option
            if (this.ShowEmptyOption)
            {
                var opt = new TagBuilder("option");
                opt.Attributes.Add("value", null);
                opt.InnerHtml.SetContent(this.EmptyOption);
                input.Append(opt.ToRawHtmlString());
            }

            // options
            foreach (var choice in this._choices)
            {
                var opt = new TagBuilder("option");
                opt.Attributes.Add("value", choice.Value);
                if (choice.Selected)
                    opt.Attributes.Add("selected", "selected");
                opt.MergeAttributes(choice.HtmlAttributes);
                opt.InnerHtml.SetContent(choice.Text);
                input.Append(opt.ToRawHtmlString());
            }

            // close select element
            input.Append(select.RenderEndTag().ToRawHtmlString());

            // add hidden tag, so that a value always gets sent for select tags
            var hidden = new TagBuilder("input");
            hidden.Attributes.Add("type", "hidden");
            hidden.Attributes.Add("id", inputName + "_hidden");
            hidden.Attributes.Add("name", inputName);
            hidden.Attributes.Add("value", string.Empty);
            html.Replace(PlaceHolders.Input, input.ToString() + hidden.RenderSelfClosingTag().ToRawHtmlString());

            // wrapper id
            html.Replace(PlaceHolders.FieldWrapperId, this.GetWrapperId());

            return html.ToString();
        }
    }
}