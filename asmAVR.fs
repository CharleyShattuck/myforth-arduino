\ asmAVR.fs

0 [if]
Copyright (C) 2013-2015 by Charles Shattuck.

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

:m interrupt ( adr)  2* 2 + target here host 2/ swap !-t  m;
:m reti,  $9518 ,-t m;

:m rel ( adr - n)  here - 2/ 1 -  m;

:m rcall, ( a)  rel $0fff and $d000 or ,-t  m;
:m lcall, ( a)  $940e ,-t  2/ ,-t  m;
:m call, ( a)  hint dup here - abs $1fff < if
   rcall, exit then  lcall, ;

:m rjmp, ( a)  rel $0fff and $c000 or ,-t  m;
:m ljmp, ( a)  $940c ,-t  2/ ,-t  m;
\ :m jump, ( a)  ljmp,  m;
:m jump, ( a)  hint dup here - abs $1fff < if
    rjmp, exit then  ljmp, ;
:m again, ( a)  jump,  m;

:m entry  here dup 0 org ljmp, org  m;

:m -: host >in @ label >in ! create
   target here host , hide postpone target
   does> @ ( talks @ if talk exit then)  target call, m;

:m : -: header  m;

:m begin,  here  m;

\ tail recursion optimized
:m exit
   edge here 4 - = if  \ lcall to ljmp
      edge @-t $940e = if  $940c edge !-t exit  then
   then
   edge here 2 - = if  \ rcall to rjmp
      edge @-t $f000 and $d000 = if
         edge @-t $1000 xor edge !-t exit  then
   then
   $9508 ,-t  m;  \ ret
:m ; exit m;

\ ----- Assembler ----- /
host \ These are 'assembler', not 'target forth', use host version of :

: ldst ( reg opcode) swap 4 lshift or ,-t  m;

: ldx,  ( reg) $900c ldst  m;
: ldx+, ( reg) $900d ldst  m;
: -ldx, ( reg) $900e ldst  m;

: ldy,  ( reg) $8008 ldst  m;
: ldy+, ( reg) $9009 ldst  m;
: -ldy, ( reg) $900a ldst  m;

: ldz,  ( reg) $8000 ldst  m;
: ldz+, ( reg) $9001 ldst  m;
: -ldz, ( reg) $9002 ldst  m;

: stx,  ( reg) $920c ldst  m;
: stx+, ( reg) $920d ldst  m;
: -stx, ( reg) $920e ldst  m;

: sty,  ( reg) $8208 ldst  m;
: sty+, ( reg) $9209 ldst  m;
: -sty, ( reg) $920a ldst  m;

: stz,  ( reg) $8200 ldst  m;
: stz+, ( reg) $9201 ldst  m;
: -stz, ( reg) $9202 ldst  m;

: lds, ( adr reg)  2* 2* 2* 2* $9000 or ,-t  ,-t  m;
: sts, ( reg adr)  swap 2* 2* 2* 2* $9200 or ,-t  ,-t  m;

: pack-RdRr ( src dest)
   4 lshift over $000f and or swap $0010 and 5 lshift or  m;

: RdRr ( op src dest)  >r pack-RdRr r> or ,-t  m;

: movw, ( src dest) swap 2/ $0f and
   swap 3 lshift $f0 and or $0100 or ,-t  m;
: mov, ( src dest) $2c00 RdRr  m;
: add, ( src dest) $0c00 RdRr  m;
: adc, ( src dest) $1c00 RdRr  m;
: sub, ( src dest) $1800 RdRr  m;
: sbc, ( src dest) $0800 RdRr  m;
: and, ( src dest) $2000 RdRr  m;
: xor, ( src dest) $2400 RdRr  m;
: or,  ( src dest) $2800 RdRr  m;
: mul, ( src dest) $9c00 RdRr  m;
: cp,  ( src dest) $1400 RdRr  m;
: cpc, ( src dest) $0400 RdRr  m;
: com, ( reg) 0 swap $9400 RdRr  m; 

: imm ( n reg opcode)  >r 4 lshift $f0 and
   swap dup $0f and swap 4 lshift $f00 and or or r> or ,-t  m;
: ldi, ( n reg)  $e000 imm m;
: ori, ( n reg)  $6000 imm m;
: andi, ( n reg)  $7000 imm m;
: subi, ( n reg)  $5000 imm m;
: sbci, ( n reg)  $4000 imm m;

: in/out ( port opcode reg)  4 lshift $1f0 and or
    swap $20 -  dup 0 $60 within 0= if  abort" port out of range" then
    dup $0f and swap 5 lshift $600 and or or ,-t  m;
