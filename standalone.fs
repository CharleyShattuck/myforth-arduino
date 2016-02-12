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

: =  -  \ falls through into 0=
: 0=  if -1 #, or then invert ;

-: ok  last c@ BL #, = if  drop ; then  drop
   [ char o ] #, emit  [ char k ] #, emit cr ;
-: tib! ( c)  tib #, dup c@ 1 #+ over c!  dup c@ + c! ;
-: huh?  [ char ? ] #, emit forward ( *)
-: ?stack  depth -if  huh? ; then  drop ;

\ Z is no longer used as a loop counter.
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
   drop space zpop c@p+ p + dup 1 #, and + 2 #, + p! ;
: words  dictionary begin  c@p while
   drop cr .word key 13 #, = if  drop ; then  drop repeat  drop ;
-: word  tib #, a! c@p 1 #+
   begin c@p+ c@+ xor if nip ; then drop 1 #- while repeat ;
-: find  zpush c@p if  drop word if
   drop zpop c@p+ p + dup 1 #, and + 2 #, + p! find ; then
   drop zpop c@p+ p + dup 1 #, and + p! @p+ ; then
   zpop drop 0 #, ;
: execute ( a)  T push,  T' push,  drop ;
-: digit? ( c - c' flag)  [ char 0 ] #, - -if  -1 #, ; then
   10 #, - -if  39 #, + then  29 #, -
   apush  base @ - -if  base @ + 0 #, apop ; then  -1 #, apop ;
-: +number?  tib #, a! 0 #, c@+  \ fall through
-: -number?
   for  apush base @ * apop c@+ digit? if
   push push push drop 0 #, pop pop pop then  drop + next  swap ;
-: number?  -1 #, [ tib 1 + ] #, c@ 45 #, xor if  drop +number? ; then
   tib #, a! c@+ 1 #- c@+ drop -number? push negate pop ;
-: interpret  apush  dictionary find if  apop execute ; then
   drop number? if  drop apop ; then  huh?
-: query  apush 0 #, tib #, !   \ fall through 
-: back
    key dup 8 #, = if
        drop tib #, c@ if
            1 #- tib #, c!  dup emit space dup emit dup
        then  2drop back ; 
    then  drop BL #, max echo BL #, xor if  BL #, xor tib! back ; then
    drop  tib #, c@ if  drop apop ; then  drop apop ok query ;
: abort  ( *) resolve cr init-stacks
: quit  query interpret ?stack ok quit ;

