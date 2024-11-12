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
    public partial class Testing : System.Web.UI.Page
    {
        private ExceptionHandler _exceptionHandler;

        // actions for when page object loads
        protected void Page_Load(object sender, EventArgs e)
        {
            // setup ExceptionHandler with the label used to display errors
            _exceptionHandler = new ExceptionHandler(lbTitle);

            try
            {
                // access connection string from AppConstants
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

                // setup connection
                SqlConnection myConn = new SqlConnection();
                myConn.ConnectionString = _myConnectionString;
                myConn.Open();

                // declare sql statement as command
                SqlCommand command = new SqlCommand("SELECT * FROM TabUser", myConn);

                // execute statement to get data
                SqlDataReader reader = command.ExecuteReader();

                // setup structure for data
                DataTable dt = new DataTable();
                dt.Columns.Add("User ID", typeof(Int32));
                dt.Columns.Add("User Name", typeof(String));
                dt.Columns.Add("Password", typeof(String));
                dt.Columns.Add("User Level", typeof(Int32));

                // read data into table
                while (reader.Read())
                {
                    DataRow row = dt.NewRow();

                    row["User ID"] = reader["UID"];
                    row["User Name"] = reader["Username"];
                    row["Password"] = reader["Password"];
                    row["User Level"] = reader["UserLevel"];

                    // insert row into data table
                    dt.Rows.Add(row);
                    Trace.Write(row.ToString());
                    Trace.Write("hello");
                }

                // attach data to UI table
                gvUser.DataSource = dt;
                gvUser.DataBind();

                // always have to close, otherwise will auto close after 15 min
                // but can only have max 5 connections open (can cause many problems in development)
                myConn.Close();
            }
            catch (Exception ex)
            {
                // handle all excetions with custom ExceptionHandler
                _exceptionHandler.HandleExceptions(ex);
            }

        }

        // triggeres whenever data is being drawn in connected gridview
        protected void gvUser_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType.Equals(DataControlRowType.DataRow))
            {
                Image img = new Image();

                if (e.Row.Cells[(int)AppConstants.TabUser.UserLevel].Text.Equals("1"))
                {
                    img.ImageUrl = "~/imgs/pawn.gif";
                }
                else if (e.Row.Cells[3].Text.Equals("2"))
                {
                    img.ImageUrl = "~/imgs/knight.gif";
                }
                else if (e.Row.Cells[3].Text.Equals("3"))
                {
                    img.ImageUrl = "~/imgs/king.gif";
                }

                e.Row.Cells[(int)AppConstants.TabUser.UserLevel].Controls.Add(img);
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            // moshi moshi;
        }
    }
}