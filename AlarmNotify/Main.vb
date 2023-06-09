Imports System.Net
Imports System.Net.Mail
Public Class Main
    Public ini As New AlarmMonitorLib.IniFile
    Private emailHost, emailPort, emailUser, emailPass, emailFrom, api_user, api_pass, from_did As String
    Private useSSL As Boolean
    Private Sub Main_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim appPath As String = Application.StartupPath()

        'Load settings ini
        ini.Load(appPath & "\AlarmNotify.ini")

        'Load Email Settings
        emailHost = ini.GetKeyValue("EmailSettings", "host")
        emailPort = ini.GetKeyValue("EmailSettings", "port")
        emailUser = ini.GetKeyValue("EmailSettings", "username")
        emailPass = ini.GetKeyValue("EmailSettings", "password")
        useSSL = Boolean.Parse(ini.GetKeyValue("EmailSettings", "ssl"))
        emailFrom = ini.GetKeyValue("EmailSettings", "from")

        'Load Voip.ms settings
        api_user = ini.GetKeyValue("VoipMSSettings", "api_username")
        api_pass = ini.GetKeyValue("VoipMSSettings", "api_password")
        from_did = ini.GetKeyValue("VoipMSSettings", "from_did")

    End Sub

    Private Function sendSMS(ByVal dest As String, ByVal message As String)
        'SOAP Objects
        Dim soap As New vms.VoIPms_Service
        Dim input As New vms.sendSMSInput
        Dim output As New Object

        'XML Elements
        Dim result_status As Xml.XmlElement

        'String Vars
        Dim status As String

        'Fill Input Object
        input.api_username = api_user
        input.api_password = api_pass
        input.did = from_did
        input.dst = dest
        input.message = message

        'Request Info
        output = soap.sendSMS(input)

        'Get status
        result_status = output(1)
        status = result_status.ChildNodes(1).InnerText

        If status = "success" Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function sendEmail(ByVal dest As String, ByVal message As String)
        Dim emailMessage As New MailMessage
        emailMessage.To.Add(dest)
        emailMessage.From = New MailAddress(emailFrom)
        emailMessage.Subject = "Refrigeration Alarm"
        emailMessage.IsBodyHtml = True
        emailMessage.Body = message

        Dim emailClient As New SmtpClient(emailHost, emailPort)
        emailClient.UseDefaultCredentials = False
        emailClient.EnableSsl = useSSL
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 'Use TLS 1.2
        Dim emailCredentials As New Net.NetworkCredential(emailUser, emailPass)
        emailClient.Credentials = emailCredentials
        emailClient.DeliveryMethod = SmtpDeliveryMethod.Network

        Try
            emailClient.SendAsync(emailMessage, Nothing)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

End Class
