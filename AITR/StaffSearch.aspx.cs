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
                    try
                    {
                        PopulateCriteriaFieldDropdown();
                    }
                    catch (Exception ex)
                    {
                        errMsgLabel.Text = $"Error occurred while loading criteria: {ex.Message}";
                        errMsgLabel.ForeColor = System.Drawing.Color.Red;
                    }
                }
                RenderSelectedCriteria();
            }
        }



        // store selected criteria in session
        private List<SearchCriterion> selectedCriteriaList
        {
            get
            {
                var list = Session["selectedCriteriaList"] as List<SearchCriterion>;
                if (list == null)
                {
                    list = new List<SearchCriterion>();
                    Session["selectedCriteriaList"] = list;
                }
                return list;
            }
            set
            {
                Session["selectedCriteriaList"] = value;
            }
        }



        /// <summary>
        /// Populate the Criteria Field Dropdown with questions that have possible answers
        /// </summary>
        private void PopulateCriteriaFieldDropdown()
        {
            // get relevant (have possible answers) questions and associated their question type
            string getQuestionsQ = @"
                SELECT 
                    Question.QTN_ID, 
                    Question.Question, 
                    Question.PossibleAnswers, 
                    Question.YesNoQuestion, 
                    QuestionType.QuestionType
                FROM Question
                INNER JOIN QuestionType ON Question.QTE_ID = QuestionType.QTE_ID
                WHERE Question.CustomAnswer = 0";

            // add (disabled) placeholder option
            criteriaFieldDropdown.Items.Add(new ListItem("-- Select Criteria Field --", "0") { Enabled = false });

            using (SqlConnection connection = new SqlConnection(_myConnectionString))
            {
                SqlCommand getQuestionsCmd = new SqlCommand(getQuestionsQ, connection);
                connection.Open();

                using (SqlDataReader reader = getQuestionsCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // get question info
                        int questionId = Convert.ToInt32(reader["QTN_ID"]);
                        string questionText = reader["Question"].ToString();
                        string questionType = reader["QuestionType"].ToString();
                        bool isYesNoQuestion = Convert.ToBoolean(reader["YesNoQuestion"]);
                        //System.Diagnostics.Debug.WriteLine($"{questionText}");

                        // determine display text for dropdown
                        string displayText;
                        if (questionType == "RespondentInfo")
                        {
                            // I forgot to add a title or something to Question table in schema so there is no way to differentiate between RespondentInfo questions
                            // So the work around is to capitalise words in those questions to use some fancy (probably shitty) regex to get those titles for the UI
                            displayText = System.Text.RegularExpressions.Regex.Match(questionText, @"\b[A-Z]+\b").Value;
                        }
                        else if (isYesNoQuestion)
                        {
                            displayText = $"Uses {questionType}";
                        }
                        else
                        {
                            displayText = questionType;
                        }

                        // add item to dropdown
                        criteriaFieldDropdown.Items.Add(new ListItem(displayText, questionId.ToString()));
                    }
                }

                connection.Close();
            }
        }



        /// <summary>
        /// Populate the Criteria Value Dropdown with the possible answers for the selectd question
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void criteriaFieldDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            // clear fieldValue dropdown
            criteriaValueDropdown.Items.Clear();
            // add (disabled) placeholder option
            criteriaFieldDropdown.Items.Add(new ListItem("-- Select Field Value --", "0") { Enabled = false });

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



        /// <summary>
        /// Adds the selected criteria to a session list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void addSelectionBtn_Click(object sender, EventArgs e)
        {
            // validate valid selections
            if (criteriaFieldDropdown.SelectedValue == "0" || criteriaValueDropdown.SelectedValue == "0")
            {
                errMsgLabel.Text = "Please select both a criteria field and value.";
                errMsgLabel.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // get selected details
            int questionID = Convert.ToInt32(criteriaFieldDropdown.SelectedValue);
            string selectedValue = criteriaValueDropdown.SelectedItem.Text;

            // check if critera already added
            if (selectedCriteriaList.Exists(c => c.QuestionID == questionID && c.CriteriaValue == selectedValue))
            {
                errMsgLabel.Text = "This criterion is already added.";
                errMsgLabel.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // add criterion to list
            selectedCriteriaList.Add(new SearchCriterion
            {
                QuestionID = questionID,
                CriteriaValue = selectedValue
            });

            // ppdate selectedCriteria div
            RenderSelectedCriteria();

            // clear error message
            errMsgLabel.Text = "";
        }



        /// <summary>
        /// Clears the selected criteria from the UI and session list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void clearSelectionBtn_Click(object sender, EventArgs e)
        {
            selectedCriteriaList.Clear();
            try
            {
                // clear session list
                selectedCriteriaList.Clear();
                Session["selectedCriteriaList"] = null;

                // clear UI container
                selectedCriteria.Controls.Clear();
            }
            catch (Exception ex)
            {
                // handle error message
                errMsgLabel.Text = $"Error occurred while clearing criteria: {ex.Message}";
                errMsgLabel.ForeColor = System.Drawing.Color.Red;
            }
        }



        /// <summary>
        /// Adds selected criteria to the UI
        /// </summary>
        private void RenderSelectedCriteria()
        {
            // clear current selected criteria container
            selectedCriteria.Controls.Clear();

            System.Diagnostics.Debug.WriteLine($"Rendering selected criteria. Current list count: {selectedCriteriaList.Count}");

            // add each criteria
            foreach (var criterion in selectedCriteriaList)
            {
                System.Diagnostics.Debug.WriteLine($"Criterion: QuestionID={criterion.QuestionID}, Value={criterion.CriteriaValue}");

                // get question text for display
                string questionText = criteriaFieldDropdown.Items.FindByValue(criterion.QuestionID.ToString())?.Text;

                // add new literal control for display
                Literal criteriaItem = new Literal
                {
                    Text = $"<div class='criteria'>{questionText}: {criterion.CriteriaValue}</div>"
                };

                selectedCriteria.Controls.Add(criteriaItem);
            }
        }






        protected void searchBtn_Click(object sender, EventArgs e)
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