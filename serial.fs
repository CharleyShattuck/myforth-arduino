\ serial.fs

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

cvariable last
: 2dup ( n1 n2 - n1 n2 n1 n2)  over over ;
: min  2dup swap
-: clip  - -if  push swap pop then  \ falls through into 2drop
: 2drop ( n1 n2)  drop drop ;
: 0max  0 #,  \ falls through into max
: max  2dup clip ;
-: echo  dup  \ falls through into emit
: emit ( c)
   apush  0 #,
   begin  drop UCSR0A #, c@  $20 #, and until
   drop UDR0 #, c!  apop ;
: key ( - c)
   apush  0 #,
   begin  drop UCSR0A #, c@ $80 #, and until
   drop UDR0 #, c@  dup last c! apop ;
: type ( adr len)  0max if  apush
      swap a! for  c@+ emit next  apop ; 
   then  2drop ;
: space  32 #, emit ;
: cr  13 #, emit 10 #, emit ;

