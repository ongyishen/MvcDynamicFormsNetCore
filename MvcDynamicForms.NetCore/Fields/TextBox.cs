using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcDynamicForms.NetCore.Fields.Abstract;
using MvcDynamicForms.NetCore.Helpers;
using System;
using System.Text;

namespace MvcDynamicForms.NetCore.Fields
{
    /// <summary>
    /// Represents an html textbox input element.
    /// </summary>
    [Serializable]
    public class TextBox : TextField
    {
        public TextBox()
        {
            this.FieldType = "TextBox";
        }

        public override string RenderHtml()
        {
            var html = new StringBuilder(this.Template);
            var inputName = this.GetHtmlId();

            // prompt label
            var prompt = new TagBuilder("label");
            prompt.InnerHtml.SetContent(this.GetPrompt());
            prompt.Attributes.Add("for", inputName);
            prompt.Attributes.Add("class", this._promptClass);
            html.Replace(PlaceHolders.Prompt, prompt.ToRawHtmlString());

            // error label
            if (!this.ErrorIsClear)
            {
                var error = new TagBuilder("label");
                error.Attributes.Add("for", inputName);
                error.Attributes.Add("class", this._errorClass);
                error.InnerHtml.SetContent(this.Error);
                html.Replace(PlaceHolders.Error, error.ToRawHtmlString());
            }

            // input element
            var txt = new TagBuilder("input");
            txt.Attributes.Add("name", inputName);
            txt.Attributes.Add("id", inputName);
            txt.Attributes.Add("type", "text");
            txt.Attributes.Add("value", this.Value);
            txt.MergeAttributes(this._inputHtmlAttributes);
            txt.TagRenderMode = TagRenderMode.SelfClosing;
            html.Replace(PlaceHolders.Input, txt.ToRawHtmlString());

            // wrapper id
            html.Replace(PlaceHolders.FieldWrapperId, this.GetWrapperId());
            var result = html.ToString();
            return html.ToString();
        }
    }
}