/*
  Forth virtual machine

  This code is in the public domain.

*/

#define RAM_SIZE 0x1000
#define S0 0x1000
#define R0 0x0f00 
#define M(a, b) {memory [a] = b;}
#define NAME(m, f, c, x, y, z) {memory [m] = f + c + (x << 8) + (y << 16) + (z << 24);}
#define LINK(m, a) {memory [m] = a;}
#define CODE(m, a) {memory [m] = a;}
 
// global variables
union Memory {
  int data [RAM_SIZE];
  void (*code []) (void);
} memory;

String tib = "";
int S = S0; // data stack pointer
int R = R0; // return stack pointer
int I = 0; // instruction pointer
int W = 0; // working register
int T = 0; // top of stack
int H = 0; // dictionary pointer, HERE
int D = 0; // dictionary list entry point
int base = 10;

/*  A word in the dictionary has these fields:
  link  32b  point to next word in list, 0 says end of list
  name  32b  a 32 bit int, made up of byte count and three letters
  code  32b  a pointer to some actually C compiled code,
        all native code is in this field
  data  32b at least  a list to execute or a data field of some kind


*/


// primitive definitions

void _lit (void) {

}



// inner interpreter

void setup () {

// initialize dictionary

  D = 0; // latest word
  H = 0; // top of dictionary

  I = 0; // instruction pointer = abort

  Serial.begin (9600);
  while (!Serial);
  Serial.println ("myForth Arm Cortex");
//  _words ();
}

// the loop function runs over and over again forever
void loop() {
  W = memory.data [I++];
  memory.code [W] ();
//  delay (100);
}

