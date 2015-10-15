\ miscAVR.fs

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

nowarn

: hello ." Forth for Arduino" ;
' hello is bootmessage

\ ----- Virtual Machine ----- /
\ Subroutine threaded.
\ reserve registers 0 and 1 for the mul instruction.
\ ***** assigned in job.fs, shared by disAVR.fs ***** /
\ 30 constant Z   31 constant Z' (Z register, used as loop counter)
\ 28 constant Y   \ address register (A register)
\ 26 constant X   \ pointer to rest of stack (S register)
\ 24 constant T   25 constant T'   \ top of stack
\ 22 constant N   23 constant N'   \ next on stack (temporary)
\ $3e constant SPH  $3d constant SPL   \ return stack pointer

\ ----- Target Forth Primitives ----- /

:m nop  nop,  m;

:m begin ( - adr)  hide here  m;
:m again ( adr)  hide rjmp,  m;
:m until ( adr)  hide 0 T adiw,  rel $7f and breq,  m;
:m -until ( adr)  hide T' T' and,  rel $7f and brpl,  m;

\ loop until bit is set
:m /until ( bit)  hide clr? again m;
\ loop until bit is clear
:m \until ( bit)  hide set? again m;

\ forward ... resolve  ( long jump)
:m forward ( - adr)  begin dup ljmp,  m;
:m resolve ( adr)  begin >r org r@ ljmp,  r> org  m;

\ ahead ... then  ( short relative jump)
:m ahead ( - adr)  begin dup rel $7f and rjmp,  m;
:m then ( adr)
    dup @-t $f000 and $c000 = if  
        begin >r org r@ rjmp, r> org host exit target then
    begin >r dup org r@  rel $7f and
    3 lshift over @-t $fc07 and or swap !-t  r> org  m;

