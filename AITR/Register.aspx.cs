using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AITR
{
    public partial class Register : System.Web.UI.Page
    {
        // connection string setup
        string _myConnectionString = ConfigurationManager.ConnectionStrings["myConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (_myConnectionString.Equals("dev"))
            {
                _myConnectionString = AppConstants.DevConnectionString;
            }

            // handle whether to show to the register page depending on whether the user is a member yet or not
            if (Session["isRegistered"] == null || !(bool)Session["isRegistered"])
            {
                // show form if user isn NOT a member
                registerForm.Visible = true;
                notLoggedInLabel.Visible = false;
            }
            else
            {
                // hide form is user IS a member
                registerForm.Visible = false;
                notLoggedInLabel.Visible = true;
            }
        }


        /// <summary>
        /// Inserts (validated) user input into the Member table referencing a Respondent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void submitBtn_Click(object sender, EventArgs e)
        {
            // clear errror message
            errMsgLabel.Text = "";

            // get user input
            string firstName = fNameInput.Text.Trim();
            string lastName = lNameInput.Text.Trim();
            string phoneNumber = phoneNumberInput.Text.Trim();
            string dobStringInput = dobInput.Text.Trim();
            int respondentId = Convert.ToInt32(Session["respondentID"]);

            // make sure all fields have an input
            if (!ValidateInputs(firstName, lastName, phoneNumber, dobStringInput, out string errorMessage))
            {
                System.Diagnostics.Debug.WriteLine($"not valid");
                errMsgLabel.Text = errorMessage;
                errMsgLabel.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // add new member to DB
            try
            {
                using (SqlConnection connection = new SqlConnection(_myConnectionString))
                {
                    connection.Open();

                    // check if respondent exists
                    string checkRespondentQ = "SELECT COUNT(*) FROM Respondent WHERE RPT_ID = @RPT_ID";
                    using (SqlCommand checkRespondentCmd = new SqlCommand(checkRespondentQ, connection))
                    {
                        checkRespondentCmd.Parameters.AddWithValue("@RPT_ID", respondentId);
                        int count = Convert.ToInt32(checkRespondentCmd.ExecuteScalar());

                        // respondent exists -> isMember = true
                        if (count > 0)
                        {
                            string updateRespondentQ = "UPDATE Respondent SET isMember = 1 WHERE RPT_ID = @RPT_ID";
                            using (SqlCommand updateRespondentCmd = new SqlCommand(updateRespondentQ, connection))
                            {
                                updateRespondentCmd.Parameters.AddWithValue("@RPT_ID", respondentId);
                                updateRespondentCmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // respondent !exists -> create respondent
                            string createRespondentQ = "INSERT INTO Respondent (isMember) VALUES (1)";
                            using (SqlCommand createRespondentCmd = new SqlCommand(createRespondentQ, connection))
                            {
                                createRespondentCmd.ExecuteNonQuery();
                            }
                        }
                    }

                    // create member referencing respondent
                    string createMemberQ = @"
                        INSERT INTO Member (RPT_ID, FirstName, LastName, DoB, PhoneNumber) 
                        VALUES (@RPT_ID, @FirstName, @LastName, @DoB, @PhoneNumber)";

                    using (SqlCommand command = new SqlCommand(createMemberQ, connection))
                    {
                        // parameterised for sql injection protection
                        command.Parameters.AddWithValue("@RPT_ID", respondentId);
                        command.Parameters.AddWithValue("@FirstName", firstName);
                        command.Parameters.AddWithValue("@LastName", lastName);
                        command.Parameters.AddWithValue("@DoB", Convert.ToDateTime(dobStringInput));
                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            errMsgLabel.Text = "You are now a Member!";
                            errMsgLabel.ForeColor = System.Drawing.Color.Green;

                            // update session variable
                            Session["isRegistered"] = true;
                            // route to home
                            Response.Redirect("Default.aspx");
                        }
                        else
                        {
                            errMsgLabel.Text = "Registration failed";
                            errMsgLabel.ForeColor = System.Drawing.Color.Red;
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                errMsgLabel.Text = $"Error occurred: {ex.Message}";
                errMsgLabel.ForeColor = System.Drawing.Color.Red;
            }
        }


        /// <summary>
        /// Validates each input to not be empty or invalid, gives an appropriate message if not
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="dob"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        private bool ValidateInputs(string firstName, string lastName, string phoneNumber, string dob, out string errMsg)
        {
            errMsg = string.Empty;

            // fName validation
            if (string.IsNullOrWhiteSpace(firstName) || firstName.Length > 50)
            {
                errMsg = "First Name is required and must be under 50 characters.";
                return false;
            }

            // lName validation
            if (string.IsNullOrWhiteSpace(lastName) || lastName.Length > 50)
            {
                errMsg = "Last Name is required and must be under 50 characters.";
                return false;
            }

            // phone number validation (stack overflow for the win on regex lol)
            if (string.IsNullOrWhiteSpace(phoneNumber) || !Regex.IsMatch(phoneNumber, @"^\d{10}$"))
            {
                errMsg = "Phone Number is required and must be a valid 10-digit number.";
                return false;
            }

            // DoB validation
            if (!DateTime.TryParse(dob, out DateTime parsedDob))
            {
                errMsg = "Date of Birth is required and must be a valid date.";
                return false;
            }

            // age is at least 18
            //if ((DateTime.Now - parsedDob).TotalDays / 365 < 18)
            //{
            //    errMsg = "You must be at least 18 years old to register.";
            //    return false;
            //}

            return true;
        }
    }
}