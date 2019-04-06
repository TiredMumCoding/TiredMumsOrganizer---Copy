<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="Administrator.aspx.vb" Inherits="Administrator" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <div id="main" runat="server"><table id="adminlist" class="welcome" runat="server">
        <tr><td class="center"><asp:LinkButton ID="addteam" runat="server" Text="Add Team" OnClick="show_AddTeam"/></td></tr>
        <tr><td class="center"><asp:LinkButton ID="editteam" runat="server" Text="Edit Team" OnClick="show_EditTeam"/></td></tr>
        <tr><td class="center"><asp:LinkButton ID="deleteteam" runat="server" Text="Delete Team" OnClick="show_DeleteTeam"/></td></tr>
        <tr><td class="center"><asp:LinkButton ID="adduser" runat="server" Text="Add User" OnClick ="Show_AddUser"/></td></tr>
        <tr><td class="center"><asp:LinkButton ID="edituser" runat="server" Text="Edit User" OnClick ="show_EditUser"/></td></tr>
        <tr><td class="center"><asp:LinkButton ID="deleteuser" runat="server" Text="Delete User" OnClick="show_DeleteUser"/></td></tr>
        <tr><td class="center"><asp:LinkButton ID="makeadmin" runat="server" Text="Make Administrator" OnClick="show_MakeAdmin"/></td></tr>
       <tr><td class="center"><asp:LinkButton ID="removeadmin" runat="server" Text="Remove Administrator" OnClick="show_RemoveAdmin"/></td></tr>
        </table></br>
       <div class="centerContent"><asp:Button ID="home" runat="server" OnClick="return_MainPage" Text="Return to Main Page" /></div>
   </div> 
     <div id="addteamdiv" runat="server">
        <table class="welcome">
       <tr><td>Team Name:</td><td><asp:TextBox ID="mainaddteam" runat="server" Text="" /></td></tr></table>
         <div class="centerContent"><asp:button ID="addteamsubmit" runat="server" OnClick="submit_AddTeam" text="Submit" />
         <asp:button ID="addteamcance" runat="server" OnClick="cancel" text="Cancel" /><br/><br />
        <asp:label ID="failedaddteam" runat="server" Text="" />
         </div></div>
     <div id="editteamdiv" runat="server">
            <table class="welcome">
            <tr><td>Team Name:</td><td><asp:DropDownList ID="MainEditTeamList" runat="server"/></td></tr></table>
        <div class="centerContent"><asp:button ID="editTeamSubmit" runat="server" OnClick="submit_EditTeam" text="Submit" />
         <asp:button ID="canc_EditTeam" runat="server" OnClick="cancel" text="Cancel" /><br/><br />
        </div> </div>
      <div id="confrirmeditteamdiv" runat="server">
        <table class="welcome">
       <tr><td>Team Name:</td><td><asp:textbox ID="confirmeditteamtext" runat="server" Text=""></asp:textbox></td></tr></table>
        <div class="centerContent">
            <asp:Label ID="confirmedittteamlabel" runat="server" Text="" />
            <asp:button ID="confirmeditteam" runat="server" OnClick="submit_ConfirmEditTeam" text="Confirm" />
         <asp:button ID="canceleditteam" runat="server" OnClick="cancel" text="Cancel" /><br/><br />
        </div></div>
    <div id="deleteteamdiv" runat="server">
        <div class="centerContent"><asp:DropDownList id="deleteTeamList" runat="Server" /></br></br>
        <asp:Button ID="deleteTeamButton" runat="server" Text="Delete" OnClick="delete_Team" />
            <asp:button ID="cancelDelTeam" runat="server" OnClick="Cancel" text="Cancel" />
        </div>
    </div>
    <div id="confirmdeleteteamdiv" runat="server">
        <div class="centerContent"><asp:label id="confirmdelteamlabel" text="" runat="Server" /></br></br>
        <asp:Button ID="confirmDelTeamButton" runat="server" Text="Confirm Delete" OnClick="confirmDelete_Team" />
             <asp:Button ID="CancConfrimDelTeam" runat="server" OnClick="Cancel" text="Cancel" />
        </div>
    </div>
     <div id="adduserdiv" runat="server">
        <table class="welcome">
       <tr><td>First Name:</td><td><asp:TextBox ID="fname" runat="server" Text="" /></td></tr>
        <tr><td>Surname:</td><td><asp:TextBox ID="sname" runat="server" Text="" /></td></tr>
        <tr><td>Team Name:</td><td><asp:DropDownList ID="addteamList" runat="server" AutoPostBack="true" /></td></tr>
        <tr><td>Password:</td><td><asp:TextBox ID="pword" runat="server" Text="" /></td></tr></table>
        <div class="centerContent"><asp:button ID="addUSerSubmit" runat="server" OnClick="submit_AddUser" text="Create User" />
           <asp:button ID="addUserCancel" runat="server" OnClick="Cancel" text="Cancel" />
         <br/><br />
          <asp:Label ID="failed" runat="server" Text=""></asp:Label></div>
        </div>
       <div id="edituserdiv" runat="server">
        <table class="welcome">
            <tr><td>Team Name:</td><td><asp:DropDownList ID="editTeamList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="update_EditUserList"/></td></tr>
       <tr><td>User Name:</td><td><asp:DropDownList ID="editUserList" runat="server" AutoPostBack="true" /></td></tr></table>
        <div class="centerContent"><asp:button ID="editUserSubmit" runat="server" OnClick="submit_EditUser" text="Submit" />
         <asp:button ID="CancelEditUser" runat="server" OnClick="cancel" text="Cancel" /><br/><br />
        </div> </div>
           <div id="confirmedituserdiv" runat="server">
        <table class="welcome">
            <tr><td>Team:</td><td><asp:dropdownlist ID="editteamname"  AutoPostBack="true" runat="server"></asp:dropdownlist></td></tr>
       <tr><td>First Name:</td><td><asp:textbox ID="editfirstname" runat="server" Text=""></asp:textbox></td></tr>
           <tr><td>Surname:</td><td><asp:textbox ID="editsurname" runat="server" Text=""></asp:textbox></td></tr></table>
        <div class="centerContent">
            <asp:Label ID="confirmLabel" runat="server" Text="" /> <br /><br />
            <asp:button ID="confirmEditUser" runat="server" OnClick="submit_ConfirmEditUser" text="Confirm" />
         <asp:button ID="cancelConfirmEditUser" runat="server" OnClick="cancel" text="Cancel" /><br/><br />
        </div></div>
        <div id="deleteusrdiv" runat="server">
         <div class="centerContent"><asp:DropDownList id="DeleteUsrTeamList" runat="Server" AutoPostBack="True" OnSelectedIndexChanged="update_DeleteUsrList"/></br></br>
        <asp:DropDownList id="DeleteUsrList" runat="Server" /></br></br>
        <asp:Button ID="deleteUsrBtn" runat="server" Text="Delete" OnClick="delete_User" />
            <asp:button ID="cancUsrBtn" runat="server" OnClick="Cancel" text="Cancel" />
        </div></div>
     <div id="confirmdeleteuserdiv" runat="server">
        <div class="centerContent"><asp:label id="confirmdeluserlabel" text="" runat="Server" /></br></br>
        <asp:Button ID="confirmdeluserbutton" runat="server" Text="Confirm Delete" OnClick="confirm_DeleteUser" />
             <asp:Button ID="canceldeluserbutton" runat="server" OnClick="Cancel" text="Cancel" />
        </div></div>
    <div id="addadmindiv" runat="server">
        <div class="centerContent"><asp:dropdownlist ID="addadminteamlist" runat="server" Autopostback="true" OnSelectedIndexChanged="update_AdminUserList" /></br></br>
        <asp:dropdownlist ID="addadminusrlist" runat="server"/></br></br>
        <asp:Button ID="addadminbtn" runat="server" Text="Add" OnClick ="add_Admin" />
            <asp:Button ID="addadmincanc" runat="server" Text="Cancel" OnClick ="Cancel" />
    </div></div>
    <div id="confirmaddadmindiv" runat="server">
        <div class="centerContent" /><asp:Label id="addadminlbl" runat="server" Text="" /></br></br>
        <asp:button id="addadminrtrn" runat="server" text="Return to Admin Page" onclick="Cancel" />
        </div></div>
        <div id="removeadmindiv" runat="server">
        <div class="centerContent"><asp:dropdownlist ID="removeadmnteamlist" runat="server" Autopostback="true" OnSelectedIndexChanged="update_RemoveUserList" /></br></br>
        <asp:dropdownlist ID="removeadminusrlist" runat="server"/></br></br>
        <asp:Button ID="removeadminbtn" runat="server" Text="Remove" OnClick ="delete_Admin" />
            <asp:Button ID="removeadmincanc" runat="server" Text="Cancel" OnClick ="Cancel" />
    </div></div>
     <div id="confirmremoveadmindiv" runat="server">
        <div class="centerContent" /><asp:Label id="rmvadminrtrn" runat="server" Text="" /></br></br>
        <asp:button id="removeadminrtrn" runat="server" text="Return to Admin Page" onclick="Cancel" />
        </div></div>
   
</asp:Content>

