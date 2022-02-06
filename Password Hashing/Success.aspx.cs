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
using System.IO;

namespace Password_Hashing
{
    public partial class Success : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        static byte[] Key = null;
        static byte[] IV = null;
        string email = null;
        byte[] creditcard = null;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    email = (string)Session["LoggedIn"];
                    lbl_Message.Text = "You are logged in!";
                    lbl_Message.ForeColor = System.Drawing.Color.Green;
                    LogoutBtn.Visible = true;
                    ChangePassBtn.Visible = true;

                    displayProfile(email);
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }

            string userstatus = Request.QueryString?["UserVerification"]?.ToString();
            if (userstatus != null)
            {
                if (userstatus.ToUpper() == "PASSWORDCHANGED")
                {
                    lbl_Message.Text = "Your password has been changed.";
                    //lbl_Message.ForeColor = Color.Green;
                }
            }
        }

        protected void LogoutOfAccount
            (object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("Login.aspx", false);

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
        protected string decryptData(byte[] cipherText)
        {
            string plainText = null;

            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();

                        }
                    }
                }
            }


            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return plainText;
        }
        protected void displayProfile(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * FROM [User] WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Email"] != DBNull.Value)
                        {
                            lbl_email.Text = reader["Email"].ToString();

                            //cipherTextNRIC = (byte[])reader["Nric"];
                        }
                        if (reader["FirstName"] != DBNull.Value)
                        {
                            lbl_fname.Text = reader["FirstName"].ToString();
                        }

                        if (reader["LastName"] != DBNull.Value)
                        {
                            lbl_lname.Text = reader["LastName"].ToString();
                        }

                        if (reader["CreditCardInfo"] != DBNull.Value)
                        {
                            creditcard = Convert.FromBase64String(reader["CreditCardInfo"].ToString());
                        }
                        if (reader["IV"] != DBNull.Value)
                        {
                            IV = Convert.FromBase64String(reader["IV"].ToString());

                            //cipherTextNRIC = (byte[])reader["Nric"];
                        }
                        if (reader["Key"] != DBNull.Value)
                        {
                            Key = Convert.FromBase64String(reader["Key"].ToString());

                            //cipherTextNRIC = (byte[])reader["Nric"];
                        }
                    }
                    lbl_credit.Text = decryptData(creditcard);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally
            {
                connection.Close();
            }
        }
        protected void ChangeOfPassword(object sender, EventArgs e)
        {
            email = (string)Session["LoggedIn"];
            DateTime TimeNow = DateTime.Now;
            DateTime MinimumPasswordLongetivity = default;

            SqlConnection con = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT MinimumPasswordLongetivity FROM [User] WHERE Email=@Email";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Email", email);
            try
            {
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        if (reader["MinimumPasswordLongetivity"] != DBNull.Value)
                        {
                            MinimumPasswordLongetivity = Convert.ToDateTime(reader["MinimumPasswordLongetivity"].ToString());
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

            int diff = DateTime.Compare(TimeNow, MinimumPasswordLongetivity);

            if (diff > 0)
            {
                Session["LoggedIn"] = lbl_email.Text;
                Response.Redirect("ChangePassword.aspx", false);
            }
            else
            {
                lbl_Message.Text = ("Password Change cooldown time is 1 minute");

            }
        }

    }

    
}