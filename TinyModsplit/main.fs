\ main.fs 

0 [if]
Copyright (C) 2016-2021 by Charles Shattuck.

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

include i2c.fs

host \ used to compile a table in flash containing bytes
: c!-t ( c a - )  there c! ;
:m c, ( c - )  HERE c!-t 1 ALLOT m;
target

: c? ( a - )  c@ . ;
: lshift ( n1 n2 - n3)  for 2* next ;
: ud.. ( d - )
    <# # # # #  char . #, hold  # # # # #> type space ;
: ms ( n - )  for 4000 #, for next next ;
: or.c! ( c a - ) swap over c@ or swap c! ;

: i2c! ( c - )  i2c.c! drop ;

\ pe for port expander
: init-pe ( a - )
    i2c.address.write drop
    $0c #, i2c!  $ff #, i2c!  $ff #, i2c! i2c.stop ;
: init-keyboard \ i2c and two port expanders
    i2c.init  $20 #, init-pe  $21 #, init-pe ;

: pe@ ( a - c1 c2) \ read both bytes from the port expander
    dup i2c.address.write drop $12 #, i2c!
    i2c.address.read drop i2c.c@.ack i2c.c@.nack i2c.stop ;

: keys (  - d) \ read four bytes in all, make them a double
    $21 #, pe@ 8 #, lshift or invert
    $20 #, pe@ 8 #, lshift or invert ;

: accumulate ( 8d1 d2 - d3 f) \ or two double numbers
\ flag = false if d2 was zero
    2dup or push  push swap  push or  pop pop or  pop ;

: stroke (  - d) \ read until a stroke has been pressed and released
    begin  0 #, 0 #,
        begin  keys accumulate until/  20 #, ms
        keys accumulate 0= while/ 2drop repeat
    begin  keys accumulate 0= until/ ;

\ make a 6 byte array
cpuHERE constant 'data  6 cpuALLOT
: data ( i - a)  'data #, + ;
: clear-data  0 #, 6 #, for dup i data c! next
    drop $80 #, 0 #, data c! ; 

\ each entry corresponds to a bit in the 32 bit word
\ returned by stroke, contains a mask and a byte number
\ for the Gemini data array
HERE constant 'table [ hex ] 
\ left hand
    00 c, 0 c, \     8000
    20 c, 1 c, \ S2- 4000
    40 c, 1 c, \ S1- 2000
    08 c, 1 c, \ K-  1000
    10 c, 1 c, \ T-  0800
    02 c, 1 c, \ W-  0400
    04 c, 1 c, \ P-  0200
    00 c, 0 c, \     0100
    20 c, 2 c, \ A-  0080
    10 c, 2 c, \ O-  0040
    01 c, 0 c, \ #6  0020
    04 c, 2 c, \ *2  0010
    08 c, 2 c, \ *1  0008
    40 c, 2 c, \ R-  0004
    01 c, 1 c, \ H-  0002
    00 c, 0 c, \     0001 
\ right hand
    20 c, 3 c, \ *3
    01 c, 3 c, \ -R
    02 c, 3 c, \ -F
    20 c, 4 c, \ -B
    40 c, 4 c, \ -P
    08 c, 4 c, \ -G
    10 c, 4 c, \ -L
    00 c, 0 c, \ 
    04 c, 3 c, \ -U
    08 c, 3 c, \ -E
    40 c, 5 c, \ #7
    01 c, 5 c, \ -Z
    01 c, 4 c, \ -D
    02 c, 4 c, \ -S
    04 c, 4 c, \ -T
    10 c, 3 c, \ *4
[ decimal ]
\ i = bit position, c1 c2 are mask and byte number
: table@ ( i - c1 c2)  zpush 2* 'table #, + p! c@p+ c@p+  zpop ;

\ n is a 16 bit part of the 32 bit stroke, i = position in table
: place-bit ( n i - n*2)
    swap -if  swap table@ data or.c! dup then  2* nip ;

\ translate stroke into the Gemini data array
: arrange ( d - )
    16 ##for  15 #, i - place-bit next  drop
    16 ##for  31 #, i - place-bit next  drop ;

\ send a Gemini packet through the serial port
: report  6 ##for  5 #, i - data c@ emit next ;

\ main program loop
: go  init-keyboard
    begin  clear-data stroke arrange report again

\ testing
: .table  32 ##for cr 31 #, i - table@ . . next ;
: show  cr 6 ##for  5 #, i - data c? next ;
: stroke-test  init-keyboard stroke ud.. cr ;
: tester  init-keyboard
    begin  cr stroke ud.. again
: .stroke  hex cr clear-data stroke 2dup ud.. arrange show ;
: stroker  init-keyboard begin .stroke again
: TWCR@  TWCR #, c@ ;
: .TWCR  TWCR@ . ;

: 5us ( n - )  zpush 11 ##for  next zpop nop nop ;
: hi  13 high, ;
: lo  13 low, ;
: init  13 output, ;
: test  init begin hi 5us lo 5us again
: a  i2c.init i2c.ping? . ;
: aa  i2c.init begin  $20 #, i2c.ping? drop 
    $21 #, i2c.ping? drop 2 #, ms again
: aaa  i2c.init
   begin  $21 #, pe@ 2drop 2 #, ms again 

