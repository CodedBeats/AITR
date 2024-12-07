using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
            // clear error message
            gvErrMsgLabel.Text = "";

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
            // clear error message
            gvErrMsgLabel.Text = "";


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

            //System.Diagnostics.Debug.WriteLine($"Rendering selected criteria. Current list count: {selectedCriteriaList.Count}");

            // add each criteria
            foreach (var criterion in selectedCriteriaList)
            {
                //System.Diagnostics.Debug.WriteLine($"Criterion: QuestionID={criterion.QuestionID}, Value={criterion.CriteriaValue}");

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



        /// <summary>
        /// Gets all matching respondents with the seleted criteria, then gets relevent respondent data and shows it in a grid view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void searchBtn_Click(object sender, EventArgs e)
        {
            // clear error message
            gvErrMsgLabel.Text = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(_myConnectionString))
                {
                    connection.Open();

                    // check for selected criteria to search by
                    if (selectedCriteriaList.Count == 0)
                    {
                        gvErrMsgLabel.Text = "Please add at least one search criteria";
                        gvErrMsgLabel.ForeColor = System.Drawing.Color.Red;
                        return;
                    }

                    // list to store matching respondent IDs
                    List<int> matchingRespondentIDs = new List<int>();

                    // loop oer criteria in selected criteria list
                    foreach (var criterion in selectedCriteriaList)
                    {
                        // get matching respondent IDs for each criteria
                        string getMatchingRespondentsQ = @"
                            SELECT DISTINCT RPT_ID
                            FROM RespondentAnswers
                            WHERE QTN_ID = @QuestionId AND RespondentsAnswer = @CriteriaValue";

                        using (SqlCommand getMatchingRespondentsCmd = new SqlCommand(getMatchingRespondentsQ, connection))
                        {
                            getMatchingRespondentsCmd.Parameters.AddWithValue("@QuestionId", criterion.QuestionID);
                            getMatchingRespondentsCmd.Parameters.AddWithValue("@CriteriaValue", criterion.CriteriaValue);

                            using (SqlDataReader reader = getMatchingRespondentsCmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int respondentId = Convert.ToInt32(reader["RPT_ID"]);

                                    // add to list if not there
                                    if (!matchingRespondentIDs.Contains(respondentId))
                                    {
                                        matchingRespondentIDs.Add(respondentId);
                                    }

                                    // debug
                                    //System.Diagnostics.Debug.WriteLine($"Matching RPT_ID: {respondentId}");
                                }
                            }
                        }
                    }

                    // check if no respondents matched
                    if (matchingRespondentIDs.Count == 0)
                    {
                        gvErrMsgLabel.Text = "No respondents matched the search criteria.";
                        gvErrMsgLabel.ForeColor = System.Drawing.Color.Red;
                        return;
                    }

                    // get relevant question columns for GridView
                    string getQuestionColumnsQ = @"
                        SELECT QTN_ID, Question
                        FROM Question
                        WHERE QTE_ID = 1";

                    List<string> columnNames = new List<string>();
                    List<int> questionIds = new List<int>();

                    using (SqlCommand getQuestionColumnsCmd = new SqlCommand(getQuestionColumnsQ, connection))
                    {
                        using (SqlDataReader reader = getQuestionColumnsCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string questionText = reader["Question"].ToString();
                                string displayText = System.Text.RegularExpressions.Regex.Match(questionText, @"\b[A-Z]+\b").Value;

                                columnNames.Add(displayText);
                                questionIds.Add(Convert.ToInt32(reader["QTN_ID"]));

                                //System.Diagnostics.Debug.WriteLine($"column name: {displayText}");
                                //System.Diagnostics.Debug.WriteLine($"question IDs: {questionIds.Count}");
                            }
                        }
                    }

                    gvResults.Columns.Clear();
                    
                    // add columns for GridView
                    // member columns
                    BoundField nameField = new BoundField
                    {
                        HeaderText = "Name",
                        DataField = "Name"
                    };
                    BoundField dobField = new BoundField
                    {
                        HeaderText = "DoB",
                        DataField = "DoB"
                    };
                    BoundField phoneField = new BoundField
                    {
                        HeaderText = "PhoneNumber",
                        DataField = "PhoneNumber"
                    };
                    gvResults.Columns.Add(nameField);
                    gvResults.Columns.Add(dobField);
                    gvResults.Columns.Add(phoneField);

                    // custom columns
                    foreach (string columnName in columnNames)
                    {
                        BoundField boundField = new BoundField
                        {
                            HeaderText = columnName,
                            DataField = columnName
                        };
                        gvResults.Columns.Add(boundField);
                    }


                    // prepare data for GridView based on matching respondentIDs
                    DataTable dataTable = new DataTable();

                    // add columns of relevant respondent  info
                    // member columnds
                    dataTable.Columns.Add("Name");
                    dataTable.Columns.Add("DoB");
                    dataTable.Columns.Add("PhoneNumber");
                    // custom columns
                    foreach (string columnName in columnNames)
                    {
                        dataTable.Columns.Add(columnName);
                    }

                    // get respondent data and their answers
                    foreach (int matchingRespondentID in matchingRespondentIDs)
                    {
                        // add new row for each respondent
                        DataRow row = dataTable.NewRow();

                        // get Name and other details based on isMember status
                        string getRespondentNameQ = @"
                            SELECT 
                                CASE WHEN r.isMember = 1 THEN CONCAT(m.FirstName, ' ', m.LastName) ELSE 'Anonymous' END AS Name,
                                CASE WHEN r.isMember = 1 THEN m.DoB ELSE NULL END AS DoB,
                                CASE WHEN r.isMember = 1 THEN m.PhoneNumber ELSE 'No Data' END AS PhoneNumber
                            FROM Respondent r
                            LEFT JOIN Member m ON r.RPT_ID = m.RPT_ID
                            WHERE r.RPT_ID = @RespondentID"; // stack overflow told me to do it this way.....and it works ¯\_(ツ)_/¯

                        using (SqlCommand getRespondentNameCmd = new SqlCommand(getRespondentNameQ, connection))
                        {
                            getRespondentNameCmd.Parameters.AddWithValue("@RespondentID", matchingRespondentID);

                            using (SqlDataReader reader = getRespondentNameCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    row["Name"] = reader["Name"].ToString();
                                    // Handle DoB safely
                                    if (reader["DoB"] == DBNull.Value) // lotssssssssssss of trial and error for this one
                                    {
                                        System.Diagnostics.Debug.WriteLine("no dob data");
                                        row["DoB"] = "No Data";
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.WriteLine("dob data");
                                        row["DoB"] = Convert.ToDateTime(reader["DoB"]).ToString("dd/MM/yyyy");
                                    }
                                    row["PhoneNumber"] = reader["PhoneNumber"].ToString();
                                }
                            }
                        }

                        // get answers for this respondent for each question column
                        foreach (int questionId in questionIds)
                        {
                            string getRespondentAnswerQ = @"
                                SELECT RespondentsAnswer
                                FROM RespondentAnswers
                                WHERE RPT_ID = @RespondentID AND QTN_ID = @QuestionID";

                            using (SqlCommand getRespondentAnswerCmd = new SqlCommand(getRespondentAnswerQ, connection))
                            {
                                getRespondentAnswerCmd.Parameters.AddWithValue("@RespondentID", matchingRespondentID);
                                getRespondentAnswerCmd.Parameters.AddWithValue("@QuestionID", questionId);

                                using (SqlDataReader reader = getRespondentAnswerCmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        row[columnNames[questionIds.IndexOf(questionId)]] = reader["RespondentsAnswer"].ToString();
                                    }
                                    else
                                    {
                                        row[columnNames[questionIds.IndexOf(questionId)]] = "No Answer";
                                    }
                                }
                            }
                        }

                        // add row into DataTable
                        dataTable.Rows.Add(row);

                        // debug
                        foreach (DataRow dataRow in dataTable.Rows) System.Diagnostics.Debug.WriteLine("Row Data: " + string.Join(", ", dataRow.ItemArray));
                    }

                    // attach data to UI table
                    gvResults.DataSource = dataTable;
                    gvResults.DataBind();

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                gvErrMsgLabel.Text = $"Error occurred: {ex.Message}";
                gvErrMsgLabel.ForeColor = System.Drawing.Color.Red;
            }
        }


        // triggeres whenever data is being drawn in connected gridview
        protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // do stuff
        }
    }
}