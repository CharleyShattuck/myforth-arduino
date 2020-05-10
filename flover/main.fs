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

: dump ( a - a')  p!  base @ hex
   8 #, begin  @p+ 0 #, <# # # # # #> type space  1 #- 
   while repeat drop  base !  p ;

: ms ( n)  for  4000 ##for next next ;

include SPI.fs

10 constant /SS
11 constant MOSI
10 constant MISO
13 constant CLK

: init-SPI
   /SS output,  /SS high,
   MOSI output,  MOSI high,
   MISO input,  MISO high,
   CLK output,  CLK high,
   $52 #, SPCR! ;

\ /SS has to be made an output every time?
: SPI-enable   /SS output, /SS low, ;
: SPI-disable  /SS output, /SS high, ;

: SPI! ( n)  SPI-enable  SPDR!
   begin  SPSR@ $80 #, and until/
   SPDR@ drop  SPI-disable ;

: test
   init-SPI  0 #,
   begin  1 #+  dup SPI!  \ SPDR!
      100 #, ms  key? 0= while/ repeat
   drop ;

