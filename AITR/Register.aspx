<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="AITR.Register" %>
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
        #registerForm {
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
        #notLoggedInLabel {
            text-align: center;
            color: red;
            font-size: 1.5em;
        }
    </style>
</head>
<body>
    <form id="register" runat="server">
        <!-- menu -->
        <aitr:SideMenuComponent ID="sideMenuC" runat="server" />
        
        <!-- page content -->
        <div class="pageContainer">
            <div class="fancyFloater">
                <div id="registerForm" runat="server">
                    <div class="formTitle">Become a Member</div>

                    <div class="formSection">
                        <asp:Label class="formLabel" runat="server" Text="First Name"></asp:Label>
                        <asp:TextBox id="fNameInput" class="formInput" runat="server"></asp:TextBox>
                    </div>
                    
                    <div class="formSection">
                        <asp:Label class="formLabel" runat="server" Text="Last Name"></asp:Label>
                        <asp:TextBox id="lNameInput" class="formInput" runat="server"></asp:TextBox>
                    </div>
                    
                    <div class="formSection">
                        <asp:Label class="formLabel" runat="server" Text="Phone Number"></asp:Label>
                        <asp:TextBox id="phoneNumberInput" class="formInput" runat="server" TextMode="Phone"></asp:TextBox>
                    </div>

                    <div class="formSection">
                        <asp:Label class="formLabel" runat="server" Text="Date of Birth"></asp:Label>
                        <asp:TextBox id="dobInput" class="formInput" runat="server" TextMode="Date"></asp:TextBox>
                    </div>

                    <asp:Button id="submitBtn" runat="server" Text="Submit" OnClick="submitBtn_Click" />
                    <asp:Label id="errMsgLabel" runat="server" Text=""></asp:Label>
                </div>
                <asp:Label id="notLoggedInLabel" runat="server" Text="You are already a member"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>
