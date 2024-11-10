using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace AITR
{
    public class ExceptionHandler
    {
        private Label _label;

        public ExceptionHandler(Label label)
        {
            _label = label;
        }

        public void HandleExceptions(Exception ex)
        {
            if (ex is InvalidCastException)
            {
                HandleInvalidCastException(ex as InvalidCastException);
            }
            else if (ex is SqlException)
            {
                HandleSqlException(ex as SqlException);
            }
            else if (ex is InvalidOperationException)
            {
                HandleInvalidOperationException(ex as InvalidOperationException);
            }
            else if (ex is ConfigurationErrorsException)
            {
                HandleConfigurationErrorsException(ex as ConfigurationErrorsException);
            }
            else
            {
                HandleGenericException(ex);
            }
        }

        private void HandleInvalidCastException(Exception ex)
        {
            _label.Text = "Data Type mismatch. \nError detail: " + ex.Message;
        }
        private void HandleSqlException(SqlException ex)
        {
            _label.Text = "Internal Error, please contact database admin. \nError detail: " + ex.Message;
        }
        private void HandleInvalidOperationException(InvalidOperationException ex)
        {
            _label.Text = "Internal Error, please contact system admin. \nError detail: " + ex.Message;
        }
        private void HandleConfigurationErrorsException(ConfigurationErrorsException ex)
        {
            _label.Text = "Internal Error, please contact system admin. \nError detail: " + ex.Message;
        }
        private void HandleGenericException(Exception ex)
        {
            _label.Text = "Internal Error, please contact system admin. \nError detail: " + ex.Message;
        }
    }
}