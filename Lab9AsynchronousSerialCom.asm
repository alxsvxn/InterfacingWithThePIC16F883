    ;I LIKE BOYS!
    ;Device Setup
;-------------------------------------------------------------------------------

; Configuration
; PIC16F883 Configuration Bit Settings

; Assembly source line config statements

; CONFIG1
  CONFIG  FOSC = XT	   
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
    TIME EQU 0x22	    ;Stores PULSE WIDTH Delay Value	
    RC_DATA EQU 0x23
    RC_STATUS EQU 0x24
    RC_SERVO EQU 0x25
    RC_DATA_THE_SEQUEL EQU 0x26
 
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
    MOVLW 0x80
    MOVWF OPTION_REG;Disables pull ups
    MOVLW 0x00
    MOVWF TRISB
 
   ;ANALOG SETUP
    MOVLW 0x01
    MOVWF ANSEL	    ;Enables Analog Inputs for AN0
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
    BSF ADCON1,7
    
   ;ANALOG SETUP
    MOVLW 0x00
    MOVWF WPUB	    ;Disables Pull ups
    MOVLW 0x01
    MOVWF TRISA
  
    
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
    MOVLW 0x41
    MOVWF ADCON0    ;Sets Fosc/8, Analog channel to AN0, & Enables ADC
;-------------------------------------------------------
    CLRF RC_STATUS
    
;Asynchronous Transmission
    BSF STATUS,5    ;BANK3
    BSF STATUS,6
    
    CLRF BAUDCTL
    
    BCF STATUS,6    ;BANK1
    
    MOVLW 0x19
    MOVWF SPBRG	    ;0000|1100 Decimal 12(calculated SPBRG)
    
    CLRF SPBRGH
    
    MOVLW 0x40
    MOVWF TRISC	    ;0100|0000 Makes Bit6 an Output(TX)
    
    MOVLW 0x2E
    MOVWF TXSTA	    ;0010|0000
    		    ;Bit 5 = TXEN (Transmit Enable Bit)
		    ;Bit 2 = BRGH (High Baud Rate Elect Bit)
		    ;Bit 1 = TRMT (Transmit Shft. Reg. Status Bit, Empty = 1)
    
    BCF STATUS,5    ;Bank0
    MOVLW 0x90
    MOVWF RCSTA	    ;Serial Port Enable Bit
		    ;Configures RX/DT(Pin 18) & TX/CK(Pin 17) as serial ports
;---------------------------------------------------------
;Timer2	
    CLRF PIR1
    CLRF TMR2
    CLRF T2CON	  ;INIT ALL 0's on Startup
   
    BSF STATUS,5
    MOVLW 0x02 ;2
    MOVWF PIE1    ;Enable TMR2 INT Flag
    
    MOVLW 0xFF
    MOVWF PR2	  ;Sets TMR2 to compare to max count
  
    MOVLW 0xC0
    MOVWF INTCON
    
    BCF STATUS,5
    MOVLW 0x26
    MOVWF T2CON  ;Sets PreSclr.1:16 & PostSclr.1:5, Turns on TMR2
;SAEVVEVVEVEVE
;----------------------------------------------------------------------   
MAIN:
    BSF	ADCON0,0		    ;Enable ADC
    BSF	ADCON0,1		    ;Start Conversion

    BTFSC PORTB,3		    ;HERE WILL BE THE LAST CHECK TO SEE IF WE HAVE
				    ;ADC DATA IN OUR RC_DATA_THE_SEQUEL
    CALL TRANSMMIT_ADC		    ;ONLY CALLS IF ADC DATA IS PRESENT
    
    BTFSC PORTB,2		    ;IS THE SERVO ON?
    GOTO SERVO			    ;If SERVO IS ON SKIP UNTIL FINISHED
    
    BTFSC PORTB,1		    ;IF SET IN RECEIVE, INDICATING $ MATCH
    GOTO SERVO_DATA		    ;IF PORTB,1 = 1 GO TO UPDATE SERVO DATA 
    
    GOTO MAIN
    
TRANSMMIT:
    MOVWF TXREG    
    BCF STATUS,2
    
RETURN
    
