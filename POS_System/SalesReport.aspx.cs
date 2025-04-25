using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POS_System
{
    public partial class SalesReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
                Response.Redirect("Login.aspx");
            if (!IsPostBack)
            {
                // Vendos datën e sotme si default
                txtFromDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                LoadSalesData();
            }
        }

        private void LoadSalesData()
        {
            DateTime fromDate = DateTime.Parse(txtFromDate.Text);
            DateTime toDate = DateTime.Parse(txtToDate.Text).AddDays(1); // Përfshin të gjithë ditën

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["POSConnection"].ConnectionString))
            {
                // Merr shitjet kryesore
                string query = @"SELECT SaleID, [Date], TotalAmount 
                                FROM Sales 
                                WHERE [Date] BETWEEN @FromDate AND @ToDate
                                ORDER BY [Date] DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@FromDate", fromDate);
                da.SelectCommand.Parameters.AddWithValue("@ToDate", toDate);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvSales.DataSource = dt;
                gvSales.DataBind();

                // Llogarit totalin e shitjeve
                decimal totalSales = 0;
                foreach (DataRow row in dt.Rows)
                {
                    totalSales += Convert.ToDecimal(row["TotalAmount"]);
                }
                lblTotalSales.Text = $"Totali i shitjeve: {totalSales:N2} €";
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadSalesData();
        }

        protected void btnDetails_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int saleId = Convert.ToInt32(btn.CommandArgument);
            lblSaleId.Text = saleId.ToString();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["POSConnection"].ConnectionString))
            {
                // Merr detajet e shitjes
                string query = @"SELECT sd.ProductID, p.Name, sd.Quantity, sd.Price, 
                                       (sd.Quantity * sd.Price) AS Total
                                FROM SalesDetails sd
                                JOIN Products p ON sd.ProductID = p.Id
                                WHERE sd.SaleID = @SaleID";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@SaleID", saleId);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvSaleDetails.DataSource = dt;
                gvSaleDetails.DataBind();
            }

            pnlDetails.Visible = true;
        }
    }
}