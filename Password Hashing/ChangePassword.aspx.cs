using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Password_Hashing
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        static string NewHash;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Success.aspx?status=passwordchanged");
                }

            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }

            string userstatus = Request.QueryString?["UserVerification"]?.ToString();
            if (userstatus != null)
            {
                if (userstatus.ToUpper() == "PASSWORDEXPIRED")
                {
                    lbl_error.Text = "Your password has expired, please key in a new password / update your password.";
                    //lbl_error.ForeColor = Color.Green;
                }
            }
        }

        protected void OnClickBtnChangePass(object sender, EventArgs e)
        {
            // old same as current
            lbl_oldpassword_errors.Text = String.Empty;

            bool validInput = ValidateInput();

            if (validInput)
            {
                string email = (string)Session["LoggedIn"];
                string currentpassword = HttpUtility.HtmlEncode(tb_oldpassword.Text).ToString().Trim();
                string newpassword = HttpUtility.HtmlEncode(tb_newpassword.Text).ToString().Trim();

                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(email);
                string dbSalt = getDBSalt(email);
                string dbHashold = getoldHash(email);

                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string passwithSalt = currentpassword + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(passwithSalt));
                    string currentHash = Convert.ToBase64String(hashWithSalt);

                    if (currentHash.Equals(dbHash))
                    {
                        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                        byte[] saltByte = new byte[8];
                        //Fills array of bytes with a cryptographically strong sequence of random values.
                        rng.GetBytes(saltByte);

                        string newpasswordWithSalt = newpassword + dbSalt;
                        byte[] newhashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(newpasswordWithSalt));
                        NewHash = Convert.ToBase64String(newhashWithSalt);

                        if (dbHashold != null)
                        {
                            if (NewHash.Equals(dbHashold))
                            {
                                lbl_cfmpassword_errors.Text = "New password cannot be similar to previous 2 passwords";
                                //lbl_pwd_errors.ForeColor = Color.Red;
                            }
                            else
                            {
                                updatePassword(email);
                                Response.Redirect("Success.aspx?status=passwordchanged");
                            }
                        }
                        else
                        {
                            if (newpassword == currentpassword)
                            {
                                lbl_cfmpassword_errors.Text = "New password cannot be similar to current password";
                                //lbl_newpassword_errors.ForeColor = Color.Red;
                            }
                            else
                            {
                                updatePassword(email);
                                Response.Redirect("Success.aspx?status=passwordchanged");
                            }
                        }
                    }
                    else
                    {
                        lbl_oldpassword_errors.Text = "Password is incorrect";

                    }
                }
            }

        }

        private int VerifyPassword(string password)
        {
            int score = 0;

            // Very Weak
            if (password.Length < 8)
            {
                return 1;
            }
            else
            {
                score = 1;
            }
            // Weak
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }
            //Medium
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }
            // Strong
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }
            // Very Strong
            if (Regex.IsMatch(password, "[!@#$%^&*]"))
            {
                score++;
            }

            return score;

        }

        private bool ValidateInput()
        {

            lbl_newpassword_errors.Text = String.Empty;
            lbl_cfmpassword_errors.Text = String.Empty;

            string oldpassword = HttpUtility.HtmlEncode(tb_oldpassword.Text).ToString().Trim();
            string password = HttpUtility.HtmlEncode(tb_newpassword.Text).ToString().Trim();
            string confirmpassword = HttpUtility.HtmlEncode(tb_confirmnewpassword.Text).ToString().Trim();

            if (password == "")
            {
                lbl_newpassword_errors.Text += "Password is required!";
            }
            else
            {

                int scores = VerifyPassword(password);
                string status = "";
                switch (scores)
                {
                    case 1:
                        status = "Please enter a stronger password!";
                        break;
                    case 2:
                        status = "Please enter a stronger password!";
                        break;
                    case 3:
                        status = "Please enter a stronger password!";
                        break;
                    case 4:
                        status = "";
                        break;
                    case 5:
                        status = "";
                        break;
                    default:
                        break;
                }
                lbl_newpassword_errors.Text = status;
                if (scores < 4)
                {
                    //lbl_newpassword_errors.ForeColor = Color.Red;
                    //tb_pwd.BorderColor = Color.Red;

                }
            }

            if (confirmpassword == "")
            {
                lbl_cfmpassword_errors.Text += "Confirm Password is required!";
            }
            else if (password != confirmpassword)
            {
                lbl_cfmpassword_errors.Text += "Password does not match!";
            }

            if (oldpassword == "")
            {
                lbl_newpassword_errors.Text += "Confirm Password is required!";
            }
            else if (oldpassword == confirmpassword)
            {
                lbl_newpassword_errors.Text = "New password cannot be similar to current password";
                lbl_cfmpassword_errors.Text = "New password cannot be similar to current password";
                //lbl_pwd_errors.ForeColor = Color.Red;
            }

            if (String.IsNullOrEmpty(lbl_newpassword_errors.Text + lbl_cfmpassword_errors.Text + lbl_oldpassword_errors.Text))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //db hash + salt
        protected string getDBHash(string email)
        {
            string hash = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM [User] WHERE Email= @Email";
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
                                hash = reader["PasswordHash"].ToString();
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
            return hash;
        }

        protected string getDBSalt(string email)
        {
            string salt = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordSalt FROM [User] WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordSalt"] != null)
                        {
                            if (reader["PasswordSalt"] != DBNull.Value)
                            {
                                salt = reader["PasswordSalt"].ToString();
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
            return salt;
        }
        //oldpasswordhash here
        protected string getoldHash(string email)
        {
            string hash = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select OldPasswordHash FROM [User] WHERE Email= @Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["OldPasswordHash"] != null)
                        {
                            if (reader["OldPasswordHash"] != DBNull.Value)
                            {
                                hash = reader["OldPasswordHash"].ToString();
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
            return hash;
        }
        //oldpasswordhash
        protected void updatePassword(string email)
        {
            try
            {
                SqlConnection con = new SqlConnection(MYDBConnectionString);
                string sql = "UPDATE [User] set PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt, OldPasswordHash = @OldPasswordHash, MinimumPasswordLongetivity =@MinimumPasswordLongetivity, MaximumPasswordLongetivity =@MaximumPasswordLongetivity WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Email", email);

                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@PasswordHash", NewHash);
                    cmd.Parameters.AddWithValue("@PasswordSalt", getDBSalt(email));
                    cmd.Parameters.AddWithValue("@OldPasswordHash", getDBHash(email));
                    cmd.Parameters.AddWithValue("@MinimumPasswordLongetivity", DateTime.Now.AddMinutes(1));
                    cmd.Parameters.AddWithValue("@MaximumPasswordLongetivity", DateTime.Now.AddMinutes(3));


                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

    }
}
