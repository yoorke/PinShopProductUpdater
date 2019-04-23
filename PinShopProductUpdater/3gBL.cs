using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using System.Net;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;

namespace PinShopProductUpdater
{
    public class _3gBL
    {
        private XmlDocument loadDataFromFile(string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            return xmlDoc;
        }

        private XmlDocument loadDataFromUrl(string url)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(url);

            return xmlDoc;
        }

        public string[] SaveProducts()
        {
            XmlDocument xmlDoc = loadDataFromFile("3gPinZr.xml");
            List<ThreegProduct> products = new List<ThreegProduct>();
            //DataTable products = new DataTable();
            //products.Columns.Add("id", typeof(int));
            //products.Columns.Add("sifra", typeof(string));
            //products.Columns.Add("naziv", typeof(string));
            //products.Columns.Add("kategorija1", typeof(string));
            //products.Columns.Add("kategorija2", typeof(string));
            //products.Columns.Add("kategorija3", typeof(string));
            //products.Columns.Add("vpCena", typeof(double));
            //products.Columns.Add("mpCena", typeof(double));
            //products.Columns.Add("rabat", typeof(double));
            //products.Columns.Add("dostupan", typeof(bool));
            //products.Columns.Add("naAkciji", typeof(bool));
            //products.Columns.Add("opis", typeof(string));
            //products.Columns.Add("barkod", typeof(string));
            //products.Columns.Add("slike", typeof(string));

            if(xmlDoc != null)
            {
                XmlNodeList list = xmlDoc.DocumentElement.SelectNodes("artikal");
                ThreegProduct product;
                foreach(XmlNode artikal in list)
                {
                    product = new ThreegProduct();
                    //product["ID"] = int.Parse(artikal.SelectSingleNode("id").InnerText.Trim());
                    product.Sifra = artikal.SelectSingleNode("sifra").InnerText.Trim();
                    product.Naziv = artikal.SelectSingleNode("naziv").InnerText.Trim();
                    product.Kategorija1 = artikal.SelectSingleNode("kategorija1").InnerText.Trim();
                    product.Kategorija2 = artikal.SelectSingleNode("kategorija2").InnerText.Trim();
                    product.Kategorija3 = artikal.SelectSingleNode("kategorija3").InnerText.Trim();
                    double vpCena = 0;
                    double.TryParse(artikal.SelectSingleNode("vpCena").InnerText.Trim().Replace('.',','), out vpCena);
                    product.VpCena = vpCena;
                    double mpCena = 0;
                    double.TryParse(artikal.SelectSingleNode("mpCena").InnerText.Trim().Replace('.',','), out mpCena);
                    product.MpCena = mpCena;
                    double rabat = 0;
                    double.TryParse(artikal.SelectSingleNode("rabat").InnerText.Trim(), out rabat);
                    product.Rabat = rabat;
                    product.Dostupan = artikal.SelectSingleNode("dostupan").InnerText.Trim() == "1" ? true : false;
                    product.NaAkciji = artikal.SelectSingleNode("naAkciji").InnerText.Trim() == "1" ? true : false;
                    product.Opis = artikal.SelectSingleNode("opis").InnerText.Trim();
                    product.Barkod = artikal.SelectSingleNode("barKod").InnerText.Trim();
                    product.Slike = artikal.SelectSingleNode("slike").OuterXml;

                    //products.Rows.Add(product);
                    products.Add(product);
                }

                //new _3gDL().SaveProducts(products);
                
            }
            return products.Count > 0 ? sendProducts(products) : new string[] { "0", "0" };
        }

        private string[] sendProducts(List<ThreegProduct> products)
        {
            string[] status = new string[2];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["webUrl"] + "/product/saveThreegProducts");
            request.Method = "POST";
            request.ContentType = "application/json;charset=utf-8";
            request.MediaType = "json";

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(JsonConvert.SerializeObject(products));
            }

            var response = (HttpWebResponse)request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var result = reader.ReadToEnd();
                status = result.Substring(1, result.Length - 2).Split(',');
            }
                

            return status;
        }
    }
}
