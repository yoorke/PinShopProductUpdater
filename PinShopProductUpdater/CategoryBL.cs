using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Configuration;

namespace PinShopProductUpdater
{
    class CategoryBL
    {
        public List<CategorySimple> GetCategories()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["webUrl"] + "category/GetCategoriesForProductUpdate");
            request.Method = "GET";
            request.ContentType = "application/json";
            request.MediaType = "json";

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return JsonConvert.DeserializeObject<List<CategorySimple>>(responseString);

        }
    }
}
