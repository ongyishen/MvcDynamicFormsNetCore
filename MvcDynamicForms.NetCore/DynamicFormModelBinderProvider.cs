using Microsoft.AspNetCore.Mvc.ModelBinding;
using MvcDynamicForms.NetCore.Fields;
using MvcDynamicForms.NetCore.Fields.Abstract;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MvcDynamicForms.NetCore
{

    public class DynamicFormModelBinderProvider : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                return Task.CompletedTask;
            }

            var postedForm = bindingContext.HttpContext.Request.Form;
            var postedFiles = bindingContext.HttpContext.Request.Form.Files;

            var allKeys = postedForm.Keys.Union(postedFiles.Select(x => x.Name).ToList()).ToList();



            var form = (Form)bindingContext.Model;
            if (form == null && !string.IsNullOrEmpty(postedForm["MvcDynamicSerializedForm"]))
            {
                string jsonStringData = postedForm["MvcDynamicSerializedForm"];
                form = SerializationUtility.Deserialize<Form>(jsonStringData);
            }

            if (form == null)
                throw new NullReferenceException("The dynamic form object was not found. Be sure to include PlaceHolders.SerializedForm in your form template.");

            foreach (var field in form.Fields)
            {
                field.Form = form;
            }
            form.Template = form.BuildDefaultTemplate();

            foreach (var key in allKeys.Where(x => x.StartsWith(form.FieldPrefix)))
            {
                string fieldKey = key.Remove(0, form.FieldPrefix.Length);
                InputField dynField = form.InputFields.SingleOrDefault(f => f.Key == fieldKey);

                if (dynField == null)
                    continue;

                if (dynField is TextField)
                {
                    var txtField = (TextField)dynField;
                    txtField.Value = postedForm[key];
                }
                else if (dynField is ListField)
                {
                    var lstField = (ListField)dynField;

                    // clear all choice selections            
                    foreach (var choice in lstField.Choices)
                        choice.Selected = false;

                    // set current selections
                    foreach (string value in postedForm[key])
                    {
                        var choice = lstField.Choices.FirstOrDefault(x => x.Value == value);
                        if (choice != null)
                            choice.Selected = true;
                    }


                }
                else if (dynField is CheckBox)
                {
                    var chkField = (CheckBox)dynField;
                    chkField.Checked = bool.Parse(postedForm[key][0]);
                }
                else if (dynField is FileUpload)
                {
                    var fileField = (FileUpload)dynField;
                    var postedFile = postedFiles.FirstOrDefault(x => x.Name.Remove(0, form.FieldPrefix.Length) == fieldKey);
                    if (postedFile != null)
                    {
                        fileField.PostedFile = postedFile;
                    }

                }
                else if (dynField is Hidden)
                {
                    var hiddenField = (Hidden)dynField;
                    hiddenField.Value = postedForm[key];
                }
            }
            form.FireModelBoundEvents();
            var result = form;
            bindingContext.Result = ModelBindingResult.Success(result);
            return Task.CompletedTask;
        }


    }
}
