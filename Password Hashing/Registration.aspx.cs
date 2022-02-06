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
using System.Text.RegularExpressions;
using System.IO;

namespace Password_Hashing
{
    public partial class Registration : System.Web.UI.Page
    {
        // get db connection string from add="nane"
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;

        static string line = "\r";

        //static string isDebug = ConfigurationManager.AppSettings["isDebug"].ToString();

        //session
        protected void Page_Load(object sender, EventArgs e)
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

        override protected void OnInit(EventArgs e)
        {

            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            InitializeComponent();
            base.OnInit(e);
        }
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            this.Load += new System.EventHandler(this.Page_Load);
        }

        private void btnUpload_Click(object sender, System.EventArgs e)
        {
            string strFileName;
            string strFilePath;
            string strFolder;
            strFolder = Server.MapPath("./");
            // Get the name of the file that is posted.
            strFileName = oFile.PostedFile.FileName;
            strFileName = Path.GetFileName(strFileName);
            if (oFile.Value != "")
            {
                // Create the directory if it does not exist.
                if (!Directory.Exists(strFolder))
                {
                    Directory.CreateDirectory(strFolder);
                }
                // Save the uploaded file to the server.
                strFilePath = strFolder + strFileName;
                if (File.Exists(strFilePath))
                {
                    lblUploadResult.Text = strFileName + " already exists on the server!";
                }
                else
                {
                    oFile.PostedFile.SaveAs(strFilePath);
                    lblUploadResult.Text = strFileName + " has been successfully uploaded.";
                }
            }
            else
            {
                lblUploadResult.Text = "Click 'Browse' to select the file to upload.";
            }
            // Display the result of the upload.
            frmConfirmation.Visible = true;
        }
    

