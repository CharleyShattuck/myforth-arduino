\ timer.fs
[ ramp @ constant 'counter  4 ramp +! ]

$12 interrupt
   cli,  N push,  1 N ldi,  T push,  X push,  X' push,
   [ 'counter $ff and ] X ldi,   
   [ 'counter 256 / $ff and ] X' ldi,
   T ldx,  N T add,  T stx+,  0 N ldi,
   T ldx,  N T adc,  T stx+,
   T ldx,  N T adc,  T stx+,
   T ldx,  N T adc,  T stx+,
   131 T ldi,  TCNT2 X ldi,  0 X' ldi,  T stx,
   X' pop,  X pop,  T pop,  N pop,
   sei,  reti,

: and! ( n a)  swap over @ and swap ! ;
: or! ( n a)  swap over @ or swap ! ;

: init-interrupt
   $01 ~#, TIMSK2 #, and!
   $03 ~#, TCCR2A #, and!
   $08 ~#, TCCR2B #, and! 
   $20 ~#,   ASSR #, and!
   $02 ~#, TIMSK2 #, and!
   $05  #, TCCR2B #, or!
   $02 ~#, TCCR2B #, and!
   131 #,   TCNT2 #, !
   $01 #,  TIMSK2 #, !
   sei, ;

: dcounter ( - d)  'counter #, a! cli, @+ @+ sei, ;
: delapsed ( d)  dnegate dcounter d+ ;

: counter ( - n)  'counter #, a! cli, @+ sei, ;
: elapsed ( n - n)  negate counter + ;
: timer ( n)  elapsed u. ;

: ms ( n)  for  4000 #, for  next next ;

: test  counter 1000 #, ms timer ;
: dtest  dcounter 2000 #, ms delapsed swap u. u. ;

