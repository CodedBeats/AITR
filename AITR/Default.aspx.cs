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
    public partial class Default : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            // init session vars only if not already set
            if (Session["respondentID"] == null)
            {
                int newRespondentId = GenerateUniqueRespondentId();
                Session["respondentID"] = newRespondentId;
            }
            if (Session["staffLoggedIn"] == null)
            {
                Session["staffLoggedIn"] = false;
            }
            if (Session["isRegistered"] == null)
            {
                Session["isRegistered"] = false;
            }
        }


        /// <summary>
        /// Generates a unique integer by incrementing the current highest respondentID
        /// </summary>
        /// <returns></returns>
        private int GenerateUniqueRespondentId()
        {
            int newRespondentId = 0;

            try
            {
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

                    // get the max RPT_ID from Respondent table
                    string query = "SELECT COUNT(RPT_ID) FROM Respondent";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        object result = command.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            // increment for unique ID
                            newRespondentId = Convert.ToInt32(result) + 1;
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating Respondent ID: {ex.Message}");
            }

            return newRespondentId;
        }
    }
}