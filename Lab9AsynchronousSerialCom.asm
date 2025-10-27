;Device Setup
;-------------------------------------------------------------------------------

; Configuration
; PIC16F883 Configuration Bit Settings

; Assembly source line config statements

; CONFIG1
  CONFIG  FOSC = HS	   
  CONFIG  WDTE = OFF            ; Watchdog Timer Enable bit (WDT disabled and can be enabled by SWDTEN bit of the WDTCON register)
  CONFIG  PWRTE = OFF           ; Power-up Timer Enable bit (PWRT disabled)
  CONFIG  MCLRE = ON            ; RE3/MCLR pin function select bit (RE3/MCLR pin function is MCLR)
  CONFIG  CP = OFF              ; Code Protection bit (Program memory code protection is disabled)
  CONFIG  CPD = OFF             ; Data Code Protection bit (Data memory code protection is disabled)
  CONFIG  BOREN = OFF           ; Brown Out Reset Selection bits (BOR disabled)
  CONFIG  IESO = OFF            ; Internal External Switchover bit (Internal/External Switchover mode is disabled)
  CONFIG  FCMEN = OFF           ; Fail-Safe Clock Monitor Enabled bit (Fail-Safe Clock Monitor is disabled)
  CONFIG  LVP = OFF             ; Low Voltage Programming Enable bit (RB3 pin has digital I/O, HV on MCLR must be used for programming)

; CONFIG2
  CONFIG  BOR4V = BOR40V        ; Brown-out Reset Selection bit (Brown-out Reset set to 4.0V)
  CONFIG  WRT = OFF             ; Flash Program Memory Self Write Enable bits (Write protection off)

// config statements should precede project file includes.
#include <xc.inc>
; Include Statements

; Code Section
;-------------------------------------------------------------------------------
    
; Register/Variable Setup
    W_SAVE   EQU    0x20    ;General Purpouse register for saving values during interrupt
    STATUS_SAVE EQU   0x21  ;General Purpouse register for saving values during interrupt
    ;WEIGHTED_BITS EQU 0x22  ;ADC Result From Bits Checked
    TIME EQU 0x23	    ;Stores PULSE WIDTH Delay Value	
    RC_DATA EQU 0x24
    RC_STATUS EQU 0x25
 ;Asign A Value To A Variable
 
; Start Of Program
    PSECT resetVect,class=CODE,delta=2	;Reset Vector Adress
    GOTO Start 

    PSECT isrVect,class=CODE,delta=2
    GOTO INTERRUPT_HANDLER
    
; Setup Code That Runs Once At Power Up/Reset
    PSECT code,class=CODE,delta=2
;------------------------------------------------------------------------   
Start:
   ;Bank 3
    BSF STATUS,5
    BSF STATUS,6
    ;MOVLW 0x80
    ;MOVWF OPTION_REG;Disables pull ups
    MOVLW 0x00
    MOVWF TRISB
 
   ;ANALOG SETUP
    ;MOVLW 0x01
    CLRF ANSEL	    ;Enables Analog Inputs for AN0
    CLRF ANSELH
    
   ;BANK 2
    BCF STATUS,5
    CLRF CM2CON1
    CLRF CM2CON0
    CLRF CM1CON0
  
   ;BANK 1
    BSF STATUS,5
    BCF STATUS,6
    CLRF IOCB
    CLRF PSTRCON
    CLRF ADCON1
    
   ;ANALOG SETUP
    ;MOVLW 0x00
    ;MOVWF WPUB	    ;Disables Pull ups
    ;MOVLW 0x01
    ;MOVWF TRISA
    ;MOVLW 0x00
    ;MOVWF TRISB
    
   ;BANK 0
    BCF STATUS,5
    CLRF PORTC
    CLRF SSPCON
    CLRF T1CON
    CLRF CCP1CON
    CLRF CCP2CON
    CLRF PORTA
    CLRF PORTB
    
   ;ANALOG STUFF
    ;MOVLW 0x41
    ;MOVWF ADCON0    ;Sets Fosc/8, Analog channel to AN0, & Enables ADC
;-------------------------------------------------------
    CLRF RC_STATUS
    
