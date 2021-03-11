\ i2c.fs
decimal

$80 constant mTWINT
$40 constant mTWEA
$20 constant mTWSTA
$10 constant mTWSTO
$08 constant mTWWC
$04 constant mTWEN
$01 constant mTWIE

\ remember the P register is the loop register
\ so don't use this in a loop
: i2c.init
    TWSR #p!  N ldz,  $fc N andi,  N stz, \ prescale = 1
    TWBR #p!  72 N ldi,  N stz,  \ 100kHz
    TWCR #p!  N ldz,  mTWEN N ori,  N stz,
    ;

: i2c.wait
    100 #, begin
        4000 #, begin
            TWCR #, c@ mTWINT #, and if/ 2drop exit then
        while  1 #- repeat drop
    while  1 #- repeat drop ;


: i2c.start
    [ mTWINT mTWEN or mTWSTA or ] #, TWCR #, c!  i2c.wait ;

: i2c.stop
    [ mTWINT mTWEN or mTWSTO or ] #, TWCR #, c! ;

: i2c.c! ( c - f)
    i2c.wait  TWDR #, c!
    [ mTWINT mTWEN or ] #, TWCR #, c!  i2c.wait  
    TWSR #, c@ $f8 #, and $18 #, = if/ 0 #, ; then \ SLA+W
    TWSR #, c@ $f8 #, and $28 #, = if/ 0 #, ; then \ data byte
    TWSR #, c@ $f8 #, and $40 #, = if/ 0 #, ; then \ SLA+R
    true #, ;

: i2c.c@.ack (  - c)
    [ mTWINT mTWEN or mTWEA or ] #, TWCR #, c!
    i2c.wait  TWDR #, c@ ;

: i2c.c@.nack (  - c)
    [ mTWINT mTWEN or ] #, TWCR #, c!
    i2c.wait  TWDR #, c@ ;

: i2c.address.write ( 7-bit-address - f)
    2* i2c.start i2c.c! 0= invert ;

: i2c.address.read ( 7-bit-address - f)
    2* 1 #+ i2c.start i2c.c! 0= invert ;

: i2c.ping? ( 7-bit-address - f)
    2* 1 #+ i2c.start i2c.c! 0= if/ i2c.c@.nack drop true #, ; then
    false #, ;

