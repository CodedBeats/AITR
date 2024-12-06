<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AITR.Login" %>
<%@ Register Src="~/components/SideMenu.ascx" TagPrefix="aitr" TagName="SideMenuComponent" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Become a Member</title>
    <style>
        body {
            margin: 0;
        }

        /* === page content === */
        .pageContainer {
            overflow: hidden;
            display: flex;
            align-items: center;
            justify-content: center;
            font-family: monospace;
            width: 100vw;
            height: 100vh;
            background-image: linear-gradient(to right, #002527, #00939b);
        }
        /* floating div */
        .fancyFloater {
            border-radius: 20px;
            background-color: white;
            padding: 40px;
            box-shadow: 5px 5px 5px #000;
            max-width: 60vw;
        }
        
        /* register form*/
        #loginForm {
            display: flex;
            flex-direction: column;
        }
        .formSection {
            margin: 10px 0;
            display: flex;
            flex-direction: column;
        }
        
        /* form content */
        .formTitle {
            text-align: center;
            font-size: 5em;
            font-weight: bold;
            margin-bottom: 20px;
        }
        .formLabel {
            font-size: 1.8em;
            margin: 5px 0;
        }
        .formInput {
            font-size: 1.5em;
        }
        #submitBtn {
            margin-top: 20px;
            font-size: 2em;
            color: white;
            font-weight: bold;
            padding: 10px;
            border: 1px solid black;
            border-radius: 5px;
            background-color: #2784fe;
            transition: 0.25s;
        }
        #submitBtn:hover {
            background-color: #0452b8;
        }

        /* err msg */
        #errMsgLabel {
            text-align: center;
            color: red;
            font-size: 1.5em;
            margin-top: 10px;
        }
    </style>
</head>
<body>
    <form id="login" runat="server">
        <!-- menu -->
        <aitr:SideMenuComponent ID="sideMenuC" runat="server" />
        
        <!-- page content -->
        <div class="pageContainer">
            <div class="fancyFloater">
                <div id="loginForm">
                    <div class="formTitle">Staff Login</div>

                    <div class="formSection">
                        <asp:Label class="formLabel" runat="server" Text="Username"></asp:Label>
                        <asp:TextBox id="usernameInput" class="formInput" runat="server"></asp:TextBox>
                    </div>
                    
                    <div class="formSection">
                        <asp:Label class="formLabel" runat="server" Text="Password"></asp:Label>
                        <asp:TextBox id="passwordInput" class="formInput" runat="server" TextMode="Password"></asp:TextBox>
                    </div>

                    <asp:Button id="submitBtn" runat="server" Text="Login" OnClick="loginBtn_Click" />
                    <asp:Label id="errMsgLabel" runat="server" Text=""></asp:Label>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
