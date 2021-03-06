﻿using Mvc.Datatables.Formatters;
using Mvc.Datatables.Output;
using Mvc.Datatables.Sample.Models;
using Mvc.Datatables.Serialization;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;

namespace Mvc.Datatables.Sample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(FilterRequest filter)
        {
            if (Request.IsAjaxRequest())
            {
                PageResponse<UserProfile> response = Post(filter);
                MutableDataTableResult result = DataTableResultFactory.CreateMutableFromResponse(response, OutputType.New, ArrayOutputType.Name);
                return result;
            }
            else
            {
                ViewBag.Message = "Mvc.Datatables.Sample.New - Sample";
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            TempData["message"] = string.Format("Editing the user {0}...", id);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            TempData["message"] = string.Format("Deleting the user {0}...", id);
            return RedirectToAction("Index");
        }

        private PageResponse<UserProfile> Post(FilterRequest filter)
        {
            // Prepare input
            string serializedObject = JsonConvert.SerializeObject(filter, new FilterRequestConverter());
            StringContent stringContent = new StringContent(
                serializedObject, UnicodeEncoding.UTF8, DatatablesMediaTypeFormatter.ApplicationDatatablesMediaType);

            // Post
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.Request.Url, this.Url.Content("~"));
            HttpResponseMessage response = client.PostAsync("api/values", stringContent).Result;
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;

            // Capture output
            PageResponse<UserProfile> resultObject = JsonConvert.DeserializeObject<PageResponse<UserProfile>>(responseBody, new PageResponseConverter());
            return resultObject;
        }
    }
}
