﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcDynamicForms.NetCore.Fields.Abstract;
using MvcDynamicForms.NetCore.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace MvcDynamicForms.NetCore.Fields
{
    public delegate void FilePostedEventHandler(FileUpload fileUploadField, EventArgs e);

    [Serializable]
    public class FileUpload : InputField
    {
        public FileUpload()
        {
            this.FieldType = "FileUpload";
        }

        public event FilePostedEventHandler Posted;

        [NonSerialized] private IFormFile _postedFile;
        private string _invalidExtensionError = "Invalid File Type";

        public string InvalidExtensionError
        {
            get { return this._invalidExtensionError; }
            set { this._invalidExtensionError = value; }
        }

        public IFormFile PostedFile
        {
            get { return this._postedFile; }
            set { this._postedFile = value; }
        }

        /// <summary>
        /// A comma delimited list of acceptable file extensions.
        /// </summary>
        public string ValidExtensions { get; set; }

        /// <summary>
        /// file input add multiple to enable multiple upload
        /// </summary>
        public bool UseMultiple { get; set; }

        public bool FileWasPosted
        {
            get { return this.PostedFile != null && !string.IsNullOrEmpty(this.PostedFile.FileName); }
        }

        public override string Response
        {
            get { return this.PostedFile?.FileName ?? string.Empty; }
        }

        public override bool Validate()
        {
            this.ClearError();

            if (this.Required && !this.FileWasPosted)
            {
                this.Error = this.RequiredMessage;
            }
            else if (!string.IsNullOrEmpty(this.ValidExtensions))
            {
                var exts = this.ValidExtensions.ToUpper().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!exts.Contains(Path.GetExtension(this.PostedFile.FileName).ToUpper()))
                {
                    this.Error = this.InvalidExtensionError;
                }
            }

            this.FireValidated();
            return this.ErrorIsClear;
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
            var input = new TagBuilder("input");
            input.Attributes.Add("name", inputName);
            input.Attributes.Add("id", inputName);
            input.Attributes.Add("type", "file");
            input.MergeAttributes(this._inputHtmlAttributes);

            if (UseMultiple)
            {
                input.Attributes.Add("multiple", "multiple");
            }
            input.TagRenderMode = TagRenderMode.SelfClosing;
            html.Replace(PlaceHolders.Input, input.ToRawHtmlString());

            // wrapper id
            html.Replace(PlaceHolders.FieldWrapperId, this.GetWrapperId());

            return html.ToString();
        }

        internal void FireFilePosted()
        {
            if (this.FileWasPosted && this.Posted != null)
                this.Posted(this, new EventArgs());
        }
    }
}