using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AITR
{
    public partial class StaffSearch : System.Web.UI.Page
    {
        // connection string setup
        string _myConnectionString = ConfigurationManager.ConnectionStrings["myConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (_myConnectionString.Equals("dev"))
            {
                _myConnectionString = AppConstants.DevConnectionString;
            }

            // handle whether to show to the staff search form depending on if the user is logged in as staff or not
            if (Session["staffLoggedIn"] == null || !(bool)Session["staffLoggedIn"])
            {
                // hide staff form and show err msg for info
                notLoggedInLabel.Visible = true;
                staffSearchForm.Visible = false;
            }
            else
            {
                // shows form if staff logged in
                notLoggedInLabel.Visible = false;
                staffSearchForm.Visible = true;

                // handle criteria field and value dropdowns
                if (!IsPostBack)
                {
                    PopulateCriteriaFieldDropdown();
                }
            }

            // init list of respondents
            ///
        }


        private void PopulateCriteriaFieldDropdown()
        {
            // get questions that have possible answers
            string query = "SELECT QTN_ID, Question, PossibleAnswers FROM Question WHERE QTE_ID = 1 AND CustomAnswer = 0";

            using (SqlConnection connection = new SqlConnection(_myConnectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // get question info
                        int questionId = Convert.ToInt32(reader["QTN_ID"]);
                        string questionText = reader["Question"].ToString();
                        System.Diagnostics.Debug.WriteLine($"{questionText}");

                        // regex to get UI names
                        string displayText = System.Text.RegularExpressions.Regex.Match(questionText, @"\b[A-Z]+\b").Value; // i'm fancy

                        // add item to dropdown
                        criteriaFieldDropdown.Items.Add(new ListItem(displayText, questionId.ToString()));
                    }
                }

                connection.Close();
            }
        }


        protected void criteriaFieldDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            // clear fieldValue dropdown
            criteriaValueDropdown.Items.Clear();

            // get selected question
            int selectedQuestionID = int.Parse(criteriaFieldDropdown.SelectedValue);

            // get possible answers for selected question
            string getPossibleAnswersQ = "SELECT PossibleAnswers FROM Question WHERE QTN_ID = @QuestionId";

            using (SqlConnection connection = new SqlConnection(_myConnectionString))
            {
                SqlCommand getPossibleAnswersCmd = new SqlCommand(getPossibleAnswersQ, connection);
                getPossibleAnswersCmd.Parameters.AddWithValue("@QuestionId", selectedQuestionID);

                connection.Open();
                object possibleAnswersObj = getPossibleAnswersCmd.ExecuteScalar();

                if (possibleAnswersObj != null)
                {
                    string possibleAnswers = possibleAnswersObj.ToString();

                    // split answers by "," or "|" for checkbox or dropdown question's amswers
                    string[] answers = possibleAnswers.Split(new[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string answer in answers)
                    {
                        criteriaValueDropdown.Items.Add(new ListItem(answer.Trim(), answer.Trim()));
                    }
                }

                connection.Close();
            }
        }




        protected void submitBtn_Click(object sender, EventArgs e)
        {
            // clear error message
            errMsgLabel.Text = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(_myConnectionString))
                {
                    connection.Open();

                    // get all respondent IDs
                    string getAllRespondentsQ = "SELECT * FROM Respondent";
                    SqlCommand getAllRespondentsCmd = new SqlCommand(getAllRespondentsQ, connection);
                    // execute statement to get data
                    SqlDataReader reader = getAllRespondentsCmd.ExecuteReader();

                    // check for no respondents
                    if (!reader.HasRows)
                    {
                        // output message
                        errMsgLabel.Text = $"There are no respondents";
                        errMsgLabel.ForeColor = System.Drawing.Color.Red;
                        return;
                    }
                    while (reader.Read())
                    {
                        // log respondent
                        System.Diagnostics.Debug.WriteLine($"ID: {reader["RPT_ID"]} \n isMember: {reader["isMember"]}");
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


        // triggeres whenever data is being drawn in connected gridview
        protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // do stuff
        }
    }
}