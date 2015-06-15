\ strings.fs

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

: string ( - a n)   pop 2* p! c@p+ p swap 2dup + dup 1 #, and + 2/ push ; 
:m s"  string 34 parse here there place here there
    [ c@ 1 + dup 1 and + ]  allot m; target
: ptype ( a n)  swap p! begin c@p+ emit 1 #- while repeat drop ;
:m ."  s" ptype m; target

