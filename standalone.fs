\ standalone.fs

0 [if]
Copyright (C) 2015 by Charles Shattuck.

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

: last  cvariable # ;
: base  variable # ;
: hex  $10 # base ! ;
: decimal  $0a # base ! ;
: =  -  \ falls through into 0=
: 0=  if -1 # or then invert ;
: 2drop  drop drop ;
: 2dup  over over ;
: min  2dup swap
-: clip  - -if  push swap pop then  drop drop ;
: 0max  0 #  \ falls through into max
: max  2dup clip ;
-: echo  dup  \ falls through into emit
: emit ( c)
   apush  0 #
   begin  drop UCSR0A # c@  $20 # and until
   drop UDR0 # c!  apop ;
: key ( - c)
   apush  0 #
   begin  drop UCSR0A # c@ $80 # and until
   drop UDR0 # c@  dup last c! apop ;
: type ( adr len)  0max if  apush
      swap a! for  c@+ emit next  apop ; 
   then  2drop ;
: space  32 # emit ;
: cr  13 # emit 10 # emit ;
-: (digit)  -10 # + -if  -39 #  + then  97 # + ;
-: digit  $0f # and (digit) emit ;
: 16/  2/ 2/ 2/ 2/ ;
-: (h.) ( n)  dup 16/ 16/ 16/ digit
   dup 16/ 16/ digit  dup 16/ digit  digit ;
: h. ( n)  (h.) space ;
: h.2 ( n)  dup 16/ digit digit space ;
-: sp@ ( - a)  dup  X T movw,  ;
: depth ( - n)  s0 # sp@ - 2/ 2 # - ;
-: ok  last c@ BL # = if  drop ; then  drop
   [ char o ] # emit  [ char k ] # emit cr ;
-: tib! ( c)  apush  tib # dup c@ 1 #+ over c!  dup c@ + c!  apop ;
-: huh?  [ char ? ] # emit forward ( *)
-: ?stack  depth -if  huh? ; then  drop ;

\ Be careful with these, the Z register is dedicated
\ as a loop counter, and must be preserved.
-: c@p+ ( - c)  dup  lpm,  0 T' ldi,  0 T mov,  1 Z adiw,  ;
-: c@p ( - c)  c@p+  1 Z sbiw,   ;
-: @p+ ( - n)  dup  lpm,  1 Z adiw,  0 T mov,
   lpm,  1 Z adiw,  0 T' mov,  ;
:m p! ( a)  T Z movw,  drop  m;
:m p ( - a)  ?dup  Z T movw,  m;
:m #p! ( adr)  [ dup $ff and ] Z ldi,
   [ 8 rshift $ff and ] Z' ldi,  m;

target
here constant dict  \ patch location of dictionary later
-: dictionary  0 #p! ;
-: .word ( c)   zpush c@p+ begin c@p+ emit 1 #- while repeat
   drop space zpop c@p+ p + dup 1 # and + 2 # + p! ;
: words  dictionary begin  c@p while
   drop cr .word key 13 # = if  drop ; then  drop repeat  drop ;
-: word  tib # a! c@p 1 #+
   begin c@p+ c@+ xor if nip ; then drop 1 #- while repeat ;
-: find  zpush c@p if  drop word if
   drop zpop c@p+ p + dup 1 # and + 2 # + p! find ; then
   drop zpop c@p+ p + dup 1 # and + p! @p+ ; then
   zpop drop 0 # ;
: execute ( a)  T push,  T' push,  drop ;
-: digit? ( c - c' flag)  [ char 0 ] # - -if  -1 # ; then
   10 # - -if  39 # + then  29 # -
   apush  base @ - -if  base @ + 0 # apop ; then  -1 # apop ;
-: +number?  tib # a! 0 # c@+  \ fall through
-: -number?
   for  apush base @ * apop c@+ digit? if
   push push push drop 0 # pop pop pop then  drop + next  swap ;
-: number?  -1 # [ tib 1 + ] # c@ 45 # xor if  drop +number? ; then
   tib # a! c@+ 1 #- c@+ drop -number? push negate pop ;
-: interpret  apush  dictionary find if  apop execute ; then
   drop number? if  drop apop ; then  huh?
-: query  0 # tib # !   \ fall through 
-: back  key dup 8 # = if  2drop cr query ; then
   drop BL # max echo BL # xor if  BL # xor tib! back ; then
   drop  apush tib # c@ if  drop apop ; then  drop apop ok query ;
: abort  ( *) resolve cr init-stacks
: quit  query interpret ?stack ok quit ;

-: first ( - addr)  variable # ;
-: sign -if  negate 45 # emit ; then  ;
-: ?digit ( n - n)  if  dup first ! digit ; then
   first @ if  over digit then  2drop ; 
: (u.) ( u)  apush 0 # first ! -1 # swap
   begin  base @ u/mod while repeat  drop
   begin  digit -until  drop ( space) apop ;
: u. ( u)  (u.) space ;
: .f ( f)  apush  sign 10000 # *.
   10000 # u/mod digit 46 # emit 1000 # u/mod digit
   100 # u/mod digit 10 # u/mod digit digit space ;
\ . is unsigned for hex, signed otherwise
: . apush base @ apop $10 # xor if  drop sign dup then  drop u. ;
: .s  apush  [ s0 4 - ] # a! depth dup (h.) 62 # emit space
   if  dup 6 # min for  @+ .  4 Y sbiw,  next then  drop apop ;
: ?  @ . ;

