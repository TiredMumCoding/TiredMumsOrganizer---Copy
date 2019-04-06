Imports System.Data
Imports System.Data.SqlClient

Partial Class _Default
    Inherits System.Web.UI.Page

    Dim connectionString As String = ConfigurationManager.ConnectionStrings("TiredMumOrgConnectionString").ConnectionString
    Dim cnn As SqlConnection = New SqlConnection(connectionString)

    Public Sub Page_Load(Sender As Object, E As EventArgs) Handles Me.Load


        If Not IsPostBack Then  'populate team list in create user section

            signinWrapper.Style.Add("display", "none")
            createAccWrapper.Style.Add("display", "none")

            cnn.Open()
            Dim cmd As New SqlCommand("select * from worklog_teams order by teamname", cnn)
            Dim DA As New SqlDataAdapter(cmd)
            Dim DS As New DataSet
            DA.Fill(DS)

            teamList.DataSource = DS
            teamList.DataTextField = "TeamName"
            teamList.DataValueField = "TeamID"

            If DS.Tables.Count > 0 Then
                teamList.DataBind()
            End If

            cnn.Close()

        End If

    End Sub

    Public Function CheckAccess(userID As Integer) As Boolean ' check whether user has access to administrator sec group
        cnn.Open()
        Dim cmd As New SqlCommand("select * from worklog_secgroups where userid = @userid and groupid = 1", cnn)
        cmd.Parameters.Add("@userid", SqlDbType.Int)
        cmd.Parameters("@userid").Value = userID
        Dim reader As SqlDataReader = cmd.ExecuteReader
        If reader.HasRows() Then
            Return True
        Else Return False
        End If
    End Function


    Public Sub sign_In() ' toggle for sign in form
        signinWrapper.Style.Add("Display", "block")
        welcomebuttons.Style.Add("display", "none")
    End Sub

    Public Sub create_Acc() 'toggle for create account form
        createAccWrapper.Style.Add("Display", "block")
        welcomebuttons.Style.Add("display", "none")
    End Sub


    Public Sub Submit_SignIn(sender As Object, e As EventArgs)

        Dim firstName As String = fname.Text
        Dim surName As String = sname.Text
        Dim Password As String = pword.Text

        fname.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
        sname.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
        pword.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")


        Dim SubmitFlag As String = "true"

        If fname.Text = "" Then
            fname.BackColor = System.Drawing.ColorTranslator.FromHtml("#ff1493")
            SubmitFlag = False
        End If

        If sname.Text = "" Then
            sname.BackColor = System.Drawing.ColorTranslator.FromHtml("#ff1493")
            SubmitFlag = False
        End If

        If pword.Text = "" Then
            pword.BackColor = System.Drawing.ColorTranslator.FromHtml("#ff1493")
            SubmitFlag = "false"
        End If

        If SubmitFlag = True Then
            cnn.Open()

            Dim cmd As New SqlCommand("select userid, firstname, teamid from WorkLog_Users where FirstName = @firstname And SurName = @surname And pword = @pword", cnn)
            cmd.Parameters.Add("@firstname", SqlDbType.VarChar)
            cmd.Parameters("@firstname").Value = firstName
            cmd.Parameters.Add("@surname", SqlDbType.VarChar)
            cmd.Parameters("@surname").Value = surName
            cmd.Parameters.Add("@pword", SqlDbType.VarChar)
            cmd.Parameters("@pword").Value = Password

            Dim Reader As SqlDataReader = cmd.ExecuteReader()
            If Reader.HasRows() Then
                ' var = fname.Text
                Reader.Read()
                '' teamVar = Reader.Item("teamid")
                ' userVar = Reader.Item("userid")
                Session("User") = fname.Text
                Session("teamID") = Reader.Item("teamid")
                Session("userID") = Reader.Item("userid")
                Reader.Close()
                cnn.Close()

                Dim lab As Label = Master.FindControl("titlelab")
                lab.Text = "welcome " & fname.Text  'update top right of page to welcoe user
                Dim Link As LinkButton = Master.FindControl("titlelink")
                Link.Text = "logout"
                If (CheckAccess(Session("userID"))) Then 'check whether the user has adminiistrator access.  IF they do add 'administrator' link 
                    Link = Master.FindControl("adminlink")
                    Link.Text = "administrator"
                    Session("admin") = 1
                End If
                Response.Redirect("main.aspx")
            Else fname.Text = ""  'reset text boxes ready for details to be re-entered
                sname.Text = ""
                pword.Text = ""
                failed.Text = "User Does Not exist. Please try again"
            End If

        Else
            failed.Text = "Please Complete Required Fields"
        End If
                End Sub


    Public Sub create_Account(Sender As Object, E As EventArgs)

        Dim firstName As String = cfname.Text
        Dim surName As String = csname.Text
        Dim Teamname As Integer = CInt(teamList.SelectedValue)
        Dim Password As String = cpword.Text

        cfname.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
        csname.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
        cpword.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")


        Dim SubmitFlag As String = "true"

        If cfname.Text = "" Then
            cfname.BackColor = System.Drawing.ColorTranslator.FromHtml("#ff1493")
            SubmitFlag = False
        End If

        If csname.Text = "" Then
            csname.BackColor = System.Drawing.ColorTranslator.FromHtml("#ff1493")
            SubmitFlag = False
        End If

        If cpword.Text = "" Then
            cpword.BackColor = System.Drawing.ColorTranslator.FromHtml("#ff1493")
            SubmitFlag = "false"
        End If

        If SubmitFlag = True Then


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
            cmd.Parameters("@pword").Value = Password

            Dim result As Integer = cmd.ExecuteNonQuery
            If result >= 1 Then

                Session("teamID") = teamList.SelectedValue 'set session variables for use in displaying welcome label determing access to edit/add tasks
                Session("User") = cfname.Text
                Dim lab As Label = Master.FindControl("titlelab")
                lab.Text = "welcome " & cfname.Text
                Dim Link As LinkButton = Master.FindControl("titlelink")
                Link.Text = "logout"
                Response.Redirect("main.aspx")
            Else
                cfname.Text = ""
                csname.Text = ""
                cpword.Text = ""
                cfailed.Text = "User Already Exists. Please try again"
            End If
            cnn.Close()

        Else cfailed.Text = "Please Complete Required Fields"

        End If

    End Sub


    Public Sub Cancel(sender As Object, e As EventArgs)
        signinWrapper.Style.Add("Display", "none")
        createAccWrapper.Style.Add("display", "none")
        welcomebuttons.Style.Add("display", "block")
    End Sub

End Class
