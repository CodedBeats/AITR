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
            var currentQuestion = questions.Find(q => q.OrderPos == currentQuestionPosition); // can I use var? is that cheating lol?
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
            // clear error message
            errMsgLabel.Text = "";

            // get questions list from session
            List<Question> questions = Session["questions"] as List<Question>;
            if (questions == null) return;

            // get current position from session
            int currentPosition = (int)Session["currentQuestionPosition"];

            // get the current question based on the position
            Question currentQuestion = questions.Find(q => q.OrderPos == currentPosition);


            string userAnswer = "";
            // get answer based on the question type
            if (currentQuestion.CustomAnswer)
            {
                // custom answer text
                userAnswer = customAnswerTextBox.Text;
            }
            else if (answerDropDown.Visible)
            {
                // selected option from dropdown
                userAnswer = answerDropDown.SelectedValue;
            }
            else
            {
                // get all answers from checkboxes
                userAnswer = string.Join(",", possibleAnswersPlaceholder.Controls.OfType<CheckBox>()
                    .Where(cb => cb.Checked)
                    .Select(cb => cb.Text));
            }
            // log answers
            System.Diagnostics.Debug.WriteLine($"User Answer: {userAnswer}");



            // VINDICATION!!!!!! (validation)
            var (isValid, errorMessage) = ValidateAnswer(userAnswer, currentQuestion);

            // handle error messages
            if (!isValid)
            {
                errMsgLabel.Text = errorMessage;
                return;
            }

            customAnswerTextBox.Text = "";
            answerDropDown.Items.Clear();


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
            var nextQuestion = questions.Find(q => q.OrderPos == nextPosition);

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
                answerDropDown.Visible = false;
                // show submit btn
                submitAnswersBtn.Visible = true;
            }
        }



        /// <summary>
        /// Takes user answer input and validates against all required parameters ouputting if it's valid and giving an error message if needed
        /// </summary>
        /// <param name="input"></param>
        /// <param name="question"></param>
        /// <returns></returns>
        public static (bool isValid, string errorMessage) ValidateAnswer(string input, Question question)
        {
            // trim input
            string trimmedInput = input?.Trim() ?? "";

            // split answers by "," for multi-answers
            string[] answers = trimmedInput.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // validate yesNoQuestion
            if (question.YesNoQuestion)
            {
                // must answer and only one option
                if (answers.Length != 1)
                {
                    return (false, "Please choose either Yes or No");
                }
            }
            else
            {
                // validate minAnswers
                if (answers.Length < question.MinAnswers)
                {
                    return (false, $"At least {question.MinAnswers} answer(s) are required for this question");
                }

                // validaten maxAnswers
                if (answers.Length > question.MaxAnswers)
                {
                    return (false, $"No more than {question.MaxAnswers} answer(s) are allowed for this question");
                }
            }

            // validation success
            return (true, "");
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

            // clear checkboxes and custom input (this was weird to figure out lol)
            possibleAnswersPlaceholder.Controls.Clear();
            customAnswerTextBox.Visible = false;
            answerDropDown.Visible = false;

            // check if customAnswer -> show custom answer textbox
            if (question.CustomAnswer)
            {
                customAnswerTextBox.Visible = true;
            }
            // question has possible answers
            else if (question.PossibleAnswers.Contains("|"))
            {
                // answers are seperated by "|" ex: "x|y|z" -> use dropdown 
                answerDropDown.Visible = true;
                string[] options = question.PossibleAnswers.Split('|');
                foreach (var option in options)
                {
                    answerDropDown.Items.Add(new ListItem(option, option));
                }
            }
            else
            {
                // answers are seperated by "," ex: "x,y,z" -> use checkboxes
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
                    // some answer strings may be in the format of "x,y,z" for when multiple answers were given.
                    // so we do fancy split (works on strings with 1-any number of answers)
                    var individualAnswers = answer.AnswerValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var individualAnswer in individualAnswers)
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "INSERT INTO RespondentAnswers (RPT_ID, QTN_ID, RespondentsAnswer) VALUES (@RespondentID, @QuestionID, @RespondentsAnswer)",
                            connection))
                        {
                            // parameterised for sql injection protection
                            cmd.Parameters.AddWithValue("@RespondentID", answer.RespondentID);
                            cmd.Parameters.AddWithValue("@QuestionID", answer.QuestionID);
                            cmd.Parameters.AddWithValue("@RespondentsAnswer", individualAnswer.Trim());

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                // insert attendenceRecord
                using (SqlCommand cmd = new SqlCommand("INSERT INTO AttendanceRecord (userIP, sessionDate) VALUES (@UserIP, @SessionDate)", connection))
                {
                    // get user IP address
                    string userIP = Request.UserHostAddress == "::1" ? "127.0.0.1" : Request.UserHostAddress; // for dev testing since ::1 is boring lol

                    // get current date and time
                    DateTime sessionDate = DateTime.Now;

                    // parameterised for sql injection protection
                    cmd.Parameters.AddWithValue("@UserIP", userIP);
                    cmd.Parameters.AddWithValue("@SessionDate", sessionDate);

                    cmd.ExecuteNonQuery();
                }

                // check if respondent exists (might already exist from becoming a member)
                string checkRespondentQ = "SELECT COUNT(*) FROM Respondent WHERE RPT_ID = @RPT_ID";
                using (SqlCommand checkRespondentCmd = new SqlCommand(checkRespondentQ, connection))
                {
                    checkRespondentCmd.Parameters.AddWithValue("@RPT_ID", Session["respondentID"]);
                    int count = Convert.ToInt32(checkRespondentCmd.ExecuteScalar());

                    // respondent !exists -> add respondent
                    if (count == 0)
                    {
                        // insert respondent
                        using (SqlCommand cmd = new SqlCommand("INSERT INTO Respondent (isMember) VALUES (@IsMember)", connection))
                        {
                            // parameterised for sql injection protection
                            cmd.Parameters.AddWithValue("@IsMember", false);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }


                /*
                 * === TRIVIA ===
                 * You may be wondering why sometimes I put the query in it's own variable and pass it,
                 * and sometimes I just put it in straight away.
                 * Well...
                 * I coded it one style one day, then switched to a different style a week later.
                 * And while I am using some great source control...I'm scared to break everything hahahahahahah
                 * ❤︎ Happy coding to you today ❤︎
                */


                connection.Close();
            }

            // clear session data + confirmation message
            Session.Remove("respondentAnswers");
            Session.Remove("currentQuestionPosition");
            questionText.Text = "Thank you! Your responses have been submitted.";
            submitAnswersBtn.Visible = false;
            answerDropDown.Visible = false;
            possibleAnswersPlaceholder.Controls.Clear();
        }

    }
}