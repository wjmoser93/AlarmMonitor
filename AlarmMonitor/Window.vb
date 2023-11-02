Imports MySql.Data.MySqlClient
Imports OpenPop.Mime
Imports OpenPop.Pop3
Imports System.Text
Imports System.Text.RegularExpressions
Imports AlarmMonitorLib


Public Class Window
    Public MysqlConn As MySqlConnection
    Public ini As New AlarmMonitorLib.IniFile
    Public error_state As Boolean
    Public testMode, useSSL As Boolean
    Public emailHost, emailPort, emailUser, emailPass As String
    Private Function dbConnect()
        'Connect to database
        MysqlConn = New MySqlConnection()
        MysqlConn.ConnectionString = "server=" & ini.GetKeyValue("DatabaseSettings", "DBHost") & ";user id=" & ini.GetKeyValue("DatabaseSettings", "DBUser") & ";password=" & ini.GetKeyValue("DatabaseSettings", "DBPass") & ";database=" & ini.GetKeyValue("DatabaseSettings", "DBName")
        lblDBStatus.Text = "Connecting..."
        Try
            MysqlConn.Open()
            lblDBStatus.Text = "Connected"
        Catch ex As MySqlException
            'MessageBox.Show("Cannot connect to database: " & ex.Message)
            WriteToEventLog(ex.Message)
            lblDBStatus.Text = "Unable to Connect"
        End Try
        Return Nothing
    End Function

    Private Function dbClose()
        'Disconnect from the database
        lblDBStatus.Text = "Disconnecting..."
        Try
            MysqlConn.Close()
            lblDBStatus.Text = "Disconnected"
            MysqlConn.Dispose()
        Catch ex As MySqlException
            'MessageBox.Show("Cannot disconnect from database: " & ex.Message)
            WriteToEventLog(ex.Message)
        End Try
        Return Nothing
    End Function

    Private Sub logError(ByVal errorMessage As String)
        Dim itemcol(2) As String
        itemcol(0) = DateTime.Now.ToString
        itemcol(1) = errorMessage
        Dim lvi As New ListViewItem(itemcol)
        lstErrors.Items.Add(lvi)
    End Sub

    'Private Function dbReconnect()
    '    'Close the database and start reconnect timer
    '    dbClose()
    '    ReconnectTimer.Start()
    '    lblDBStatus.Text = "Waiting 10 seconds to reconnect..."
    '    Return Nothing
    'End Function

    Public Function WriteToEventLog(ByVal Entry As String)
        'Write to system event log
        Dim appName As String = "Alarm Monitor"
        Dim eventType As EventLogEntryType = EventLogEntryType.Error
        Dim logName = "Application"

        Dim objEventLog As New EventLog()

        logError(Entry)

        Try
            If Not EventLog.SourceExists(appName) Then
                EventLog.CreateEventSource(appName, logName)
            End If
            objEventLog.Source = appName
            objEventLog.WriteEntry(Entry, eventType)
            Return True
        Catch Ex As Exception
            Return False
        End Try
    End Function

    Private Sub Window_Load(sender As Object, e As EventArgs) Handles Me.Load
        error_state = False
        Dim appPath As String = Application.StartupPath()

        'Load settings ini
        ini.Load(appPath & "\AlarmMonitor.ini")

        'Check for Test Mode (Runs program without checking for emails)
        If ini.GetKeyValue("ProgramSettings", "Mode") = 0 Then
            testMode = True
        Else
            testMode = False
        End If

        'Load Email Settings
        emailHost = ini.GetKeyValue("EmailSettings", "host")
        emailPort = ini.GetKeyValue("EmailSettings", "port")
        emailUser = ini.GetKeyValue("EmailSettings", "username")
        emailPass = ini.GetKeyValue("EmailSettings", "password")
        useSSL = Boolean.Parse(ini.GetKeyValue("EmailSettings", "ssl"))

        'Connect to the database
        dbConnect()

        timerAction()
    End Sub

    Private Sub Window_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        dbClose()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnProcess.Click
        timerAction()
    End Sub

    Private Sub parseMessage(ByVal pmessage As Message)
        Dim subject As String = pmessage.Headers.Subject.ToString
        Dim msgText As String = Encoding.Default.GetString(pmessage.MessagePart.Body)
        Dim seperator As String = "------------------------------------------------"

        Dim alarmMessages() As String = Split(msgText, seperator, , CompareMethod.Text)
        If alarmMessages.Length > 0 Then
            Dim filteredMessages As New List(Of String)
            For i = 0 To alarmMessages.Length - 1
                If Not String.IsNullOrWhiteSpace(alarmMessages(i)) Then
                    filteredMessages.Add(alarmMessages(i).Trim)
                End If
            Next

            If filteredMessages.Count = 0 Then
                Exit Sub
            End If
            Dim store As String = ""

            For Each key In ini.GetSection("Stores").Keys
                If subject.Contains(key.Name) Then
                    store = key.value
                    Exit For
                End If
            Next

            For Each msg As String In filteredMessages
                Dim rack_name, alarm_type, alarm_unit, alarm_value, alarm_time_string As String
                Dim end_pos, prev_endpos As Integer
                Dim alarm_cleared As Boolean

                end_pos = 0
                prev_endpos = 0
                Try
                    Dim m As Match = Regex.Match(msg, "(Rack|System|HVAC|Misc)[^\n]*", RegexOptions.None)
                    end_pos = m.Index + m.Length
                    rack_name = m.Value
                Catch ex As Exception
                    rack_name = ""
                End Try
                Try
                    Dim m As Match = Regex.Match(msg.Substring(end_pos, msg.Length - end_pos), "[\n][^\n]*", RegexOptions.None)
                    prev_endpos = end_pos
                    end_pos = m.Index + m.Length + prev_endpos
                    alarm_type = m.Value
                Catch ex As Exception
                    alarm_type = ""
                End Try
                Try
                    Dim m As Match = Regex.Match(msg.Substring(end_pos, msg.Length - end_pos), "[\n][^\n]*", RegexOptions.None)
                    prev_endpos = end_pos
                    end_pos = m.Index + m.Length + prev_endpos
                    alarm_unit = m.Value
                Catch ex As Exception
                    alarm_unit = ""
                End Try
                Try
                    If msg.Substring(end_pos, msg.Length - end_pos).Contains("at") Then
                        Dim m As Match = Regex.Match(msg.Substring(end_pos, msg.Length - end_pos), "Alarm occurred: (.+?(?=at|\n))", RegexOptions.None)
                        prev_endpos = end_pos
                        alarm_time_string = m.Groups(1).Value
                    ElseIf msg.Substring(end_pos, msg.Length - end_pos).Contains("when") Then
                        Dim m As Match = Regex.Match(msg.Substring(end_pos, msg.Length - end_pos), "Alarm occurred: (.+?(?=when|\n))", RegexOptions.None)
                        prev_endpos = end_pos
                        alarm_time_string = m.Groups(1).Value
                    Else
                        Dim m As Match = Regex.Match(msg.Substring(end_pos, msg.Length - end_pos), "Alarm occurred: (.+?(?=\n))", RegexOptions.None)
                        prev_endpos = end_pos
                        alarm_time_string = m.Groups(1).Value
                    End If

                Catch ex As Exception
                    alarm_time_string = ""
                End Try
                Try
                    If msg.Substring(prev_endpos, msg.Length - prev_endpos).Contains("psi") Then
                        Dim m As Match = Regex.Match(msg.Substring(prev_endpos, msg.Length - prev_endpos), "at (([^psi]*)psi)", RegexOptions.None)
                        alarm_value = m.Groups(1).Value
                    ElseIf msg.Substring(prev_endpos, msg.Length - prev_endpos).Contains("when") Then
                        Dim m As Match = Regex.Match(msg.Substring(prev_endpos, msg.Length - prev_endpos), "when (Off|On)", RegexOptions.None)
                        alarm_value = m.Groups(1).Value
                    ElseIf msg.Substring(prev_endpos, msg.Length - prev_endpos).Contains("%") Then
                        Dim m As Match = Regex.Match(msg.Substring(prev_endpos, msg.Length - prev_endpos), "at (([^%]*)%)", RegexOptions.None)
                        alarm_value = m.Groups(1).Value
                    Else
                        Dim m As Match = Regex.Match(msg.Substring(prev_endpos, msg.Length - prev_endpos), "at (([^F]*)F)", RegexOptions.None)
                        alarm_value = m.Groups(1).Value
                    End If

                Catch ex As Exception
                    alarm_value = ""
                End Try
                Try
                    If Regex.IsMatch(msg.Substring(prev_endpos, msg.Length - prev_endpos), "Alarm cleared", RegexOptions.None) Then
                        alarm_cleared = True
                    Else
                        alarm_cleared = False
                    End If
                Catch ex As Exception
                    alarm_cleared = False
                End Try
                'MsgBox(rack_name.Trim + vbCrLf + alarm_type.Trim + vbCrLf + alarm_unit.Trim + vbCrLf + alarm_time_string.Trim + vbCrLf + alarm_value.Trim + vbCrLf + alarm_cleared.ToString)
                dbInsert(store, rack_name.Trim, alarm_type.Trim, alarm_unit.Trim, alarm_time_string.Trim, alarm_value.Trim, alarm_cleared)
            Next

        End If
    End Sub

    Private Sub dbInsert(ByVal store As String, ByVal rack_name As String, ByVal alarm_type As String, ByVal alarm_unit As String, ByVal alarm_time_string As String, ByVal alarm_value As String, ByVal alarm_cleared As Boolean)
        If alarm_unit.Contains("'") Then
            alarm_unit = alarm_unit.Replace("'", "")
        End If
        Try
            Dim dt As DataSet = loadSQL("SELECT `alarm_id` FROM `alarms` WHERE store = '" + store + "' AND alarm_unit = '" + alarm_unit + "' AND alarm_cleared = 0")
            If dt.Tables(0).Rows.Count = 0 Then
                Dim command As New MySqlCommand
                command.Connection = MysqlConn
                command.CommandText = "INSERT INTO alarms(store, rack_name, alarm_type, alarm_unit, alarm_time, alarm_value, alarm_cleared, alarm_ack) VALUES(@store, @rack, @type, @unit, @time, @value, @cleared, @cleared)"
                command.Parameters.Add("@store", MySqlDbType.TinyText).Value = store
                command.Parameters.Add("@rack", MySqlDbType.TinyText).Value = rack_name
                command.Parameters.Add("@type", MySqlDbType.TinyText).Value = alarm_type
                command.Parameters.Add("@unit", MySqlDbType.TinyText).Value = alarm_unit
                command.Parameters.Add("@time", MySqlDbType.DateTime).Value = DateTime.Parse(alarm_time_string)
                command.Parameters.Add("@value", MySqlDbType.TinyText).Value = alarm_value
                command.Parameters.Add("@cleared", MySqlDbType.Int16).Value = Convert.ToInt32(alarm_cleared)
                command.ExecuteNonQuery()

                'Check if input is already in database, if not, add it
                storeInput(alarm_unit, store)
            ElseIf dt.Tables(0).Rows.Count = 1 Then
                If alarm_cleared Then
                    Dim command As New MySqlCommand
                    command.Connection = MysqlConn
                    command.CommandText = "UPDATE `alarms` SET alarm_cleared = 1 WHERE store = '" + store + "' AND alarm_unit = '" + alarm_unit + "'"

                    command.ExecuteNonQuery()
                End If
            End If
        Catch ex As Exception
            WriteToEventLog(ex.Message)
            error_state = True
        End Try

        If Not error_state Then
            deleteMessages()
        End If
        error_state = False
    End Sub

    Private Sub storeInput(ByVal inputName As String, ByVal storeName As String)
        Try
            Dim dt As DataSet = loadSQL("SELECT alarm_input_id FROM alarm_inputs WHERE input_name = '" + inputName + "' AND input_store = '" + storeName + "'")
            If dt.Tables(0).Rows.Count = 0 Then
                Dim command As New MySqlCommand
                command.Connection = MysqlConn
                command.CommandText = "INSERT INTO alarm_inputs(input_name, input_store) VALUES(@input, @store)"
                command.Parameters.Add("@input", MySqlDbType.VarString).Value = inputName
                command.Parameters.Add("@store", MySqlDbType.VarString).Value = storeName
                command.ExecuteNonQuery()
            End If
        Catch ex As Exception
            WriteToEventLog(ex.Message)
        End Try
    End Sub

    Private Function getMessages()
        Try
            Dim p3client As Pop3Client = New Pop3Client
            Dim msgCount As Integer = 0
            Dim allMessages As List(Of Message)

            Using p3client
                p3client.Connect(emailHost, Integer.Parse(emailPort), useSSL)
                p3client.Authenticate(emailUser, emailPass)
                msgCount = p3client.GetMessageCount
                allMessages = New List(Of Message)(msgCount)
                For i = 1 To msgCount
                    allMessages.Add(p3client.GetMessage(i))
                Next
                p3client.Disconnect()
            End Using
            Return allMessages
        Catch ex As Exception
            WriteToEventLog(ex.Message)
            Dim allMessages As New List(Of Message)
            Return allMessages
        End Try
    End Function

    Private Sub deleteMessages()
        Try
            Dim p3client As Pop3Client = New Pop3Client

            Using p3client
                p3client.Connect(emailHost, Integer.Parse(emailPort), useSSL)
                p3client.Authenticate(emailUser, emailPass)

                p3client.DeleteAllMessages()

                p3client.Disconnect()
            End Using
        Catch ex As Exception
            WriteToEventLog(ex.Message)
        End Try
    End Sub

    Private Sub deleteMessage(ByVal msgNumber As String)
        Try
            Dim p3client As Pop3Client = New Pop3Client

            Using p3client
                p3client.Connect(emailHost, Integer.Parse(emailPort), useSSL)
                p3client.Authenticate(emailUser, emailPass)

                p3client.DeleteMessage(msgNumber)

                p3client.Disconnect()
            End Using
        Catch ex As Exception
            WriteToEventLog(ex.Message)
        End Try
    End Sub

    Private Function loadSQL(ByVal query As String)
        'Load SQL query into dataset
        Dim command As New MySqlCommand(query, MysqlConn)
        Dim adapter As New MySqlDataAdapter(command)
        Dim dt As New DataSet

        adapter.Fill(dt)
        Return dt
    End Function

    Private Sub processTimer_Tick(sender As Object, e As EventArgs) Handles processTimer.Tick
        timerAction()
    End Sub

    Private Sub timerAction()
        If Not testMode Then
            If MysqlConn.State = ConnectionState.Open Then
                Try
                    Dim messageList As List(Of Message)
                    messageList = getMessages()
                    For Each pmessage As Message In messageList
                        parseMessage(pmessage)
                    Next
                    lblLastProcess.Text = "Last Processed: " + DateAndTime.Now.ToString
                    getCurrentAlarms()
                Catch ex As Exception
                    WriteToEventLog(ex.Message)
                End Try

            Else
                Try
                    dbClose()
                    dbConnect()
                Catch ex As Exception
                    WriteToEventLog(ex.Message)
                End Try
            End If
        Else
            getCurrentAlarms()
        End If

    End Sub

    Private Sub getCurrentAlarms()
        If MysqlConn.State = ConnectionState.Open Then
            lstCurAlarms.Items.Clear()

            Dim dt As DataSet = loadSQL("SELECT `store`, `rack_name`, `alarm_type`, `alarm_unit`, `alarm_time`, `alarm_value`, `alarm_ack` FROM `alarms` WHERE alarm_cleared = 0")
            lblCurAlarms.Text = "Current Alarms: " + dt.Tables(0).Rows.Count.ToString

            For z = 0 To dt.Tables(0).Rows.Count - 1
                Dim itemcol(100) As String
                For y = 0 To 6
                    If y = 6 Then
                        If dt.Tables(0).Rows(z)(y).ToString = "True" Then
                            itemcol(y) = "Yes"
                        Else
                            itemcol(y) = "No"
                        End If
                    Else
                        itemcol(y) = dt.Tables(0).Rows(z)(y).ToString
                    End If
                Next
                Dim lvi As New ListViewItem(itemcol)
                lstCurAlarms.Items.Add(lvi)
            Next
            lstCurAlarms.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
            lstCurAlarms.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)
        End If
    End Sub
End Class
