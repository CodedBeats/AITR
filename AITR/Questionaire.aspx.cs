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
            if (!IsPostBack)
            {
                // load questions into list
                List<Question> questions = LoadQuestions();

                // store questions in session
                Session["questions"] = questions;

                // current question for handling place
                Session["currentQuestionPosition"] = 1;

                // display first question
                var firstQuestion = questions.FirstOrDefault(q => q.OrderPos == 1);
                DisplayQuestion(firstQuestion);
            }
        }


        protected void NextQuestionBtn_Click(object sender, EventArgs e)
        {
            // get questions list from session
            List<Question> questions = Session["questions"] as List<Question>;
            if (questions == null) return;

            // get current position from session
            int currentPosition = (int)Session["currentQuestionPosition"];

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
                nextQuestionBtn.Enabled = false;
                questionText.Text = "You have completed the questionnaire!";
                possibleAnswersPlaceholder.Controls.Clear();
                customAnswerTextBox.Visible = false;
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


        private void DisplayQuestion(Question question)
        {
            // setup question #
            questionLabel.Text = $"Question {question.OrderPos}";
            // setup question text
            questionText.Text = question.QuestionText;

            // clear checkboxes (this was weird to figure out)
            possibleAnswersPlaceholder.Controls.Clear();

            // check if question has possible answers
            if (!string.IsNullOrEmpty(question.PossibleAnswers))
            {
                // split possible answers by ,
                string[] answers = question.PossibleAnswers.Split(',');

                foreach (var answer in answers)
                {
                    // create checkBox foreach possible answer
                    CheckBox checkBox = new CheckBox
                    {
                        Text = answer,
                        ID = "chk_" + answer.Trim(),
                        AutoPostBack = false  // stack overflow told me to do this :)
                    };

                    // add checkboxs to placeholder control
                    possibleAnswersPlaceholder.Controls.Add(checkBox);
                    possibleAnswersPlaceholder.Controls.Add(new Literal { Text = "<br />" });
                }
            }

            // if customAnswer -> show custom answer textbox
            customAnswerTextBox.Visible = question.CustomAnswer;
        }
    }
}
