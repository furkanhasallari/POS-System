using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

namespace POS_System
{
    public partial class PuntorDashboard : Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["POSConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Puntor")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblUsername.Text = Session["Username"]?.ToString() ?? "Puntor";
                LoadProducts();
                InitializeSaleTable();
            }
        }

        private void LoadProducts()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT Id, Name FROM Products WHERE Quantity > 0 ORDER BY Name", con);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlProducts.DataSource = reader;
                    ddlProducts.DataTextField = "Name";
                    ddlProducts.DataValueField = "Id";
                    ddlProducts.DataBind();

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                ShowError("Gabim në ngarkimin e produkteve: " + ex.Message);
            }
        }

        private void InitializeSaleTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ProductId", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Quantity", typeof(int));
            dt.Columns.Add("Price", typeof(decimal));
            dt.Columns.Add("Total", typeof(decimal));
            Session["SaleItems"] = dt;
        }

        protected void btnAddToSale_Click(object sender, EventArgs e)
        {
            pnlMessage.Visible = false;
            pnlError.Visible = false;

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                ShowError("Ju lutem shkruani një sasi të vlefshme");
                return;
            }

            int productId = int.Parse(ddlProducts.SelectedValue);

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT Name, Price FROM Products WHERE Id = @Id", con);
                    cmd.Parameters.AddWithValue("@Id", productId);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string name = reader["Name"].ToString();
                        decimal price = Convert.ToDecimal(reader["Price"]);
                        decimal total = price * quantity;

                        DataTable dt = (DataTable)Session["SaleItems"];
                        dt.Rows.Add(productId, name, quantity, price, total);
                        Session["SaleItems"] = dt;

                        gvSaleItems.DataSource = dt;
                        gvSaleItems.DataBind();

                        CalculateTotal();
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                ShowError("Gabim në shtimin e produktit në shitje: " + ex.Message);
            }
        }

        private void CalculateTotal()
        {
            DataTable dt = (DataTable)Session["SaleItems"];
            decimal total = 0;

            foreach (DataRow row in dt.Rows)
            {
                total += Convert.ToDecimal(row["Total"]);
            }

            lblTotal.Text = total.ToString("0.00");
        }

        protected void btnCompleteSale_Click(object sender, EventArgs e)
        {
            DataTable saleItems = (DataTable)Session["SaleItems"];
            if (saleItems == null || saleItems.Rows.Count == 0)
            {
                ShowError("Nuk ka produkte për të kryer shitjen");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();

                    try
                    {
                        SqlCommand cmd = new SqlCommand(
                            "INSERT INTO Sales ([Date], TotalAmount) OUTPUT INSERTED.SaleID VALUES (@Date, @Total)",
                            con, transaction);

                        cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Total", Convert.ToDecimal(lblTotal.Text));

                        int saleId = (int)cmd.ExecuteScalar();

                        foreach (DataRow row in saleItems.Rows)
                        {
                            SqlCommand detailCmd = new SqlCommand(
                                "INSERT INTO SalesDetails (SaleID, ProductID, Quantity, Price) VALUES (@SaleID, @ProductID, @Quantity, @Price)",
                                con, transaction);

                            detailCmd.Parameters.AddWithValue("@SaleID", saleId);
                            detailCmd.Parameters.AddWithValue("@ProductID", row["ProductId"]);
                            detailCmd.Parameters.AddWithValue("@Quantity", row["Quantity"]);
                            detailCmd.Parameters.AddWithValue("@Price", row["Price"]);
                            detailCmd.ExecuteNonQuery();

                            SqlCommand updateStock = new SqlCommand(
                                "UPDATE Products SET Quantity = Quantity - @Quantity WHERE Id = @ProductId",
                                con, transaction);

                            updateStock.Parameters.AddWithValue("@Quantity", row["Quantity"]);
                            updateStock.Parameters.AddWithValue("@ProductId", row["ProductId"]);
                            updateStock.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        ShowSuccess($"Shitja u krye me sukses! ID e shitjes: {saleId}");

                        InitializeSaleTable();
                        gvSaleItems.DataSource = null;
                        gvSaleItems.DataBind();
                        lblTotal.Text = "0.00";
                        LoadProducts();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ShowError("Gabim në kryerjen e shitjes: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Gabim në lidhjen me databazën: " + ex.Message);
            }
        }

        protected void btnPrintInvoice_Click(object sender, EventArgs e)
        {
            DataTable saleItems = (DataTable)Session["SaleItems"];
            if (saleItems == null || saleItems.Rows.Count == 0)
            {
                ShowError("Nuk ka produkte për të printuar faturën");
                return;
            }

            lblInvoiceNumber.Text = DateTime.Now.Ticks.ToString();
            lblInvoiceDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            lblCashierName.Text = Session["Username"]?.ToString() ?? "Puntor";

            gvInvoiceItems.DataSource = saleItems;
            gvInvoiceItems.DataBind();

            decimal total = 0;
            foreach (DataRow row in saleItems.Rows)
            {
                total += Convert.ToDecimal(row["Total"]);
            }
            lblInvoiceTotal.Text = total.ToString("N2");

            ClientScript.RegisterStartupScript(this.GetType(), "PrintInvoice", "printInvoice()", true);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("Login.aspx");
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