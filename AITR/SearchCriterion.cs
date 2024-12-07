using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AITR
{
    // didn't use "SearchCriteria" just cause worried about naming issues
    public class SearchCriterion
    {
        public int QuestionID { get; set; }
        public string CriteriaValue { get; set; }
    }
}