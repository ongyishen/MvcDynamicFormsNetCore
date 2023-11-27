using Microsoft.AspNetCore.Mvc;
using MvcDynamicForms.NetCore;
using MvcDynamicForms.NetCore.Fields;
using MVCDynamicForms.DemoNetCore.Models;
using System.Diagnostics;

namespace MVCDynamicForms.DemoNetCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var form = FormProvider.GetForm();
            // we are going to store the form and 
            // the field objects on the page across requests
            form.Serialize = true;

            return this.View("Demo", form);
        }

        [HttpPost]
        public IActionResult Index(Form form)
        {
            // no need to retrieve the form object from anywhere
            // just use a parameter on the Action method that we are posting to
            if (form.Validate()) //input is valid
                return this.View("Responses", form);

            // input is not valid
            return this.View("Demo", form);
        }

        //public async Task<IActionResult> Demo2()
        //{
        //    // recreate the form and set the keys
        //    var form = FormProvider.GetForm();
        //    this.DemoSetKeys(form);

        //    // set user input on recreated form
        //    //this.UpdateModel(form);
        //    // set user input on recreated form
        //    await TryUpdateModelAsync(form);

        //    if (this.Request.Method == "POST" && form.Validate()) // input is valid
        //        return this.View("Responses", form);

        //    // input is not valid
        //    return this.View("Demo", form);
        //}

        void DemoSetKeys(Form form)
        {
            int key = 1;
            foreach (var field in form.InputFields)
            {
                field.Key = key++.ToString();
            }
        }

        public ActionResult Demo2()
        {
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

            var form = new Form();
            form.AddFields(name);
            form.Serialize = true;
            DemoSetKeys(form);
            return this.View("Demo", form);
        }

        [HttpPost]
        public IActionResult Demo2(Form form)
        {
            // no need to retrieve the form object from anywhere
            // just use a parameter on the Action method that we are posting to

            if (form.Validate()) //input is valid
                return this.View("Responses", form);

            // input is not valid
            return this.View("Demo", form);
        }

        public ActionResult Demo3()
        {
            var attr1 = new Dictionary<string, string>();
            attr1.Add("class", "form-control datepicker");
            attr1.Add("placeholder", "Please Enter Birthday");

            var birth1 = new TextBox
            {
                ResponseTitle = "Birthday1",
                Prompt = "Enter your birthday1:",
                DisplayOrder = 20,
                Required = true,
                RequiredMessage = "Your birthday1 is required",
                InputHtmlAttributes = attr1
            };

            var attr2 = new Dictionary<string, string>();
            attr2.Add("class", "form-control");
            attr2.Add("placeholder", "Please Enter Birthday");
            var birth2 = new TextBox
            {
                Key = "Birthday",
                ResponseTitle = "Birthday2",
                Prompt = "Enter your birthday2:",
                DisplayOrder = 20,
                Required = true,
                RequiredMessage = "Your birthday2 is required",
                InputHtmlAttributes = attr2
            };

            var form = new Form();
            form.AddFields(birth1);
            form.AddFields(birth2);
            form.Serialize = true;
            return this.View("Demo", form);
        }

        [HttpPost]
        public ActionResult Demo3(Form form)
        {
            // no need to retrieve the form object from anywhere
            // just use a parameter on the Action method that we are posting to

            if (form.Validate()) //input is valid
                return this.View("Responses", form);

            // input is not valid
            return this.View("Demo", form);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}