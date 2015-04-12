\ main.fs

\ This is where your application goes.
\ It should have a main loop called "go". 

: ms ( n)  for 4000 # for next next ;
: go  7 DDRC sbi,   \ pin 13 is an output
   begin  7 PORTC sbi,  500 # ms
          7 PORTC cbi,  500 # ms
   again

