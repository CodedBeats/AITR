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

                // store questions in session
                Session["questions"] = questionss;
                // init answers list in session
                Session["respondentAnswers"] = new List<RespondentAnswers>();

                // current question for handling place
                Session["currentQuestionPosition"] = 1;

                // hide submit btn
                submitAnswersBtn.Visible = false;
            }

            // get current question pos and display question
            int currentQuestionPosition = (int)Session["currentQuestionPosition"];
            List<Question> questions = (List<Question>)Session["questions"];
            var currentQuestion = questions.FirstOrDefault(q => q.OrderPos == currentQuestionPosition);
            DisplayQuestion(currentQuestion);
        }


        protected void NextQuestionBtn_Click(object sender, EventArgs e)
        {
            // get questions list from session
            List<Question> questions = Session["questions"] as List<Question>;
            if (questions == null) return;

            // get current position from session
            int currentPosition = (int)Session["currentQuestionPosition"];


            Question currentQuestion = questions.FirstOrDefault(q => q.OrderPos == currentPosition);

            if (currentQuestion != null)
            {
                // get answer based on the question type
                string userAnswer = currentQuestion.CustomAnswer ? customAnswerTextBox.Text :
                                    string.Join(",", possibleAnswersPlaceholder.Controls.OfType<CheckBox>()
                                    .Where(cb => cb.Checked)
                                    .Select(cb => cb.Text));

                // log checkbox stuff
                System.Diagnostics.Debug.WriteLine($"User Answer: {userAnswer}");
                System.Diagnostics.Debug.WriteLine($"Number of controls in possibleAnswersPlaceholder: {possibleAnswersPlaceholder.Controls.Count}");
                foreach (CheckBox cb in possibleAnswersPlaceholder.Controls.OfType<CheckBox>())
                {
                    System.Diagnostics.Debug.WriteLine($"Checkbox Text: {cb.Text}, Checked: {cb.Checked}");
                }

                // create obj
                RespondentAnswers answer = new RespondentAnswers
                {
                    RespondentID = 1, // replace
                    QuestionID = currentQuestion.QTN_ID,
                    AnswerValue = userAnswer
                };

                // add obj to session list
                var respondentAnswers = Session["respondentAnswers"] as List<RespondentAnswers>;
                respondentAnswers.Add(answer);
                Session["respondentAnswers"] = respondentAnswers;
            }


            // get next question
            var nextQuestion = questions.FirstOrDefault(q => q.OrderPos == currentPosition + 1);

            // there is another question
            if (nextQuestion != null)
            {
                // increment pos and update session
                Session["currentQuestionPosition"] = currentPosition + 1;
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

                // get all questions
                string query = "SELECT * FROM Question ORDER BY OrderPos ASC";
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // create obj of question
                        Question question = new Question
                        {
                            QTN_ID = Convert.ToInt32(reader["QTN_ID"]),
                            QTE_ID = Convert.ToInt32(reader["QTE_ID"]),
                            QuestionText = reader["Question"].ToString(),
                            PossibleAnswers = reader["PossibleAnswers"].ToString(),
                            YesNoQuestion = Convert.ToBoolean(reader["YesNoQuestion"]),
                            MaxAnswers = Convert.ToInt32(reader["MaxAnswers"]),
                            MinAnswers = Convert.ToInt32(reader["MinAnswers"]),
                            CustomAnswer = Convert.ToBoolean(reader["CustomAnswer"]),
                            OrderPos = Convert.ToInt32(reader["OrderPos"])
                        };

                        questions.Add(question);
                    }
                }

                connection.Close();
            }

            return questions;
        }


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
                connection.Close();
            }

            // clear session data + confirmation message
            Session.Remove("respondentAnswers");
            questionText.Text = "Thank you! Your responses have been submitted.";
            submitAnswersBtn.Visible = false;
            possibleAnswersPlaceholder.Controls.Clear();
        }

    }
}
