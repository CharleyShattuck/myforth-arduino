"C:\Program Files (x86)\Arduino\hardware\tools\avr/bin/avrdude" ^
-C"C:\Program Files (x86)\Arduino\hardware\tools\avr/etc/avrdude.conf". ^
-v -patmega328p -carduino -PCOMx -b115200 -D -Uflash:w:chip07Dec17.bin
pause
