using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POS_System
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["POSConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblUsername.Text = Session["Username"]?.ToString() ?? "Admin";

                // Show products section by default
                productsSection.Visible = true;
                salesReportSection.Visible = false;

                LoadProducts();

                // Set default dates for report
                txtFromDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        protected void btnProducts_Click(object sender, EventArgs e)
        {
            productsSection.Visible = true;
            salesReportSection.Visible = false;
        }

        protected void btnSalesReport_Click(object sender, EventArgs e)
        {
            productsSection.Visible = false;
            salesReportSection.Visible = true;
            LoadSalesData();
        }

        private void LoadProducts()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Products ORDER BY Name", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvProducts.DataSource = dt;
                    gvProducts.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShowError("Gabim në ngarkimin e produkteve: " + ex.Message);
            }
        }

        private void LoadSalesData()
        {
            DateTime fromDate = DateTime.Parse(txtFromDate.Text);
            DateTime toDate = DateTime.Parse(txtToDate.Text).AddDays(1); // Include the entire day

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    // Get main sales
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

                    // Calculate total sales
                    decimal totalSales = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        totalSales += Convert.ToDecimal(row["TotalAmount"]);
                    }
                    lblTotalSales.Text = $"Totali i shitjeve: {totalSales:N2} €";
                }
            }
            catch (Exception ex)
            {
                ShowError("Gabim në ngarkimin e të dhënave të shitjeve: " + ex.Message);
            }
        }

        protected void btnDetails_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int saleId = Convert.ToInt32(btn.CommandArgument);
            lblSaleId.Text = saleId.ToString();

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    // Get sale details
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
            catch (Exception ex)
            {
                ShowError("Gabim në ngarkimin e detajeve të shitjes: " + ex.Message);
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadSalesData();
            pnlDetails.Visible = false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query;
                    SqlCommand cmd;

                    if (string.IsNullOrEmpty(hfProductId.Value))
                    {
                        query = "INSERT INTO Products (Name, Price, Quantity) VALUES (@Name, @Price, @Quantity)";
                    }
                    else
                    {
                        query = "UPDATE Products SET Name=@Name, Price=@Price, Quantity=@Quantity WHERE Id=@Id";
                    }

                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Price", Convert.ToDecimal(txtPrice.Text));
                    cmd.Parameters.AddWithValue("@Quantity", Convert.ToInt32(txtQuantity.Text));

                    if (!string.IsNullOrEmpty(hfProductId.Value))
                        cmd.Parameters.AddWithValue("@Id", Convert.ToInt32(hfProductId.Value));

                    con.Open();
                    cmd.ExecuteNonQuery();

                    string message = string.IsNullOrEmpty(hfProductId.Value)
                        ? "Produkti u shtua me sukses!"
                        : "Produkti u përditësua me sukses!";

                    ShowSuccess(message);
                    ResetForm();
                    LoadProducts();
                }
            }
            catch (Exception ex)
            {
                ShowError("Gabim në ruajtjen e produktit: " + ex.Message);
            }
        }

        protected void gvProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditProduct")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                int id = Convert.ToInt32(gvProducts.DataKeys[index].Value);

                try
                {
                    using (SqlConnection con = new SqlConnection(connStr))
                    {
                        SqlCommand cmd = new SqlCommand("SELECT * FROM Products WHERE Id=@Id", con);
                        cmd.Parameters.AddWithValue("@Id", id);
                        con.Open();
                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.Read())
                        {
                            hfProductId.Value = dr["Id"].ToString();
                            txtName.Text = dr["Name"].ToString();
                            txtPrice.Text = Convert.ToDecimal(dr["Price"]).ToString("0.00");
                            txtQuantity.Text = dr["Quantity"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowError("Gabim në editimin e produktit: " + ex.Message);
                }
            }
            else if (e.CommandName == "DeleteProduct")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                int id = Convert.ToInt32(gvProducts.DataKeys[index].Value);

                try
                {
                    using (SqlConnection con = new SqlConnection(connStr))
                    {
                        SqlCommand cmd = new SqlCommand("DELETE FROM Products WHERE Id=@Id", con);
                        cmd.Parameters.AddWithValue("@Id", id);
                        con.Open();
                        cmd.ExecuteNonQuery();

                        ShowSuccess("Produkti u fshi me sukses!");
                        LoadProducts();
                    }
                }
                catch (Exception ex)
                {
                    ShowError("Gabim në fshirjen e produktit: " + ex.Message);
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("Login.aspx");
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowError("Ju lutem shkruani emrin e produktit");
                return false;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                ShowError("Ju lutem shkruani një çmim të vlefshëm");
                return false;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                ShowError("Ju lutem shkruani një sasi të vlefshme");
                return false;
            }

            return true;
        }

        private void ResetForm()
        {
            hfProductId.Value = "";
            txtName.Text = "";
            txtPrice.Text = "";
            txtQuantity.Text = "";
        }

        private void ShowSuccess(string message)
        {
            pnlMessage.Visible = true;
            lblMessage.Text = message;
            pnlError.Visible = false;
        }

        private void ShowError(string error)
        {
            pnlError.Visible = true;
            lblError.Text = error;
            pnlMessage.Visible = false;
        }
    }
}