\ each of the following matches up with "then" for a short relative jump
:m if ( - adr)  0 T adiw,  ( here) begin dup rel $7f and breq,  m;
:m -if ( - adr)  T' T' and, ( here) begin dup rel $7f and brpl,  m;
:m while ( a - a' a)  if [ swap ]  m;
:m -while ( a - a' a)  -if [ swap ]  m;
:m repeat ( a a')  again then  m;
:m /if ( - adr)  hide clr? ahead m;
:m \if ( - adr)  hide set? ahead m;

\ these pop the stack as in classic Forth
:m drop  hint  T ldx+,  T' ldx+,  m;  \ 2 or 0
:m if/ ( - adr)  0 T adiw,  drop  begin dup rel $7f and breq,  m;
:m until/ ( adr)  0 T adiw,  drop hide rel $7f and breq,  m;
:m while/ ( a - a' a)  if/ [ swap ]  m;

\ stack
:m dup  T' -stx,  T -stx,  m;  \ 2 or 0
:m ?dup  \ removes redundant "drop dup" pairs
   edge here [ 4 - - if ] dup [ exit then ]
   edge @-t $918d =  edge 2 + @-t $919d =  [ and if ]
      -4 allot  [ exit then ] hint dup  m;

:m nip  N ldx+,  N' ldx+,  m;  \ 2
:m |swap  nip  dup  N T movw,  m;  \ 5
:m |over  nip  N' -stx,  N -stx,  dup  N T movw,  m;  \ 7
:m push  T push,  T' push,  drop  m;  \ 2 or 4
:m pop  ?dup  T' pop,  T pop,  m;  \ 2 or 4

\ binary
:m |+  nip  N T add,  N' T' adc,  m;  \ 4
:m +'  nip  N T adc,  N' T' adc,  m;  \ 4
:m |and  nip  N T and,  N' T' and,  m;  \ 4
:m |xor  nip  N T xor,  N' T' xor,  m;  \ 4
:m |or  nip  N T or,  N' T' or,  m;  \ 4
:m |-  T N movw,  drop  N T sub,  N' T' sbc,  m;  \ 5

\ hand optimization
:m #+ ( n)  [ dup 0 64 within 0= abort" number out of range" ]
   T adiw,  m;  \ 1
:m #- ( n)  [ dup 0 64 within 0= abort" number out of range" ]
   T sbiw,  m;  \ 1

\ unary
:m invert  T com,  T' com,  m;  \ 2
:m |negate  invert  1 #+  m;  \ 3
:m 2*  T T add,  T' T' adc,  m;  \ 2
:m |2/  7 T' bst,  T' ror,  T ror,  7 T' bld,  m;  \ 4

\ memory  (A=y)
:m a!  T Y movw,  drop  m;  \ 3
:m a  ?dup  Y T movw,  m;  \ 3 or 1
:m @+  ?dup  T ldy+,  T' ldy+,  m;  \ 2 or 4
:m c@+  ?dup  T ldy+,  0 T' ldi,  m;  \ 2 or 4
:m !+  T sty+,  T' sty+,  drop  m;  \ 2 or 4
:m c!+  T sty+,  drop  m;  \ 1 or 3
\ 16 bit special function registers want
\ high byte *written* first but *read* last
:m |@  T Y movw,  T ldy+,  T' ldy+,  m;  \ 3
:m |!  T Y movw,  drop  2 Y adiw,  T' -sty,  T -sty,  drop  m;  \ 8
:m |c@  T Y movw,  T ldy+,  0 T' ldi,  m;  \ 3
:m |c!  T Y movw,  drop  T sty+,  drop  m;  \ 6

\ literal
:m #, ( n)  ?dup  [ dup $ff and ] T ldi,  \ 2 or 4
   [ 8 rshift $ff and ] T' ldi,  m;
:m ~#, ( n)  host invert  target #  m;  \ 2 or 4

\ counted loop, be careful about using the Z register inside!
\ 10 for counts from 10 down to 1 in Z (R), but i shows the index
\ as 9 down to 0. r@ gets the unmodified index, 10 to 1,
\ or whatever else may be in Z.
:m for ( - adr)  hide
   Z' push,  Z push,  T Z movw,  drop begin  m;  \ 5 (once)
:m next ( adr)  1 Z sbiw,  [ rel $7f and ] brne,  \ 2 (inside loop)
   Z pop,  Z' pop,  hide m;  \ 2 (at finish)
:m r@ ( - n)  ?dup  Z T movw,  m;
:m i ( - n)  r@  1 T sbiw,  m;

\ 32 bit result in 2,3,4,5
:m |16*16=32
   nip  20 20 xor,  \ multiply T,T' N,N'
   T' N' mul,  0 4 mov,  1 5 mov,
   T  N  mul,  0 2 mov,  1 3 mov,
   T  N' mul,  0 3 add,  1 4 adc,  20 5 adc,
   T' N  mul,  0 3 add,  1 4 adc,  20 5 adc,  m;

\ dividend in 2,3,4,5 as left by 16*16=32 ; divisor in T
\ remainder in 4,5 ; quotient in 2,3
\ zero in 8 ; counter in N ; bit in 7
:m |32/16=16,16
   N' N' xor,  \ preload zero for comparison later
   $10 N ldi,  \ loop counter in N
   begin
      6 6 xor,  2 2 add,  3 3 adc,
         4 4 adc,  5 5 adc,  6 6 adc,  \ shift
      T 4 cp,  T' 5 cpc,  N' 6 cpc,  \ trial subtraction
      3 brcs,  2 inc,  T 4 sub,  T' 5 sbc,  \ actual subtraction
      N dec,  2 breq,  ljmp,  m;

\ hand optimizing
:m apush  Y' push,  Y push,  m;
:m apop  Y pop,  Y' pop,  m;
:m zpush  Z' push,  Z push,  m;
:m zpop  Z pop,  Z' pop,  m;

:m variable  :  cpuHERE #, 2 cpuALLOT  exit m;
:m cvariable  :  cpuHERE #, 1 cpuALLOT  exit m;