;Asynchronous Transmission
    BSF STATUS,5    ;BANK3
    BSF STATUS,6
    
    CLRF BAUDCTL
    
    BCF STATUS,6    ;BANK1
    ;MOVLW 0x1A
    ;MOVWF PIE1	    ;0001|1010 RCIE & TXIE Int. Enable Bits & TIMRIF
    
    MOVLW 0x0C
    MOVWF SPBRG	    ;0000|1100 Decimal 12(calculated SPBRG)
    
    CLRF SPBRGH
    
    MOVLW 0x40
    MOVWF TRISC	    ;0100|0000 Makes Bit6 an Output(TX)
    
    MOVLW 0x20
    MOVWF TXSTA	    ;0010|0000
    		    ;Bit 5 = TXEN (Transmit Enable Bit)
		    ;Bit 2 = BRGH (High Baud Rate Elect Bit)
		    ;Bit 1 = TRMT (Transmit Shft. Reg. Status Bit, Empty = 1)
    
    BCF STATUS,5    ;Bank0
    BSF RCSTA,7	    ;Serial Port Enable Bit
		    ;Configures RX/DT(Pin 18) & TX/CK(Pin 17) as serial ports
    BSF RCSTA,4
;---------------------------------------------------------
;Timer2	
    CLRF PIR1
    CLRF TMR2
    CLRF T2CON	  ;INIT ALL 0's on Startup
   
    BSF STATUS,5
    MOVLW 0x20 ;2
    MOVWF PIE1    ;Enable TMR2 INT Flag
    BCF PIE1,4
    
    MOVLW 0xFF
    MOVWF PR2	  ;Sets TMR2 to compare to max count
  
    MOVLW 0xC0
    MOVWF INTCON
    
    BCF STATUS,5
    MOVLW 0x25
    MOVWF T2CON  ;Sets PreSclr.1:16 & PostSclr.1:5, Turns on TMR2
  

    ;CALL TRANSMMIT
;----------------------------------------------------------------------   

MAIN:
    NOP
    NOP
    NOP
    NOP
GOTO MAIN
;----------------------------------------------------------------------
TRANSMMIT:
    ;BTFSS PORTB,0
;RETURN
    
    MOVF RC_DATA,0
    BANKSEL TXREG
    MOVWF TXREG ;PIN18 RX
   
    BANKSEL TXSTA
    BTFSS TXSTA,1
    GOTO $-1 
    
RETURN
    
RECEIVE:
    
    MOVF RCREG,0	;Takes Receive data
    MOVWF RC_DATA	;Lovingly places it into RC_DATA
    
    ;IF previous byte was '$', process current as data, then clear the flag
    BTFSC RC_STATUS,0	;Check if the last byte was a $
    CALL PROCESS_DATA	;If it was not a $ treat as data
    
    ;Now we decide what the 'current' byte is. If '$' set RC_STATUS
    ;If not '$' explicity clear the flag 
    MOVF RC_DATA,0	;Gather all avaliable data
    XORLW 0x24		;Check to see if $
    BTFSC STATUS,2	;Check ZERO flag, if set = $ from XOR
    GOTO SAW_DOLLAR
    
 NOT_DOLLAR:
    BCF	RC_STATUS,0	;sets user flag indicating  $$$$
    RETURN
    
SAW_DOLLAR:
    BSF RC_STATUS,0
    CALL TRANSMMIT
    
RETURN

PROCESS_DATA:
    ;Should obly enter if previous byte was '$'
    MOVF RC_DATA,0
    CALL TRANSMMIT			;Data magic happens
    BCF RC_STATUS,0	;When RC_S,0 cleared, ready for new data

    RETURN
    
TIMER2_STUFF:
    NOP
    NOP
    BANKSEL PIR1
    BCF PIR1,1    ;Clears TMR2 Interrupt flag
 RETURN
;----------------------------------------------------------------------
    
INTERRUPT_HANDLER:

    MOVWF W_SAVE
    MOVF STATUS,0
    MOVWF STATUS_SAVE   ;Saving our location when interrupted
    
    BANKSEL PIR1
    BTFSC PIR1,5
    CALL RECEIVE
    
    BTFSC PIR1,1
    CALL TIMER2_STUFF
    
    MOVF STATUS_SAVE,0	    ;Moves F -> W
    MOVWF STATUS	    ;Restores Status
    MOVF W_SAVE,0	    ;Restores W
    
RETFIE
    
;    BANKSEL PORTB
;    MOVLW 0xFF
;    XORWF PORTB,0
;    MOVWF PORTB
;----------------------------------------------------------------------    
END 