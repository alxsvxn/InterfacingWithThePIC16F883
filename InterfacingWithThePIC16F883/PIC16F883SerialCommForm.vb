Imports System.IO.Ports
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
'Alexis Villagran
'October 2025
Public Class PIC16F883SerialCommForm

    Private Sub PIC16F883SerialCommForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetDefaults()
        RxTimer.Interval = 50     ' ms, adjust if you like
        RxTimer.Enabled = False   ' start off
    End Sub

    'Connect Button Click Event
    Private Sub ConnectButton_Click(sender As Object, e As EventArgs) Handles ConnectButton.Click
        Select Case COMPort_ComboBox.Text <> ""  'Wait for COM Port Selection
            Case True
                Connect()   'Connect to Serial Port
            Case False
                MessageBox.Show("Please select a COM Port")
        End Select

    End Sub

    'TrackBar Scroll Event
    Private Sub PositionTrackBar_Scroll(sender As Object, e As EventArgs) Handles PositionTrackBar.Scroll
        TX()
    End Sub

    Sub SetDefaults()
        'set TrackBar defaults
        PositionTrackBar.Value = 0
        DataSentTextBox.Text = "Position:" & 1.ToString
        PositionTrackBar.Enabled = False

        COMPort_ComboBox.Items.Add("COM1")
        COMPort_ComboBox.Items.Add("COM2")
        COMPort_ComboBox.Items.Add("COM3")
        COMPort_ComboBox.Items.Add("COM4")
        COMPort_ComboBox.Items.Add("COM5")
    End Sub

    'when Free Run is selected disable TX Button

    Sub Connect()

        Try
            Dim comPort As String = COMPort_ComboBox.Text
            SerialPort1.Close()
            SerialPort1.BaudRate = 9600 'Q@ Board Default
            SerialPort1.Parity = IO.Ports.Parity.None   'No Parity
            SerialPort1.StopBits = IO.Ports.StopBits.One    '1 Stop Bit
            SerialPort1.DataBits = 8    '8 Data Bits
            SerialPort1.Handshake = IO.Ports.Handshake.None
            SerialPort1.PortName = comPort 'Change to your COM Port


            SerialPort1.Open()  'Open Serial Port

            If SerialPort1.IsOpen Then
                MessageBox.Show("Connected to " & SerialPort1.PortName)
                PositionTrackBar.Enabled = True
                RxTimer.Start()
            End If

            'SerialPort1.Close() 'Close Serial Port

        Catch ex As Exception
            'Show error message if port is invalid
            MessageBox.Show("Error: " & ex.Message)
            Return
        End Try
    End Sub

    Private Sub CloseButton_Click(sender As Object, e As EventArgs) Handles CloseButton.Click
        Try
            RxTimer.Stop()
            If SerialPort1.IsOpen Then SerialPort1.Close()
        Finally
            Me.Close()
        End Try
    End Sub

    Sub TX()
        Try
            Dim comPort As String = COMPort_ComboBox.Text
            SerialPort1.Close()
            SerialPort1.BaudRate = 9600 'Q@ Board Default
            SerialPort1.Parity = IO.Ports.Parity.None   'No Parity
            SerialPort1.StopBits = IO.Ports.StopBits.One    '1 Stop Bit
            SerialPort1.DataBits = 8    '8 Data Bits
            SerialPort1.PortName = comPort 'Change to your COM Port
            SerialPort1.Open()  'Open Serial Port
            Dim data(1) As Byte
            data(0) = &H24 'First byte (send an interupt to the PIC "$")
            Dim Position = PositionSelect() 'Increase count for case statement
            Select Case Position
                Case 1
                    data(1) = &H8   'Position  (0000 1000)
                Case 2
                    data(1) = &H10  'Position  (0001 0000)
                Case 3
                    data(1) = &H18  'Position  (0001 1000)
                Case 4
                    data(1) = &H20  'Position  (0010 0000)
                Case 5
                    data(1) = &H28  'Position  (0010 1000)
                Case 6
                    data(1) = &H30  'Position  (0011 0000)
                Case 7
                    data(1) = &H38  'Position  (0011 1000)
                Case 8
                    data(1) = &H40  'Position  (0100 0000)
                Case 9
                    data(1) = &H48  'Position  (0100 1000)
                Case 10
                    data(1) = &H50  'Position  (0101 0000)
                Case 11
                    data(1) = &H60  'Position  (0110 0000)
                Case 12
                    data(1) = &H68  'Position  (0110 1000)
                Case 13
                    data(1) = &H70  'Position  (0111 0000)
                Case 14
                    data(1) = &H78  'Position  (0111 1000)
                Case 15
                    data(1) = &H80  'Position  (1000 0000)
                Case 16
                    data(1) = &H88  'Position  (1000 1000)
                Case 17
                    data(1) = &H90  'Position  (1001 0000)
                Case 18
                    data(1) = &H98  'Position  (1001 1000)
                Case 19
                    data(1) = &HA0  'Position  (1010 0000)
                Case 20
                    data(1) = &H18  'Position  (1010 1000)
            End Select

            ShowTx(data)

            SerialPort1.Write(data, 0, 2) 'Send 2 bytes of data
            'SerialPort1.Close() 'Close Serial Port

        Catch ex As Exception
            'Show error message if port is invalid
            MessageBox.Show("Error: " & ex.Message)
            Return
        End Try
    End Sub

    Function PositionSelect() As Integer
        'Get TrackBar Value
        Dim position As Integer
        position = PositionTrackBar.Value
        'Update Position Label
        DataSentTextBox.Text = "Position:" & position.ToString
        Return position
    End Function

    Private Sub ShowTx(data() As Byte)
        Dim hexLine As String = ""
        Dim asciiLine As String = ""
        Dim binLine As String = ""

        For Each b As Byte In data
            ' HEX (two digits)
            hexLine &= b.ToString("X2") & " "

            ' ASCII (printables 32..126, else a dot)
            If b >= 32 AndAlso b <= 126 Then
                asciiLine &= ChrW(b)
            Else
                asciiLine &= "."
            End If

            ' BINARY (8 bits, padded)
            binLine &= Convert.ToString(b, 2).PadLeft(8, "0"c) & " "
        Next

        HexTextBox.AppendText(hexLine.TrimEnd() & Environment.NewLine)
        AsciiTextBox.AppendText(asciiLine & Environment.NewLine)
        BinaryTextBox.AppendText(binLine.TrimEnd() & Environment.NewLine)
    End Sub

    Private Sub RxTimer_Tick(sender As Object, e As EventArgs) Handles RxTimer.Tick
        If Not SerialPort1.IsOpen Then Exit Sub
        Dim n As Integer = SerialPort1.BytesToRead
        If n <= 0 Then Exit Sub

        Dim buf(n - 1) As Byte
        Dim got As Integer = SerialPort1.Read(buf, 0, n)
        If got > 0 Then AppendRx(buf, got)
    End Sub

    Private Sub AppendRx(data() As Byte, count As Integer)
        Dim hexLine As New System.Text.StringBuilder()
        Dim asciiLine As New System.Text.StringBuilder()
        Dim binLine As New System.Text.StringBuilder()

        For i = 0 To count - 1
            Dim b = data(i)
            hexLine.Append(b.ToString("X2")).Append(" ")
            asciiLine.Append(If(b >= 32 AndAlso b <= 126, ChrW(b), "."c))
            binLine.Append(Convert.ToString(b, 2).PadLeft(8, "0"c)).Append(" ")
        Next

        HexReadTextBox.AppendText(hexLine.ToString().TrimEnd() & Environment.NewLine)
        AsciiReadTextBox.AppendText(asciiLine.ToString() & Environment.NewLine)
        BinaryReadTextBox.AppendText(binLine.ToString().TrimEnd() & Environment.NewLine)
    End Sub
End Class
