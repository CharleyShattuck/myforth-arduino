\ fraction.fs  14 bit fractions on the host

$4000 constant +1
: *. ( n n - n)  +1 */ ;
: /. ( n n - n)  +1 swap */ ;
: >f ( n - f)  10000 /. ;
: #.####  dup abs <# # # # # 46 hold # sign #> type space ;
: .f ( f)  10000 *. #.#### ;

