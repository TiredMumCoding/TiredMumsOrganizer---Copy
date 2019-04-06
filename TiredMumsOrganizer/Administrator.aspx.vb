Imports System.Data
Imports System.Data.SqlClient

Partial Class Administrator
    Inherits System.Web.UI.Page

    Dim connectionString As String = ConfigurationManager.ConnectionStrings("TiredMumOrgConnectionString").ConnectionString
    Dim TeamDS As New DataSet
    Dim USerDS As New DataSet
    Dim FetchUserDS As New DataSet
    Dim UsersforTeamDS As New DataSet
    Dim TasksforUserDS As New DataSet
    Dim firstName As String
    Dim surName As String
    Dim Teamname As Integer
    Dim password As String
    Dim checkFlag As String
    Dim Reader As SqlDataReader


    Public Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            hideAll()
            main.Style.Add("display", "block")
        End If
    End Sub


    Public Sub hideAll()
        main.Style.Add("display", "none")
        addteamdiv.Style.Add("display", "none")
        editteamdiv.Style.Add("display", "none")
        confrirmeditteamdiv.Style.Add("display", "none")
        deleteteamdiv.Style.Add("display", "none")
        confirmdeleteteamdiv.Style.Add("display", "none")
        adduserdiv.Style.Add("display", "none")
        edituserdiv.Style.Add("display", "none")
        confirmedituserdiv.Style.Add("display", "none")
        deleteusrdiv.Style.Add("display", "none")
        confirmdeleteuserdiv.Style.Add("display", "none")
        addadmindiv.Style.Add("display", "none")
        confirmaddadmindiv.Style.Add("display", "none")
        removeadmindiv.Style.Add("display", "none")
        confirmremoveadmindiv.Style.Add("display", "none")
    End Sub


    Public Sub getTeamList(teamList As DropDownList)
        Dim cnn As New SqlConnection(connectionString)
        cnn.Open()
        Dim cmd As New SqlCommand("select * from worklog_teams", cnn)
        Dim TeamDA As New SqlDataAdapter(cmd)
        TeamDA.Fill(TeamDS)

        teamList.DataSource = TeamDS
        teamList.DataTextField = "teamname"
        teamList.DataValueField = "teamid"
        teamList.DataBind()

        cnn.Close()
    End Sub

    Public Sub getUserList(userList As DropDownList, teamID As Integer)
        Dim cnn As New SqlConnection(connectionString)
        cnn.Open()
        Dim cmd As New SqlCommand("SELECT userid, teamid, firstname + ' '+ surname as thename FROM [WorkLog].[dbo].[WorkLog_Users] where teamid=@teamid", cnn)
        cmd.Parameters.Add("@teamid", SqlDbType.Int)
        cmd.Parameters("@teamid").Value = teamID
        Dim DA As New SqlDataAdapter(cmd)
        DA.Fill(USerDS)
        userList.DataSource = USerDS
        userList.DataTextField = "thename"
        userList.DataValueField = "userid"
        userList.DataBind()
        cnn.Close()
    End Sub


    Public Sub fetchteam(teamname As String, teamid As Integer) 'find out whether a teamname already exists.  If it does, display a warning message, otherwise perform the edit
        Dim cnn As New SqlConnection(connectionString)
        cnn.Open()
        Dim cmd As New SqlCommand("select * from worklog_teams where teamname = @teamname", cnn)
        cmd.Parameters.Add("@teamname", SqlDbType.VarChar)
        cmd.Parameters("@teamname").Value = teamname
        Reader = cmd.ExecuteReader()
        If Reader.HasRows() Then
            Session("checkflag") = "true"
            confirmedittteamlabel.Text = "There is already a team by the name of " + teamname + ". Are you sure you want to create a team by the same name?"
        Else Session("checkflag") = "false"
            updateteam(teamname, teamid)
        End If
        cnn.Close()
    End Sub

    Public Sub updateteam(teamname As String, teamid As Integer)
        Dim cnn As New SqlConnection(connectionString)
        cnn.Open()
        Dim cmd As New SqlCommand("insert into WorkLog_Audit values " _
        & "( " _
        & "(case when " _
        & "not exists (select * from WorkLog_Audit where TeamID = @teamid and ChangeType = 'Edit_Team') then 1 " _
        & "else (select MAX(theOrder) from Worklog_Audit where teamid = @teamid and ChangeType = 'Edit_Team') + 1 " _
        & "End), " _
        & "(select '' as TaskName), " _
        & "(select '' as TaskID), " _
        & "(select '' as UserID), " _
        & "@teamid, " _
        & "(select '' as stageid), " _
        & "'Edit_Team', " _
        & "GETDATE()) " _
        & "update worklog_teams set teamname = @teamname where teamid = @teamid", cnn)
        cmd.Parameters.Add("@teamname", SqlDbType.VarChar)
        cmd.Parameters("@teamname").Value = teamname
        cmd.Parameters.Add("@teamid", SqlDbType.Int)
        cmd.Parameters("@teamid").Value = teamid


        cmd.ExecuteNonQuery()
        cnn.Close()
        hideAll()
        main.Style.Add("display", "block")
        Session("checkflag") = Nothing 'update the flag and label used to control whether a user exists already/whether to overwrite
        checkFlag = "true"
        confirmedittteamlabel.Text = ""

    End Sub

    Public Sub fetchuser(fname As String, sname As String, teamid As Integer, userid As Integer) 'find out whether user of same name exists on a team

        Dim cnn As New SqlConnection(connectionString)
        cnn.Open()
        Dim cmd As New SqlCommand("select * from worklog_users where firstname = @firstname and surname = @surname and teamid = @id", cnn)
        cmd.Parameters.Add("@firstname", SqlDbType.VarChar)
        cmd.Parameters("@firstname").Value = fname
        cmd.Parameters.Add("@surname", SqlDbType.VarChar)
        cmd.Parameters("@surname").Value = sname
        cmd.Parameters.Add("@id", SqlDbType.Int)
        cmd.Parameters("@id").Value = teamid

        Reader = cmd.ExecuteReader()
        If Reader.HasRows() Then
            confirmLabel.Text = "There is already a user by this name on this team.  Are you sure you want to add another user by the same name?"
            Session("checkFlag") = "True"
        Else Session("checkFlag") = "False"
            Update_User(fname, sname, teamid, userid)
        End If
        Reader.Close()
        cnn.Close()
    End Sub


    Public Sub Get_UsersForTeam(teamid As Integer)  'get users for a given team and add to a dataset
        Dim cnn As New SqlConnection(connectionString)
        cnn.Open()
        Dim cmd As New SqlCommand("select userid, teamid from worklog_users where teamid = @teamID", cnn)
        cmd.Parameters.Add("@teamid", SqlDbType.Int)
        cmd.Parameters("@teamid").Value = teamid
        Dim DA As New SqlDataAdapter(cmd)
        DA.Fill(UsersforTeamDS)
        cnn.Close()
    End Sub


    Public Sub Get_TasksForuser(userid As Integer)
        Dim cnn As New SqlConnection(connectionString)
        cnn.Open()
        Dim cmd As New SqlCommand("select taskid, userid from WorkLog_Tasks where userid = @userid", cnn)
        cmd.Parameters.Add("@userid", SqlDbType.Int)
        cmd.Parameters("@userid").Value = userid
        Dim DA As New SqlDataAdapter(cmd)
        DA.Fill(TasksforUserDS)
        cnn.Close()
    End Sub


    Public Sub Update_User(firstname As String, surname As String, teamid As Integer, userid As Integer)
        Dim cnn As New SqlConnection(connectionString)
        cnn.Open()
        Dim cmd As New SqlCommand("insert into WorkLog_Audit values " _
        & "( " _
        & "(case when " _
        & "not exists (select * from WorkLog_Audit where UserID = @userid and ChangeType = 'Edit_User') then 1 " _
        & "else (select MAX(theOrder) from Worklog_Audit where userid = @userid and ChangeType = 'Edit_User') + 1 " _
        & "End), " _
        & "(select '' as TaskName), " _
        & "(select '' as TaskID), " _
        & "(select userid from Worklog_users where userid = @userid), " _
        & "(select teamid from WorkLog_Users where UserID = @userid), " _
        & "(select '' as stageid), " _
        & "'Edit_User', " _
        & "GETDATE()) " _
        & "update worklog_users set Firstname = @firstname, surname = @surname, teamid = @teamid where userid = @userid", cnn)
        cmd.Parameters.Add("@firstname", SqlDbType.VarChar)
        cmd.Parameters("@firstname").Value = firstname
        cmd.Parameters.Add("@surname", SqlDbType.VarChar)
        cmd.Parameters("@surname").Value = surname
        cmd.Parameters.Add("@teamid", SqlDbType.Int)
        cmd.Parameters("@teamid").Value = teamid
        cmd.Parameters.Add("@userid", SqlDbType.Int)
        cmd.Parameters("@userid").Value = userid

        cmd.ExecuteNonQuery()
        cnn.Close()
        hideAll()
        main.Style.Add("display", "block")
        Session("checkFlag") = Nothing 'update the flag and label used to control whether a user exists already/whether to overwrite
        confirmLabel.Text = ""
    End Sub


    Public Function Remove_AsAdmin(userid As Integer, teamid As Integer)
        Dim cnn As New SqlConnection(connectionString)
        cnn.Open()
        Dim cmd As New SqlCommand("If exists (select * From WorkLog_SecGroups where userid = @userid and groupid = 1) " _
        & "Begin " _
        & "Delete from worklog_secgroups where userid = @userid and groupid = 1 " _
        & "insert into worklog_audit values( " _
        & "(case when " _
        & "Not exists (select * from WorkLog_Audit where userid = @userid And Changetype = 'Remove_Admin') then 1 " _
        & "Else (Select MAX(theOrder) from Worklog_Audit where userid = @userid And ChangeType = 'Remove_Admin') + 1 " _
        & "End), " _
        & "'', 0, @userid, @teamid, 0, 'Remove_Admin', getdate()) End", cnn)

        cmd.Parameters.Add("@userid", SqlDbType.Int)
        cmd.Parameters("@userid").Value = userid
        cmd.Parameters.Add("@teamid", SqlDbType.VarChar)
        cmd.Parameters("@teamid").Value = teamid

        Dim ret As Integer = cmd.ExecuteNonQuery
        Return ret
    End Function


    Public Sub Show_AddTeam(sender As Object, e As EventArgs)
        hideAll()
        addteamdiv.Style.Add("display", "block")
        failedaddteam.Text = ""
        mainaddteam.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
    End Sub


    Public Sub submit_AddTeam(sender As Object, e As EventArgs)
        If mainaddteam.Text = "" Then
            failedaddteam.Text = "Please Complete Required Fields"
            mainaddteam.BackColor = System.Drawing.ColorTranslator.FromHtml("#ff1493")
        Else
            Dim cnn As New SqlConnection(connectionString)
            cnn.Open()
            Dim cmd As New SqlCommand("select * from worklog_teams where teamname = @teamname", cnn)
            cmd.Parameters.Add("@teamname", SqlDbType.VarChar)
            cmd.Parameters("@teamname").Value = mainaddteam.Text
            Reader = cmd.ExecuteReader()
            If Reader.HasRows() Then
                failedaddteam.Text = "Team Already exists.  Please try again"
                Reader.Close()
            Else
                Reader.Close()
                Dim cmd1 As New SqlCommand("If Not exists (select * from WorkLog_Teams where teamname =  @teamName) " _
                & "Begin " _
                & "insert into WorkLog_Teams values (@teamName) " _
                & "insert into WorkLog_Audit values  (1,'', 0, 0, " _
                & "(select teamid from WorkLog_Teams where TeamName = @teamName), " _
                & "0, " _
                & "'New_Team', " _
                & "getdate() " _
                & ") " _
                & "End", cnn)
                cmd1.Parameters.Add("@teamname", SqlDbType.VarChar)
                cmd1.Parameters("@teamname").Value = mainaddteam.Text
                cmd1.ExecuteNonQuery()
                hideAll()
                main.Style.Add("display", "block")
            End If
        End If
    End Sub


    Public Sub Show_EditTeam(sender As Object, e As EventArgs)
        hideAll()
        editteamdiv.Style.Add("display", "block")
        getTeamList(MainEditTeamList) 'populate drop-down list of teams
    End Sub


    Public Sub Submit_EditTeam(sender As Object, e As EventArgs)
        hideAll()
        confrirmeditteamdiv.Style.Add("display", "block")
        confirmeditteamtext.Text = MainEditTeamList.SelectedItem.Text
        confirmeditteamtext.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
        confirmedittteamlabel.Text = ""
    End Sub


    Public Sub submit_ConfirmEditTeam(sender As Object, e As EventArgs)
        If confirmeditteamtext.Text = "" Then
            confirmeditteamtext.BackColor = System.Drawing.ColorTranslator.FromHtml("#ff1493")
            confirmedittteamlabel.Text = "Please Complete Required Fields"
        Else
            If Session("checkflag") Is Nothing Then 'if the flag isn't populated then haven't yet checked whether team name aleardy exists.  If it is populated then it has been checked and a deicsion has been made to edit the team anyway
                fetchteam(confirmeditteamtext.Text, MainEditTeamList.SelectedItem.Value)
            Else
                updateteam(confirmeditteamtext.Text, MainEditTeamList.SelectedItem.Value)
            End If
        End If
    End Sub


    Public Sub show_DeleteTeam(sener As Object, e As EventArgs)
        hideAll()
        deleteteamdiv.Style.Add("display", "block")
        getTeamList(deleteTeamList)
    End Sub


    Public Sub delete_Team(sender As Object, e As EventArgs)
        hideAll()
        confirmdeleteteamdiv.Style.Add("display", "block")
        confirmdelteamlabel.Text = "Please confirm that you would Like To delete team " + deleteTeamList.SelectedItem.Text + ". All Users And Tasks associated With this team will be deleted."
        confirmDelTeamButton.CommandArgument = deleteTeamList.SelectedValue
    End Sub

    Public Sub confirmDelete_Team(sender As Object, e As EventArgs)
        Dim cnn As New SqlConnection(connectionString)
        cnn.Open()
        Dim cmd3 As New SqlCommand("insert into WorkLog_Audit values ( 1,'', '', '', @teamid, '', 'Delete_Team', getdate()) delete from worklog_teams where teamid = @teamid", cnn)
        cmd3.Parameters.Add("@teamid", SqlDbType.Int)

        Get_UsersForTeam(sender.commandargument)
        If UsersforTeamDS.Tables(0).Rows.Count >= 1 Then 'if the team has users, iterate through deleting tasks and users
            Dim i As Integer = 0
            Dim cmd As New SqlCommand(" execute Worklog_Tables_Delete_Tasks @usrid, @taskid", cnn)
            cmd.Parameters.Add("@usrid", SqlDbType.Int)
            cmd.Parameters.Add("@taskid", SqlDbType.Int)
            Dim cmd2 As New SqlCommand("insert into WorkLog_Audit values ( 1,'', '', @userid, @teamid, '', 'Delete_User', getdate()) delete from worklog_users where userid = @userid and teamid = @teamid", cnn)
            cmd2.Parameters.Add("@userid", SqlDbType.Int)
            cmd2.Parameters.Add("@teamid", SqlDbType.Int)
            Dim cmd4 As New SqlCommand("select * from WorkLog_SecGroups where userid = @userid", cnn)
            cmd4.Parameters.Add("@userid", SqlDbType.Int)

            For Each usrrow As DataRow In UsersforTeamDS.Tables(0).Rows
                Get_TasksForuser(UsersforTeamDS.Tables(0).Rows(i)(0)) 'for the ith user returned by the get_usersforteam function, check for tasks
                If TasksforUserDS.Tables(0).Rows.Count >= 1 Then 'if there are tasks, iterate through deleting each
                    cmd.Parameters("@usrid").Value = UsersforTeamDS.Tables(0).Rows(i)(0)
                    Dim p As Integer = 0

                    For Each tskrow As DataRow In TasksforUserDS.Tables(0).Rows
                        cmd.Parameters("@taskid").Value = TasksforUserDS.Tables(0).Rows(p)(0)
                        cmd.ExecuteNonQuery()
                        p = p + 1
                    Next
                End If

                Remove_AsAdmin(UsersforTeamDS.Tables(0).Rows(i)(0), sender.commandargument) 'delete secgroup rows if administrator

                cmd2.Parameters("@userid").Value = UsersforTeamDS.Tables(0).Rows(i)(0)
                cmd2.Parameters("@teamid").Value = UsersforTeamDS.Tables(0).Rows(i)(1)
                cmd2.ExecuteNonQuery() 'then delete the user

                i = i + 1
                TasksforUserDS.Clear()
            Next

        End If

        cmd3.Parameters("@teamid").Value = sender.commandargument
        cmd3.ExecuteNonQuery()
        cnn.Close()  'finally delete the team

        hideAll()
        main.Style.Add("display", "block")
    End Sub


    Public Sub Show_AddUser(sender As Object, e As EventArgs)
        hideAll()
        adduserdiv.Style.Add("display", "block")
        getTeamList(addteamList)
        failed.Text = ""
        fname.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
        sname.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
        pword.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
    End Sub


    Public Sub Submit_AddUser(sender As Object, e As EventArgs)
        Dim firstName = fname.Text
        Dim surName = sname.Text
        Dim Teamname = CInt(addteamList.SelectedValue)
        Dim password = pword.Text

        failed.Text = ""
        fname.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
        sname.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
        pword.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")

        checkFlag = "True"

        If fname.Text = "" Then
            fname.BackColor = System.Drawing.ColorTranslator.FromHtml("#ff1493")
            checkFlag = False
        End If

        If sname.Text = "" Then
            sname.BackColor = System.Drawing.ColorTranslator.FromHtml("#ff1493")
            checkFlag = False
        End If

        If pword.Text = "" Then
            pword.BackColor = System.Drawing.ColorTranslator.FromHtml("#ff1493")
            checkFlag = False
        End If

        If checkFlag = True Then
            Dim cnn As New SqlConnection(connectionString)
            cnn.Open()
            Dim cmd As New SqlCommand("If Not exists (Select * from WorkLog_Users where FirstName = @firstname And SurName = @surname And pword = @pword) " _
        & "Begin " _
        & "insert into WorkLog_Users values (@teamid, @firstname, @surname, @pword) " _
        & "insert into WorkLog_Audit values " _
        & "(1,'', 0, " _
        & "(Select userid from WorkLog_Users where FirstName = @firstname And SurName = @surname and pword = @pword), " _
        & "0, 0, 'New_User', getdate()) " _
        & "End", cnn)

            cmd.Parameters.Add("@firstname", SqlDbType.VarChar)
            cmd.Parameters("@firstname").Value = firstName
            cmd.Parameters.Add("@surname", SqlDbType.VarChar)
            cmd.Parameters("@surname").Value = surName
            cmd.Parameters.Add("@teamid", SqlDbType.Int)
            cmd.Parameters("@teamid").Value = Teamname
            cmd.Parameters.Add("@pword", SqlDbType.VarChar)
            cmd.Parameters("@pword").Value = password

            Dim result As Integer = cmd.ExecuteNonQuery
            If result >= 1 Then

                Response.Redirect("administrator.aspx")
            Else
                fname.Text = ""
                sname.Text = ""
                pword.Text = ""
                failed.Text = "User Already Exists. Please try again"
            End If
            cnn.Close()

        Else failed.Text = "Please Complete Required Fields"

        End If
        checkFlag = ""

    End Sub


    Public Sub Show_EditUser(sender As Object, e As EventArgs)
        hideAll()
        edituserdiv.Style.Add("display", "block")
        getTeamList(editTeamList)
        getUserList(editUserList, CInt(editTeamList.SelectedItem.Value))
        confirmLabel.Text = ""
        editfirstname.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
        editsurname.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
    End Sub


    Public Sub update_EditUserList(sender As Object, e As EventArgs) 'user list needs to update when team list updates
        getUserList(editUserList, CInt(editTeamList.SelectedItem.Value))

    End Sub

    Public Sub submit_EditUSer(sender As Object, e As EventArgs)
        hideAll()
        confirmedituserdiv.Style.Add("display", "block")
        Dim checkArray() = editUserList.SelectedItem.Text.Split(" ")

        getTeamList(editteamname)
        editteamname.SelectedValue = editTeamList.SelectedValue 'team name defaults to the team name of the edited user
        editfirstname.Text = checkArray(0)
        editsurname.Text = checkArray(1)
        confirmEditUser.CommandArgument = editUserList.SelectedItem.Value  'attach id of user to be edited to confirm edit user button
    End Sub


    Public Sub submit_ConfirmEditUser(sender As Object, e As EventArgs)
        confirmLabel.Text = ""
        editfirstname.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
        editsurname.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
        checkFlag = True
        If editfirstname.Text = "" Then
            editfirstname.BackColor = System.Drawing.ColorTranslator.FromHtml("#ff1493")
            checkFlag = False
        End If
        If editsurname.Text = "" Then
            editsurname.BackColor = System.Drawing.ColorTranslator.FromHtml("#ff1493")
            checkFlag = False
        End If

        If checkFlag = True Then
            Dim o As Object = Session("checkFlag") ' use this variable to check whether the is this user on this team already code has run
            If o Is Nothing Then 'run checking code if it hasn't already run
                fetchuser(editfirstname.Text, editsurname.Text, CInt(editteamname.SelectedItem.Value), sender.commandargument)
            Else
                Update_User(editfirstname.Text, editsurname.Text, CInt(editteamname.SelectedItem.Value), sender.commandargument)
                'if there is a value means that checking code has already run and user of same name is deliberately being added
            End If
        Else
            confirmLabel.Text = "Please Complete Required Fields"
        End If
    End Sub

    Public Sub show_DeleteUser(sender As Object, e As EventArgs)
        hideAll()
        deleteusrdiv.Style.Add("display", "block")
        getTeamList(DeleteUsrTeamList)
        getUserList(DeleteUsrList, CInt(DeleteUsrTeamList.SelectedItem.Value))
    End Sub


    Public Sub update_DeleteUsrList() 'delete user list needs to change when team list does
        getUserList(DeleteUsrList, CInt(DeleteUsrTeamList.SelectedItem.Value))
    End Sub


    Public Sub delete_User(sender As Object, e As EventArgs)
        hideAll()
        confirmdeleteuserdiv.Style.Add("display", "block")
        confirmdeluserlabel.Text = "Please confirm that you would like to delete use " + DeleteUsrList.SelectedItem.Text + ". All tasks associated with this user will be deleted."
        confirmdeluserbutton.CommandArgument = DeleteUsrList.SelectedValue
        confirmdeluserbutton.CommandName = DeleteUsrTeamList.SelectedValue
    End Sub


    Public Sub confirm_DeleteUser(sender As Object, e As EventArgs)
        Dim cnn As New SqlConnection(connectionString)
        cnn.Open()
        Dim cmd As New SqlCommand(" execute Worklog_Tables_Delete_Tasks @usrid, @taskid", cnn)
        cmd.Parameters.Add("@usrid", SqlDbType.Int)
        cmd.Parameters.Add("@taskid", SqlDbType.Int)
        Dim cmd2 As New SqlCommand("insert into WorkLog_Audit values ( 1,'', '', @userid, @teamid, '', 'Delete_User', getdate()) delete from worklog_users where userid = @userid and teamid = @teamid", cnn)
        cmd2.Parameters.Add("@userid", SqlDbType.Int)
        cmd2.Parameters.Add("@teamid", SqlDbType.Int)
        cmd2.Parameters("@userid").Value = sender.commandargument
        cmd2.Parameters("@teamid").Value = sender.commandname
        Get_TasksForuser(sender.commandargument)

        If TasksforUserDS.Tables(0).Rows.Count >= 1 Then 'check whether there are tasks for a user
            cmd.Parameters("@usrid").Value = sender.commandargument
            Dim i As Integer = 0
            For Each tskrow As DataRow In TasksforUserDS.Tables(0).Rows 'if so, iterate through them and delete
                cmd.Parameters("@taskid").Value = TasksforUserDS.Tables(0).Rows(i)(0)
                cmd.ExecuteNonQuery()
                i = i + 1
            Next

        End If

        cmd2.ExecuteNonQuery() 'then delete the user
        hideAll()
        main.Style.Add("display", "block")

        cnn.Close()
    End Sub

    Public Sub show_MakeAdmin(sender As Object, e As EventArgs)
        hideAll()
        addadmindiv.Style.Add("display", "block")
        getTeamList(addadminteamlist)
        getUserList(addadminusrlist, addadminteamlist.SelectedItem.Value)
    End Sub

    Public Sub update_AdminUserList(sender As Object, e As EventArgs) 'user list updates when team list does
        getUserList(addadminusrlist, CInt(addadminteamlist.SelectedItem.Value))
    End Sub

    Public Sub add_admin(sender As Object, e As EventArgs)
        Dim TeamID = addadminteamlist.SelectedItem.Value 'Split(sender.commandargument, ",")(0)
        Dim UsrID = addadminusrlist.SelectedItem.Value 'Split(sender.commandargument, ",")(1)

        Dim cnn As New SqlConnection(connectionString)
        cnn.Open()
        Dim cmd As New SqlCommand("If Not exists (select * from worklog_secgroups where userid = @userid And groupid = 1) " _
        & "Begin " _
        & "insert into worklog_secgroups values (@userid, 1)" _
        & "insert into worklog_audit values(" _
        & "(case when " _
        & "Not exists (select * from WorkLog_Audit where userid = @userid And ChangeType = 'Add_Admin') then 1 " _
        & "Else (Select MAX(theOrder) from Worklog_Audit where userid = @userid And ChangeType = 'Add_Admin') + 1 " _
        & "End), " _
        & "'', 0, @userid, @teamid, 0, 'Add_Admin', getdate() ) " _
        & "End", cnn)
        cmd.Parameters.Add("@userid", SqlDbType.Int)
        cmd.Parameters("@userid").Value = UsrID
        cmd.Parameters.Add("@teamid", SqlDbType.Int)
        cmd.Parameters("@teamid").Value = TeamID
        Dim ret As Integer = cmd.ExecuteNonQuery()

        hideAll()
        confirmaddadmindiv.Style.Add("display", "block")
        If ret >= 1 Then 'if the user already has an entry in the secgroups table then the update won't happen and ret < 1
            addadminlbl.Text = addadminusrlist.SelectedItem.Text + " has been added as an administrator"
        Else
            addadminlbl.Text = "System not updated. " + addadminusrlist.SelectedItem.Text + " is already an administrator"
        End If
    End Sub

    Public Sub show_RemoveAdmin(sender As Object, e As EventArgs)
        hideAll()
        removeadmindiv.Style.Add("display", "block")
        getTeamList(removeadmnteamlist)
        getUserList(removeadminusrlist, removeadmnteamlist.SelectedItem.Value)
    End Sub


    Public Sub update_RemoveUserList(sender As Object, e As EventArgs)
        getUserList(removeadminusrlist, removeadmnteamlist.SelectedItem.Value)
    End Sub

    Public Sub delete_admin(sender As Object, e As EventArgs) 'put this code in a function because it's also used for deleting teams and users
        hideAll()
        confirmremoveadmindiv.Style.Add("display", "block")

        If Remove_AsAdmin(removeadminusrlist.SelectedItem.Value, removeadmnteamlist.SelectedItem.Value) >= 1 Then 'update only happens if the user has an entry in the secgroups table, otherwise ret <1
            rmvadminrtrn.Text = removeadminusrlist.SelectedItem.Text + " has been removed as an administrator"
        Else
            rmvadminrtrn.Text = "System not updated " + removeadminusrlist.SelectedItem.Text + " is not an administrator"
        End If
    End Sub


    Public Sub Cancel(sender As Object, e As EventArgs)
        hideAll()
        main.Style.Add("display", "block")
    End Sub

    Public Sub return_MainPage(sender As Object, e As EventArgs)
        Response.Redirect("main.aspx")
    End Sub


End Class
