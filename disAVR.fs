\ disAVR.fs  Partial AVR disassembler

0 [if]
Copyright (C) 2013 by Charles Shattuck.

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

only forth also definitions

variable inst  \ current instruction word

: .xxxx ( adr)
   base @ >r hex 0 <# # # # # #> type space r> base ! ;

: .label ( a)  dup 2* label? dup if
   >red show >black drop exit then drop . ;

: .label; ( a)  .label >red ."  ;" >black ;

: .bad  ." -----" cr ;

\ gforth already has these words.
\ : dec. ( n)  base @ >r decimal . r> base ! ;
\ : hex. ( n)  [char] $ emit base @ >r hex . r> base ! ;

create 'regs  char N c, char T c, char X c, char Y c, char Z c,
: .reg ( n)  dup 22 < if  dec. exit  then
   -22 over + 2/ 'regs + c@ emit  1 and if  [char] ' emit  then
   space ;

: .io ( n)
   dup  3 = if  ." PINB "  drop exit  then
   dup  4 = if  ." DDRB "  drop exit  then
   dup  5 = if  ." PORTB " drop exit  then
   dup  6 = if  ." PINC "  drop exit  then
   dup  7 = if  ." DDRC "  drop exit  then
   dup  8 = if  ." PORTC " drop exit  then
   dup  9 = if  ." PIND "  drop exit  then
   dup 10 = if  ." DDRD "  drop exit  then
   dup 11 = if  ." PORTD " drop exit  then
   base @ >r hex . r> base ! ;

: .Rd  inst @ 4 rshift $1f and .reg ;
: .RrRd  inst @ dup $0f and swap $200 and 5 rshift or .reg .Rd ;
: .rel  inst @ 3 rshift $3f and dup $20 and if
   $3f invert or then dec. ;
: .long ( a1 - a2)  2 + dup @-t cr 11 spaces .xxxx ;
: .iw  inst @ dup $0f and swap 2 rshift $30 and or dec.
   inst @ 3 rshift $1e and 24 + .reg ;
: .bit  inst @ 7 and dec. ;
: .bitIO  .bit  inst @ 3 rshift $1f and .io ;

: match ( opcode mask - flag)  inst @ and = ;
: exact ( opcode - flag)  >black inst @ = ;

: .x0
   inst @ 0= if  ." nop," cr exit  then
   $0400 $0c00 match if  .RrRd ." cpc," cr exit  then
   $0800 $0c00 match if  .RrRd ." sbc," cr exit  then
   $0c00 $0c00 match if  .RrRd ." add,"
      dup $0f86 exact if  ."  (+"  then 
      $0f88 exact if  ."  (2*"  then  cr drop exit  then
   $0100 $ff00 match if
      inst @ dup $0f and 2* .reg 3 rshift $1e and .reg
      ." movw," cr exit  then
   .bad ;

: .x1
   $1c00 $1c00 match if  .RrRd ." adc," cr exit  then
   $1800 $1c00 match if  .RrRd ." sub," cr exit  then
   $1400 $1c00 match if  .RrRd ." cp," cr exit  then
   .bad ;

: .x2
   $2c00 $2c00 match if  .RrRd ." mov," cr exit  then
   $2000 $2c00 match if  .RrRd ." and,"
      $2386 exact if  ."  (and"  then
      $2399 exact if  ."  (-if"  then
      cr exit  then
   $2400 $2c00 match if  .RrRd ." xor,"
      $2786 exact if  ."  (xor"  then  cr exit  then
   $2800 $2c00 match if  .RrRd ." or,"
      $2b86 exact if  ."  (or"  then  cr exit  then
   .bad ;

: .x3 .bad ;
: .x4 .bad ;
: .x5 .bad ;
: .x6 .bad ;
: .x7 .bad ;

: .x8
   $8000 $fe0f match if  .Rd ." ldz," cr exit  then
   $8008 $fe0f match if  .Rd ." ldy," cr exit  then
   $8200 $fe06 match if  .Rd ." stz," cr exit  then
   $8208 $fe06 match if  .Rd ." sty," cr exit  then
   .bad ;

