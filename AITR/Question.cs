using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AITR
{
    public class Question
    {
        // pk
        public int QTN_ID { get; set; }
        // fk
        public string QuestionType { get; set; }
        public string QuestionText { get; set; }
        public string PossibleAnswers { get; set; }
        public bool YesNoQuestion { get; set; } = false;
        public int MaxAnswers { get; set; } = 10;
        public int MinAnswers { get; set; } = 0;
        public bool CustomAnswer { get; set; } = false;
        public int OrderPos { get; set; }


        // default constructro
        public Question() { }


        // constructor with question field properties
        public Question(
            int qtnId,
            string qestionType,
            string questionText,
            string possibleAnswers,
            bool yesNoQuestion,
            int maxAnswers,
            int minAnswers,
            bool customAnswer,
            int orderPos
        )
        {
            QTN_ID = qtnId;
            QuestionType = qestionType;
            QuestionText = questionText;
            PossibleAnswers = possibleAnswers;
            YesNoQuestion = yesNoQuestion;
            MaxAnswers = maxAnswers;
            MinAnswers = minAnswers;
            CustomAnswer = customAnswer;
            OrderPos = orderPos;
        }
    }
}
