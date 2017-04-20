\ main.fs  modified for extra S- key

\ wired to make TX Bolt protocol easy,
\ can read six bits at a time in correct order

\ PORTC bit  columns
\       0    S R F T
\       1    T A R S
\       2    K O P D
\       3    P * B Z
\       4    W E L #
\       5    H U G S1

\ Gemini protocol will require some shuffling

\ byte->   0   1   2   3   4   5
\ -------------------------------
\ bit  0)  1   0   0   0   0   0
\      1)  0   S1  R-  0   -P  0
\      2)  0   S2  A-  0   -B  0
\      3)  0   T-  O-  0   -L  0
\      4)  0   K-  *1  -E  -G  0
\      5)  0   P-  0   -U  -T  0
\      6)  0   W-  0   -F  -S  #C
\      7)  0   H-  0   -R  -D  -Z


13 constant LED

cvariable b0
cvariable b1
cvariable b2
cvariable b3

\ weak pullup on PORTC and PORTD pins
\ high impedence on column pins
: init  0 N ldi,  N DDRC out,  N DDRD out,  N PORTD out,
    $ff N ldi,  N PORTC out,  N PORTD out,
    $f0 N ldi,  N PORTB out, ;
    
: +col0  PB0 output, PB0 low, ;
: -col0  PB0 input, ;
: +col1  PB1 output, PB1 low, ;
: -col1  PB1 input, ;
: +col2  PB2 output, PB2 low, ;
: -col2  PB2 input, ;
: +col3  PB3 output, PB3 low, ;
: -col3  PB3 input, ;

: read ( - b)  0 #,  PINC T in,  $3f #, xor ;

\ it seems that a delay is required after changing columns
\ before reading the next one, to avoid spurious characters
: us ( n)  2* 2* for next ;
: wait  10 #, us ;
: or!c ( n a - )  swap over c@ or swap c! ;
: look ( - flag)
    +col3 wait read dup b0 or!c -col3
    +col2 wait read dup b1 or!c -col2 or
    +col1 wait read dup b2 or!c -col1 or
    +col0 wait read dup b3 or!c -col0 or ;
\    +col0 wait read dup $20 #, and if/
\        1 #, b0 or!c
\    then  dup $1f #, and b3 or!c -col0 or ;

: ms ( n)  for 4000 #, for next next ;
: zero  b0 a! 0 #, dup c!+ dup c!+ dup c!+ c!+ ;
: scan
    begin zero
        begin look until/  20 #, ms look until/
    LED high,
    begin look while/ repeat
    LED low, ;

\ b accumulates bits for next byte in protocol
\ md is the bit mask for destination
\ ms is the bit mask for source
\ a is the address of the input variable
\ so shuffle moves a bit into a new position
\ and leaves it in the accumulater
: shuffle ( w md ms a - w)
    c@ and if/  over or ; then  drop ;

: send-TX
    b0 c@ if dup emit then drop
    b1 c@ if dup $40 #, or emit then drop
    b2 c@ if dup $80 #, or emit then drop
    b3 c@ if $c0 #, or then emit ;

: send-gemini
    $80 #, emit
    0 #, 
       $40 #, $20 #, b3 shuffle \ S1
       $20 #, $01 #, b0 shuffle \ S2
       $10 #, $02 #, b0 shuffle \ T-
       $08 #, $04 #, b0 shuffle \ K-
       $04 #, $08 #, b0 shuffle \ P-
       $02 #, $10 #, b0 shuffle \ W-
       $01 #, $20 #, b0 shuffle \ H-
       emit
    0 #,
       $40 #, $01 #, b1 shuffle \ R-
       $20 #, $02 #, b1 shuffle \ A-
       $10 #, $04 #, b1 shuffle \ O-
       $08 #, $08 #, b1 shuffle \ *1
       emit
    0 #,
       $08 #, $10 #, b1 shuffle \ -E
       $04 #, $20 #, b1 shuffle \ -U
       $02 #, $01 #, b2 shuffle \ -F
       $01 #, $02 #, b2 shuffle \ -R
       emit
    0 #,
       $40 #, $04 #, b2 shuffle \ -P
       $20 #, $08 #, b2 shuffle \ -B
       $10 #, $10 #, b2 shuffle \ -L
       $08 #, $20 #, b2 shuffle \ -G
       $04 #, $01 #, b3 shuffle \ -T
       $02 #, $02 #, b3 shuffle \ -S
       $01 #, $04 #, b3 shuffle \ -D
       emit
    0 #,
       $02 #, $10 #, b3 shuffle \ #C
       $01 #, $08 #, b3 shuffle \ -Z
       emit
    ;

: send-hex  hex  b0 c@ .  b1 c@ .  b2 c@ .  b3 c@ .  cr ;

: send
\   send-hex
\   send-TX
    send-gemini    
    ;

: go  init
    begin scan send again

