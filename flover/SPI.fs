\ SPI.fs

\ SPI Special Function Registers

: SPCR! ( c)  T SPCR out,  drop ;
: SPCR@ (  - c)  0 #,  SPCR T in, ;

: SPSR! ( c)  T SPSR out,  drop ;
: SPSR@ (  - c)  0 #,  SPSR T in, ;

: SPDR! ( c)  T SPDR out,  drop ;
: SPDR@ (  - c)  0 #,  SPDR T in, ;

10 constant /SS
11 constant MOSI
12 constant MISO
13 constant CLK

[
:m enable_SPI   /SS output,  /SS low, m;
:m disable_SPI  /SS output,  /SS high, m;
]

: SPI ( c1 - c2)
   enable_SPI  SPDR!
   begin  SPSR@ $80 #, and until/
   SPDR@ disable_SPI ;

: init_SPI
    disable_SPI  MOSI output, MOSI high,
    MISO output, MISO high,  CLK output,  CLK high,
    $52 #, SPCR! ;

