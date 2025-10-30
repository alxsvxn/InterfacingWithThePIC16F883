Imports System.IO.Ports
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
'========================================
'Alexis Villagran
'PIC16F883 Serial Communication Form
'October 2025
'========================================

Public Class PIC16F883SerialCommForm

    '========================================
    '-------------- Form Setup -------------
    'Purpose: Start the app in a known, safe state.
    '========================================
    Private Sub PIC16F883SerialCommForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetDefaults()
        RxTimer.Interval = 50            'Check for incoming bytes ~20x/second
        RxTimer.Enabled = False          'We only enable after connecting
    End Sub

    Private Sub SetDefaults()
        'UI comes up disabled until a port is connected
        PositionTrackBar.Value = 0
        DataSentTextBox.Text = "Position:" & 1.ToString
        PositionTrackBar.Enabled = False

        'Common COM ports pre-filled for quick selection
        COMPort_ComboBox.Items.AddRange({"COM1", "COM2", "COM3", "COM4", "COM5"})
    End Sub

    '========================================
    '---------- Button Event Handlers ------
    'Purpose: Simple, direct actions tied to UI buttons.
    '========================================
    Private Sub ConnectButton_Click(sender As Object, e As EventArgs) Handles ConnectButton.Click
        If COMPort_ComboBox.Text <> "" Then
            Connect()                    'Open the serial port with chosen settings
        Else
            MessageBox.Show("Please select a COM Port")
        End If
    End Sub

    Private Sub CloseButton_Click(sender As Object, e As EventArgs) Handles CloseButton.Click
        Try
            RxTimer.Stop()               'Stop polling before closing the port
            If SerialPort1.IsOpen Then SerialPort1.Close()
        Finally
            Me.Close()
        End Try
    End Sub

    Private Sub ClearButton_Click(sender As Object, e As EventArgs) Handles ClearButton.Click
        'Quick way to reset the display and slider position
        DataSentTextBox.Clear()
        HexTextBox.Clear()
        AsciiTextBox.Clear()
        BinaryTextBox.Clear()
        HexReadTextBox.Clear()
        AsciiReadTextBox.Clear()
        BinaryReadTextBox.Clear()
        ADCTextBox.Clear()
        HighByteTextBox.Clear()
        LowByteTextBox.Clear()
        If CmdTextBox IsNot Nothing Then CmdTextBox.Clear()

        'Return slider to 0 so the UI reflects a fresh start
        PositionTrackBar.Value = 0
        DataSentTextBox.Text = "Position:" & PositionTrackBar.Value.ToString
    End Sub

    '========================================
    '-------- Serial Communication ---------
    'Purpose: Open and setup port
    '========================================
    Private Sub Connect()
        Try
            Dim comPort As String = COMPort_ComboBox.Text
            SerialPort1.Close()          'Ensure clean state before opening

            With SerialPort1
                .BaudRate = 9600         'Matches PIC UART config
                .Parity = Parity.None
                .StopBits = StopBits.One
                .DataBits = 8
                .Handshake = Handshake.None
                .PortName = comPort
                .Open()
            End With

            If SerialPort1.IsOpen Then
                MessageBox.Show("Connected to " & SerialPort1.PortName)
                PositionTrackBar.Enabled = True
                RxTimer.Start()          'Begin reading incoming data on a timer
            End If

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        End Try
    End Sub

    '========================================
    '---------- Transmit Section -----------
    'Purpose: Convert UI (slider) into a 2-byte command and send it.
    '========================================
    Private Sub PositionTrackBar_Scroll(sender As Object, e As EventArgs) Handles PositionTrackBar.Scroll
        Transmit()                              'Send immediately when the slider moves
    End Sub

    Private Sub Transmit()
        Try
            If Not SerialPort1.IsOpen Then Connect()  'Auto-connect if needed

            Dim data(1) As Byte
            data(0) = &H24                             'Leader byte: '$'
            data(1) = PositionToByte(PositionSelect()) 'Map slider to command byte

            ShowTransmit(data)
            SerialPort1.Write(data, 0, 2)              'Send exactly 2 bytes
            ReadTimer.Enabled = True                   'Allow any reply to be read

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        End Try
    End Sub

    Private Function PositionSelect() As Integer
        'Keep UI label in sync with the slider and return its value
        Dim position As Integer = PositionTrackBar.Value
        DataSentTextBox.Text = "Position:" & position.ToString
        Return position
    End Function

    Private Function PositionToByte(position As Integer) As Byte
        'Simple lookup: each position maps to a specific command value
        Select Case position
            Case 1 : Return &H0
            Case 2 : Return &H1
            Case 3 : Return &H2
            Case 4 : Return &H3
            Case 5 : Return &H4
            Case 6 : Return &H5
            Case 7 : Return &H6
            Case 8 : Return &H7
            Case 9 : Return &H8
            Case 10 : Return &H9
            Case 11 : Return &HA
            Case 12 : Return &HB
            Case 13 : Return &HC
            Case 14 : Return &HD
            Case 15 : Return &HE
            Case 16 : Return &HF
            Case 17 : Return &H10
            Case 18 : Return &H11
            Case 19 : Return &H12
            Case 20 : Return &H13
            Case Else : Return 0
        End Select
    End Function

    Private Sub ShowTransmit(data() As Byte)
        'Display sent bytes in three views: hex, ascii, binary
        Dim hexLine, asciiLine, binLine As String

        For Each b As Byte In data
            hexLine &= b.ToString("X2") & " "
            asciiLine &= If(b >= 32 AndAlso b <= 126, ChrW(b), ".")
            binLine &= Convert.ToString(b, 2).PadLeft(8, "0"c) & " "
        Next

        HexTextBox.AppendText(hexLine.TrimEnd() & Environment.NewLine)
        AsciiTextBox.AppendText(asciiLine & Environment.NewLine)
        BinaryTextBox.AppendText(binLine.TrimEnd() & Environment.NewLine)
    End Sub

    '========================================
    '-------- Receive (Rx) Section ----------
    'Purpose: Pull bytes from the port and parse ADC packets.
    '========================================
    Private Sub RxTimer_Tick(sender As Object, e As EventArgs) Handles RxTimer.Tick
        If Not SerialPort1.IsOpen Then Exit Sub
        Dim n As Integer = SerialPort1.BytesToRead
        If n <= 0 Then Exit Sub

        Dim buf(n - 1) As Byte
        Dim got As Integer = SerialPort1.Read(buf, 0, n)
        If got > 0 Then AppendRx(buf, got)
    End Sub

    Private Sub AppendRx(data() As Byte, count As Integer)
        'Packet format expected: 0x26, ADRESH, ADRESL (right-justified ADC)
        Dim i As Integer = 0

        While i < count
            Dim b As Byte = data(i)
            Select Case adcStage
                Case 0
                    'Wait for the start marker (0x26) before reading ADC bytes
                    If b = CMD_ADC Then
                        adcSync = True
                        adcStage = 1
                        If CmdTextBox IsNot Nothing Then CmdTextBox.Text = b.ToString("X2")
                    Else
                        Exit While             'Not an ADC packet start; log below
                    End If
                Case 1
                    'First ADC byte (high)
                    adch = b
                    HighByteTextBox.Text = adch.ToString("X2")
                    adcStage = 2
                Case 2
                    'Second ADC byte (low) → combine and show volts
                    adcl = b
                    LowByteTextBox.Text = adcl.ToString("X2")
                    Dim raw As Integer = ((CInt(adch) << 8) Or CInt(adcl)) And &H3FF
                    raw = Math.Min(Math.Max(raw, 0), AdcMax)
                    Dim volts As Double = raw * AdcVref / AdcMax
                    ADCTextBox.AppendText($"ADC: {raw}  ({volts:0.000} V){Environment.NewLine}")
                    adcSync = False
                    adcStage = 0
            End Select
            i += 1
        End While

        'Any extra bytes that weren’t part of an ADC packet get logged here
        If i < count Then
            Dim hexLine As New System.Text.StringBuilder()
            Dim asciiLine As New System.Text.StringBuilder()
            Dim binLine As New System.Text.StringBuilder()

            While i < count
                Dim b = data(i)
                hexLine.Append(b.ToString("X2")).Append(" ")
                asciiLine.Append(If(b >= 32 AndAlso b <= 126, ChrW(b), "."c))
                binLine.Append(Convert.ToString(b, 2).PadLeft(8, "0"c)).Append(" ")
                i += 1
            End While

            HexReadTextBox.AppendText(hexLine.ToString().TrimEnd() & Environment.NewLine)
            AsciiReadTextBox.AppendText(asciiLine.ToString() & Environment.NewLine)
            BinaryReadTextBox.AppendText(binLine.ToString().TrimEnd() & Environment.NewLine)
        End If
    End Sub

    '========================================
    '----------- ADC Control Logic ---------
    'Purpose: Constants + simple timer to request ADC samples.
    '========================================
    Private Const CMD_SERVO As Byte = &H24
    Private Const CMD_ADC_REQUEST As Byte = &H26
    Private Const CMD_ADC As Byte = &H26

    Private Const AdcMax As Integer = 1023          '10-bit ADC range
    Private AdcVref As Double = 5.0                 'Adjust if your PIC uses 3.3V

    'Simple parser state for the 3-byte ADC packet
    Private adcSync As Boolean = False
    Private adcStage As Integer = 0
    Private adch As Byte = 0
    Private adcl As Byte = 0
    Private expectingAdc As Boolean = False
    Private adcIndex As Integer = 0
    Private adcBuf(1) As Byte

    Private Sub ADCCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles ADCCheckBox.CheckedChanged
        'Toggle timed ADC reads on/off
        If ADCCheckBox.Checked AndAlso SerialPort1.IsOpen Then
            ADCTimer.Interval = 2000                 ' Request every 2 seconds
            ADCTimer.Start()
        Else
            ADCTimer.Stop()
        End If
    End Sub

    Private Sub ADCTimer_Tick(sender As Object, e As EventArgs) Handles ADCTimer.Tick
        'Timer asks the PIC for one ADC sample using 0x26
        If Not SerialPort1.IsOpen Then Exit Sub
        Dim outBytes() As Byte = {&H26}

        Try
            SerialPort1.Write(outBytes, 0, outBytes.Length)
            ShowTransmit(outBytes)                          'Show the request we sent
            expectingAdc = True
            adcIndex = 0
        Catch ex As Exception
            ADCTimer.Stop()
            MessageBox.Show("ADC request failed: " & ex.Message)
        End Try
    End Sub

End Class
