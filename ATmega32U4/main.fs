\ main.fs

\ This is where your application goes.
\ It should have a main loop called "go". 

: ms ( n)  for 4000 # for next next ;
: go   13 output,
   begin  13 high,  500 # ms
          13 low,  500 # ms
   again

