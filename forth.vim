" Vim syntax file
" Language   : Forth
" Maintainer : Charley Shattuck
" Last change: 11May2010

" For version 5.x: Clear all syntax items
" For version 6.x: Quit when a syntax file was already loaded
if version < 600
	syntax clear
elseif exists("b:current_syntax")
	finish
endif

" Synchronization method
" syn sync ccomment maxlines=500
syntax sync fromstart

syntax case ignore

" Characters allowed in keywords
if version >= 600
	setlocal iskeyword=!,@,33-35,%,$,38-64,A-Z,91-96,a-z,123-126,128-255
else
	set iskeyword=!,@,33-35,%,$,38-64,A-Z,91-96,a-z,123-126,128-255
endif

" Text is yellow by default
syntax region Default start='^.*' end='%$' contains=Comment,Colon,Code,Variable

" Colon body is green
syntax region Colon start='\<:\>\s*' end='^$\|;$' contained contains=Head,Comment,Interpret,Branch
syntax region Colon start='\<:m\>\s*' end='^$\|;m$' contained contains=Head,Comment,Interpret,Branch
syntax region Colon start='\<-:\>\s*' end='^$\|;$' contained contains=Head,Comment,Interpret,Branch
syntax region Colon start='\<interrupt\>\s*' end='^$' contained contains=Head,Comment,Interpret,Branch,Definer

" Name is red
syntax match Head '\<:\>\s*[^ \t]\+\>' contained contains=Definer
syntax match Head '\<:m\>\s*[^ \t]\+\>' contained contains=Definer
syntax match Head '\<-:\>\s*[^ \t]\+\>' contained contains=Definer

" Comments are white
syntax region Comment start=/( / end=/)/ contained
syntax region Comment start=/\<{\>/ end=/}/ contained
syntax region Comment start=/\<0 \[if\]\>/ end=/\[then\]/ contained
syntax region Comment start=/\<\[ 0 \] \[if\]\>/ end=/\[then\]/ contained
syntax match Comment '\\\s.*$' contained
syntax match Comment '\\\\.*$' contained
syntax match Comment '\\s.*$' contained
syntax match Comment '\\G.*$' contained
syntax match Comment '\\$' contained

" variables and similar words are red
syntax match Variable '\s*variable\s*[^ \t]\+\>' contained contains=Definer
syntax match Variable '\s*cvariable\s*[^ \t]\+\>' contained contains=Definer
syntax match Variable '\s*2variable\s*[^ \t]\+\>' contained contains=Definer
syntax match Variable '\<equ\>\s*[^ \t]\+\>' contained contains=Definer
syntax match Variable '\<create\>\s*[^ \t]\+\>' contained contains=Definer
syntax match Variable '\s*constant\>\s*[^ \t]\+\>' contained contains=Definer
syntax match Variable '\s*2constant\>\s*[^ \t]\+\>' contained contains=Definer
syntax match Variable '\<defer\>\s*[^ \t]\+\>' contained contains=Definer

syntax match Definer '\s*variable\s*\>' contained
syntax match Definer '\s*cvariable\s*\>' contained
syntax match Definer '\s*2variable\s*\>' contained
syntax match Definer '\<equ\>\s*\>' contained
syntax match Definer '\<create\>\s*\>' contained
syntax match Definer '\s*constant\>\s*\>' contained
syntax match Definer '\s*2constant\>\s*\>' contained
syntax match Definer '\<defer\>\s*\>' contained
syntax match Definer '\<:\>\s*\>' contained
syntax match Definer '\<:m\>\s*\>' contained
syntax match Definer '\<-:\>\s*\>' contained
syntax match Definer '\<interrupt\>' contained

" Yellow within brackets inside a colon definition
syntax region Interpret start='\<\[\>'hs=e+1 end='\<]\>'he=s-1 contained

syntax match Branch "\<begin\>"
syntax match Branch "\<begin,\>"
syntax match Branch "\<again\>"
syntax match Branch "\<again,\>"
syntax match Branch "\<while\>"
syntax match Branch "\<-while\>"
syntax match Branch "\<until\>"
syntax match Branch "\<until,\>"
syntax match Branch "\<-until\>"
syntax match Branch "\<-until,\>"
syntax match Branch "\<0until,\>"
syntax match Branch "\<repeat\>"
syntax match Branch "\<if\>"
syntax match Branch "\<if,\>"
syntax match Branch "\<-if\>"
syntax match Branch "\<-if,\>"
syntax match Branch "\<0if,\>"
syntax match Branch "\<then\>"
syntax match Branch "\<then,\>"
syntax match Branch "\<ahead\>"
syntax match Branch "\<if/\>"
syntax match Branch "\<until/\>"
syntax match Branch "\<while/\>"
syntax match Branch "\<for\>"
syntax match Branch "\<##for\>"
syntax match Branch "\<next\>"
syntax match Branch "\<next,\>"

highlight Default ctermfg=Yellow guifg=Yellow
highlight Interpret ctermfg=Yellow guifg=Yellow
highlight Head ctermfg=Red guifg=Red
highlight Code ctermfg=Red guifg=Red
highlight Comment ctermfg=LightGray guifg=LightGray
highlight Colon ctermfg=Green guifg=Green
highlight Variable ctermfg=Red guifg=Red
highlight Definer ctermfg=Magenta guifg=Magenta
highlight Brack ctermfg=Magenta guifg=Magenta
highlight Branch ctermfg=Cyan guifg=Green

let b:current_syntax = "forth"

" vim:ts=4:sw=4:nocindent:smartindent:
