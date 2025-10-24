<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PIC16F883SerialCommForm
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
        Me.SerialPort1 = New System.IO.Ports.SerialPort(Me.components)
        Me.RxTimer = New System.Windows.Forms.Timer(Me.components)
        Me.HexTextBox = New System.Windows.Forms.TextBox()
        Me.ConnectButton = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.AsciiTextBox = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.BinaryTextBox = New System.Windows.Forms.TextBox()
        Me.PositionTrackBar = New System.Windows.Forms.TrackBar()
        Me.DataSentTextBox = New System.Windows.Forms.TextBox()
        Me.COMPort_ComboBox = New System.Windows.Forms.ComboBox()
        Me.CloseButton = New System.Windows.Forms.Button()
        Me.HexReadTextBox = New System.Windows.Forms.TextBox()
        Me.AsciiReadTextBox = New System.Windows.Forms.TextBox()
        Me.BinaryReadTextBox = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        CType(Me.PositionTrackBar, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RxTimer
        '
        Me.RxTimer.Enabled = True
        '
        'HexTextBox
        '
        Me.HexTextBox.Location = New System.Drawing.Point(8, 75)
        Me.HexTextBox.Multiline = True
        Me.HexTextBox.Name = "HexTextBox"
        Me.HexTextBox.Size = New System.Drawing.Size(99, 302)
        Me.HexTextBox.TabIndex = 0
        '
        'ConnectButton
        '
        Me.ConnectButton.Location = New System.Drawing.Point(127, 12)
        Me.ConnectButton.Name = "ConnectButton"
        Me.ConnectButton.Size = New System.Drawing.Size(75, 23)
        Me.ConnectButton.TabIndex = 1
        Me.ConnectButton.Text = "CONNECT"
        Me.ConnectButton.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(43, 380)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(29, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "HEX"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(146, 380)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(34, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "ASCII"
        '
        'AsciiTextBox
        '
        Me.AsciiTextBox.Location = New System.Drawing.Point(112, 75)
        Me.AsciiTextBox.Multiline = True
        Me.AsciiTextBox.Name = "AsciiTextBox"
        Me.AsciiTextBox.Size = New System.Drawing.Size(103, 302)
        Me.AsciiTextBox.TabIndex = 3
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(249, 380)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(47, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "BINARY"
        '
        'BinaryTextBox
        '
        Me.BinaryTextBox.Location = New System.Drawing.Point(220, 75)
        Me.BinaryTextBox.Multiline = True
        Me.BinaryTextBox.Name = "BinaryTextBox"
        Me.BinaryTextBox.Size = New System.Drawing.Size(105, 302)
        Me.BinaryTextBox.TabIndex = 5
        '
        'PositionTrackBar
        '
        Me.PositionTrackBar.Location = New System.Drawing.Point(167, 405)
        Me.PositionTrackBar.Name = "PositionTrackBar"
        Me.PositionTrackBar.Size = New System.Drawing.Size(478, 45)
        Me.PositionTrackBar.TabIndex = 16
        '
        'DataSentTextBox
        '
        Me.DataSentTextBox.Location = New System.Drawing.Point(333, 450)
        Me.DataSentTextBox.Multiline = True
        Me.DataSentTextBox.Name = "DataSentTextBox"
        Me.DataSentTextBox.Size = New System.Drawing.Size(138, 25)
        Me.DataSentTextBox.TabIndex = 17
        '
        'COMPort_ComboBox
        '
        Me.COMPort_ComboBox.FormattingEnabled = True
        Me.COMPort_ComboBox.Location = New System.Drawing.Point(0, 14)
        Me.COMPort_ComboBox.Name = "COMPort_ComboBox"
        Me.COMPort_ComboBox.Size = New System.Drawing.Size(121, 21)
        Me.COMPort_ComboBox.TabIndex = 19
        '
        'CloseButton
        '
        Me.CloseButton.Location = New System.Drawing.Point(666, 405)
        Me.CloseButton.Name = "CloseButton"
        Me.CloseButton.Size = New System.Drawing.Size(75, 23)
        Me.CloseButton.TabIndex = 20
        Me.CloseButton.Text = "Close"
        Me.CloseButton.UseVisualStyleBackColor = True
        '
        'HexReadTextBox
        '
        Me.HexReadTextBox.Location = New System.Drawing.Point(499, 75)
        Me.HexReadTextBox.Multiline = True
        Me.HexReadTextBox.Name = "HexReadTextBox"
        Me.HexReadTextBox.Size = New System.Drawing.Size(98, 302)
        Me.HexReadTextBox.TabIndex = 21
        '
        'AsciiReadTextBox
        '
        Me.AsciiReadTextBox.Location = New System.Drawing.Point(603, 75)
        Me.AsciiReadTextBox.Multiline = True
        Me.AsciiReadTextBox.Name = "AsciiReadTextBox"
        Me.AsciiReadTextBox.Size = New System.Drawing.Size(98, 302)
        Me.AsciiReadTextBox.TabIndex = 22
        '
        'BinaryReadTextBox
        '
        Me.BinaryReadTextBox.Location = New System.Drawing.Point(707, 75)
        Me.BinaryReadTextBox.Multiline = True
        Me.BinaryReadTextBox.Name = "BinaryReadTextBox"
        Me.BinaryReadTextBox.Size = New System.Drawing.Size(98, 302)
        Me.BinaryReadTextBox.TabIndex = 23
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.0!)
        Me.Label4.Location = New System.Drawing.Point(100, 47)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(126, 25)
        Me.Label4.TabIndex = 24
        Me.Label4.Text = "SENT DATA"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.0!)
        Me.Label5.Location = New System.Drawing.Point(568, 47)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(172, 25)
        Me.Label5.TabIndex = 25
        Me.Label5.Text = "RECIEVED DATA"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(534, 380)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(29, 13)
        Me.Label6.TabIndex = 26
        Me.Label6.Text = "HEX"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(638, 380)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(34, 13)
        Me.Label7.TabIndex = 27
        Me.Label7.Text = "ASCII"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(736, 380)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(47, 13)
        Me.Label8.TabIndex = 28
        Me.Label8.Text = "BINARY"
        '
        'PIC16F883SerialCommForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(809, 487)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.BinaryReadTextBox)
        Me.Controls.Add(Me.AsciiReadTextBox)
        Me.Controls.Add(Me.HexReadTextBox)
        Me.Controls.Add(Me.CloseButton)
        Me.Controls.Add(Me.COMPort_ComboBox)
        Me.Controls.Add(Me.DataSentTextBox)
        Me.Controls.Add(Me.PositionTrackBar)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.BinaryTextBox)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.AsciiTextBox)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ConnectButton)
        Me.Controls.Add(Me.HexTextBox)
        Me.Name = "PIC16F883SerialCommForm"
        Me.Text = "Form1"
        CType(Me.PositionTrackBar, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents SerialPort1 As IO.Ports.SerialPort
    Friend WithEvents RxTimer As Timer
    Friend WithEvents HexTextBox As TextBox
    Friend WithEvents ConnectButton As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents AsciiTextBox As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents BinaryTextBox As TextBox
    Friend WithEvents PositionTrackBar As TrackBar
    Friend WithEvents DataSentTextBox As TextBox
    Friend WithEvents COMPort_ComboBox As ComboBox
    Friend WithEvents CloseButton As Button
    Friend WithEvents HexReadTextBox As TextBox
    Friend WithEvents AsciiReadTextBox As TextBox
    Friend WithEvents BinaryReadTextBox As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label8 As Label
End Class
