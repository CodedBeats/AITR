using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AITR.components
{
    public partial class SideMenu : System.Web.UI.UserControl
    {
        /// <summary>
        /// Handles what text to display depending on authentication state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // check if logged in
            bool isStaffLoggedIn = Session["staffLoggedIn"] != null && (bool)Session["staffLoggedIn"];

            // set login text
            if (isStaffLoggedIn)
            {
                loginBtn.InnerHtml = "Staff";
            }
            else
            {
                loginBtn.InnerHtml = "Anonymous >";
            }
        }
    }
}