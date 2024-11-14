using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AITR
{
    public class AttendanceRecord
    {
        // pk
        public int ARD_ID {  get; set; }
        public string UserIP { get; set; }
        public DateTime SessionDate { get; set; }


        // default constructor
        public AttendanceRecord() { }


        // constructor with question field properties
        public AttendanceRecord(int ardId, string userIP, DateTime sessionDate) 
        { 
            ARD_ID = ardId;
            UserIP = userIP;
            SessionDate = sessionDate;
        }
    }
}