using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AITR
{
    public partial class Login : System.Web.UI.Page
    {
        // connection string setup
        string _myConnectionString = ConfigurationManager.ConnectionStrings["myConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // idk
        }


        /// <summary>
        /// Takes user input and queries the DB to check if username and password match an existing user. Thein either successfully loggin in staff or showing a failed error message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void loginBtn_Click(object sender, EventArgs e)
        {
            string username = usernameInput.Text.Trim();
            string password = passwordInput.Text.Trim();

            // clear previous err msg
            errMsgLabel.Visible = false;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                // err msg if fields are empty
                ShowErrorMessage("Please fill in both username and password");
                return;
            }

            try
            {
                if (_myConnectionString.Equals("dev"))
                {
                    _myConnectionString = AppConstants.DevConnectionString;
                }
                using (SqlConnection connection = new SqlConnection(_myConnectionString))
                {
                    connection.Open();

                    // query for checking username and password
                    string query = "SELECT COUNT(*) FROM Staff WHERE Username = @Username AND Password = @Password";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // parameterised for sql injection protection
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        int userCount = (int)command.ExecuteScalar();

                        if (userCount > 0)
                        {
                            // success -> update staff logged in session
                            System.Diagnostics.Debug.WriteLine("success");
                            Session["staffLoggedIn"] = true;
                            // route to home
                            Response.Redirect("StaffSearch.aspx");
                        }
                        else
                        {
                            // failed
                            System.Diagnostics.Debug.WriteLine("failed");
                            ShowErrorMessage("Your username or password are invalid");
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                // log err
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                ShowErrorMessage("An unexpected error occurred");
            }
        }


        /// <summary>
        /// Takes a string message input to set and show the error message text on the form
        /// </summary>
        /// <param name="message"></param>
        private void ShowErrorMessage(string message)
        {
            // add error label to page to put error msg in
            errMsgLabel.Text = message;
            errMsgLabel.Visible = true;
        }
    }
}