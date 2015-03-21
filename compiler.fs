\ Generic Target Compiler.

0 [if]
Copyright (C) 2009 by Charles Shattuck.

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
vocabulary targ

nowarn
: target only forth also targ also definitions ; immediate
: ] postpone target ; immediate
: host only targ also forth also definitions ; immediate
: [ postpone host ; immediate
host
warn

: :m postpone target : ;
: m; postpone ; postpone host ; immediate

\ 0 constant start  \ Reset vector.
\ 8192 constant target-size
create target-image target-size allot
target-image target-size $ff fill  \ ROM erased.
: there   ( a1 - a2)   target-image + ( start +) ;
: c!-t   ( c a - )   there c! ;
: c@-t   ( a - c)   there c@ ;
: !-t  ( n a - )   there over 8 rshift over 1 + c! c! ;
: @-t  ( a - n)  there count swap c@ 8 lshift + ;

variable tdp  \ Rom pointer.
:m HERE   (  - a)   tdp @ m;
:m ORG   ( a - )   tdp ! m;
:m ALLOT   ( n - )   tdp +! m;
:m ,   ( n - )   HERE !-t 2 ALLOT m;
: ,-t   ( n - )   target , m;

variable trp  \ Ram pointer.
: cpuHERE  (  - a)   trp @ ;
: cpuORG  ( a - )  trp ! ; 8 cpuORG
: cpuALLOT  ( n - )  trp +! ;
: report  cr ." HERE=" target HERE host u. cr ;

\ ----- Optimization ----- /
variable 'edge
: hide target-size 1 - 'edge ! ; hide
: hint target here host 'edge ! ;
: edge 'edge @ ;

\ ----- Labels ----- /
nowarn
variable labels 0 labels !
warn
: label  (  - )
	[ labels @ here labels ! , ] HERE host , BL word count here
	over char+ allot place align ;
: show  ( a - ) 2 cells + count type ;
: label?  ( a - 0|a)
	>r labels begin @ dup while  dup cell+ @ r@ = if
	r> drop exit  then  repeat  r> drop ;
nowarn
: (words  words ;
: .words  labels begin  @ dup while  dup cell+ @ u. dup show 2
   spaces  repeat  drop ;
: target-words
   labels begin  @ dup while  dup show space  repeat  drop ;
warn

\ ----- Headers on the target ----- /
variable thp
create target-heads target-size allot
create end-of-heads
: headsize end-of-heads thp @ - ;
target-heads target-size + 3 - thp !
0 value heads
nowarn
: header (  - )
   thp @ >r  labels @ cell+ dup cell+ dup c@ 3 + dup 1 and + negate thp +!
   thp @ over c@ 1 + dup 1 and + move  @ 2/ dup 8 rshift  r@ 1 - c!  r> 2 - c! ;
warn
\ Tack headers onto end of code.
: headers  (  - )
	target-size target here host headsize + - 0<
	abort" Target memory overflow"
	thp @ target here host dup to heads there headsize move
	headsize tdp +! ;

0 value save-fid
: save  (  - )
	0 to save-fid   s" chip.bin" delete-file drop
	s" chip.bin" r/w create-file abort" Error creating chip.bin"
	to save-fid target-image target-size
	save-fid write-file abort" Error writing chip.bin"
	save-fid close-file abort" Error closing chip.bin" ;

