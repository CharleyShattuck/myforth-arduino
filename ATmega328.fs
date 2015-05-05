\ ATmega328.fs

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

$15 constant TIFR0
$16 constant TIFR1
$17 constant TIFR2
$1b constant PCIFR

$23 constant GTCCR
$24 constant TCCR0A
$25 constant TCCR0B
$26 constant TCNT0
$27 constant OCR0A
$28 constant OCR0B

$34 constant MCUSR
$35 constant MCUCR
$3f constant SREG
$3e constant SPH
$3d constant SPL

\ ----- "High" Special Function Registers ----- /
\ can't be addressed with IN or OUT

$60 constant WDTCSR
$6e constant TIMSK0
$6f constant TIMSK1
$70 constant TIMSK2

$80 constant TCCR1A
$81 constant TCCR1B
$82 constant TCCR1C
$84 constant TCNT1
$86 constant ICR1
$88 constant OCR1A
$8a constant OCR1B

$b0 constant TCCR2A
$b1 constant TCCR2B
$b2 constant TCNT2
$b3 constant OCR2A
$b4 constant OCR2B
$b6 constant ASSR

$c6 constant UDR0    \ USART I/O Data Register
$c5 constant UBRR0H  \ USART Baud Rate Register High
$c4 constant UBRR0L  \ USART Baud Rate Register Low
$c2 constant UCSR0C
$c1 constant UCSR0B
$c0 constant UCSR0A

: PORT ( n - bit port)   dup 8 < if PORTD exit then  -8 + PORTB ;
: DDR ( n - bit port)   dup 8 < if DDRD exit then  -8 + DDRB ;
: PIN ( n - bit port)   dup 8 < if PIND exit then  -8 + PINB ;
   
