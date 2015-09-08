\ ATmega32U4.fs

\ ----- "Low" Special Function Registers ----- /
\ can be addressed with IN or OUT

$23 constant PINB
$24 constant DDRB
$25 constant PORTB
$26 constant PINC
$27 constant DDRC
$28 constant PORTC
$29 constant PIND
$2a constant DDRD
$2b constant PORTD
$2c constant PINE
$2d constant DDRE
$2e constant PORTE
$2f constant PINF
$30 constant DDRF
$31 constant PORTF

$35 constant TIFR0
$36 constant TIFR1
$38 constant TIFR3
$39 constant TIFR4
$3b constant PCIFR
$3c constant EIFR
$3d constant EIMSK
$3e constant GPIOR0
$3f constant EECR
$40 constant EEDR
$41 constant EEARL
$42 constant EEARH
$43 constant GTCCR
$44 constant TCCR0A
$45 constant TCCR0B
$46 constant TCNT0
$47 constant OCR0A
$48 constant OCR0B
$49 constant PLLCSR
$4a constant GPIOR1
$4b constant GPIOR2
$4c constant SPCR
$4d constant SPSR
$4e constant SPDR

$50 constant ACSR
$51 constant OCDR/MONDR
$52 constant PLLFRQ
$53 constant SMCR
$54 constant MCUSR
$55 constant MCUCR
$57 constant SPMCSR
$5b constant RAMPZ
$5d constant SPL
$5e constant SPH
$5f constant SREG

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
$cf constant OCR4A
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

create arduino-pins
    2 c, PORTD c,
    3 c, PORTD c,
    1 c, PORTD c,
    0 c, PORTD c,
    4 c, PORTD c,
    6 c, PORTC c,
    7 c, PORTD c,
    6 c, PORTE c,
    4 c, PORTB c,
    5 c, PORTB c,
    6 c, PORTB c,
    7 c, PORTB c,
    6 c, PORTD c,
    7 c, PORTC c,

: PORT ( i - bit adr)  2* arduino-pins + dup c@ swap 1+ c@ ;
: DDR ( i - bit adr)  PORT 1 - ;
: PIN ( i - bit adr)  PORT 2 - ;

