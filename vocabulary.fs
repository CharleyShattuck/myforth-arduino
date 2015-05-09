\ Necessary Words

\ defining new words
:   \ Defines the following word. Compiles a subroutine call later.
-:  \ Defines a new word, but leaves name out of the target.
;   \ Exit the subroutine.

\ control structures (looping)
begin  again    \ Endless loop
begin  until    \ Loop until top of stack is not zero
begin  -until   \ Loop until top of stack is negative
if  then        \ If top of stack is not zero
-if  then       \ If top of stack is negative
begin  while  repeat   \ Repeat while top of stack is not zero
begin  -while  repeat  \ Repeat while top of stack is negative
for  i next     \ Countdown from top of stack to zero

\ stack operators
dup      \ copy top of stack
drop     \ discard top of stack
nip      \ discard second on stack
swap     \ swap top of stack with second on stack
over     \ copy second on stack to top
r@       \ copy top of return stack to top of stack
push     \ push top of data stack to return stack
pop      \ pop return stack onto data stack
rot      \ rotate third item to top of stack
apush    \ hand optimized pushes
apop     \ and pops
zpush
zpop

\ binary operators
+        \ add top two numbers
+'       \ add with carry
and      \ and bits of top two numbers
xor      \ exclusive or bits of top two numbers
or       \ inclusive or bits of top two numbers
-        \ subtract top of stack from second on stack
min      \ discard the larger of two numbers
max      \ discard the lesser of two numbers

\ unary operators
invert   \ invert all the bits of top of stack
abs      \ absolute value of top of stack
negate   \ change sign of top of stack
2*       \ left shift, multiply by two
2/       \ right shift, divide by two

\ memory operators
@        \ 16 bit word fetch
!        \ 16 bit word store
c@       \ 8 bit character fetch
c!       \ 8 bit character store
a        \ fetch address register to stack
a!       \ store stack in address register
@+       \ fetch via and increment address register
c@+      \ byte fetch and increment
!+       \ store via and increment address register
c!+      \ byte store and increment

#,       \ put a literal on the stack
~#       \ put inverted literal on stack
#+       \ add literal, hand optimized
#-       \ subtract literal, hand optimized

\ multiplication and division
um*      \ unsigned mixed multiply
um/mod   \ unsigned mixed division
u/mod    \ unsigned integer division
*        \ signed integer multiply
m*       \ signed mixed multiply
sm/rem   \ symmetric division
/mod     \ signed division with remainder
mod      \ mod or remainder
/        \ divide
*/       \ star slash, multiply then divide
+1       \ put a fractional one on the stack
*.       \ 14 bit fractional multiply
/.       \ fractional division

\ radix (base) and variables
variable ( - adr)   \ name an address in RAM, allot word
cvariable ( - adr)  \ name an address in RAM, allot byte
hex                 \ change radix to hexadecimal
decimal             \ change radix to decimal
: base  variable # ;  \ an example of a variable

\ numeric display (serial terminal)
u.        \ display unsigned number
.f        \ display fraction
.         \ display signed number
.s        \ display the whole stack
?         \ fetch then display (@ .)
>f        \ change an integer to a fraction

\ character display (serial terminal)
emit      \ print a character on the serial terminal
key       \ get a character from the serial terminal
space     \ print a space to the serial terminal
cr        \ print carriage return and line feed
words     \ show words in the target dictionary

\ I/O pins
\ Assembler words, n is *not* a literal
input, ( n)    \ configure pin n to an input
output, ( n)   \ configure pin n to an output
high, ( n)     \ take pin n high (5 volts)
low, ( n)      \ take pin n low (ground)
toggle, ( n)   \ toggle pin n
clr? ( n)      \ skip next instruction if pin high
set? ( n)      \ skip next instruction if pin low
