
Partial Class MasterPage
    Inherits System.Web.UI.MasterPage


    Public Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        Dim o As Object = Session("User")
        Dim a As Object = Session("admin")

        If Not o Is Nothing Then
            titlelab.Text = "welcome " & Session("User")
            titlelink.Text = "logout"
        End If

        If Not a Is Nothing Then
            adminlink.Text = "administrator"
        End If

    End Sub

    Public Sub admin_page(sender As Object, e As EventArgs)
        Response.Redirect("administrator.aspx")
    End Sub

    Public Sub Login_Logout(sender As Object, e As EventArgs)
        If sender.text = "logout" Then
            Session("User") = Nothing
            Session("admin") = Nothing
            Session("User") = Nothing
        End If
        Response.Redirect("welcome.aspx")
    End Sub

End Class

