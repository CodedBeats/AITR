<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Questionaire.aspx.cs" Inherits="AITR.Questionaire" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Questionaire</title>
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
        #dropdownMenu {
            display: none;
            flex-direction: column;
            margin-top: 10px;
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
            min-width: 40%
        }
        
        /* questionaire */
        .questionaire {
            display: flex;
            flex-direction: column;
        }
        #questionLabel {
            font-weight: bold;
            font-size: 3.5em;
            margin-bottom: 20px;
        }
        #questionText {
            font-size: 3em;
            margin-bottom: 40px;
        }
        .checkboxes {
            display: flex;
            overflow: auto;
        }
        .checkboxOption {
            margin-right: 20px;
            font-size: 2em;
        }
        #customAnswerTextBox {
            font-size: 2em;
        }
        #nextQuestionBtn {
            margin-top: 30px;
            margin-left: auto;
            width: 20%;
            color: white;
            font-weight: bold;
            font-size: 1.5em;
            padding: 10px;
            border: 1px solid black;
            border-radius: 5px;
            background-color: #2784fe;
            transition: 0.25s;
        }
        #nextQuestionBtn:hover {
            background-color: #0452b8;
        }
    </style>
</head>
<body>
    <form id="questionaire" runat="server">
        <!-- side menu -->
        <div id="sideMenu">
            <div class="menuTitle">AITR</div>

            <div class="menuItems">
                <a href="Default.aspx" class="menuItem">Home</a>
                <a href="Register.aspx" class="menuItem">Become a Member</a>
                <a href="StaffSearch.aspx" class="menuItem">Staff Search</a>
            </div>

            <div class="currentUser">
                <img class="userImg" src="imgs/king.gif" alt="User Image" />
                <span>Anonymous ^</span>
            </div>
            <!-- dropup menu -->
            <div id="dropdownMenu">
                <span class="dropdownMenuItem">Option 1</span>
                <span class="dropdownMenuItem">Option 2</span>
            </div>
        </div>
        
        <!-- page content -->
        <div class="pageContainer">
            <div class="fancyFloater">
                <div class="questionaire">
                    <!-- question number -->
                    <asp:Label id="questionLabel" runat="server" Text="Question Number:"></asp:Label>
    
                    <!-- question text -->
                    <asp:Label id="questionText" runat="server" Text="Question will appear here"></asp:Label>
    
                    <!-- generated possible checkbox answers -->
                    <div class="checkboxes">
                        <asp:PlaceHolder id="possibleAnswersPlaceholder" runat="server"></asp:PlaceHolder>
                    </div>

                    <!-- custom input textbox -->
                    <asp:TextBox id="customAnswerTextBox" runat="server" Visible="false" Placeholder="Enter your answer here"></asp:TextBox>
    
                    <!-- next question -->
                    <asp:Button id="nextQuestionBtn" runat="server" Text="Next" OnClick="NextQuestionBtn_Click" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
