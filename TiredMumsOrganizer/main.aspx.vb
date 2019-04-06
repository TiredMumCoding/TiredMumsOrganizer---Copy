
Imports System.Data
Imports System.Data.SqlClient

Partial Class main
    Inherits System.Web.UI.Page

    Dim connectionString As String = ConfigurationManager.ConnectionStrings("TiredMumOrgConnectionString").ConnectionString
    Dim fetchDS As New DataSet
    Dim StageDS As New DataSet
    Dim userDS As New DataSet
    Dim flag As Integer = 0

    Public Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not IsPostBack Then
            HideAll()
            main.Style.Add("display", "block")

        End If
        MainRepeater()

    End Sub


    Public Sub MainRepeater() ' populate main repeater
        Dim cnn As New SqlConnection(connectionString)
        cnn.Open()

        Dim cmd As New SqlDataAdapter(" select * from WorkLog_Teams", cnn)
        Dim DS As New DataSet
        cmd.Fill(DS, "teamNames")

        Dim cmd1 As New SqlDataAdapter("select teamid, firstname + ' ' + surname as username, userid,  taskname, [1] as On_List, [2] as Planning, [3] as [Check], [4] as Do, [5] as Test, [6] as Review from (select taskid, taskname, firstname, surname, t.userid, u.teamid, stageid from WorkLog_Tasks t inner join WorkLog_users u on t.UserID = u.userid) x pivot (count(taskid) for stageid in ([1], [2], [3], [4], [5], [6])) p", cnn)

        cmd1.Fill(DS, "tasks")

        DS.Relations.Add("myRelation",
        DS.Tables("teamNames").Columns("teamID"),
        DS.Tables("tasks").Columns("teamid")
        )

        parentRepeater.DataSource = DS.Tables("teamNames")
        parentRepeater.DataBind()
    End Sub


    Public Sub ChldFunction(Sender As Object, e As RepeaterItemEventArgs) 'turn stage pink if it's in progress

        Dim stageList As List(Of String) = New List(Of String)
        stageList.Add("onlist")  'correspond to names of labels on the main table
        stageList.Add("planning")
        stageList.Add("check")
        stageList.Add("do")
        stageList.Add("test")
        stageList.Add("rev")

        For Each item In stageList
            If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then 'code to exclude header
                Dim Label As Label = e.Item.FindControl(item)
                If Label.Text = "1" Then
                    Label.BackColor = System.Drawing.ColorTranslator.FromHtml("#ff1493")
                    Label.Text = "progress"
                    Label.ForeColor = System.Drawing.ColorTranslator.FromHtml("#ff1493")
                End If
                If Label.Text = "0" Then
                    Label.Text = " "
                End If
            End If
        Next
    End Sub


    Public Sub HideAll()
        main.Style.Add("display", "none")
        addtaskdiv.Style.Add("display", "none")
        existingtaskdiv.Style.Add("display", "none")
        edittaskdiv.Style.Add("display", "none")
        noaccessdiv.Style.Add("display", "none")
        deletetaskdiv.Style.Add("display", "none")
    End Sub


    Public Sub canc_Button(sender As Object, e As EventArgs) 'if an action is cancelled, reset to main page
        HideAll()
        main.Style.Add("display", "block")
    End Sub


    Public Sub set_Stages(list As DropDownList) 'update dropdown list of stages when adding or editing tasks
        Dim cnn As New SqlConnection(connectionString)
        cnn.Open()
        Dim cmd As New SqlCommand("select stageid, stage FROM [WorkLog].[dbo].[WorkLog_Stages]  where StageID between 1 and 6", cnn)
        Dim DA As New SqlDataAdapter(cmd)
        DA.Fill(StageDS)

        list.DataSource = StageDS
        list.DataTextField = "stage"
        list.DataValueField = "stageid"
        list.DataBind()
        cnn.Close()
    End Sub


    Public Sub set_Users(list As DropDownList)   'update dropdown list of users when adding or editing tasks
        Dim cnn As New SqlConnection(connectionString)
        cnn.Open()
        Dim cmd As New SqlCommand("SELECT FirstName + ' ' + SurName as theUser, userID FROM WorkLog_Users where teamid=@teamID", cnn)
        cmd.Parameters.Add("@teamID", SqlDbType.Int)
        cmd.Parameters("@teamID").Value = Session("teamID")

        Dim DA As New SqlDataAdapter(cmd)
        DA.Fill(userDS)

        list.DataSource = userDS
        list.DataTextField = "theUser"
        list.DataValueField = "UserID"
        list.DataBind()
        cnn.Close()
    End Sub


    Public Sub Fetch(toCheck As String) 'check whether a task exists before inserting new one or get a task ready to update
        Dim Items() As String = toCheck.Split(",")
        Dim task As String = Items(0)
        Dim User As Integer = CInt(Items(1))

        Dim cnn As New SqlConnection(connectionString)
        Dim cmd As New SqlCommand("select * from WorkLog_Tasks where TaskName = @task and UserID = @user", cnn)
        cmd.Parameters.Add("@task", SqlDbType.VarChar)
        cmd.Parameters("@task").Value = task
        cmd.Parameters.Add("@user", SqlDbType.Int)
        cmd.Parameters("@user").Value = User

        Dim DA As New SqlDataAdapter(cmd)
        DA.Fill(fetchDS)
        cnn.Close()

    End Sub


    Public Function Check_Access(taskname As String, userid As Integer) As Boolean 'check whether the user logged on is in the same team as the usr who's task they're editing

        Dim teamid As Integer

        Dim cnn As New SqlConnection(connectionString)
        cnn.Open()
        Dim cmd As New SqlCommand("SELECT te.teamid FROM Worklog_users u inner join WorkLog_Tasks t on u.userid = t.userid inner join worklog_teams te on u.teamid = te.teamid where t.userid= @usr and t.taskname = @tsk", cnn)
        cmd.Parameters.Add("@usr", SqlDbType.VarChar)
        cmd.Parameters("@usr").Value = userid
        cmd.Parameters.Add("@tsk", SqlDbType.VarChar)
        cmd.Parameters("@tsk").Value = taskname


        Dim reader As SqlDataReader = cmd.ExecuteReader

        If reader.HasRows() Then
            reader.Read()
            teamid = reader.Item("teamid")
            If teamid = Session("teamID") Then
                Return True
            End If
        End If
        reader.Close()
    End Function


    Public Sub InsertTask(task As String, user As Integer, stage As Integer)
        Dim cnn As New SqlConnection(connectionString)
        cnn.Open()
        Dim cmd As New SqlCommand("insert into WorkLog_Tasks values (@taskname, @user, @stage) Declare @taskid int = (select taskid from worklog_tasks where taskname = @taskname and userid = @user) insert into WorkLog_Audit values (1, @taskname, @taskid, @user, (select u.teamid from WorkLog_users u inner join worklog_tasks t on u.UserID = t.UserID where t.TaskID = @taskid), @stage, 'New_Task', getdate())", cnn)
        cmd.Parameters.Add("@taskname", SqlDbType.VarChar)
        cmd.Parameters("@taskname").Value = task
        cmd.Parameters.Add("@user", SqlDbType.Int)
        cmd.Parameters("@user").Value = user
        cmd.Parameters.Add("@stage", SqlDbType.Int)
        cmd.Parameters("@stage").Value = stage


        cmd.ExecuteNonQuery()
        cnn.Close()
    End Sub


    Public Sub OverwriteTask(task As String, user As Integer, Stage As Integer)
        Fetch(task & "," & user)
        Dim Task_ID As String = CInt(fetchDS.Tables(0).Rows(0)(0).ToString)

        Dim cnn As New SqlConnection(connectionString)
        cnn.Open()
        Dim cmd As New SqlCommand("insert into WorkLog_Audit values ((case when  Not exists (select * from WorkLog_Audit where TaskID = @taskid And ChangeType = 'stage' ) then 1 Else (Select MAX(theOrder) from Worklog_Audit where taskid = @taskid And ChangeType = 'stage' ) + 1 End), (select taskname from Worklog_Tasks where TaskID = @taskid), (select taskid from WorkLog_Tasks where TaskID = @taskid), (select userid from Worklog_tasks where Taskid = @taskid), (select u.teamid from WorkLog_users u inner join worklog_tasks t on u.UserID = t.UserID where t.TaskID = @taskid), (select stageid from WorkLog_Tasks where TaskID = @taskid), 'Edit', GETDATE()) update WorkLog_Tasks set StageID = @newStage, taskname = @newtask, userid = @newuser where TaskID = @taskid", cnn)
        cmd.Parameters.Add("@taskid", SqlDbType.Int)
        cmd.Parameters("@taskid").Value = Task_ID
        cmd.Parameters.Add("@newStage", SqlDbType.Int)
        cmd.Parameters("@newstage").Value = Stage
        cmd.Parameters.Add("@newtask", SqlDbType.VarChar)
        cmd.Parameters("@newtask").Value = task
        cmd.Parameters.Add("@newuser", SqlDbType.VarChar)
        cmd.Parameters("@newuser").Value = user

        cmd.ExecuteNonQuery()
        cnn.Close()

    End Sub

    Public Sub Delete_Task(taskid As Integer)

        Dim cnn As New SqlConnection(connectionString)
        cnn.Open()
        Dim cmd As New SqlCommand("insert into WorkLog_Audit values ((case when not exists (select * from WorkLog_Audit where TaskID = @taskid and ChangeType = 'stage') then 1 else (select MAX(theOrder) from Worklog_Audit where taskid = @taskid and ChangeType = 'stage') + 1 End), (select taskname from Worklog_Tasks where TaskID = @taskid), (select taskid from WorkLog_Tasks where TaskID = @taskid), (select userid from Worklog_tasks where Taskid = @taskid), (select u.teamid from WorkLog_users u inner join worklog_tasks t on u.UserID = t.UserID where t.TaskID = @taskid), 7, 'Deleted', GETDATE()) delete from worklog_tasks where taskid = @taskid", cnn)
        cmd.Parameters.Add("@taskid", SqlDbType.Int)
        cmd.Parameters("@taskid").Value = taskid

        cmd.ExecuteNonQuery()
        cnn.Close()
        MainRepeater()
        HideAll()
        main.Style.Add("display", "block")


    End Sub


    Public Sub show_AddTask(sender As Object, e As EventArgs) 'show the add task form and populate drop-down lists
        HideAll()
        addtaskdiv.Style.Add("display", "block")
        taskName.Text = ""
        set_Users(taskUser)
        set_Stages(taskStage)
    End Sub


    Public Sub submit_AddTask(sender As Object, e As EventArgs) 'when a task to add is submitted, check whether already exists for that user.  If not insert,  otherwise give option to overwrite

        Dim task As String = taskName.Text
        Dim User As Integer = CInt(taskUser.SelectedItem.Value)
        Dim UserName As String = taskUser.SelectedItem.Text
        Dim Stage As Integer = CInt(taskStage.SelectedItem.Value)

        Dim checkval As String = task & "," & User.ToString
        Fetch(checkval)

        If fetchDS.Tables(0).Rows.Count >= 1 Then
            HideAll()
            existingtaskdiv.Style.Add("display", "block") 'existing task div has a different button, which also says submit task but overwrites insteaad of inserting 
            taskexists.Text = "Task " + task + " already exists for user " + UserName + ". Do you want to overwrite?"

        Else
            InsertTask(task, User, Stage)
            MainRepeater()
            HideAll()
            main.Style.Add("display", "block")

        End If

    End Sub

    Public Sub submit_ExistingTask()
        Dim task As String = taskName.Text
        Dim User As Integer = CInt(taskUser.SelectedItem.Value) 'taskuser and taskstage are dropdown lists from the show_add task div
        Dim Stage As Integer = CInt(taskStage.SelectedItem.Value)

        OverwriteTask(task, User, Stage)
        MainRepeater()
        HideAll()
        main.Style.Add("display", "block")

    End Sub


    Public Sub show_EditTask(sender As Object, e As EventArgs)
        Dim task As String = sender.commandargument
        Dim user As String = sender.commandname
        Dim checkval As String
        HideAll()


        If (Check_Access(task, user)) Then
            edittaskdiv.Style.Add("display", "block")
            checkval = task & "," & user.ToString
            Fetch(checkval)

            editTaskName.Text = task
            set_Users(editTaskUser)
            editTaskUser.ClearSelection() 'clear default selection on drop-down list and re-populate with the task to be edited
            editTaskUser.Items.FindByValue(user).Selected = True
            set_Stages(editTaskStage)
            editTaskStage.ClearSelection()
            editTaskStage.Items.FindByValue(fetchDS.Tables(0).Rows(0)("stageid")).Selected = True 'getting stageid from the table returned by the fetch function
            SubmitEditTask.CommandArgument = fetchDS.Tables(0).Rows(0)("taskid") 'add the task id to the submit task button, so when we submit the task, we know which one we're editing

        Else
            HideAll()
            noaccessdiv.Style.Add("display", "block")
        End If
    End Sub


    Public Sub Submit_EditTask(sender As Object, e As EventArgs)
        Dim btn As Button = sender

        Dim task As String = editTaskName.Text
        Dim taskid As Integer = sender.commandargument
        Dim User As Integer = CInt(editTaskUser.SelectedItem.Value)
        Dim UserName As String = editTaskUser.SelectedItem.Text
        Dim Stage As Integer = CInt(editTaskStage.SelectedItem.Value)
        Dim StageName As String = editTaskStage.SelectedItem.Text

        Dim checkval As String = task & "," & User.ToString
        Fetch(checkval)

        If fetchDS.Tables(0).Rows.Count >= 1 Then
            If taskid <> fetchDS.Tables(0).Rows(0)("taskid") Then 'if task by the same name as the task that was clicked on exists for the selected user, overwrite it and delete the one that was clicked on
                Delete_Task(taskid)
            End If
            OverwriteTask(task, User, Stage)
            MainRepeater()
            HideAll()
            main.Style.Add("display", "block")
        Else
            Dim cnn As New SqlConnection(connectionString)
            cnn.Open()
            Dim cmd As New SqlCommand("insert into WorkLog_Audit values ((case when  Not exists (select * from WorkLog_Audit where TaskID = @taskid And ChangeType = 'stage' ) then 1 Else (Select MAX(theOrder) from Worklog_Audit where taskid = @taskid And ChangeType = 'stage' ) + 1 End), (select taskname from Worklog_Tasks where TaskID = @taskid), (select taskid from WorkLog_Tasks where TaskID = @taskid), (select userid from Worklog_tasks where Taskid = @taskid), (select u.teamid from WorkLog_users u inner join worklog_tasks t on u.UserID = t.UserID where t.TaskID = @taskid), (select stageid from WorkLog_Tasks where TaskID = @taskid), 'Edit', GETDATE()) update WorkLog_Tasks set StageID = @newStage, taskname = @newtask, userid = @newuser where TaskID = @taskid", cnn)
            cmd.Parameters.Add("@taskid", SqlDbType.Int)
            cmd.Parameters("@taskid").Value = taskid
            cmd.Parameters.Add("@newStage", SqlDbType.Int)
            cmd.Parameters("@newstage").Value = Stage
            cmd.Parameters.Add("@newtask", SqlDbType.VarChar)
            cmd.Parameters("@newtask").Value = task
            cmd.Parameters.Add("@newuser", SqlDbType.VarChar)
            cmd.Parameters("@newuser").Value = User

            cmd.ExecuteNonQuery()
            cnn.Close()
            MainRepeater()
            HideAll()
            main.Style.Add("display", "block")
        End If
    End Sub

    Public Sub Show_DeleteTask(Sender As Object, e As EventArgs)
        Dim task As String = Sender.commandargument
        Dim user As String = Sender.commandname
        Dim taskID As Integer
        Dim checkval As String
        Dim cnn As New SqlConnection(connectionString)
        HideAll()

        If (Check_Access(task, user)) Then
            deletetaskdiv.Style.Add("display", "block")
            checkval = task & "," & user.ToString
            Fetch(checkval)
            taskID = fetchDS.Tables(0).Rows(0)("taskid")
            cnn.Open()
            Dim cmd As New SqlCommand(" select t.taskid, u.firstname + ' ' + u.surname as UserName, stage from worklog_tasks t inner join worklog_users u on t.userid = u.userid inner join worklog_stages s on t.stageid = s.stageid where taskid = @taskID", cnn)
            cmd.Parameters.Add("@taskid", SqlDbType.Int)
            cmd.Parameters("@taskid").Value = taskID
            Dim Reader As SqlDataReader = cmd.ExecuteReader()
            If Reader.HasRows() Then
                Reader.Read()
                deleteTaskUser.Text = Reader.Item("username") 'get user and stage ready to dislay on confirmation screen
                deleteTaskStage.Text = Reader.Item("stage")
            End If
            Reader.Close()
            cnn.Close()
            deleteTaskName.Text = task
            submitDeleteTask.CommandArgument = taskID
        Else
            noaccessdiv.Style.Add("display", "block")
        End If

    End Sub

    Public Sub Submit_DeleteTask(sender As Object, e As EventArgs)

        Delete_Task(sender.commandargument)

    End Sub

End Class