TRANSMMIT_ADC:
    BANKSEL ADCON0
WaitDone:
    BTFSC ADCON0,1
    GOTO WaitDone
    
    BANKSEL ADRESH
    MOVF ADRESH,0
    BANKSEL TXREG
    MOVWF TXREG			    ;MOVES DATA FROM ADRESH TO TXREG FOR TRANSMIT
    
    BANKSEL TXSTA
    BTFSS TXSTA,1
    GOTO $-1
    
    BANKSEL ADRESL
    MOVF ADRESL,0
    
    BANKSEL TXREG
    MOVWF TXREG			    ;MOVES DATA FROM ADRESL TO TXREG FOR TRANSMIT
 
    BCF PORTB,3			    ;CLEARS FLAG INDICATING TRANSMIT COMPLETE
    
    BCF RC_STATUS,3            ; ADC_PENDING = 0
    BCF RC_STATUS,2            ; ADC_MODE = 0
RETURN
SERVO_DATA:
    BTFSC RC_STATUS,2          ; ADC_MODE ?
    GOTO MAIN
    
    MOVF RCREG,0		    ;MOVES RC_DATA -> W
    MOVWF RC_DATA		    ;W -> LOVINGLY CURRATED REGISTER

    BCF PORTB,1			    ;AFTER DATA IS 
    GOTO MAIN
    
RECEIVE:
    MOVF RCREG,0
    MOVWF RC_DATA		     ;LOVINGLY STORES DATA HERE
  
    MOVF RCREG,0
    MOVWF RC_DATA_THE_SEQUEL	    ;ALSO STORES DATA HERE
    
    MOVF RC_DATA,0		    ;F -> W
    XORLW 0x24		    
    BTFSC STATUS,2		    ;CHECKS STATUS OF RESULT 
				    ;1 = MATCH
				    ;0 = NO MATCH SKIP LINE 191
    BSF PORTB,1			    ;SETS PORTB,1 IF $ IS READ
    BCF RC_STATUS,2		    ;ADC_MODE = 0
    XORLW 0x24
    CALL TRANSMMIT		    ;SINCE 0 WAS IN W, XOR IT AGAIN FOR RESULT IN W
    
    MOVF RC_DATA_THE_SEQUEL,0	    ;F -> W
    XORLW 0x26			    
    BTFSC STATUS,2		    ;CHECKS FOR #, IF NO MATCH SKIP LINE 202
    BSF PORTB,3
    
    BTFSC STATUS,2
    BSF RC_STATUS,2            ; ADC_MODE = 1
    BTFSC STATUS,2
    BSF RC_STATUS,3            ; ADC_PENDING = 1
    
    BCF	STATUS,2		    ;CLEAR ZERO FLAG TO MAKE SURE ITS READY FOR USE AGAIN
    RETURN
;-------------------------------------------------------------------------------
INTERRUPT_HANDLER:
    MOVWF W_SAVE	;Move w to w_Save
    MOVF STATUS,0	;Move Status to w
    MOVWF STATUS_SAVE	;Move w to Stat_Save
    
    BTFSC PIR1,5
    CALL RECEIVE
    
    ;BTFSC PIR1,1
    ;BSF	PORTB,2
    
BTFSC PIR1,1           ; TMR2IF?
BTFSC RC_STATUS,2      ; ADC_MODE=1 ?
GOTO  SkipServoTick    ; yes -> don't poke servo flag while ADC active
BTFSC PIR1,1
BSF   PORTB,2
SkipServoTick:
    
    CLRF PIR1
    
    MOVF STATUS_SAVE,0	;Move STAT_SAVE to w
    MOVWF STATUS	;Move w to STATUS
    MOVF W_SAVE,0	;Move w_SAVE to w
      
    RETFIE		;Return from interupt, renable Global interrupts
;-------------------------------------------------------------------------------   

SERVO:
    BTFSC RC_STATUS,2          ; ADC_MODE ?
    GOTO MAIN
    
    MOVF RC_DATA,0
    GOTO POSITION
    
