\ SPI.fs

\ SPI Special Function Registers

: SPCR! ( c)  T SPCR out,  drop ;
: SPCR@ (  - c)  0 #,  SPCR T in, ;

: SPSR! ( c)  T SPSR out,  drop ;
: SPSR@ (  - c)  0 #,  SPSR T in, ;

: SPDR! ( c)  T SPDR out,  drop ;
: SPDR@ (  - c)  0 #,  SPDR T in, ;