: in, ( port reg)  $b000 swap in/out  m;
: out, ( reg port)  swap $b800 swap in/out  m;

: ?io-reg ( n - n)  dup 0 $21 within 0= abort" not an io register" ;
: io-bit ( bit reg - n)  $20 - ?io-reg 3 lshift or ;

: sbi, ( bit reg) io-bit  $9a00 or ,-t  m;
: cbi, ( bit reg) io-bit  $9800 or ,-t  m;

: clc,  $9488 ,-t  m;
: sec,  $9468 ,-t  m;
: set,  $9468 ,-t  m;  \ set T flag
: clt,  $94e8 ,-t  m;  \ clr T flag
: swap,  4 lshift $01f0 and $9402 or ,-t  m;
: ror, ( reg) 4 lshift $01f0 and $9407 or ,-t  m;
: bld, ( bit reg) 4 lshift $01f0 and $f800 or or ,-t  m;
: bst, ( bit reg) 4 lshift $01f0 and $fa00 or or ,-t  m;

: brne, ( rel) 3 lshift $f401 or ,-t  m;
: 0until,  ( a)  rel $7f and brne,  m;
: 0if,  ( - a)  begin, dup rel $7f and brne,  m;
: breq, ( rel) 3 lshift $f001 or ,-t  m;
: until, ( a)  rel $7f and breq,  m;
: if, ( - a)  begin, dup rel $7f and breq,  m;
: brcs, ( rel) 3 lshift $f000 or ,-t  m;
: brlo, ( rel)  brcs,  m;
: brcc, ( rel) 3 lshift $f400 or ,-t  m;
: brsh, ( rel)  brcc,  m;
: brpl, ( rel) 3 lshift $f402 or ,-t  m;
: -until, ( a)  rel $7f and brpl,  m;
: -if, ( - a)  begin, dup rel $7f and brpl,  m;
: brmi, ( rel) 3 lshift $f002 or ,-t  m;
: then, ( a)  begin, >r dup org r@  rel $7f and
    3 lshift over @-t $fc07 and or swap !-t  r> org  m;
: tif, ( - a)  begin, [ dup rel $7f and ]  brtc,  m;

: brvc, ( rel) 3 lshift $f403 or ,-t  m;
: brvs, ( rel) 3 lshift $f003 or ,-t  m;
: brge, ( rel) 3 lshift $f404 or ,-t  m;
: brlt, ( rel) 3 lshift $f004 or ,-t  m;
: brhc, ( rel) 3 lshift $f405 or ,-t  m;
: brhs, ( rel) 3 lshift $f005 or ,-t  m;
: brtc, ( rel) 3 lshift $f406 or ,-t  m;
: brts, ( rel) 3 lshift $f006 or ,-t  m;
: brid, ( rel) 3 lshift $f407 or ,-t  m;
: brie, ( rel) 3 lshift $f007 or ,-t  m;

: sbic, ( bit reg) io-bit  $9900 or ,-t  m;
: sbis, ( bit reg) io-bit  $9b00 or ,-t  m;
: sbrc, ( bit reg) 4 lshift or $fc00 or ,-t  m;
: sbrs, ( bit reg) 4 lshift or $fe00 or ,-t  m;

: push, ( reg) 4 lshift $1f0 and $920f or ,-t  m;
: pop, ( reg) 4 lshift $1f0 and $900f or ,-t  m;

: xxiw ( n reg opcode)  >r 24 - 3 lshift  over $0f and or
   swap $30 and 2 lshift or  r> or ,-t  m;
: adiw, ( n reg) $9600 xxiw  m;
: sbiw, ( n reg) $9700 xxiw  m;

: cli,  $94f8 ,-t  m;
: sei,  $9478 ,-t  m;

: ijmp,  $9409 ,-t  m;
: icall,  $9509 ,-t  m;

: lpm,  $95c8 ,-t  m;
: nop,  0 ,-t  m;

: inc, ( reg)  4 lshift $9403 or ,-t  m;
: dec, ( reg)  4 lshift $940a or ,-t  m;

: high, ( bit)  PORT sbi, m;
: low, ( bit)  PORT cbi, m;
: input, ( bit)  DDR cbi, m;
: output, ( bit)  DDR sbi, m;
: toggle, ( bit)  PIN sbi, m;

\ If not true then skip next instruction word.
\ e.g.  begin  12 set? again
\ loops as long as pin 12 is set.
: set? ( bit)  PIN sbic, m;
: clr? ( bit)  PIN sbis, m;

\ Watchdog reset
: wdr,  $95a8 ,-t ;

