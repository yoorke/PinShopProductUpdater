using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Net;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using System.Xml;
using System.Web;

namespace PinShopProductUpdater
{
    public class EweBL
    {
        public List<CategorySimple> GetEweSubCategories(int categoryID, int eweCategoryID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["webUrl"] + "category/GetEweSubCategories?categoryID=" + categoryID + "&eweCategoryID=" + eweCategoryID);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.MediaType = "json";

            //string postdata = "eweCategoryID=" + eweCategoryID.ToString();
            //var data = Encoding.UTF8.GetBytes(postdata);

            //request.ContentLength = data.Length;

            //using (var stream = request.GetRequestStream())
            //{
            //stream.Write(data, 0, data.Length);
            //}

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return JsonConvert.DeserializeObject<List<CategorySimple>>(responseString);
        }

        public string[] GetEweCategoryForCategory(int categoryID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["webUrl"] + "category/GetEweCategoryForCategory?categoryID=" + categoryID.ToString());
            request.Method = "GET";
            request.ContentType = "application/json";
            request.MediaType = "json";

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return JsonConvert.DeserializeObject<string[]>(responseString);
        }

        public string[] ParseProductsForSaving(string eweCategory, string[] eweSubcategories, int categoryID, string logFilename)
        {
            Common.log("Preuzimam proizvode...", true, logFilename);
            List<EweProduct> products = new List<EweProduct>();
            for (int i = 0; i < eweSubcategories.Length; i++)
            {
                XmlDocument xmlDoc = getXml(eweCategory, eweSubcategories[i], true, true);
                if (xmlDoc != null)
                {
                    XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("product");

                    foreach (XmlNode xmlNode in nodeList)
                    {
                        EweProduct product = new EweProduct();
                        product.Code = xmlNode.SelectSingleNode("id").InnerText.Trim();
                        product.Brand = xmlNode.SelectSingleNode("manufacturer").InnerText.Trim();
                        product.Name = xmlNode.SelectSingleNode("name").InnerText.Trim();
                        product.Price = xmlNode.SelectSingleNode("price").InnerText.Replace('.', ',').Trim();
                        product.PriceRebate = xmlNode.SelectSingleNode("price_rebate").InnerText.Trim();
                        product.Vat = xmlNode.SelectSingleNode("vat").InnerText.Trim();
                        product.Category = xmlNode.SelectSingleNode("category").InnerText.Trim();
                        product.Ean = xmlNode.SelectSingleNode("ean").InnerText.Trim();
                        product.Images = xmlNode.SelectSingleNode("images").OuterXml.Trim();
                        product.Specification = xmlNode.SelectSingleNode("specifications") != null ? xmlNode.SelectSingleNode("specifications").OuterXml.Trim() : string.Empty;
                        product.Subcategory = xmlNode.SelectSingleNode("subcategory").InnerText.Trim();
                        product.UpdateCategory = eweCategory;
                        product.CategoryID = categoryID.ToString();
                        if (xmlNode.SelectSingleNode("subcategory").InnerText.Trim() != string.Empty)
                            products.Add(product);
                    }
                }
            }
            Common.log("Preuzeto " + products.Count.ToString() + " proizvoda", true, logFilename);
            return sendEweProducts(products, eweCategory, categoryID, logFilename, eweSubcategories);
            //return 0;
        }

        private XmlDocument getXml(string category, string subcategory, bool images, bool attributes)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                /*WebRequest request = WebRequest.Create(@"http://www.ewe.rs/share/backend_231/?user=pinservis&secretcode=754fc&images=1&attributes=1&category=NOTEBOOK");
                WebResponse response = request.GetResponse();
                object xml = response.ResponseUri;*/


                //string xml = @"<products><product><id><![CDATA[ NOT04915 ]]></id><manufacturer><![CDATA[ TARGUS ]]></manufacturer><name><![CDATA[ CleanVu cleaning pad TXA002EU ]]></name><category><![CDATA[ NOTEBOOK ]]></category><subcategory><![CDATA[ DODATNA OPREMA ]]></subcategory><price><![CDATA[ 108.09 ]]></price><price_rebate><![CDATA[ 108.09 ]]></price_rebate><vat><![CDATA[ 20 ]]></vat><ean><![CDATA[ 5051794006100 ]]></ean><images><image><![CDATA[ http://www.ewe.rs/slike-proizvoda/NOT04915_v.jpg ]]></image><image><![CDATA[ http://www.ewe.rs/slike-proizvoda/NOT04915_1.jpg ]]></image><image><![CDATA[ http://www.ewe.rs/slike-proizvoda/NOT04915_2.jpg ]]></image></images><specifications><attribute_group name='Karakteristike'><attribute name='Tip'><value><![CDATA[ Cleaning pad ]]></value></attribute></attribute_group><attribute_group name='Fizičke karakteristike'><attribute name='Dimenzije'><value><![CDATA[ 76mm x 76mm ]]></value></attribute><attribute name='Masa'><value><![CDATA[ 44g ]]></value></attribute><attribute name='Boja'><value><!CDATA[ Crna ]]></value></attribute></attribute_group><attribute_group name='Garancija'><attribute name='Garancija'><value><![CDATA[ 2 godine ]]></value></attribute></attribute_group></specifications></product></products>";
                string url = @"http://www.ewe.rs/share/backend_231/?user=pinservis&secretcode=754fc";
                if (images)
                    url += "&images=1";
                if (attributes)
                    url += "&attributes=1";
                if (category != string.Empty)
                    url += "&category=" + HttpUtility.UrlEncode(category);
                if (subcategory != string.Empty)
                    url += "&subcategory=" + HttpUtility.UrlEncode(subcategory);

                //url = url.Substring(0, url.IndexOf("category=") + 9) + System.Web.HttpUtility.UrlEncode(url.Substring(url.IndexOf("category=") + 9));
                xmlDoc.Load(url);
                //xmlDoc.Load(HttpContext.Current.Server.MapPath("~") + "xml.xml");
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Root element is missing"))
                    return null;
                //throw new BLException("Ne postoje podaci za kategoriju", e);
            }
            return xmlDoc;
        }

        private string[] sendEweProducts(List<EweProduct> products, string eweCategory, int categoryID, string logFilename, string[] eweSubcategories)
        {
            Common.log("Šaljem proizvode u bazu...", true, logFilename);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["webUrl"] + "/product/saveEweProducts");
            request.Method = "POST";
            request.ContentType = "application/json;charset=utf-8";
            request.MediaType = "json";



            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(JsonConvert.SerializeObject(products));
            }

            var response = (HttpWebResponse)request.GetResponse();
            string[] status = new string[2];
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var result = reader.ReadToEnd();
                status = result.Substring(1, result.Length - 2).Split(',');
            }
            Common.log("Proizvodi poslati. Ukupno ažurirano: " + status[1] + ". Ukupno novih: " + status[0], true, logFilename);
            Common.log("-----------------", true, logFilename);
            return status;
        }
    }
}
