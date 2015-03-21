\ interactive.fs

:m |dup  dup m;
:m |drop  drop m;
:m |nip  nip m;
:m |invert  invert m;
\ :m |negate  negate m;
:m |2*  2* m;
\ :m |2/  2/ m;

\ :m |@  @ m;
:m |a  a m;
:m |a!  a! m;
:m |@+  @+ m;
:m |!+  !+ m;
:m |c@+  c@+ m;
:m |c!+  c!+ m;

target
: dup  |dup ;
: drop  |drop ;
: nip  |nip ;
: invert  |invert ;
\ : negate  |negate ;
: 2*  |2* ;
\ : 2/  |2/ ;

\ : @  |@ ;
: a  |a ;
: a!  |a! ;
: @+  |@+ ;
: !+  |!+ ;
: c@+  |c@+ ;
: c!+  |c!+ ;

