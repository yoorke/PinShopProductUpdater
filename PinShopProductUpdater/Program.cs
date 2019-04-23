using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using System.Data;
using System.Net.Mail;

namespace PinShopProductUpdater
{
    class Program
    {
        static string logFilename;
        static string startDateTime;

        static void Main(string[] args)
        {
            save3gProducts();
            try
            { 
                logFilename = string.Format("{0:00}", DateTime.Now.Day) + string.Format("{0:00}", DateTime.Now.Month) + DateTime.Now.Year.ToString() + string.Format("{0:00}", DateTime.Now.Hour) + string.Format("{0:00}", DateTime.Now.Minute) + ".log";
                startDateTime = DateTime.Now.ToString();
                saveProductsForCategories();
            }
            catch(Exception ex)
            {
                Common.log(ex.Message, true, logFilename);
            }
        }

        private static void saveProductsForCategories()
        {
            Common.log("Započeto ažuriranje proizvoda", true, logFilename);
            Common.log("Preuzimam kategorije obeležene za ažuriranje...", true, logFilename);
            List<CategorySimple> categories = new CategoryBL().GetCategories();

            Common.log("Kategorije za ažuriranje:", true, logFilename);
            foreach (CategorySimple category in categories)
                Common.log(category.Name, true, logFilename);

            string subcategories = string.Empty;
            StringBuilder updateStatus = new StringBuilder();
            updateStatus.Append("<p>Ažuriranje započeto: " + startDateTime + "</p>");
            updateStatus.Append("<br/>");
            updateStatus.Append("<table width='100%' border='0' style='border:1px solid #eeeeee'>");
            updateStatus.Append("<tr>");
            updateStatus.Append("<td style='background-color:#175e87;color:#eeeeee'><strong>Kategorija</strong></td>");
            updateStatus.Append("<td style='background-color:#175e87;color:#eeeeee'><strong>Ukupno novih</strong></td>");
            updateStatus.Append("<td style='background-color:#175e87;color:#eeeeee'><strong>Ukupno ažuriranih</strong></td>");
            updateStatus.Append("</tr>");
            int index = 0;

            foreach (CategorySimple category in categories)
            {
                Common.log("Preuzimam selektovane Ewe kategorije za kategoriju: " + category.Name, true, logFilename);
                subcategories = string.Empty;
                string[] eweCategory = new EweBL().GetEweCategoryForCategory(category.CategoryID);
                int eweCategoryID = int.Parse(eweCategory[0]);
                string eweCategoryName = eweCategory[1];
                List<CategorySimple> eweSubCategories = new EweBL().GetEweSubCategories(category.CategoryID, eweCategoryID);
                

                if (eweSubCategories.Count > 0)
                {
                    foreach (CategorySimple eweSubCategory in eweSubCategories)
                        subcategories += eweSubCategory.Name + "|";
                    Common.log("Selektovane Ewe kategorije: " + subcategories.Substring(0, subcategories.Length - 1), true, logFilename);

                    string[] status = new EweBL().ParseProductsForSaving(eweCategoryName, subcategories.Substring(0, subcategories.Length - 1).Split('|'), category.CategoryID, logFilename, eweCategoryID);
                    updateStatus.Append("<tr>");
                    updateStatus.Append("<td style='padding:0.5em" + (index % 2 == 0 ? ";background-color:#f8f8f8" : "") + "'>" + "<a href='" + ConfigurationManager.AppSettings["webshopAdminUrl"] + "/getProducts.aspx?categoryID=" + category.CategoryID + "'>" + category.Name.PadLeft(50) + "</a>" + "</td>" + "<td style='padding:0.5em" + (index % 2 == 0 ? ";background-color:#f8f8f8" : "") + "'>" + "<a href='" + ConfigurationManager.AppSettings["webshopAdminUrl"] + "/getProducts.aspx?categoryID=" + category.CategoryID + "'>" + status[0] + "</a>" + "</td>" + "<td style='padding:0.5em" + (index % 2 == 0 ? ";background-color:#f8f8f8" : "") + "'>" + "<a href='" + ConfigurationManager.AppSettings["webshopAdminUrl"] + "/getProducts.aspx?categoryID=" + category.CategoryID + "'>" + status[1] + "</a>" + "</td>");
                    updateStatus.Append("</tr>");
                }
                else
                { 
                    Common.log("Nema selektovanih Ewe kategorija za kategoriju " + category.Name, true, logFilename);
                    Common.log("-----------------", true, logFilename);
                    updateStatus.Append("<tr>");
                    updateStatus.Append("<td style='padding:0.5em" + (index % 2 == 0 ? ";background-color:#f8f8f8" : "") + "'>" + "<a href='" + ConfigurationManager.AppSettings["webshopAdminUrl"] + "/getProducts.aspx?categoryID=" + category.CategoryID + "'>" + category.Name.PadLeft(50) + "</a>" + "</td>" + "<td style='padding:0.5em" + (index % 2 == 0 ? ";background-color:#f8f8f8" : "") + "' colspan='2'>Nema selektovanih Ewe kategorija</td>");
                    updateStatus.Append("</tr>");
                }
                index++;
            }
            updateStatus.Append("</table>");
            updateStatus.Append("<p>Ažuriranje završeno: " + DateTime.Now.ToString() + "</p>");
            Common.sendMail(updateStatus.ToString(), "success", "Ažuriranje proizvoda uspešno završeno");
        }

        private static void save3gProducts()
        {
            new _3gBL().SaveProducts();
        }

        

        

        

        

        

        

        
    }
}
