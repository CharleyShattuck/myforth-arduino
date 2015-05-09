\ main.fs

\ wired to make TX Bolt protocol easy,
\ can read six bits at a time in correct order

\ PORTC bit  columns
\       0    S R F T
\       1    T A R S
\       2    K O P D
\       3    P * B Z
\       4    W E L #
\       5    H U G

cvariable b0
cvariable b1
cvariable b2
cvariable b3

\ weak pullup on PORTC pins
\ high impedence on column pins
: init  0 N ldi,  N DDRC out,  $ff N ldi,  N PORTC out,
    2 input,  3 input,  4 input,  5 input, ;

: +col0  2 output, 2 low, ;
: -col0  2 input, ;
: +col1  3 output, 3 low, ;
: -col1  3 input, ;
: +col2  4 output, 4 low, ;
: -col2  4 input, ;
: +col3  5 output, 5 low, ;
: -col3  5 input, ;

\ \ cover up a wiring mistake, bits 4 and 5 are swapped
: read ( - b)  0 #,  PINC T in,  $3f #, xor ;
\    dup $0f #, and swap $30 #, and dup 2* swap 2/ or $30 #, and or ;

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

: ms ( n)  for 4000 #, for next next ;
: zero  b0 a! 0 #, dup c!+ dup c!+ dup c!+ c!+ ;
: scan  zero 0 #, begin drop look until  5 #, ms
    begin drop look while repeat drop ;

: send
    b0 c@ if dup emit then drop
    b1 c@ if dup $40 #, or emit then drop
    b2 c@ if dup $80 #, or emit then drop
    b3 c@ if $c0 #, or then emit ;
\ : show  hex  b0 c@ .  b1 c@ .  b2 c@ .  b3 c@ .  cr ;

: go  init
   begin scan send again
\   begin scan show again

