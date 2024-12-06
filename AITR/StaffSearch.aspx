<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StaffSearch.aspx.cs" Inherits="AITR.StaffSearch" %>
<%@ Register Src="~/components/SideMenu.ascx" TagPrefix="aitr" TagName="SideMenuComponent" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Staff Search</title>
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
        
        /* === criteria === */
        .criteriaContainer {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            margin-bottom: 50px;
        }
        .criteriaTitle {
            font-size: 4em;
            font-weight: bold;
            margin-bottom: 5px;
        }

        /* selected criteria */
        .selectedCriteria {
            display: flex;
            gap: 20px;
            width: 100%;
            padding: 5px 0;
            background-color: #b5b5b5;
            border: 1px solid black;
            border-radius: 10px 10px 0 0;
        }
        .criteria {
            padding: 5px;
            margin-left: 10px;
            background-color: #9cffa2;
            border: 1px solid black;
            border-radius: 5px;
            text-align: center;
            font-size: 1.2em;
        }

        /* criteria selector */
        .criteriaSelectorContainer {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            padding: 10px 0;
            width: 100%;
            background-color: #b5b5b5;
            border: 1px solid black;
            border-top: none;
            border-radius: 0 0 10px 10px;
        }
        .criteriaSelectors {
            display: flex;
            flex-direction: row;
            gap: 30px;
        }
        .criteriaSelector {
            display: flex;
            flex-direction: column;
        }
        .criteriaSelectorLabel {
            font-size: 1.5em;
        }
        #criteriaFieldDropdown {

        }
        #criteriaValueDropdown {

        }
        #addSelectionBtn {
            margin-top: 10px;
            font-size: 1.2em;
            color: white;
            font-weight: bold;
            padding: 5px;
            border: 1px solid black;
            border-radius: 5px;
            background-color: #49d051;
            transition: 0.25s;
        }
        #addSelectionBtn:hover {
            background-color: #269f2d;
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

        /* results */
        .resultsContainer {
            width: 100%;
            padding: 20px;
            border: 1px solid black;
            border-radius: 10px;
            background-color: #b5b5b5;
        }
        #resultsPlaceholder {
            text-align: center;
            font-size: 1.2em;
        }
        #gvResults {
            display: none;
        }
        .demoInfo {
            color: red;
        }

        /* err msg */
        #notLoggedInLabel {
            text-align: center;
            color: red;
            font-size: 1.5em;
        }
    </style>
</head>
<body>
    <form id="staffSearch" runat="server">
        <!-- menu -->
        <aitr:SideMenuComponent ID="sideMenuC" runat="server" />
        
        <!-- page content -->
        <div class="pageContainer">
            <div class="fancyFloater">
                <div id="staffSearchForm" runat="server">
                    <div class="criteriaContainer">
                        <div class="criteriaTitle">Add Criteria to refine your search</div>

                        <div class="selectedCriteria">
                            <div class="criteria">Gender: Female</div>
                            <div class="criteria">State: VIC</div>
                            <div class="criteria">Postcode: 3070</div>
                        </div>

                        <div class="criteriaSelectorContainer">
                            <div class="criteriaSelectors">
                                <div class="criteriaSelector">
                                    <asp:Label class="criteriaSelectorLabel" runat="server" Text="Select Criteria Field"></asp:Label>
                                    <asp:DropDownList id="criteriaFieldDropdown" runat="server"></asp:DropDownList>
                                </div>
                                <div class="criteriaSelector">
                                    <asp:Label class="criteriaSelectorLabel" runat="server" Text="Select Field Value"></asp:Label>
                                    <asp:DropDownList id="criteriaValueDropdown" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="criteriaSelector">
                                <asp:Button id="addSelectionBtn" runat="server" Text="Add Selection" />
                            </div>
                        </div>

                        <asp:Button id="submitBtn" runat="server" Text="Search" />
                    </div>

                    <!-- results in grid view with search criteria applied -->
                    <div class="resultsContainer">
                        <div id="resultsPlaceholder">
                            Hit search to load respondent results based on your selected criteria
                            <br />
                            <span class="demoInfo">The filled criteria are just for demonstration</span>
                        </div>
                        <asp:GridView id="gvResults" runat="server" OnRowDataBound="gvResults_RowDataBound"></asp:GridView>
                    </div>
                </div>
                <asp:Label id="notLoggedInLabel" runat="server" Text="Please Login to use this page"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>
