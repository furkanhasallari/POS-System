using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web;
namespace POS_System
{
    public partial class Login : System.Web.UI.Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Ju lutem plotësoni të gjitha fushat!";
                lblError.Visible = true;
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["POSConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT Role FROM Users WHERE Username=@Username AND Password=@Password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                try
                {
                    con.Open();
                    string role = cmd.ExecuteScalar()?.ToString();

                    if (!string.IsNullOrEmpty(role))
                    {
                        // Shtimi i Cookie për të mbajtur mend përdoruesin
                        HttpCookie userCookie = new HttpCookie("POSUserSettings");
                        userCookie["Username"] = username;
                        userCookie["LastLogin"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        userCookie.Expires = DateTime.Now.AddDays(30); // Ruaj për 30 ditë
                        Response.Cookies.Add(userCookie);

                        // Vendosja e Session
                        Session["UserRole"] = role;
                        Session["Username"] = username;

                        // Redirect bazuar në rol
                        if (role == "Admin")
                            Response.Redirect("AdminDashboard.aspx");
                        else
                            Response.Redirect("PuntorDashboard.aspx");
                    }
                    else
                    {
                        lblError.Text = "Username ose Password i gabuar!";
                        lblError.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    lblError.Text = "Gabim në lidhje me databazën: " + ex.Message;
                    lblError.Visible = true;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Kontrollo nëse ekziston cookie për username dhe plotëso automatikisht
            if (!IsPostBack && Request.Cookies["POSUserSettings"] != null)
            {
                HttpCookie cookie = Request.Cookies["POSUserSettings"];
                txtUsername.Text = cookie["Username"];
            }
        }
    }
}