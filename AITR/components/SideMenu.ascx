<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SideMenu.ascx.cs" Inherits="AITR.components.SideMenu" %>

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

<style>
    /* === side ,enu === */
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

    /* menu items */
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
        transform: translateY(-60px);
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
</style>
