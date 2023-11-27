# MvcDynamicForms.NetCore

# About

This repository contains a project that demonstrates how to create dynamic forms in ASP.NET MVC Net 7. The project is written in C#, a popular and versatile programming language.

**ASP.NET MVC** is a framework for building scalable, standards-based web applications using well-established design patterns and the power of ASP.NET and the .NET platform. This project specifically uses the latest version, **ASP.NET MVC Net 7**.

**Dynamic forms** are a central part of many web applications. They allow for user interaction and data submission. In this project, we explore how to effectively implement dynamic forms in ASP.NET MVC Net 7.

This project is **open source**, meaning that anyone can contribute to its development. Contributions can range from reporting bugs to suggesting new features, improving documentation, and submitting pull requests.

Please note that this project is still under development, so some features may not be fully implemented or may change in the future.

Stay tuned for more updates and happy coding!

![Demo GIF](https://github.com/ongyishen/MvcDynamicFormsNetCore/blob/main/Demo01.gif)

![Demo GIF](https://github.com/ongyishen/MvcDynamicFormsNetCore/blob/main/Demo02.gif)

![Demo GIF](https://github.com/ongyishen/MvcDynamicFormsNetCore/blob/main/Demo03.gif)

# Fork Source

This is fork of ronnieoverby's project [ASP.NET MVC Dynamic Forms](https://github.com/lettucebo/MvcDynamicForms/).

# Requirements

this library requires .NET 7 and above.

## Installation

To install the "MvcDynamicForms.NetCore" package, follow these steps:

1. Download the source code from the [GitHub repository](https://github.com/ongyishen/MvcDynamicFormsNetCore.git).
2. Build your own DLL using the downloaded source code.
3. Once you have the DLL properly built and referenced in your project, you can include calls to the package's functions in your code.

For a sample implementation, you can check the [Demo](https://github.com/ongyishen/MvcDynamicFormsNetCore/tree/main/MVCDynamicForms.DemoNetCore) folder.

Add the following namespaces to use the library:

```csharp
using MvcDynamicForms.NetCore;
using MvcDynamicForms.NetCore.Fields;
```

# Getting Started

This code snippet is a comment block that serves as an introductory explanation for the code. It provides an overview of the purpose of the code and its different components.

## Demos

First off, there are multiple demos available, each with a different approach to persisting the `Form` object across requests. These demos may appear the same from an end user's perspective, but the difference lies in how the `Form` object is managed.

## InputFields and Persistence

In most cases, it is necessary to keep the original `Form` and `Field` objects as long as the user is working on completing the form. This is because the `InputField` objects are constructed with a new GUID.

To manually manage the `InputFields`, you can set the `InputField.Key` property. If you do this and can guarantee that the fields and their keys will not change after reconstructing all objects, you don't need to persist the objects across requests.

For examples and further details, refer to [How to #1](#how-to-1) and [How to #2](#how-to-2).

## FormProvider.cs

The `FormProvider.cs` file provides demo data to showcase how `MvcDynamicForms.NetCore` works. It contains more detailed information in its comment block.

---

I hope this provides a clearer understanding of the code snippet. Let me know if you have any further questions!

### How to #1 Passing Form through ModelBinding

In this Demo, the Form object graph is serialized to a string and stored in a hidden field in the page's HTML.

```csharp
public ActionResult Index()
{
    var form = FormProvider.GetForm();

    // we are going to store the form and
    // the field objects on the page across requests
    form.Serialize = true;

    return this.View("Demo", form);
}
```

Showing the form html

```html
@model MvcDynamicForms.Core.Form @using (Html.BeginForm(null, null,
FormMethod.Post, new { enctype = "multipart/form-data" })) {
@Html.Raw(Model.RenderErrorSummary()) @Html.Raw(Model.RenderHtml(true))

<input type="submit" value="Submit" />
}
```

### How to #2 Show the response

In this Demo, simply show how to echo the responses.

```csharp
[HttpPost]
public ActionResult Demo1(Form form)
{
    // no need to retrieve the form object from anywhere
    // just use a parameter on the Action method that we are posting to

    if (form.Validate()) //input is valid
        return this.View("Responses", form);

    // input is not valid
    return this.View("Demo", form);
}
```

```html
@model MvcDynamicForms.Core.Form foreach (var response in
Model.GetResponses(true)) {
<tr>
  <td>@response.Title</td>
  <td>@response.Value</td>
</tr>
}
```

### How to #3 Custom Html Attribute

```csharp
var attr = new Dictionary<string, string>();
attr.Add("class", "form-control");
attr.Add("placeholder", "Please Enter Name");

var name = new TextBox
{
    ResponseTitle = "Name",
    Prompt = "Enter your full name:",
    DisplayOrder = 20,
    Required = true,
    RequiredMessage = "Your full name is required",
    InputHtmlAttributes = attr
};
```

### Note

The serialization approach (demo 1) results in more concise code in the controller. Serializing the Form is also more reliable, in my opinion.

However, response time increases because of serialized data and the (de)serialization process takes time, as well.

The approach you take depends on your needs.

If this project helped you reduce time to develop, please consider buying me a cup of coffee :)
<a href="https://www.buymeacoffee.com/ongyishen" 
target="_blank">
<img src="https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png" 
alt="Buy Me A Coffee" style="height: 41px !important;width: 174px !important;box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;-webkit-box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;" ></a>