: .x9
   $9001 $9e0f match if  .Rd ." ldz+," cr exit  then
   $9002 $9e0f match if  .Rd ." -ldz," cr exit  then
   $9009 $9e0f match if  .Rd ." ldy+,"
      $9199 exact if  ."  @+)"  then  cr exit  then
   $900a $9e0f match if  .Rd ." -ldy," cr exit  then
   $900c $9e0f match if  .Rd ." ldx,"  cr exit  then
   $900d $9e0f match if  .Rd ." ldx+,"
      $918d exact if  ."  (drop"  then
      $916d exact if  ."  (nip"  then  cr exit  then
   $900e $9e0f match if  .Rd ." -ldx," cr exit  then
   $9201 $9e0f match if  .Rd ." stz+," cr exit  then
   $9202 $9e0f match if  .Rd ." -stz," cr exit  then
   $9209 $9e0f match if  .Rd ." sty+," cr exit  then
   $920a $9e0f match if  .Rd ." -sty," cr exit  then
   $920d $9e0f match if  .Rd ." stx+," cr exit  then
   $920e $9e0f match if  .Rd ." -stx,"
      $939e exact if  ."  (dup"  then  cr exit  then
   $9508 inst @ = if  ." ret, " >black ." ;" cr exit  then
   $940e inst @ = if  ." call," >yellow .long cr exit  then
   $940c inst @ = if  ." jump," >yellow .long cr exit  then
   $9400 $fe0f match if  .Rd ." com,"
      $9580 exact if  ."  (invert"  then  cr exit  then
   $9403 $fe0f match if  .Rd ." inc,"  cr exit  then
   $9407 $fe0f match if  .Rd ." ror," cr exit  then
   $940a $fe0f match if  .Rd ." dec,"  cr exit  then
   $900f $fe0f match if  .Rd ." pop,"
      $918f exact if  ."  (pop"  then  cr exit  then
   $920f $fe0f match if  .Rd ."  push,"
      $93ff exact if  ."  (for"  then
      $939f exact if  ."  (push"  then
      cr exit  then
   $9600 $ff00 match if  .iw ." adiw,"
      $9600 exact if  ."  (if"  then  cr exit  then
   $9700 $ff00 match if  .iw ." sbiw,"
      $9731 exact if  ."  (next"  then  cr exit  then
   $9a00 $ff00 match if  .bitIO ." sbi,"  cr exit  then
   $9800 $ff00 match if  .bitIO ." cbi,"  cr exit  then
   $9900 $ff00 match if  .bitIO ." sbic," cr exit  then
   $9b00 $ff00 match if  .bitIO ." sbis," cr exit  then
   $9c00 $fc00 match if  .RrRd ." mul," cr exit  then
   $94f8 inst @ = if  ." cli,"  cr exit  then
   $9478 inst @ = if  ." sei,"  cr exit  then
   $95c8 inst @ = if  ." lpm,"  cr exit  then
   $9518 inst @ = if  ." reti," cr exit  then
   .bad ;

: .xa .bad ;

: .xb
   $b000 $f800 match if
      inst @ dup $0f and swap 5 rshift $30 and or .io  .Rd
      ." in," >black ."  in)" cr exit  then
   $b800 $f800 match if
      .Rd  inst @ dup $0f and swap 5 rshift $30 and or .io
      ." out," >black ."  (out" cr exit  then
   .bad ;

: .xc  inst @ $0fff and dup $0800 and if  $fffff000 or then
   dup .  ." rjmp, "  >black over 2/ + 1 +
   base @ >r hex ( .) .label;  r> base ! cr ;
: .xd  inst @ $0fff and dup $0800 and if  $fffff000 or then
   dup .  ." rcall, "  >black over 2/ + 1 +
   base @ >r hex ( .) .label  r> base ! cr ;

: (#) ( op - n)  dup $0f and swap 4 rshift $f0 and or ;

: .xe
   $e000 $f000 match if  inst @ (#) hex.
      inst @ 4 rshift $0f and 16 + .reg ." ldi,"
      $e080 $f0f0 match if  >black ."  (# " 
      inst @ (#) over 2 + @-t (#) 8 lshift or dec. then
      cr exit  then
   .bad ;

: .xf
   $f800 $fe00 match if  .bit .Rd ." bld,"  cr exit then
   $fa00 $fe00 match if  .bit .Rd ." bst,"  cr exit then  
   $fc00 $fe00 match if  .bit .Rd ." sbrc,"
      $fd97 exact if  ."  -if"  then  cr exit  then
   $fe00 $fe00 match if  .bit .RD ." sbrs," cr exit  then
   $f401 $fc07 match if  .rel ." brne,"  cr exit  then
   $f001 $fc07 match if  .rel ." breq,"  cr exit  then
   $f402 $fc07 match if  .rel ." brpl,"  cr exit  then
   $f000 $fc07 match if  .rel ." brcs,"  cr exit  then
   $f002 $fc07 match if  .rel ." brmi,"  cr exit  then
   .bad ;

: .dis ( instruction)
   dup inst !  12 rshift 
   dup $0 = if  drop .x0 exit  then
   dup $1 = if  drop .x1 exit  then
   dup $2 = if  drop .x2 exit  then
   dup $3 = if  drop .x3 exit  then
   dup $4 = if  drop .x4 exit  then
   dup $5 = if  drop .x5 exit  then
   dup $6 = if  drop .x6 exit  then
   dup $7 = if  drop .x7 exit  then
   dup $8 = if  drop .x8 exit  then
   dup $9 = if  drop .x9 exit  then
   dup $a = if  drop .xa exit  then
   dup $b = if  drop .xb exit  then
   dup $c = if  drop .xc exit  then
   dup $d = if  drop .xd exit  then
   dup $e = if  drop .xe exit  then
   dup $f = if  drop .xf exit  then
   drop ;


nowarn
: show-name ( a)  >red label? dup if  5 spaces dup show cr  then
   drop >black ;

: decode ( adr)  cr 2*
   begin  dup show-name
      dup >yellow 2/ .xxxx dup @-t dup .xxxx space >green .dis
      key 13 - while  2 +  repeat  >black drop ;

: see  also targ ' >body @ 2/ decode previous ;
warn
