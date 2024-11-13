using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AITR
{
    public class RespondentAnswers
    {
        // pk/fk
        public int RespondentID { get; set; }
        // pk/fk
        public int QuestionID { get; set; }
        // i did this bit stupid, this value will get complicated
        public string AnswerValue { get; set; }

        // default constructor
        public RespondentAnswers() { }

        // constructor with RespondentAnswers field properties
        public RespondentAnswers(int respondentID, int questionID, string answerValue)
        {
            RespondentID = respondentID;
            QuestionID = questionID;
            AnswerValue = answerValue;
        }
    }
}