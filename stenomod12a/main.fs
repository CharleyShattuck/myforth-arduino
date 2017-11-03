\ main.fs  modified for extra S- key
\ and added Gemini PR protocol

0 [if]
Copyright (C) 2016-2017 by Charles Shattuck.

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

\ Gemini protocol will require some shuffling

\ byte->   0   1   2   3   4   5
\ -------------------------------
\ bit  7)  1   0   0   0   0   0
\      6)  0   S1  R-  0   -P  0
\      5)  0   S2  A-  0   -B  0
\      4)  0   T-  O-  0   -L  0
\      3)  0   K-  *1  -E  -G  0
\      2)  0   P-  0   -U  -T  0
\      1)  0   W-  0   -F  -S  #C
\      0)  0   H-  0   -R  -D  -Z


\ begin configuration
false value key-repeat?   \ allow strokes to repeat if held
true value merge-S?      \ merge the split S keys into one
false value gemini?      \ choose the protocol
\ end configuration

: ms ( n)  for 4000 #, for next next ;

13 constant LED

cvariable b0
cvariable b1
cvariable b2
cvariable b3

\ weak pullup on PORTC and PORTD pins
\ high impedence on column pins
\ : init  0 N ldi,  N DDRC out,  N DDRD out,  N PORTD out,
\    $ff N ldi,  N PORTC out,  N PORTD out,
\    $f0 N ldi,  N PORTB out, ;

\ no, output high on column pins
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
merge-S? [if]  \ merge the second S key in with the 1st
    $20 #, and if/  1 #, b0 or!c then
    dup $1f #, and
[then]
\ *** just disable the extra key...
     \ drop $1f #, and dup \ ***
    b3 or!c -col0 or ;

gemini? [if]  \ define "send" in Gemini or TX

\ b accumulates bits for next byte in protocol
\ md is the bit mask for destination
\ ms is the bit mask for source
\ a is the address of the input variable
\ so shuffle moves a bit into a new position and
\ and leaves it in an accumulator at the top of the stack
: shuffle ( b md ms a - b')
    c@ and if/  or ; then  drop ;

: send   \ Gemini PR
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

[else]

: send   \ TX Bolt
    b0 c@ if dup emit then drop
    b1 c@ if dup $40 #, or emit then drop
    b2 c@ if dup $80 #, or emit then drop
    b3 c@ if $c0 #, or then emit ;

[then]

\ remember the stroke
cvariable b0'
cvariable b1'
cvariable b2'
cvariable b3'
: keep  b0 c@ b0' c!  b1 c@ b1' c!
    b2 c@ b2' c!  b3 c@ b3' c! ;
: recall  b0' c@ b0 c!  b1' c@ b1 c!
    b2' c@ b2 c!  b3' c@ b3 c! ;
: same ( - f)  b0 c@ b0' c@ =  b1 c@ b1' c@ = and
    b2 c@ b2' c@ = and  b3 c@ b3' c@ = and ;

: zero  keep
    b0 a! 0 #, dup c!+ dup c!+ dup c!+ c!+ ;

variable repeating
variable timing
variable short
: norepeat  zero keep 0 #,
    dup timing ! dup repeating ! short ! ;

\ check for release every ms or so.
\ when keys are released exit two levels up
\ to avoid the unnecessary send
: check ( n)  for 4000 #, for next
    look 0= if/  norepeat pop drop ; then next ;

: threshold (  - n)  3000 #, ;
: sub-threshold (  - n)  750 #, ;

\ only a very short first tap allows a repeat
: ?short  timing @ sub-threshold - 0< short ! ;    

: timeout? ( - ?)  1 #, timing @ + dup timing !
    threshold swap - 0<  same and  short @ and ;
    
: ?repeat  repeating @ if/  100 #, check recall send ; then
    timeout? if/  -1 #, repeating ! keep then ;

: scan  begin  zero 
    0 #, dup timing ! repeating !
    begin  look until/  20 #, ms look until/
    LED high,
    begin  look while/ 
key-repeat? [if]  ?repeat  [then]
    repeat  
key-repeat? [if]  ?short  [then]
    send  LED low, ;

: go  init norepeat 
    begin scan again

