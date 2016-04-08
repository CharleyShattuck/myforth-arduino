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

host
:m # ( n)  T ldi,  m;
:m @ ( reg)  ldy,  m;
:m ! ( reg)  sty,  m;
:m ? ( reg)  [ dup ] and,  m;
:m ##a! ( n)  Y ldi,  0 Y' ldi,  m;
:m #- ( n)  T subi,  m;
:m #+ ( n)  [ negate $ff and ]  T subi,  m;
:m #and ( n)  T andi,  m;
:m #or ( n)  T ori,  m;
:m push ( reg)  -stx,  m;
:m pop ( reg)  ldx+,  m;
:m ##ms ( n)  ##for 4000 ##for next next m;
:m ##us ( n)  [ 2* 2* ] ##for next m;

target
: emit ( T)  UCSR0A ##a!
    begin,  N @  $20 N andi,  until,
    UDR0 ##a!  T !  ;

: cr  13 # emit  10 # emit ; 
: space  32 # emit ;

: digit ( T - T)  $0f #and  $0A #-  -if,  
        $3A #+  ;  then,  $41 #+  ;
: h. ( T)   T push  T ror,  T ror,  T ror,  T ror,
    digit emit  T pop  digit emit  space  ;


host
\ registers used
0 constant b0
1 constant b1
2 constant b2
3 constant b3
4 constant keys

\ weak pullup on PORTC pins
\ high impedence on column pins
:m init  0 #  T DDRC out,  $ff #  T PORTC out,
    2 input, 2 low,  3 input, 3 low,
    4 input, 4 low,  5 input, 5 low, m;

:m +col0  2 output, m;
:m -col0  2 input, m;
:m +col1  3 output, m;
:m -col1  3 input, m;
:m +col2  4 output, m;
:m -col2  4 input, m;
:m +col3  5 output, m;
:m -col3  5 input, m;

:m zero  b0 b0 xor,  b1 b1 xor,  b2 b2 xor,  b3 b3 xor,  m;

target
: read  ( - T)  PINC N in,  $3f #  N T xor,  ;

\ it seems that a delay is required after changing columns
\ before reading the next one, to avoid spurious characters
: wait  10 ##us ;

: look ( - T)
    +col3 wait read  T b0 or, -col3  T keys mov,
    +col2 wait read  T b1 or, -col2  T keys or,
    +col1 wait read  T b2 or, -col1  T keys or,
    +col0 wait read  T b3 or, -col0  keys T or, ;

: scan
    begin, zero
        begin, look until,  20 ##ms look until,
    begin, look 0until, ;

: send ( x - x)  13 high,
    b0 ? if,  b0 T mov, emit  then,
    b1 ? if,  b1 T mov,  $40 #or emit  then,
    b2 ? if,  b2 T mov,  $80 #or emit  then,
    b3 T mov,  b3 ? if,  $c0 #or  then,  emit 
    13 low, ;

: show
    b0 T mov, h. 
    b1 T mov, h. 
    b2 T mov, h. 
    b3 T mov, h. 
    cr ;

: go  init
    begin, scan ( show) send again,

