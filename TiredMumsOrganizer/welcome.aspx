<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="welcome.aspx.vb" Inherits="_Default" %>
<%@ MasterType VirtualPath ="~/MasterPage.master"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="centerContent"><h2>Sign-in or Create an Account</h2></div>
    <div id ="welcomebuttons" runat="server">
    <table class="welcome">
     <tr><td class="center"><asp:button ID="signin" onclick="sign_In" runat="server" Text="Sign In"/></td></tr>
      <tr><td class="center"><asp:button ID="createAccount" OnClick="create_Acc" runat="server" Text="Create Account"/></td></tr>
    </table></div>
      <div runat="server" id="signinWrapper"><h3><table class="welcome">
       <tr><td>First Name:</td><td><asp:TextBox ID="fname" runat="server" Text="" /></td></tr>
        <tr><td>Surname:</td><td><asp:TextBox ID="sname" runat="server" Text="" /></td></tr>
        <tr><td>Password:</td><td><asp:TextBox ID="pword" runat="server" Text="" /></td></tr>
       </table></h3>
    <div class="centerContent"><h3><asp:button ID="submitsignin" runat="server" Text="Submit Sign In" OnClick="Submit_SignIn"/>
    <asp:button ID="cancsignin" runat="server" OnClick="Cancel" Text="Cancel"/></h3></div>
    <div class="centerContent"><asp:label ID="failed" runat="server" Text=""></asp:label></div></div> 
    <div id="createAccWrapper" runat="server"><h3><table class="welcome">
       <tr><td>First Name:</td><td><asp:TextBox ID="cfname" runat="server" Text="" /></td></tr>
        <tr><td>Surname:</td><td><asp:TextBox ID="csname" runat="server" Text="" /></td></tr>
        <tr><td>Team Name:</td><td><asp:DropDownList ID="teamList" runat="server" AutoPostBack="true" /></td></tr>
        <tr><td>Password:</td><td><asp:TextBox ID="cpword" runat="server" Text="" /></td></tr>
       </table>
    <div class="centerContent"><h3><asp:button ID="submitCreateAccount" runat="server" OnClick="create_Account" Text="Submit New Account"/>
      <asp:button ID="canccreate" runat="server" OnClick="Cancel" Text="Cancel"/></h3></div>
        <div class="centerContent"><asp:label ID="cfailed" runat="server" Text=""></asp:label></div></div>
        </h3>
</asp:Content>

