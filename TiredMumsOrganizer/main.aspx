<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="main.aspx.vb" Inherits="main" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div id="main" runat="server"><asp:Repeater id="parentRepeater" runat="server">
    <ItemTemplate><h2><%# DataBinder.Eval(Container.DataItem, "teamName") %></h2>
    <table class="main"><asp:Repeater ID="childRepeater" onitemdatabound="chldFunction" runat="server"  DataSource='<%# Container.DataItem.Row.GetChildRows("myrelation")%>'> 
       <HeaderTemplate>
        <tr class="main"><td class="main">Username</td><td class="main">Task Name</td><td class="main">Task On List</td><td class="main">Planning</td><td class="main">Check</td><td class="main">Do</td><td class="main">Test</td><td class="main">Review</td><td class="main"></td><td class="main"></td></tr>
         </HeaderTemplate>      
        <ItemTemplate><tr class="main">
            <td class="main"><asp:label id="uname" runat="server" text=<%# Container.DataItem("username")%>/></td>
            <td class="main"><asp:label id="task" runat="server" text=<%# Container.DataItem("taskname")%> /></td>
            <td class="main"><asp:label id="onlist" runat="server" text=<%# Container.DataItem("on_list")%> /></td>
            <td class="main"><asp:label id="planning" runat="server" text=<%# Container.DataItem("planning")%> /></td>
            <td class="main"><asp:label id="check" runat="server" text=<%# Container.DataItem("check")%> /></td>
            <td class="main"><asp:label id="do" runat="server" text=<%# Container.DataItem("do")%> /></td>
            <td class="main"><asp:label id="test" runat="server" text=<%# Container.DataItem("test")%> /></td>
            <td class="main"><asp:label id="rev" runat="server" text=<%# Container.DataItem("review")%> /></td>
            <td class="main"><asp:LinkButton ID="edit" runat="server" Text="edit" OnClick="show_EditTask" CommandArgument=<%# Container.DataItem("taskname")%> CommandName=<%# Container.DataItem("userID")%> /></td>
            <td class="main"><asp:LinkButton ID="delete" runat="server" Text="delete" OnClick="show_DeleteTask" CommandArgument=<%# Container.DataItem("taskname")%> CommandName=<%# Container.DataItem("userID")%>/></td></tr>
        </ItemTemplate>
        </asp:Repeater>
        </table>
        </ItemTemplate>
    </asp:Repeater><br /><br />
    <div class="centerContent"><asp:button ID="addTask" runat="server" OnClick="show_AddTask" text="Add Task" />
        
    </div></div>
    
    <div id="addtaskdiv" runat="server">
        <table class="welcome">
       <tr><td>Task:</td><td><asp:textbox id="taskName" runat="server" Text=""></asp:textbox></td></tr>
       <tr><td>User:</td><td><asp:dropdownlist id="taskUser" runat="server"></asp:dropdownlist></td></tr>
       <tr><td>Stage:</td><td><asp:dropdownlist id="taskStage" runat="server"></asp:dropdownlist></td></tr></table>
        <div class="centerContent"><h3><asp:button ID="addTaskSubmit" runat="server" OnClick="submit_AddTask" text="Submit Task" />
        <asp:button ID="cancelTaskSubmit" runat="server" OnClick="canc_Button" text="Cancel" /></h3></div>
        </div>
        <div id="existingtaskdiv" runat="server">
        <div class="centerContent"><asp:label ID="taskexists" runat="server" Text=""></asp:label></div>
            <asp:label ID="hiddenTask_name" runat="server" text="" CssClass="hide"/>
        <div class="centerContent"><h3><asp:button ID="SubmitExisting" runat="server" OnClick="submit_ExistingTask" text="Submit Task" />
        <asp:button ID="CancExisting" runat="server" OnClick="canc_Button" text="Cancel" /></h3></div>     
        </div>
        <div id="edittaskdiv" runat="server">
        <table class="welcome">
        <tr><td>Task:</td><td><asp:textbox id="editTaskName" runat="server" Text=""></asp:textbox></td></tr>
        <tr><td>User:</td><td><asp:dropdownlist id="editTaskUser" runat="server"></asp:dropdownlist></td></tr>
       <tr><td>Stage:</td><td><asp:dropdownlist id="editTaskStage" runat="server"></asp:dropdownlist></td></tr>
         </table>
          <div class="centerContent"><h3><asp:button ID="SubmitEditTask" runat="server" OnClick="submit_EditTask" text="Submit Task" />
        <asp:button ID="CancEditTask" runat="server" OnClick="canc_Button" text="Cancel" /></h3></div>  
   </div>
            <div id="deletetaskdiv" runat="server">
        <table class="welcome">
        <tr><td>Task:</td><td><asp:label id="deleteTaskName" runat="server" Text=""></asp:label></td></tr>
        <tr><td>User:</td><td><asp:label id="deleteTaskUser" runat="server" Text=""></asp:label></td></tr>
       <tr><td>Stage:</td><td><asp:label id="deleteTaskStage" runat="server" Text=""></asp:label></td></tr>
         </table>
          <div class="centerContent"><h3><asp:button ID="submitDeleteTask" runat="server" OnClick="submit_DeleteTask" text="Submit Task" />
        <asp:button ID="Button2" runat="server" OnClick="canc_Button" text="Cancel" /></h3></div>  
   </div>
    <div id="noaccessdiv" runat="server">
        <div class="centerContent"><h3><asp:label ID="noaccess" runat="server" Text="You do not have edit/delete access for this team"></asp:label></div>
        <div class="centerContent"><asp:button ID="returnmain" runat="server" OnClick="canc_Button" text="Return to Main Page" /></h3> </div> 
        </div>
        
    
  
</asp:Content>

