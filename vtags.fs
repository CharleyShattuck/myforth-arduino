\ Vtags support for GNU Forth.

\ This file is an slight modification of etags.fs, which is part of Gforth.
\ The following copyright notice was found in etags.fs and is reproduced here.
\ References to emacs should be mentally translated into references to vi,
\ as this modification allows tag files to be created in vi format.

\ ----- Begin copyright notice for etags.fs, part of Gforth. -----

\ Copyright (C) 1995,1998 Free Software Foundation, Inc.

\ This file is part of Gforth.

\ Gforth is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public License
\ as published by the Free Software Foundation; either version 2
\ of the License, or (at your option) any later version.

\ This program is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\ GNU General Public License for more details.

\ You should have received a copy of the GNU General Public License
\ along with this program; if not, write to the Free Software
\ Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.


\ This does not work like etags; instead, the TAGS file is updated
\ during the normal Forth interpretation/compilation process.

\ The present version has several shortcomings: It always overwrites
\ the TAGS file instead of just the parts corresponding to the loaded
\ files, but you can have several tag tables in emacs. Every load
\ creates a new etags file and the user has to confirm that she wants
\ to use it.

\ Communication of interactive programs like emacs and Forth over
\ files is clumsy. There should be better cooperation between them
\ (e.g. via shared memory)

\ This is ANS Forth with the following serious environmental
\ dependences: the variable LAST must contain a pointer to the last
\ header, NAME>STRING must convert that pointer to a string, and
\ HEADER must be a deferred word that is called to create the name.

\ ----- End copyright notice for etags.fs, part of Gforth. -----

\ require search.fs
\ require extend.fs

: tags-file-name ( -- c-addr u )
    \ for now use just tags; this may become more flexible in the
    \ future
    s" tags" ;

variable tags-file 0 tags-file !

create tags-line 128 chars allot
    
: tags-file-id ( -- file-id )
    tags-file @ 0= if
		tags-file-name w/o create-file throw
		tags-file !
    endif
    tags-file @ ;

create last-loadfilename 128 allot
variable last-sourceline

: put-load-file-name ( file-id -- )
    >r
    sourcefilename last-loadfilename place
    sourceline# last-sourceline !
    sourcefilename
    r@ write-file throw
    rdrop ;

: put-tags-entry ( -- )
    \ write the entry for the last name to the TAGS file
    \ if the input is from a file and it is not a local name
    source-id dup 0<> swap -1 <> and	\ input from a file
    current @ locals-list <> and	\ not a local name
    last @ 0<> and	\ not an anonymous (i.e. noname) header
    if
	tags-file-id >r 
	last @ name>string r@ write-file throw
	#tab r@ emit-file throw
	r@ put-load-file-name
	#tab r@ emit-file throw
	[char] + r@ emit-file throw
	base @ decimal   sourceline# 0 <# #s #> r@
	   write-line throw   base !
	rdrop
    endif ;

: (no-tags-header)  (  - )
	defers header ;

: (tags-header) ( -- )
    defers header
    put-tags-entry ;

\ ' (tags-header) IS header

: use-tags  (  - ) ['] (tags-header) IS header ;
: no-tags  (  - ) ['] (no-tags-header) IS header ;

\ Shell out to vi to edit the named file.
: vi   (  - )
	tags-file-id flush-file throw
	s" vi " pad place
	#lf parse pad +place 
	pad count system ;

: edit   vi ;

\ Edit the word which follows in the input stream.
: e   (  - )
	tags-file-id flush-file throw
	s" vi -t " pad place
	#lf parse pad +place 
	pad count system ;

\ Go directly the latest error.
: fix   (  - )
	tags-file-id flush-file throw
	s" vi " pad place
	last-loadfilename count pad +place 
	s"  +" pad +place
	last-sourceline @ 0 <# #s #> pad +place
	pad count system ;

use-tags  \ Tag everything from now on.
