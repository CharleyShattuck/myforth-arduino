\ main.fs
include ../strings.fs

: greet  s" This is a string!" ptype cr ;
: this  ." This was also a string!" space .s ;
: test 10 ##for r@ . i . this cr next ;