POSITION:
    ADDWF PCL,1   
    GOTO POS_1	
    GOTO POS_2	
    GOTO POS_3	
    GOTO POS_4	
    GOTO POS_5	
    GOTO POS_6
    GOTO POS_7
    GOTO POS_8
    GOTO POS_9
    GOTO POS_10	
    GOTO POS_11	
    GOTO POS_12	
    GOTO POS_13	
    GOTO POS_14	
    GOTO POS_15	
    GOTO POS_16	
    GOTO POS_17	
    GOTO POS_18 
    GOTO POS_19	
    GOTO POS_20	
    GOTO POS_21	
    GOTO POS_22	

    
     POS_1:
    MOVLW 0x32
    MOVWF TIME    ;Set Delay value for desired Pulse Width
    BSF	PORTB,0 ;Set bit high for Servo's Pulse Width
    GOTO PW_DELAY 
    
    POS_2:
    MOVLW 0x3C   
    MOVWF TIME    
    BSF	PORTB,0 
    GOTO PW_DELAY  
    
    POS_3:
    MOVLW 0x46   
    MOVWF TIME   
    BSF	PORTB,0 
    GOTO PW_DELAY 
    
    POS_4:
    MOVLW 0x50 
    MOVWF TIME    
    BSF	PORTB,0
    GOTO PW_DELAY 
    
    POS_5:
    MOVLW 0x5A  
    MOVWF TIME  
    BSF	PORTB,0 
    GOTO PW_DELAY  
    
    POS_6:
    MOVLW 0x64  
    MOVWF TIME 
    BSF PORTB,0
    GOTO PW_DELAY  
    
    POS_7:
    MOVLW 0x6E
    MOVWF TIME   
    BSF	PORTB,0 
    GOTO PW_DELAY  
    
    POS_8:
    MOVLW 0x78   
    MOVWF TIME   
    BSF	PORTB,0 
    GOTO PW_DELAY   
    
    POS_9:
    MOVLW 0x82  
    MOVWF TIME   
    BSF	PORTB,0 
    GOTO PW_DELAY   
	
    POS_10:
    MOVLW 0x8C  
    MOVWF TIME  
    BSF	PORTB,0 
    GOTO PW_DELAY  
    
    POS_11:
    MOVLW 0x96  
    MOVWF TIME   
    BSF	PORTB,0 
    GOTO PW_DELAY  
    
    POS_12:
    MOVLW 0xA0   
    MOVWF TIME    
    BSF	PORTB,0 
    GOTO PW_DELAY 
    
    POS_13:
    MOVLW 0xAA    
    MOVWF TIME   
    BSF	PORTB,0 
    GOTO PW_DELAY   
    
    POS_14:
    MOVLW 0xB4  
    MOVWF TIME   
    BSF	PORTB,0 
    GOTO PW_DELAY  
    
    POS_15:
    MOVLW 0xBE   
    MOVWF TIME    
    BSF	PORTB,0 
    GOTO PW_DELAY   
    
    POS_16:
    MOVLW 0xC8   
    MOVWF TIME    
    BSF	PORTB,0 
    GOTO PW_DELAY  
    
    POS_17:
    MOVLW 0xD2    
    MOVWF TIME    
    BSF	PORTB,0 
    GOTO PW_DELAY
   
    POS_18:
    MOVLW 0xDC   
    MOVWF TIME   
    BSF PORTB,0 
    GOTO PW_DELAY  
    
    POS_19:
    MOVLW 0xE6  
    MOVWF TIME    
    BSF PORTB,0
    GOTO PW_DELAY  
    
    POS_20:
    MOVLW 0xF0   
    MOVWF TIME    
    BSF PORTB,0 
    GOTO PW_DELAY   
    
    POS_21:
    MOVLW 0xF8    
    MOVWF TIME    
    BSF PORTB,0 
    GOTO PW_DELAY 
    
    POS_22:
    MOVLW 0xFE    
    MOVWF TIME    
    BSF PORTB,0 
    GOTO PW_DELAY 
    
PW_DELAY:
    NOP		   
    NOP		   
    NOP		  
    NOP		   
    NOP		   
    NOP		   
    NOP		   
    DECFSZ TIME   
    GOTO PW_DELAY
    BCF PORTB,0
    BCF PORTB,2
    GOTO MAIN   
END 