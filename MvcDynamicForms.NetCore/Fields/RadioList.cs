using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcDynamicForms.NetCore.Enums;
using MvcDynamicForms.NetCore.Fields.Abstract;
using MvcDynamicForms.NetCore.Helpers;
using System;
using System.Linq;
using System.Text;

namespace MvcDynamicForms.NetCore.Fields
{
    /// <summary>
    /// Represents a list of html radio button inputs.
    /// </summary>
    [Serializable]
    public class RadioList : OrientableField
    {
        public RadioList()
        {
            this.FieldType = "RadioList";
        }

        public override string RenderHtml()
        {
            var html = new StringBuilder(this.Template);
            var inputName = this.GetHtmlId();

            // prompt label
            var prompt = new TagBuilder("label");
            prompt.AddCssClass(this._promptClass);
            prompt.InnerHtml.SetContent(this.GetPrompt());
            html.Replace(PlaceHolders.Prompt, prompt.ToRawHtmlString());

            // error label
            if (!this.ErrorIsClear)
            {
                var error = new TagBuilder("label");
                error.AddCssClass(this._errorClass);
                error.InnerHtml.SetContent(this.Error);
                html.Replace(PlaceHolders.Error, error.ToRawHtmlString());
            }

            // list of radio buttons        
            var input = new StringBuilder();
            var ul = new TagBuilder("ul");
            ul.Attributes.Add("class",
                this._orientation == Orientation.Vertical ? this._verticalClass : this._horizontalClass);
            ul.AddCssClass(this._listClass);
            input.Append(ul.RenderStartTag().ToRawHtmlString());

            var choicesList = this._choices.ToList();
            for (int i = 0; i < choicesList.Count; i++)
            {
                ListItem choice = choicesList[i];
                string radId = inputName + i;

                // open list item
                var li = new TagBuilder("li");
                input.Append(li.RenderStartTag().ToRawHtmlString());

                // radio button input
                var rad = new TagBuilder("input");
                rad.Attributes.Add("type", "radio");
                rad.Attributes.Add("name", inputName);
                rad.Attributes.Add("id", radId);
                rad.Attributes.Add("value", choice.Value);
                if (choice.Selected)
                    rad.Attributes.Add("checked", "checked");
                rad.MergeAttributes(this._inputHtmlAttributes);
                rad.MergeAttributes(choice.HtmlAttributes);
                rad.TagRenderMode = TagRenderMode.SelfClosing;
                input.Append(rad.ToRawHtmlString());

                // checkbox label
                var lbl = new TagBuilder("label");
                lbl.Attributes.Add("for", radId);
                lbl.Attributes.Add("class", this._inputLabelClass);
                lbl.InnerHtml.SetContent(choice.Text);
                input.Append(lbl.ToRawHtmlString());

                // close list item
                input.Append(li.RenderEndTag().ToRawHtmlString());
            }
            input.Append(ul.RenderEndTag().ToRawHtmlString());
            html.Replace(PlaceHolders.Input, input.ToString());

            // wrapper id
            html.Replace(PlaceHolders.FieldWrapperId, this.GetWrapperId());

            return html.ToString();
        }
    }
}