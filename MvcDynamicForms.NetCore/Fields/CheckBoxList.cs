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
    /// Represents a list of html checkbox inputs.
    /// </summary>
    [Serializable]
    public class CheckBoxList : OrientableField
    {
        public CheckBoxList()
        {
            this.FieldType = "CheckBoxList";
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
                ;
                error.InnerHtml.SetContent(this.Error);
                html.Replace(PlaceHolders.Error, error.ToRawHtmlString());
            }

            // list of checkboxes
            var input = new StringBuilder();
            var ul = new TagBuilder("ul");
            ul.AddCssClass(this._orientation == Orientation.Vertical ? this._verticalClass : this._horizontalClass);
            ul.AddCssClass(this._listClass);
            input.Append(ul.RenderStartTag().ToRawHtmlString());

            var choicesList = this._choices.ToList();
            for (int i = 0; i < choicesList.Count; i++)
            {
                ListItem choice = choicesList[i];
                string chkId = inputName + i;

                // open list item
                var li = new TagBuilder("li");

                input.Append(li.RenderStartTag().ToRawHtmlString());

                // checkbox input
                var chk = new TagBuilder("input");
                chk.Attributes.Add("type", "checkbox");
                chk.Attributes.Add("name", inputName);
                chk.Attributes.Add("id", chkId);
                chk.Attributes.Add("value", choice.Value);
                if (choice.Selected)
                    chk.Attributes.Add("checked", "checked");
                chk.MergeAttributes(this._inputHtmlAttributes);
                chk.MergeAttributes(choice.HtmlAttributes);
                input.Append(chk.RenderSelfClosingTag().ToRawHtmlString());

                // checkbox label
                var lbl = new TagBuilder("label");
                lbl.Attributes.Add("for", chkId);
                lbl.AddCssClass(this._inputLabelClass);
                lbl.InnerHtml.SetContent(choice.Text);
                input.Append(lbl.ToRawHtmlString());

                // close list item
                input.Append(li.RenderEndTag().ToRawHtmlString());
            }
            input.Append(ul.RenderEndTag().ToRawHtmlString());

            // add hidden tag, so that a value always gets sent
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