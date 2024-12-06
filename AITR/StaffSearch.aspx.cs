using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AITR
{
    public partial class StaffSearch : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            // handle whether to show to the staff search form depending on if the user is logged in as staff or not
            if (Session["staffLoggedIn"] == null || !(bool)Session["staffLoggedIn"])
            {
                // hide staff form and show err msg for info
                notLoggedInLabel.Visible = true;
                staffSearchForm.Visible = false;
            }
            else
            {
                // shows form if staff logged in
                notLoggedInLabel.Visible = false;
                staffSearchForm.Visible = true;
            }
            
        }


        // triggeres whenever data is being drawn in connected gridview
        protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // do stuff
        }
    }
}