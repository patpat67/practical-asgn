using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;

namespace Password_Hashing
{
    public partial class Login : System.Web.UI.Page
    {

        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        //static string errorMsg = "";
        protected void Page_Load(object sender, EventArgs e)
        {


            if (!IsPostBack)
            {
                Session.Clear();
                Session.Abandon();
                Session.RemoveAll();

                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                    Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
                }

                if (Request.Cookies["AuthToken"] != null)
                {
                    Response.Cookies["AuthToken"].Value = string.Empty;
                    Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
                }
            }
            string statString = Request.QueryString?["UserVerification"]?.ToString();
            if (statString != null)
            {
                if (statString.ToUpper() == "REGISTERED")
                {
                    lb_error.Text = "Your account has been registered, please login to continue.";
                    //lb_error.ForeColor = Color.Green;
                }
            }

        }

        protected void btn_Login_Submit(object sender, EventArgs e)
        {
            //if (VALIDATECAPTCHA())
            //{
                string email = HttpUtility.HtmlEncode(tb_emaddr.Text).ToString().Trim();
                string password = HttpUtility.HtmlEncode(tb_pwd.Text).ToString().Trim();

                int UserVerification = 0;
                DateTime MaxPasswordAge = default;
                DateTime TimeNow = DateTime.Now;

                SqlConnection con = new SqlConnection(MYDBConnectionString);
                string sql = "SELECT UserVerification, MaximumPasswordLongetivity FROM [User] WHERE Email=@Email";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Email", email);
                try
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["UserVerification"] != DBNull.Value)
                            {
                                UserVerification = Convert.ToInt32(reader["UserVerification"].ToString());
                            }

                            if (reader["MaximumPasswordLongetivity"] != DBNull.Value)
                            {
                                MaxPasswordAge = Convert.ToDateTime(reader["MaximumPasswordLongetivity"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }

                finally
                {
                    con.Close();
                }
                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(email);
                string dbSalt = getDBSalt(email);

                if (dbHash == null)
                {
                    lb_error.Text = "Email or Password is not valid. Please try again.";
                    //lb_error.ForeColor = Color.Red;
                }
                try
                {
                    if (UserVerification < 3)
                    {
                        if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                        {
                            string pwdWithSalt = password + dbSalt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);

                            if (userHash.Equals(dbHash))
                            {

                                SucessLogin(email);

                                Session["LoggedIn"] = email;

                                string guid = Guid.NewGuid().ToString();
                                Session["AuthToken"] = guid;

                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                int diff = DateTime.Compare(TimeNow, MaxPasswordAge);

                                if (diff > 100)
                                {
                                    Response.Redirect("ChangePass.aspx?status=passwordexpired", false);
                                }
                                else
                                {
                                    Response.Redirect("Success.aspx", false);
                                }
                            }
                            else
                            {
                                unsucessfulLogin(email);
                                lb_error.Text = "Email or Password is not valid. Please try again.";
                                //lb_error.ForeColor = Color.Red;
                            }
                        }
                    }
                    else
                    {
                        lb_error.Text = "Account is locked.";
                        //lb_error.ForeColor = Color.Red;
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                finally
                {

                }
            //}
        }
        //password salt
        //changed user id to email
        protected string getDBSalt(string email)
        {

            string saltvalue = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PASSWORDSALT FROM [User] WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            //changed this from user id to email ( based off practical 6 ) cuz email can be used as a unique identifier
            command.Parameters.AddWithValue("@Email", email);

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALT"] != null)
                        {
                            if (reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                saltvalue = reader["PASSWORDSALT"].ToString();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { connection.Close(); }
            return saltvalue;

        }

        protected string getDBHash(string email)
        {

            string hashvalue = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM [User] WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                hashvalue = reader["PasswordHash"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { connection.Close(); }
            return hashvalue;
        }
        //captcha
        public bool VALIDATECAPTCHA()
        {
            bool results = true;

            string captchaResponse = Request.Form["g-recaptcha-response"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
                ("https://www.google.com/recaptcha/api/siteverify?secret=SECRETKEY &response=" + captchaResponse);

            try
            {
                using (WebResponse webResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(webResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);
                        results = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return results;
            }
            catch (WebException ex)
            {
                //System.Diagnostics.Debug.WriteLine("test4");
                //throw ex;
                //Response.Redirect("~/CustomError/Error500", true);
                Response.StatusCode = 500;
                Response.Flush();
                Response.End();
                return results;
            }
        }

        //both salt and hash functions practical 6 have.
        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }

        }
        protected void SucessLogin(string email)
        {
            SqlConnection con = new SqlConnection(MYDBConnectionString);
            string sql = "UPDATE [User] set UserVerification = 0 WHERE EMAIL = @EMAIL";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("Email", email);

            using (SqlDataAdapter sda = new SqlDataAdapter())
            {
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        protected void unsucessfulLogin(string email)
        {
            SqlConnection con = new SqlConnection(MYDBConnectionString);
            string sql = "UPDATE [User] set UserVerification = UserVerification + 1 WHERE EMAIL = @EMAIL";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("Email", email);
            using (SqlDataAdapter sda = new SqlDataAdapter())
            {
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        protected void btn_Register_Submit(object sender, EventArgs e)
        {
            Response.Redirect("Registration.aspx");
        }


    }
}