<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Window
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Window))
        Me.btnProcess = New System.Windows.Forms.Button()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.lblDBStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.processTimer = New System.Windows.Forms.Timer(Me.components)
        Me.lblLastProcess = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblCurAlarms = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lstCurAlarms = New System.Windows.Forms.ListView()
        Me.Store = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Rack = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.AlarmType = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.AlarmUnit = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.AlarmTime = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.AlarmValue = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.AlarmAck = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.lstErrors = New System.Windows.Forms.ListView()
        Me.Time = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ErrorC = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnProcess
        '
        Me.btnProcess.Location = New System.Drawing.Point(12, 402)
        Me.btnProcess.Name = "btnProcess"
        Me.btnProcess.Size = New System.Drawing.Size(255, 23)
        Me.btnProcess.TabIndex = 0
        Me.btnProcess.Text = "Maunal Process Messages"
        Me.btnProcess.UseVisualStyleBackColor = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblDBStatus, Me.lblCurAlarms, Me.lblLastProcess})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 428)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(800, 22)
        Me.StatusStrip1.TabIndex = 1
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'lblDBStatus
        '
        Me.lblDBStatus.AutoSize = False
        Me.lblDBStatus.Name = "lblDBStatus"
        Me.lblDBStatus.Size = New System.Drawing.Size(200, 17)
        Me.lblDBStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'processTimer
        '
        Me.processTimer.Enabled = True
        Me.processTimer.Interval = 60000
        '
        'lblLastProcess
        '
        Me.lblLastProcess.AutoSize = False
        Me.lblLastProcess.Name = "lblLastProcess"
        Me.lblLastProcess.Size = New System.Drawing.Size(340, 17)
        Me.lblLastProcess.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblCurAlarms
        '
        Me.lblCurAlarms.AutoSize = False
        Me.lblCurAlarms.Name = "lblCurAlarms"
        Me.lblCurAlarms.Size = New System.Drawing.Size(200, 17)
        Me.lblCurAlarms.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lstCurAlarms
        '
        Me.lstCurAlarms.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.Store, Me.Rack, Me.AlarmType, Me.AlarmUnit, Me.AlarmTime, Me.AlarmValue, Me.AlarmAck})
        Me.lstCurAlarms.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lstCurAlarms.HideSelection = False
        Me.lstCurAlarms.Location = New System.Drawing.Point(12, 12)
        Me.lstCurAlarms.Name = "lstCurAlarms"
        Me.lstCurAlarms.Size = New System.Drawing.Size(776, 209)
        Me.lstCurAlarms.TabIndex = 2
        Me.lstCurAlarms.UseCompatibleStateImageBehavior = False
        Me.lstCurAlarms.View = System.Windows.Forms.View.Details
        '
        'Store
        '
        Me.Store.Text = "Store"
        '
        'Rack
        '
        Me.Rack.Text = "Rack"
        '
        'AlarmType
        '
        Me.AlarmType.Text = "Alarm Type"
        '
        'AlarmUnit
        '
        Me.AlarmUnit.Text = "Alarm Unit"
        '
        'AlarmTime
        '
        Me.AlarmTime.Text = "Alarm Time"
        '
        'AlarmValue
        '
        Me.AlarmValue.Text = "Alarm Value"
        '
        'AlarmAck
        '
        Me.AlarmAck.Text = "Acknowledged"
        '
        'lstErrors
        '
        Me.lstErrors.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.Time, Me.ErrorC})
        Me.lstErrors.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lstErrors.HideSelection = False
        Me.lstErrors.Location = New System.Drawing.Point(12, 227)
        Me.lstErrors.Name = "lstErrors"
        Me.lstErrors.Size = New System.Drawing.Size(776, 169)
        Me.lstErrors.TabIndex = 3
        Me.lstErrors.UseCompatibleStateImageBehavior = False
        Me.lstErrors.View = System.Windows.Forms.View.Details
        '
        'Time
        '
        Me.Time.Text = "Time"
        '
        'ErrorC
        '
        Me.ErrorC.Text = "Error"
        '
        'Window
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.lstErrors)
        Me.Controls.Add(Me.lstCurAlarms)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.btnProcess)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimizeBox = False
        Me.Name = "Window"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Alarm Monitor"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnProcess As Button
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents lblDBStatus As ToolStripStatusLabel
    Friend WithEvents processTimer As Timer
    Friend WithEvents lblLastProcess As ToolStripStatusLabel
    Friend WithEvents lblCurAlarms As ToolStripStatusLabel
    Friend WithEvents lstCurAlarms As ListView
    Friend WithEvents Store As ColumnHeader
    Friend WithEvents Rack As ColumnHeader
    Friend WithEvents AlarmType As ColumnHeader
    Friend WithEvents AlarmUnit As ColumnHeader
    Friend WithEvents AlarmTime As ColumnHeader
    Friend WithEvents AlarmValue As ColumnHeader
    Friend WithEvents AlarmAck As ColumnHeader
    Friend WithEvents lstErrors As ListView
    Friend WithEvents Time As ColumnHeader
    Friend WithEvents ErrorC As ColumnHeader
End Class
