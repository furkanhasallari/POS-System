using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POS_System
{
    public partial class Products : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
                Response.Redirect("Login.aspx");

            if (!IsPostBack)
                LoadProducts();
        }

        private void LoadProducts()
        {
            string connStr = ConfigurationManager.ConnectionStrings["POSConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Products ORDER BY Name", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvProducts.DataSource = dt;
                gvProducts.DataBind();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            pnlMessage.Visible = false;
            pnlError.Visible = false;

            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtPrice.Text) ||
                string.IsNullOrWhiteSpace(txtQuantity.Text))
            {
                ShowError("Ju lutem plotësoni të gjitha fushat!");
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price))
            {
                ShowError("Çmimi nuk është i vlefshëm.");
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity))
            {
                ShowError("Sasia nuk është e vlefshme.");
                return;
            }

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["POSConnection"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = "INSERT INTO Products (Name, Price, Quantity) VALUES (@Name, @Price, @Quantity)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadProducts();
                txtName.Text = txtPrice.Text = txtQuantity.Text = "";
                ShowSuccess("Produkti u shtua me sukses!");
            }
            catch (Exception ex)
            {
                ShowError("Gabim në shtimin e produktit: " + ex.Message);
            }
        }

        protected void gvProducts_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvProducts.EditIndex = e.NewEditIndex;
            LoadProducts();
        }

        protected void gvProducts_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvProducts.EditIndex = -1;
            LoadProducts();
        }

        protected void gvProducts_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(gvProducts.DataKeys[e.RowIndex].Value);
                string name = ((TextBox)gvProducts.Rows[e.RowIndex].Cells[1].Controls[0]).Text;
                decimal price = Convert.ToDecimal(((TextBox)gvProducts.Rows[e.RowIndex].Cells[2].Controls[0]).Text);
                int quantity = Convert.ToInt32(((TextBox)gvProducts.Rows[e.RowIndex].Cells[3].Controls[0]).Text);

                string connStr = ConfigurationManager.ConnectionStrings["POSConnection"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Products SET Name=@Name, Price=@Price, Quantity=@Quantity WHERE Id=@Id", con);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                gvProducts.EditIndex = -1;
                LoadProducts();
                ShowSuccess("Produkti u përditësua me sukses!");
            }
            catch (Exception ex)
            {
                ShowError("Gabim në përditësimin e produktit: " + ex.Message);
            }
        }

        protected void gvProducts_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(gvProducts.DataKeys[e.RowIndex].Value);

                string connStr = ConfigurationManager.ConnectionStrings["POSConnection"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Products WHERE Id=@Id", con);
                    cmd.Parameters.AddWithValue("@Id", id);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadProducts();
                ShowSuccess("Produkti u fshi me sukses!");
            }
            catch (Exception ex)
            {
                ShowError("Gabim në fshirjen e produktit: " + ex.Message);
            }
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