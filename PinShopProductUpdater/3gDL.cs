using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;

namespace PinShopProductUpdater
{
    public class _3gDL
    {
        public void SaveProducts(DataTable products)
        {
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(objConn))
                {
                    objConn.Open();
                    sqlBulkCopy.BatchSize = 1000;
                    sqlBulkCopy.BulkCopyTimeout = 3600;
                    sqlBulkCopy.DestinationTableName = "dbo.threegProduct";
                    sqlBulkCopy.ColumnMappings.Add(0, "id");
                    sqlBulkCopy.ColumnMappings.Add(1, "sifra");
                    sqlBulkCopy.ColumnMappings.Add(2, "naziv");
                    sqlBulkCopy.ColumnMappings.Add(3, "kategorija1");
                    sqlBulkCopy.ColumnMappings.Add(4, "kategorija2");
                    sqlBulkCopy.ColumnMappings.Add(5, "kategorija3");
                    sqlBulkCopy.ColumnMappings.Add(6, "vpCena");
                    sqlBulkCopy.ColumnMappings.Add(7, "mpCena");
                    sqlBulkCopy.ColumnMappings.Add(8, "rabat");
                    sqlBulkCopy.ColumnMappings.Add(9, "dostupan");
                    sqlBulkCopy.ColumnMappings.Add(10, "naAkciji");
                    sqlBulkCopy.ColumnMappings.Add(11, "opis");
                    sqlBulkCopy.ColumnMappings.Add(12, "barkod");
                    sqlBulkCopy.ColumnMappings.Add(13, "slike");

                    sqlBulkCopy.WriteToServer(products);
                }
            }
        }
    }
}
