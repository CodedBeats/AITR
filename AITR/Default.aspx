<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AITR.Default" %>
<%@ Register Src="~/components/SideMenu.ascx" TagPrefix="aitr" TagName="SideMenuComponent" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Home</title>
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
            transition: 0.25s;
        }
        #bigAssBtn:hover {
            background-color: #0452b8;
        }
    </style>
</head>
<body>
    <form id="home" runat="server">
        <!-- menu -->
        <aitr:SideMenuComponent ID="sideMenuC" runat="server" />

        <!-- page content -->
        <div class="pageContainer">
            <div class="fancyFloater">
                <a href="Questionaire.aspx" id="bigAssBtn">Start Questionaire</a>
            </div>
        </div>
    </form>
</body>
</html>
