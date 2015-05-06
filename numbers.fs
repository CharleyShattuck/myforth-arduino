\ numbers.fs

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

: holder ( - adr)  variable #, ;
: hold ( c)  holder @ 1 #- dup holder ! c! ;
: sign ( n)  -if char - #, hold then drop ;

: <# ( ud - ud)  pad #, holder ! ;
: #> ( ud - adr len) 2drop holder @ pad #, over - ;

: # ( ud1 - ud2)  base @ ud/mod rot 9 #, over - -if
    drop 7 #, + dup then  drop 48 #, + hold ;
: #s ( ud - 0 0)  begin # over over or while drop repeat drop ;

: h. ( u)  base @ push hex 0 #, <# # # # # #>
    type space pop base ! ;
: ud. ( ud)  <# #s #> type space ;
: u. ( u)  0 #, ud. ;

: d. ( d)  dup push dabs <# #s pop sign #> type space ;
: 0< ( n - flag)  -if drop -1 #, ; then drop 0 #, ;
: . ( n)  dup 0< d. ;

: .f ( f)  10000 #, *. dup push abs 0 #,
    <# # # # # char . #, hold # pop sign #> type space ;

