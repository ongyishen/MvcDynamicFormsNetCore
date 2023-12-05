﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcDynamicForms.NetCore.CustomJsonConverters;
using MvcDynamicForms.NetCore.Fields;
using MvcDynamicForms.NetCore.Fields.Abstract;
using MvcDynamicForms.NetCore.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MvcDynamicForms.NetCore
{
    /// <summary>
    /// Represents an html input form that can be dynamically rendered at runtime.
    /// </summary>
    [Serializable]
    [ModelBinder(typeof(DynamicFormModelBinderProvider))]
    public class Form
    {
        private string _fieldPrefix = "MvcDynamicField_";
        private FieldList _fields;

        public string Template { get; set; }

        /// <summary>
        /// A collection of Field objects.
        /// </summary>
        [JsonConverter(typeof(FieldConverter))]
        public FieldList Fields
        {
            get { return this._fields; }
            set { this._fields = value; }
        }

        /// <summary>
        /// Gets or sets the string that is used to prefix html input elements' id and name attributes.
        /// This value must comply with the naming rules for HTML id attributes and Input elements' name attributes.
        /// </summary>
        public string FieldPrefix
        {
            get { return this._fieldPrefix; }
            set { this._fieldPrefix = value ?? ""; }
        }

        /// <summary>
        /// Gets or sets the boolean value determining if the form should serialize itself into the string returned by the RenderHtml() method.
        /// </summary>
        public bool Serialize { get; set; }

        /// <summary>
        /// Returns an enumeration of Field objects that are of type InputField.
        /// </summary>
        public IEnumerable<InputField> InputFields
        {
            get { return this._fields.OfType<InputField>(); }
        }

        public Form()
        {
            this._fields = new FieldList(this);
            this.Template = this.BuildDefaultTemplate();
        }

        public string BuildDefaultTemplate()
        {
            var formWrapper = new TagBuilder("div");
            formWrapper.AddCssClass("MvcDynamicForm");
            formWrapper.InnerHtml.AppendHtml(PlaceHolders.Fields + PlaceHolders.SerializedForm + PlaceHolders.DataScript);
            return formWrapper.ToRawHtmlString();
        }

        /// <summary>
        /// Renders a script block containing a JSON representation of each fields' client side data.
        /// </summary>
        /// <param name="jsVarName">Name of the js variable.</param>
        /// <returns>System.String.</returns>
        public string RenderDataScript(string jsVarName)
        {
            if (string.IsNullOrEmpty(jsVarName))
                jsVarName = "MvcDynamicFieldData";

            if (this._fields.Any(x => x.HasClientData))
            {
                var data = new Dictionary<string, Dictionary<string, DataItem>>();
                foreach (var field in this._fields.Where(x => x.HasClientData))
                    data.Add(field.Key, field.DataDictionary);

                var script = new TagBuilder("script");
                script.Attributes["type"] = "text/javascript";
                script.InnerHtml.AppendHtml(string.Format("{0}var {1} = {2};",
                    Environment.NewLine,
                    jsVarName,
                    data.ToJson()));

                return script.ToRawHtmlString();
            }

            return null;
        }

        /// <summary>
        /// Validates each displayed InputField object contained in the Fields collection. 
        /// Validation causes the Error property to be set for each InputField object.
        /// </summary>
        /// <returns>Returns true if every InputField object is valid. False is returned otherwise.</returns>
        public bool Validate()
        {
            return this.Validate(true);
        }

        /// <summary>
        /// Validates each displayed InputField object contained in the Fields collection. 
        /// Validation causes the Error property to be set for each InputField object.
        /// </summary>
        /// <param name="onlyDisplayed">Whether to validate only displayed fields.</param>
        /// <returns>Returns true if every InputField object is valid. False is returned otherwise.</returns>
        public bool Validate(bool onlyDisplayed)
        {
            bool isValid = true;

            foreach (var field in this.InputFields.Where(x => !onlyDisplayed || x.Display))
                isValid = isValid & field.Validate();

            return isValid;
        }

        // Renders the error summary for the current object.
        //
        // Returns:
        //     A string containing the rendered error summary.
        public string RenderErrorSummary()
        {
            var errorSummary = new StringBuilder();
            foreach (var field in this.InputFields)
            {
                if (!string.IsNullOrEmpty(field.Error))
                {
                    errorSummary.AppendLine($"<li class=\"MvcDynamicFieldError\">{field.GetPrompt()} {field.Error}</li>");
                }
            }

            if (errorSummary.Length > 0)
            {
                var html = new StringBuilder("<ul>");
                html.Append(errorSummary);
                html.Append("</ul>");

                return html.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns a string containing the rendered HTML of every contained Field object.
        /// Optionally, the form's serialized state and/or JavaScript data can be included in the returned HTML string.
        /// </summary>        
        /// <param name="formatHtml">Determines whether to format the generated html with indentation and whitespace for readability.</param>
        /// <returns>Returns a string containing the rendered html of every contained Field object.</returns>
        public string RenderHtml(bool formatHtml)
        {
            var fieldsHtml = new StringBuilder();
            var myList = this._fields.Where(x => x.Display).OrderBy(x => x.DisplayOrder).ToList();
            foreach (var field in myList)
            {
                var result = field.RenderHtml().ToString();
                fieldsHtml.Append(result);
            }


            var html = new StringBuilder(this.Template);
            html.Replace(PlaceHolders.Fields, fieldsHtml.ToString());

            if (this.Serialize)
            {
                var hdn = new TagBuilder("input");
                hdn.Attributes["type"] = "hidden";
                hdn.Attributes["id"] = MagicStrings.MvcDynamicSerializedForm;
                hdn.Attributes["name"] = MagicStrings.MvcDynamicSerializedForm;
                hdn.Attributes["value"] = SerializationUtility.Serialize(this);
                hdn.TagRenderMode = TagRenderMode.SelfClosing;
                html.Replace(PlaceHolders.SerializedForm, hdn.RenderSelfClosingTag().ToRawHtmlString());
            }

            html.Replace(PlaceHolders.DataScript, this.RenderDataScript(null));

            PlaceHolders.RemoveAll(html);

            if (formatHtml)
                return XDocument.Parse(html.ToString()).ToString();

            return html.ToString();
        }

        /// <summary>
        /// Returns a string containing the rendered html of every contained Field object. The html can optionally include the Form object's state serialized into a hidden field.
        /// </summary>
        /// <returns>Returns a string containing the rendered html of every contained Field object.</returns>
        public string RenderHtml()
        {
            return this.RenderHtml(false);
        }

        /// <summary>
        /// This method clears the Error property of each contained InputField.
        /// </summary>
        public void ClearAllErrors()
        {
            foreach (var inputField in this.InputFields)
                inputField.ClearError();
        }

        /// <summary>
        /// This method provides a convenient way of adding multiple Field objects at once.
        /// </summary>
        /// <param name="fields">Field object(s)</param>
        public void AddFields(params Field[] fields)
        {
            foreach (var field in fields)
            {
                this._fields.Add(field);
            }
        }

        /// <summary>
        /// Provides a convenient way the end users' responses to each InputField
        /// </summary>
        /// <param name="completedOnly">Determines whether to return a Response object for only InputFields that the end user completed.</param>
        /// <returns>List of Response objects.</returns>
        public List<Response> GetResponses(bool completedOnly)
        {
            var responses = new List<Response>();
            foreach (var field in this.InputFields.OrderBy(x => x.DisplayOrder))
            {
                var response = new Response
                {
                    Title = field.GetResponseTitle(),
                    Value = field.Response,
                    Key = field.Key
                };

                if (completedOnly && string.IsNullOrEmpty(response.Value))
                    continue;

                responses.Add(response);
            }

            return responses;
        }

        /// <summary>
        /// Provides a convenient way to set the template for multiple fields.
        /// </summary>
        /// <param name="template">The fields' HTML template.</param>
        public void SetFieldTemplates(string template, params Field[] fields)
        {
            foreach (var field in fields)
                field.Template = template;
        }

        internal void FireModelBoundEvents()
        {
            foreach (var fileUpload in this.Fields.Where(x => x is FileUpload).Cast<FileUpload>())
            {
                fileUpload.FireFilePosted();
            }
        }
    }
}