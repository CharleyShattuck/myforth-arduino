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

: dump ( a - a')  2* p!  base @ hex
   p 2/ h. [ char : ] #, emit space
   8 #, begin  @p+ h.  1 #- while repeat drop
   base !  p 2/ ;

: ms ( n)  for  4000 ##for next next ;

include SPI.fs

: test
   init_SPI  0 #,
   begin  1 #+  dup SPI drop
      100 #, ms  key? 0= while/ repeat
   drop ;

