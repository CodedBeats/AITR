using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AITR
{
    public partial class Questionaire : System.Web.UI.Page
    {
        // actions for when page object loads
        protected void Page_Load(object sender, EventArgs e)
        {
            // load questions only on first page load
            if (!IsPostBack)
            {
                // load questions into list
                List<Question> questionss = LoadQuestions();

                // setup session shiz
                Session["questions"] = questionss;
                Session["respondentAnswers"] = new List<RespondentAnswers>();
                Session["currentQuestionPosition"] = 1;

                // hide submit btn
                submitAnswersBtn.Visible = false;
            }

            // get current question pos and display question
            int currentQuestionPosition = (int)Session["currentQuestionPosition"];
            List<Question> questions = (List<Question>)Session["questions"];
            var currentQuestion = questions.FirstOrDefault(q => q.OrderPos == currentQuestionPosition); // can I use var? is that cheating lol?
            DisplayQuestion(currentQuestion);
        }


        /// <summary>
        /// Saves answered question to a session list.<br />
        /// Handles what question to display next dynamically based on respondent's answers.<br />
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void NextQuestionBtn_Click(object sender, EventArgs e)
        {
            // get questions list from session
            List<Question> questions = Session["questions"] as List<Question>;
            if (questions == null) return;

            // get current position from session
            int currentPosition = (int)Session["currentQuestionPosition"];

            // get the current question based on the position
            Question currentQuestion = questions.FirstOrDefault(q => q.OrderPos == currentPosition);
            // get answer based on the question type
            string userAnswer = currentQuestion.CustomAnswer ? customAnswerTextBox.Text :
                                string.Join(",", possibleAnswersPlaceholder.Controls.OfType<CheckBox>()
                                .Where(cb => cb.Checked)
                                .Select(cb => cb.Text));

            // log checkbox stuff
            /*
            System.Diagnostics.Debug.WriteLine($"User Answer: {userAnswer}");
            foreach (CheckBox cb in possibleAnswersPlaceholder.Controls.OfType<CheckBox>())
            {
                System.Diagnostics.Debug.WriteLine($"Checkbox Text: {cb.Text}, Checked: {cb.Checked}");
            }
            */

            // create answer obj
            RespondentAnswers answer = new RespondentAnswers
            {
                RespondentID = (int)Session["respondentID"],
                QuestionID = currentQuestion.QTN_ID,
                AnswerValue = userAnswer
            };

            // add answer obj to session list
            var respondentAnswers = Session["respondentAnswers"] as List<RespondentAnswers>;
            respondentAnswers.Add(answer);
            Session["respondentAnswers"] = respondentAnswers;


            // determine next question pos
            int nextPosition = currentPosition + 1;
            if (currentQuestion.YesNoQuestion && userAnswer.ToLower() == "no")
            {
                // check if next question is sub question by comparing QuestionType
                string currentType = currentQuestion.QuestionType;
                if (nextPosition < questions.Count && questions[nextPosition].QuestionType.StartsWith(currentType + "_"))
                {
                    nextPosition += 2;  // skip all sub questions if answer was "no"
                }
                // skip next question since it's of same type
                nextPosition++;
            }


            // get next question
            var nextQuestion = questions.FirstOrDefault(q => q.OrderPos == nextPosition);

            // there is another question
            if (nextQuestion != null)
            {
                // update the current question position in session
                Session["currentQuestionPosition"] = nextPosition;
                // next question
                DisplayQuestion(nextQuestion);
            }
            // that was the final question
            else
            {
                // end of the questionnaire
                questionText.Text = "You have completed the questionnaire!";
                // hide question stuff
                questionLabel.Visible = false;
                possibleAnswersPlaceholder.Controls.Clear();
                customAnswerTextBox.Visible = false;
                nextQuestionBtn.Visible = false;
                // show submit btn
                submitAnswersBtn.Visible = true;
            }
        }


        /// <summary>
        /// Reads from the DB to join Question and QuestionType tables to get all questions and their question types
        /// </summary>
        /// <returns>A list of Question objects</returns>
        private List<Question> LoadQuestions()
        {
            List<Question> questions = new List<Question>();

            // connection string setup
            string _myConnectionString = ConfigurationManager.ConnectionStrings["myConnection"].ConnectionString;
            if (_myConnectionString.Equals("dev"))
            {
                _myConnectionString = AppConstants.DevConnectionString;
            }
            else if (_myConnectionString.Equals("test"))
            {
                _myConnectionString = AppConstants.TestConnectionString;
            }
            else _myConnectionString = AppConstants.ProdConnectionString;

            using (SqlConnection connection = new SqlConnection(_myConnectionString))
            {
                connection.Open();

                // get all questions and use inner join to get questionType string
                string query = @"
                    SELECT Question.*, QuestionType.QuestionType
                    FROM Question
                    INNER JOIN QuestionType ON Question.QTE_ID = QuestionType.QTE_ID
                    ORDER BY Question.OrderPos ASC";
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // create obj of question
                        Question question = new Question
                        {
                            QTN_ID = Convert.ToInt32(reader["QTN_ID"]),
                            QuestionType = reader["QuestionType"].ToString(), // i'm fancy
                            QuestionText = reader["Question"].ToString(),
                            PossibleAnswers = reader["PossibleAnswers"].ToString(),
                            YesNoQuestion = Convert.ToBoolean(reader["YesNoQuestion"]),
                            MaxAnswers = reader["MaxAnswers"] != DBNull.Value ? Convert.ToInt32(reader["MaxAnswers"]) : 0,
                            MinAnswers = reader["MinAnswers"] != DBNull.Value ? Convert.ToInt32(reader["MinAnswers"]) : 0,
                            CustomAnswer = Convert.ToBoolean(reader["CustomAnswer"]),
                            OrderPos = Convert.ToInt32(reader["OrderPos"]),
                        };
                        // System.Diagnostics.Debug.WriteLine(question.QuestionType);

                        questions.Add(question);
                    }
                }

                connection.Close();
            }

            return questions;
        }


        /// <summary>
        /// Splits possible answers by "," and creates check box answers to be displayed in the UI
        /// </summary>
        /// <param name="question"></param>
        private void GeneratePossibleAnswersCheckBoxes(Question question)
        {
            // clear previous checkboxes
            possibleAnswersPlaceholder.Controls.Clear();

            // split by possible answers by ,
            var answers = question.PossibleAnswers?.Split(',');

            if (answers != null)
            {
                foreach (var answer in answers)
                {
                    CheckBox cb = new CheckBox();
                    cb.Text = answer;
                    cb.ID = "cb_" + answer;
                    cb.CssClass = "checkboxOption";
                    //cb.AutoPostBack = false;  // stack overflow told me to do this :)
                    possibleAnswersPlaceholder.Controls.Add(cb);
                }
            }
        }


        /// <summary>
        /// Handles all data to be displayed in the UI along with displaying either checkboxes or textbox depending on the question
        /// </summary>
        /// <param name="question"></param>
        private void DisplayQuestion(Question question)
        {
            // setup question #
            questionLabel.Text = $"Question {question.OrderPos}";
            // setup question text
            questionText.Text = question.QuestionText;

            // clear checkboxes and custom input (this was weird to figure out)
            possibleAnswersPlaceholder.Controls.Clear();
            customAnswerTextBox.Visible = false;

            // check if customAnswer -> show custom answer textbox
            if (question.CustomAnswer)
            {
                customAnswerTextBox.Text = "";
                customAnswerTextBox.Visible = question.CustomAnswer;
            }
            // check if question has possible answers
            else
            {
                //show possible answers as checkboxes
                customAnswerTextBox.Visible = false;
                possibleAnswersPlaceholder.Visible = true;
                GeneratePossibleAnswersCheckBoxes(question);
            }
        }


        /// <summary>
        /// Inserts into DB: RespondentAnswers, AttendanceRecord and Respondent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SubmitAnswersBtn_Click(object sender, EventArgs e)
        {
            // get answers list from session
            var respondentAnswers = Session["respondentAnswers"] as List<RespondentAnswers>;
            if (respondentAnswers == null || respondentAnswers.Count == 0) return;

            // connection string setup
            string _myConnectionString = ConfigurationManager.ConnectionStrings["myConnection"].ConnectionString;
            if (_myConnectionString.Equals("dev"))
            {
                _myConnectionString = AppConstants.DevConnectionString;
            }
            else if (_myConnectionString.Equals("test"))
            {
                _myConnectionString = AppConstants.TestConnectionString;
            }
            else _myConnectionString = AppConstants.ProdConnectionString;

            using (SqlConnection connection = new SqlConnection(_myConnectionString))
            {
                connection.Open();

                // insert respondentAnswers
                foreach (var answer in respondentAnswers)
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO RespondentAnswers (RPT_ID, QTN_ID, RespondentsAnswer) VALUES (@RespondentID, @QuestionID, @RespondentsAnswer)", connection))
                    {
                        cmd.Parameters.AddWithValue("@RespondentID", answer.RespondentID);
                        cmd.Parameters.AddWithValue("@QuestionID", answer.QuestionID);
                        cmd.Parameters.AddWithValue("@RespondentsAnswer", answer.AnswerValue);

                        cmd.ExecuteNonQuery();
                    }
                }

                // insert attendenceRecord
                using (SqlCommand cmd = new SqlCommand("INSERT INTO AttendanceRecord (userIP, sessionDate) VALUES (@UserIP, @SessionDate)", connection))
                {
                    // get user IP address
                    string userIP = Request.UserHostAddress == "::1" ? "127.0.0.1" : Request.UserHostAddress; // for dev testing since ::1 is boring lol

                    // get current date and time
                    DateTime sessionDate = DateTime.Now;

                    cmd.Parameters.AddWithValue("@UserIP", userIP);
                    cmd.Parameters.AddWithValue("@SessionDate", sessionDate);

                    cmd.ExecuteNonQuery();
                }

                // insert respondent
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Respondent (isMember) VALUES (@IsMember)", connection))
                {
                    cmd.Parameters.AddWithValue("@IsMember", false);

                    cmd.ExecuteNonQuery();
                }

                connection.Close();
            }

            // clear session data + confirmation message
            Session.Remove("respondentAnswers");
            Session.Remove("currentQuestionPosition");
            questionText.Text = "Thank you! Your responses have been submitted.";
            submitAnswersBtn.Visible = false;
            possibleAnswersPlaceholder.Controls.Clear();
        }

    }
}
