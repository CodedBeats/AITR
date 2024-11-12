<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AITR.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Side Menu Example</title>
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
            padding: 80px 40px;
            box-shadow: 5px 5px 5px #000;
        }
        #bigAssBtn {
            color: white;
            background-color: #2784fe;
            padding: 15px;
            text-align: center;
            text-decoration: none;
            font-size: 3em;
            border-radius: 20px;
        }
    </style>
</head>
<body>
    <form id="home" runat="server">
        <!-- side menu -->
        <div id="sideMenu">
            <div class="menuTitle">AITR</div>

            <div class="menuItems">
                <a href="Default.aspx" class="menuItem">Home</a>
                <a href="Register.aspx" class="menuItem">Become a Member</a>
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
                <a href="Default.aspx" id="bigAssBtn">Start Questionaire</a>
            </div>
        </div>
    </form>
</body>
</html>
