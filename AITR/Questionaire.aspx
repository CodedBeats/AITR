<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Questionaire.aspx.cs" Inherits="AITR.Questionaire" %>
<%@ Register Src="~/components/SideMenu.ascx" TagPrefix="aitr" TagName="SideMenuComponent" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Questionaire</title>
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
        #submitAnswersBtn {
            margin-top: 30px;
            margin-left: auto;
            width: 20%;
            color: white;
            font-weight: bold;
            font-size: 1.5em;
            padding: 10px;
            border: 1px solid black;
            border-radius: 5px;
            background-color: #27fe35;
            transition: 0.25s;
        }
        #submitAnswersBtn:hover {
            background-color: #08a312;
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
    <form id="questionaire" runat="server">
        <!-- menu -->
        <aitr:SideMenuComponent ID="sideMenuC" runat="server" />
        
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

                    <!-- dropdown for selecting answers -->
                    <asp:DropDownList id="answerDropDown" runat="server" Visible="false">
                       <asp:ListItem Text="Select an answer" Value="" />
                    </asp:DropDownList>

                    <!-- custom input textbox -->
                    <asp:TextBox id="customAnswerTextBox" runat="server" Visible="false" Placeholder="Enter your answer here"></asp:TextBox>
    
                    <!-- next question -->
                    <asp:Button id="nextQuestionBtn" runat="server" Text="Next" OnClick="NextQuestionBtn_Click" />
                    <asp:Button id="submitAnswersBtn" runat="server" Text="Submit" OnClick="SubmitAnswersBtn_Click" />

                    <!-- error message -->
                    <asp:Label id="errMsgLabel" runat="server" Text=""></asp:Label>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