    protected void btn_Register_Click(object sender, EventArgs e)
        {
            bool RightInput = ValidateInput();

            if (RightInput)
            {
                string pwd = HttpUtility.HtmlEncode(tb_pwd.Text).ToString().Trim();

                //Generate random "salt" 
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] saltByte = new byte[8];
                //Fills array of bytes with a cryptographically strong sequence of random values.
                rng.GetBytes(saltByte);
                salt = Convert.ToBase64String(saltByte);
                SHA512Managed hashing = new SHA512Managed();
                string pwdWithSalt = pwd + salt;

                //byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));

                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                finalHash = Convert.ToBase64String(hashWithSalt);

                //lb_error1.Text = "Salt:" + salt;
                //lb_error2.Text = "Hash with salt:" + finalHash;

                RijndaelManaged cipher = new RijndaelManaged();
                cipher.GenerateKey();
                Key = cipher.Key;
                IV = cipher.IV;


                createAccount();

                Response.Redirect("Login.aspx?status=" + HttpUtility.UrlEncode("registered"));

            }
        }

        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                //ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { }
            return cipherText;
        }

        protected void createAccount()
        {
            string fname = HttpUtility.HtmlEncode(tb_fname.Text);
            string lname = HttpUtility.HtmlEncode(tb_lname.Text);
            string credit = HttpUtility.HtmlEncode(tb_cci.Text);
            string email = HttpUtility.HtmlEncode(tb_email.Text);
            string dob = HttpUtility.HtmlEncode(tb_dob.Text);

            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    //                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Users (Email, Fname, Lname, CreditCard, PasswordHash, PasswordSalt, Dob, Status, IV, Keys, MaxPasswordAge)" +
                    //"VALUES (@Email, @Fname, @Lname, @CreditCard, @PasswordHash, @PasswordSalt, @Dob, @Status, @IV, @Keys, @MaxPasswordAge)"))
                    // can remove mobile verified + email verified and add maxpassword age
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO [User] VALUES(@FirstName, @LastName,@CreditCardInfo,@Email,@PasswordHash,@PasswordSalt,@DateofBirth,@OldPasswordHash,@UserVerification,@MaximumPasswordLongetivity,@MinimumPasswordLongetivity,@IV,@Key)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            //trim removes all whitespaces etc
                            cmd.Parameters.AddWithValue("@FirstName", fname.Trim());
                            cmd.Parameters.AddWithValue("@LastName", lname.Trim());
                            cmd.Parameters.AddWithValue("@CreditCardInfo", Convert.ToBase64String(encryptData(credit.Trim())));
                            cmd.Parameters.AddWithValue("@Email", email.Trim());
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@DateofBirth", dob.Trim());
                            cmd.Parameters.AddWithValue("@OldPasswordHash", DBNull.Value);

                            cmd.Parameters.AddWithValue("@UserVerification", 0);
                            //cmd.Parameters.AddWithValue("@MaximumPasswordLongetivity", DateTime.Now.AddMinutes(3));
                            //datetime fuking error , used dbnull instead
                            cmd.Parameters.AddWithValue("@MaximumPasswordLongetivity", DBNull.Value);
                            cmd.Parameters.AddWithValue("@MinimumPasswordLongetivity", DBNull.Value);

                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));

                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        private bool VerifyEmail(String email)
        {
            //(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$") --> regex for emails
            bool IsValid = false;
            Regex r = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (r.IsMatch(email))
                IsValid = true;
            return IsValid;
        }

        private int VerifyPassword(string password)
        {
            int score = 0;

            // Very Weak
            //cuz min password 12 characters
            if (password.Length < 12)
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
            lbl_fname_errors.Text = String.Empty;
            lbl_lname_errors.Text = String.Empty;
            lbl_cc_errors.Text = String.Empty;
            lb_emaddr_errors.Text = String.Empty;
            // password
            lbl_pwd_error.Text = String.Empty;
            //confirm password
            lbl_cpassword_errors.Text = String.Empty;
            lbl_dob_errors.Text = String.Empty;

            string fname = HttpUtility.HtmlEncode(tb_fname.Text).ToString().Trim();
            string lname = HttpUtility.HtmlEncode(tb_lname.Text).ToString().Trim();
            string credit = HttpUtility.HtmlEncode(tb_cci.Text).ToString().Trim();
            string email = HttpUtility.HtmlEncode(tb_email.Text).ToString().Trim();
            string pwd = HttpUtility.HtmlEncode(tb_pwd.Text).ToString().Trim();
            string pwd2 = HttpUtility.HtmlEncode(tb_cpassword.Text).ToString().Trim();
            string dob = HttpUtility.HtmlEncode(tb_dob.Text).ToString().Trim();
            string repeatedemail = null;

            SqlConnection con = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT Email FROM [User] WHERE Email=@Email";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Email", email);
            try
            {
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        repeatedemail = reader["Email"].ToString();
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

            if (fname == "")
            {
                lbl_fname_errors.Text += "First Name is required!";
            }
            else if (!Regex.IsMatch(fname, "^[A-Za-z]+$"))
            {
                lbl_fname_errors.Text += "Only Alphabets allowed!";
            }

            if (lname == "")
            {
                lbl_lname_errors.Text += "Last name is required!";
            }
            else if (!Regex.IsMatch(lname, "^[A-Za-z]+$"))
            {
                lbl_lname_errors.Text += "Only Alphabets allowed!";
            }

            if (credit == "")
            {
                lbl_cc_errors.Text += "Credit Card is required!";
            }
            else if (!Regex.IsMatch(credit, "^[0-9]*$"))
            {
                lbl_cc_errors.Text += "Only Numbers allowed!";
            }

            if (email == "")
            {
                lb_emaddr_errors.Text += "Email is required!";
            }
            else if (VerifyEmail(email) == false)
            {
                lb_emaddr_errors.Text += "Enter Valid Email!";
            }
            else if (repeatedemail != null)
            {
                lb_emaddr_errors.Text += "Email already exists!" + "<br/>";
            }

            if (pwd == "")
            {
                lbl_pwd_error.Text += "Password is required!";

            }
            else
            {

                int scores = VerifyPassword(pwd);
                string status = "";
                switch (scores)
                {
                    case 1:
                        status = "Please enter a stronger password!\n" +
                            "at least 12 characters with a mixture of one special charac, eg .,!@#?\n" +
                            "mixture of letters and numbers";
                        break;
                    case 2:
                        //status = "Please enter a stronger password!";
                        status = "Please enter a stronger password!\n" +
                            "at least 12 characters with a mixture of one special charac, eg .,!@#?\n" +
                            "mixture of letters and numbers";
                        break;
                    case 3:
                        status = "Please enter a stronger password!\n" +
    "at least 12 characters with a mixture of one special charac, eg .,!@#?\n" +
    "mixture of letters and numbers";
                        //status = "Please enter a stronger password!";
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
                lbl_pwd_error.Text = status;
                if (scores < 4)
                {
                    //lbl_pwd_error.ForeColor = Color.Red;
                    //tb_pwd.BorderColor = Color.Red;

                }
            }

            if (pwd2 == "")
            {
                lbl_cpassword_errors.Text += "Confirm Password is required!";
            }
            else if (pwd != pwd2)
            {
                lbl_cpassword_errors.Text += "Password does not match!";
            }

            if (dob == "")
            {
                lbl_dob_errors.Text += "Date Of Birth is required!";
            }

            if (String.IsNullOrEmpty(lbl_fname_errors.Text + lbl_lname_errors.Text + lbl_cc_errors.Text + lb_emaddr_errors.Text + lbl_pwd_error.Text + lbl_cpassword_errors.Text + lbl_dob_errors.Text))
            {
                return true;
            }
            else
            {
                return false;
            }
        }



    }
}