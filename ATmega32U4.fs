\ ATmega32U4.fs

\ ----- "Low" Special Function Registers ----- /
\ can be addressed with IN or OUT

$03 constant PINB
$04 constant DDRB
$05 constant PORTB
$06 constant PINC
$07 constant DDRC
$08 constant PORTC
$09 constant PIND
$0a constant DDRD
$0b constant PORTD
$0c constant PINE
$0d constant DDRE
$0e constant PORTE
$0f constant PINF
$10 constant DDRF
$11 constant PORTF

$15 constant TIFR0
$16 constant TIFR1
$18 constant TIFR3
$19 constant TIFR4
$1b constant PCIFR
$1c constant EIFR
$1d constant EIMSK
$1e constant GPIOR0
$1f constant EECR
$20 constant EEDR
$21 constant EEARL
$22 constant EEARH
$23 constant GTCCR
$24 constant TCCR0A
$25 constant TCCR0B
$26 constant TCNT0
$27 constant OCR0A
$28 constant OCR0B
$29 constant PLLCSR
$2a constant GPIOR1
$2b constant GPIOR2
$2c constant SPCR
$2d constant SPSR
$2e constant SPDR

$30 constant ACSR
$31 constant OCDR/MONDR
$32 constant PLLFRQ
$33 constant SMCR
$34 constant MCUSR
$35 constant MCUCR
$37 constant SPMCSR
$3b constant RAMPZ
$3d constant SPL
$3e constant SPH
$3f constant SREG

\ ----- "High" Special Function Registers ----- /
\ can't be addressed with IN or OUT

$60 constant WDTCSR
$61 constant CLKPR
$64 constant PRR0
$65 constant PRR1
$66 constant OSCCAL
$67 constant RCCTRL
$68 constant PCICR
$69 constant EICRA
$6a constant EICRB
$6b constant PCMSK0
$6e constant TIMSK0
$6f constant TIMSK1
$71 constant TIMSK3
$72 constant TIMSK4

$78 constant ADCL
$79 constant ADCH
$7a constant ADCSRA
$7b constant ADCSRB
$7c constant ADMUX
$7d constant DIDR2
$7e constant DIDR0
$7f constant DIDR1

$80 constant TCCR1A
$81 constant TCCR1B
$82 constant TCCR1C
$84 constant TCNT1L
$85 constant TCNT1H
$86 constant ICR1L
$87 constant ICR1H
$88 constant OCR1AL
$89 constant OCR1AH
$8a constant OCR1BL
$8b constant OCR1BH
$8c constant OCR1CL
$8d constant OCR1CH

$90 constant TCCR3A
$91 constant TCCR3B
$92 constant TCCR3C
$94 constant TCNT3L
$95 constant TCNT3H
$96 constant ICR3L
$97 constant ICR3H
$98 constant OCR3AL
$99 constant OCR3AH
$9a constant OCR3BL
$9b constant OCR3BH
$9c constant OCR3CL
$9d constant OCR3CH

$b8 constant TWBR
$b9 constant TWSR
$ba constant TWAR
$bb constant TWDR
$bc constant TWCR
$bd constant TWAMR
$be constant TCNT4
$bf constant TC4H

$c0 constant TCCR4A
$c1 constant TCCR4B
$c2 constant TCCR4C
$c3 constant TCCR4D
$c4 constant TCCR4E
$c5 constant CLKSEL0
$c6 constant CLKSEL1
$c7 constant CLKSTA
$c8 constant UCSR1A
$c9 constant UCSR1B
$ca constant UCSR1C

$cc constant UBRR1L
$cd constant UBRR1H
$ce constant UDR1
$cd constant OCR4A
$d0 constant OCR4B
$d1 constant OCR4C
$d2 constant OCR4D

$d4 constant DT4
$d7 constant UHWCON
$d8 constant USBCON
$d9 constant USBSTA
$da constant USBINT

$e0 constant UDCON
$e1 constant UDINT
$e2 constant UDIEN
$e3 constant UDADDR
$e4 constant UDFNUML
$e5 constant UDFNUMH
$e6 constant UDMFN

$e8 constant UEINTX
$e9 constant UENUM
$ea constant UERST
$eb constant UECONX
$ec constant UECFG0X
$ed constant UECFG1X
$ee constant UESTA0X
$ef constant UESTA1X

$f0 constant UEIENX
$f1 constant UEDATX
$f2 constant UEBCLX
$f3 constant UEBCHX
$f4 constant UEINT

: PORT ( n - bit port)   dup 8 < if PORTD exit then  -8 + PORTB ;
: DDR ( n - bit port)   dup 8 < if DDRD exit then  -8 + DDRB ;
: PIN ( n - bit port)   dup 8 < if PIND exit then  -8 + PINB ;
   
