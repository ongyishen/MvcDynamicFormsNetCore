using MvcDynamicForms.NetCore.Fields;
using MvcDynamicForms.NetCore.Fields.Abstract;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace MvcDynamicForms.NetCore.CustomJsonConverters
{
    public class FieldConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Field));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load the JSON array
            JArray jArray = JArray.Load(reader);

            // Create a list to hold the fields
            FieldList fields = new FieldList(null);

            // Iterate over the array elements
            foreach (JObject jObject in jArray.Children<JObject>())
            {
                // Get the value of the FieldType property
                string fieldType = (string)jObject["FieldType"];

                Field field;

                // Decide which type of field to create based on the FieldType value
                switch (fieldType)
                {
                    case "CheckBox":
                        field = new CheckBox();
                        break;
                    case "CheckBoxList":
                        field = new CheckBoxList();
                        break;
                    case "FileUpload":
                        field = new FileUpload();
                        break;
                    case "Hidden":
                        field = new Hidden();
                        break;
                    case "Literal":
                        field = new Literal();
                        break;
                    case "RadioList":
                        field = new RadioList();
                        break;
                    case "Select":
                        field = new Select();
                        break;
                    case "TextArea":
                        field = new TextArea();
                        break;
                    case "TextBox":
                        field = new TextBox();
                        break;
                    default:
                        throw new ArgumentException($"Invalid field type: {fieldType}");
                }
                field.IsDataBinding = true;
                // Populate the field properties
                serializer.Populate(jObject.CreateReader(), field);
                field.Template = field.BuildDefaultTemplate();
                // Add the field to the list
                fields.Add(field);
            }


            return fields;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }

}
