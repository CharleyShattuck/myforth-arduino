\ main.fs 

0 [if]
Copyright (C) 2016-2018 by Charles Shattuck.

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

For LGPL information:   http://www.gnu.org/copyleft/lesser.txt

[then]


\ wired to make TX Bolt protocol easy,
\ can read six bits at a time in correct order

\ PORTC bit  columns
\       0    S R F T
\       1    T A R S
\       2    K O P D
\       3    P * B Z
\       4    W E L #
\       5    H U G S1


: ms ( n)  for 4000 #, for next next ;

13 constant LED

cvariable b0
cvariable b1
cvariable b2
cvariable b3

\ output high on column pins
: init  0 N ldi,  N DDRC out,  N DDRD out,  \ inputs
    $ff N ldi,  N PORTC out,  N PORTD out,  \ weak pullups
    $0f N ldi,  N DDRB out,  \ columns output, rest input
    $ff N ldi,  N PORTB out, ;  \ columns high, rest weak pullup


\ in the -colx words, yank the column high to deactivate
: us ( n)  2* 2* for next ;
: wait  10 #, us ;
: +col0  PB0 low, ;
: -col0  PB0 high, ;
: +col1  PB1 low, ;
: -col1  PB1 high, ;
: +col2  PB2 low, ;
: -col2  PB2 high, ;
: +col3  PB3 low, ;
: -col3  PB3 high, ;

: read ( - b)  0 #,  PINC T in,  $3f #, xor ;

\ it seems that a delay is required after changing columns
\ before reading the next one
: or!c ( n a - )  swap over c@ or swap c! ;
: look ( - flag)
    +col3 wait read dup b0 or!c -col3
    +col2 wait read dup b1 or!c -col2 or
    +col1 wait read dup b2 or!c -col1 or
    +col0 wait read dup 
    drop $1f #, and dup 
    b3 or!c -col0 or ;

: send   \ TX Bolt
    b0 c@ if dup emit then drop
    b1 c@ if dup $40 #, or emit then drop
    b2 c@ if dup $80 #, or emit then drop
    b3 c@ if $c0 #, or then emit ;

: zero  b0 a! 0 #,  dup c!+ dup c!+ dup c!+ c!+ ;

: scan  begin  zero 
    begin  look until/  20 #, ms look until/
    LED high,
    begin  look while/ 
    repeat  
    send  LED low, ;

: go  init  begin scan again

