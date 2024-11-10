<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AITR.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="lbTitle" runat="server" Text="Welcome to AITR" Font-Size="60" Font-Bold="True" ForeColor="#FF0066" />
            <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
            <asp:GridView ID="gvUser" runat="server"
                OnRowDataBound="gvUser_RowDataBound">
            </asp:GridView>
        </div>
    </form>
</body>
</html>
