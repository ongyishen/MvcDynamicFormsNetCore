using MvcDynamicForms.NetCore.Fields.Abstract;
using System;
using System.Text;

namespace MvcDynamicForms.NetCore.Fields
{
    /// <summary>
    /// Represents html to be rendered on the form.
    /// </summary>
    [Serializable]
    public class Literal : Field
    {
        public Literal()
        {
            this.FieldType = "Literal";

        }
        /// <summary>
        /// The html to be rendered on the form.
        /// </summary>
        public string Html { get; set; }

        public override string RenderHtml()
        {
            var html = new StringBuilder(this.Template);
            html.Replace(PlaceHolders.Literal, this.Html);

            // wrapper id
            html.Replace(PlaceHolders.FieldWrapperId, this.GetWrapperId());

            return html.ToString();
        }

        public override string BuildDefaultTemplate()
        {
            return PlaceHolders.Literal;
        }
    }
}