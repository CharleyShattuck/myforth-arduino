\ math.fs

0 [if]
Copyright (C) 2009 by Charles Shattuck.

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

-: 16*16=32  |16*16=32 ;  \ basic multiplication
: um* ( u1 u2 - ud)  16*16=32
   3 -stx,  2 -stx,  4 T mov,  5 T' mov,  ;

-: 32/16=16,16  |32/16=16,16 ;
\ put dividend in 2,3,4,5 as if by 16*16=32
\ leave divisor in T,T'
\ get remainder from 4,5 quotient from 2,3
: um/mod
   4 ldx+,  5 ldx+,  2 ldx+,  3 ldx+,
   32/16=16,16  5 -stx,  4 -stx,  2 T mov,  3 T' mov,  ;
: u/mod ( u1 u2 - rem quo)  push 0 # pop um/mod ;

: abs ( n - n')  -if  negate then ;
: ?negate ( u n - n')  -if  drop negate ; then  drop ;
: dnegate ( d1 - d2)  swap invert swap invert 1 # 0 #  \ fall through
: d+ ( d1 d2 - d3)  push swap push + pop pop +' ;
: dabs ( d - +d)  -if  dnegate then  ;
: s>d ( n - d)  dup  \ fall through into 0<
: 0< ( n1 - flag)  -if  drop -1 # ; then  drop 0 # ;

: sm/rem ( d n - r q)
   over push over over xor push \ save signs
   push dabs pop abs  \ everything positive
   um/mod pop ?negate  \ apply sign to quotient
   swap pop ?negate swap ;  \ apply sign to remainder

\ signed integers
: *  16*16=32  2 T mov,  3 T' mov,  ; 

: m* ( n1 n2 - d)  over over xor invert push
   push abs pop abs um* pop -if drop ; then
   drop dnegate ;

: /mod ( n1 n2 - rem quo)  push s>d pop sm/rem ;
: mod ( n1 n2 - rem)  /mod drop ;
: / ( n1 n2 - quo)  /mod nip ;

: */ ( n1 n2 n3 - n4)  push m* pop sm/rem nip ;

\ signed fractions where +1=$4000
: *.  16*16=32
   3 3 add,  4 4 adc,  5 5 adc,
   3 3 add,  4 4 adc,  5 5 adc,
   4 T mov,  5 T' mov,  ;
: +1  $4000 # ;
: /.  +1 swap */ ;
: >f  10000 # /. ;

