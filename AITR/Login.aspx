<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AITR.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Become a Member</title>
    <style>
        body {
            margin: 0;
        }

        /* === Side Menu === */
        #sideMenu {
            display: flex;
            flex-direction: column;
            width: 10vw;
            background-color: #262626;
            color: #fff;
            font-family: monospace;
            padding: 20px;
            height: 100vh;
            position: fixed;
            top: 0;
            left: 0;
            border-right: solid;
            border-color: black;
            border-width: 2px;
        }

        .menuTitle {
            font-size: 4em;
            font-weight: bold;
            text-align: center;
            color: #00ffd8;
        }

        /* menu */
        .menuItems {
            margin-top: 20px;
            display: flex;
            flex-direction: column;
        }
        .menuItem {
            font-size: 2em;
            color: #ccc;
            padding: 15px 0;
            text-decoration: none;
            cursor: pointer;
            transition: 0.25s;
        }
        .menuItem:hover {
            color: #fff;
        }
        
        /* current user */
        .currentUser {
            display: flex;
            align-items: center;
            margin-top: auto;
            transform: translateY(-60px); /* adjust for side menu padding*/
            font-size: 1.5em;
            cursor: pointer;
        }
        .userImg {
            width: 30px;
            height: 30px;
            margin-right: 10px;
        }
        #loginBtn {
            color: #ccc;
            text-decoration: none;
            cursor: pointer;
            transition: 0.25s;
        }
        #loginBtn:hover {
            color: #fff;
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
    </style>
</head>
<body>
    <form id="login" runat="server">
        <!-- side menu -->
        <div id="sideMenu">
            <div class="menuTitle">AITR</div>

            <div class="menuItems">
                <a href="Default.aspx" class="menuItem">Home</a>
                <a href="Register.aspx" class="menuItem">Become a Member</a>
                <a href="StaffSearch.aspx" class="menuItem">Staff Search</a>
            </div>

            <div class="currentUser">
                <img class="userImg" src="imgs/user_icon.png" alt="User Image" />
                <a href="Login.aspx" id="loginBtn">Anonymous ^</a>
            </div>
        </div>
        
        <!-- page content -->
        <div class="pageContainer">
            <div class="fancyFloater">
                <div id="loginForm">
                    <div class="formTitle">Login</div>

                    <div class="formSection">
                        <asp:Label class="formLabel" runat="server" Text="Username"></asp:Label>
                        <asp:TextBox id="usernameInput" class="formInput" runat="server"></asp:TextBox>
                    </div>
                    
                    <div class="formSection">
                        <asp:Label class="formLabel" runat="server" Text="Password"></asp:Label>
                        <asp:TextBox id="passwordInput" class="formInput" runat="server"></asp:TextBox>
                    </div>

                    <asp:Button id="submitBtn" runat="server" Text="Submit" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
