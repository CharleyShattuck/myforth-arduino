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
int memory[RAM_SIZE]; // RAM is 32 bit word-addressed
int S = S0; // data stack pointer
int R = R0; // return stack pointer
int I = 0; // instruction pointer
int W = 0; // working register
int T = 0; // top of stack
int H = 0; // dictionary pointer, HERE
int D = 0; // dictionary list entry point

/*  A word in the dictionary has these fields:
  link  32b  point to next word in list, 0 says end of list
  name  32b  a 32 bit int, made up of byte count and three letters
  code  32b  the 32 bit token to be compiled,
        negative if it's a primitive, (inverted)
        positive address if it's a colon definition
  data  32b at least  a list to execute or a data field of some kind

  when compiling the code value is compiled, unless this is an
  immediate word, in which case the word is executed
  a bit in the the count field should be the immediate bit
  and another bit will be the primitive bit
*/

// function prototypes for the primitives
void _lit (void);
void _exit (void);
void _branch (void);
void _0branch (void);
void _key (void);
void _emit (void);
void _dup (void);
void _drop (void);
void _swap (void);
void _over (void);
void _fetch (void);
void _store (void);
void _comma (void);
void _plus (void);
void _minus (void);
void _and (void);
void _or (void);
void _xor (void);
void _invert (void);
void _abs (void);
void _negate (void);
void _twostar (void);
void _twoslash (void);
void _times (void);
void _divide (void);
void _mod (void);
void _starslash (void);
void _dot (void);
void _dotS (void);
void _cr (void);
void _space (void);
void _zeroequal (void);
void _zeroless (void);
void _words (void);
void _find (void);
void _execute (void);
void _word (void);
void _hdot (void);
void _qdup (void);
void _initS (void);
void _initR (void);
void _ok (void);
void _dnum (void);

// primitive function array
void (*primitive []) (void) = {
  _lit,
#define _LIT ~0
  _exit,
#define _EXIT ~1
  _branch,
#define _BRANCH ~2
  _0branch,
#define _0BRANCH ~3
  _key,
#define _KEY ~4
  _emit,
#define _EMIT ~5
  _dup,
#define _DUP ~6
  _drop,
#define _DROP ~7
  _swap,
#define _SWAP ~8
  _over,
#define _OVER ~9
  _fetch,
#define _FETCH ~10
  _store,
#define _STORE ~11
  _comma,
#define _COMMA ~12
  _plus,
#define _PLUS ~13
  _minus,
#define _MINUS ~14
  _and,
#define _AND ~15
  _or,
#define _OR ~16
  _xor,
#define _XOR ~17
  _invert,
#define _INVERT ~18
  _abs,
#define _ABS ~19
  _negate,
#define _NEGATE ~20
  _twostar,
#define _TWOSTAR ~21
  _twoslash,
#define _TWOSLASH ~22
  _times,
#define _TIMES ~23
  _divide,
#define _DIVIDE ~24
  _mod,
#define _MOD ~25
  _starslash,
#define _STARSLASH ~26
  _dot,
#define _DOT ~27
  _hdot,
#define _HDOT ~28
  _dotS,
#define _DOTS ~29
  _cr,
#define _CR ~30
  _space,
#define _SPACE ~31
  _twostar,
#define _ZEROEQUAL ~32
  _zeroless,
#define _ZEROLESS ~33
  _words,
#define _WORDS ~34
  _find,
#define _FIND ~35
  _execute,
#define _EXECUTE ~36
  _word,
#define _WORD ~37
  _qdup,
#define _QDUP ~38
  _initS,
#define _INITS ~39
  _initR,
#define _INITR ~40
  _ok,
#define _OK ~41
  _dnum
#define _DNUM ~42
};

// primitive definitions

void _lit (void) {
  memory [--S] = memory [I++];
}

void _exit (void) {
  I = memory [R++];
}

void _branch (void) {
  I = memory [I];
}

void _0branch (void) {
  T = memory [S++];
  if (T == 0) {
    I = memory [I];
    return;
  }
  I += 1;
}

void _key (void) {
  while (!Serial.available ()) ;
  T = Serial.read ();
  memory [--S] = T;
}

void _emit (void) {
  char c = memory [S++];
  Serial.write (c);
}

void _dup (void) {
  T = memory [S--];
  memory [S] = T;
}

void _drop (void) {
  S += 1;
}

void _swap (void) {
  T = memory [S++];
  W = memory [S];
  memory [S--] = T;
  memory [S] = W;
}

void _over (void) {
  T = memory [S++];
  W = memory [S];
  memory [--S] = T;
  memory [--S] = W;
}

void _fetch (void) {
  W = memory [S];
  T = memory [W];
  memory [S] = T;
}

void _store (void) {
  T = memory [S++];
  W = memory [S++];
  memory [T] = W;
}

void _comma (void) {
  T = memory [S++];
  memory [H++] = T;
}

void _plus (void) {
  T = memory [S++];
  T = T + memory [S];
  memory [S] = T;
}

void _minus (void) {
  T = memory [S++];
  W = memory [S] - T;
  memory [S] = W;
}

void _and (void) {
  T = memory [S++];
  W = T & memory [S];
  memory [S] = W;
}

void _or (void) {
  T = memory [S++];
  W = T | memory [S];
  memory [S] = W;
}
void _xor (void) {
  T = memory [S++];
  W = T ^ memory [S];
  memory [S] = W;
}

void _invert (void) {
  T = memory [S];
  memory [S] = ~T;
}

void _abs (void) {
  T = memory [S];
  memory [S] = abs (T);
}

void _negate (void) {
  T = memory [S];
  memory [S] = -T;
}

void _twostar (void) {
  T = memory [S];
  memory [S] = (T * 2);
}

void _twoslash (void) {
  T = memory [S];
  memory [S] = (T / 2);
}

void _times (void) {
  T = memory [S++];
  W = memory [S];
  memory [S] = (T * W);
} 

void _divide (void) {
  T = memory [S++];
  W = memory [S];
  memory [S] = (W / T);
}

void _mod (void) {
  T = memory [S++];
  W = memory [S];
  memory [S] = (W % T);
}

void _starslash (void) {
  // requires double int
}

void _zeroequal (void) {
  T = memory [S];
  if (T == 0) {
    memory [S] = -1;
  } else {
    memory [S] = 0;
  }
}

void _zeroless (void) {
  T = memory [S];
  if (T < 0) {
    memory [S] = -1;
  } else {
    memory [S] = 0;
  }
}

void _qdup (void) {
  T = memory [S];
  if (T == 0) return;
  memory [--S] = T;
}

void _dot (void) {
  T = memory [S++];
  Serial.print (T);
  Serial.write (' ');
}

void _hdot (void) {
  T = memory [S++];
  Serial.print (T, HEX);
  Serial.write (' ');
}

void _dotS (void) {
  if (S == S0) {
    Serial.print ("empty ");
    return;
  }
  W = S0;
  while (W > S) {
    Serial.print (memory [--W]);
    Serial.write (' ');      
  }
}

void _cr (void) {
  Serial.println (" ");
}

void _space (void) {
  Serial.write (' ');
}

void _words (void) {
  int C = 0;
  int X = 0;
  W = D;
  while (W) {
    T = memory [W];
    C = (T & 0xff);
    X = ((T >> 8) & 0xff);
    memory [--S] = X;
    _emit ();
    X = ((T >> 16) & 0xff);
    memory [--S] = X;
    if (X != ' ') _emit ();
    X = ((T >> 24) & 0xff);
    memory [--S] = X;
    if (X != ' ') _emit ();
    C -= 4;
    while (!(C < 0)) {
      Serial.print ("_");
      C -= 1;
    }
    _space ();
    W = memory [++W];
  }
  _cr ();
}

void _find (void) {
  int X = memory [S++];
  W = D;
  while (W != 0) {
    T = (memory [W]);
    if (T == X) {
      memory [--S] = W;
      return;
    }
    W = memory [++W];
  }
  memory [--S] = 0;
}

void _execute (void) {
  T = memory [S++];
  memory [--R] = I;
  I = (T + 2);
 }

void _word (void) {
  int count = 0;
  W = 0;
  while (1) {
    T = 0;
    while (Serial.available () == 0); 
    T = Serial.read ();
    if (T <= ' ') {
      W |= count;
      memory [--S] = W;
      return;
    }
    count += 1;
    if (count == 1) {
      W |= (T << 8);
    }
    if (count == 2) {
      W |= (T << 16);
    }
    if (count == 3) {
      W |= (T << 24);
    }
    if (count > 3) {
    }
  }  
}

void _initS (void) {
  S = S0;
}

void _initR (void) {
  R = R0;
}

void _ok (void) {
  Serial.println (" Ok");
}

void _dnum (void) {
  int count = 0;
  W = 0;
  while (1) {
    T = 0;
    while (Serial.available () == 0); 
    T = Serial.read ();
    if (T <= ' ') {
      memory [--S] = W;
      return;
    }
    W *= 10;
    T -= '0';
    W = (T + W);
  }  
}

// the setup function runs once when you press reset or power the board
// This will setup stacks and other pointers, initial machine state
// and the initial dictionary
void setup() {
  S = S0; // initialize data stack
  R = R0; // initialize return stack
// initialize dictionary
  M(0, 0)
// exit
  NAME(4, 0, 4, 'e', 'x', 'i')
  LINK(5, 1)
  CODE(6, _EXIT)
// key
  NAME(13, 0, 3, 'k', 'e', 'y')
  LINK(14, 10)
  CODE(15, _KEY)
  CODE(16, _EXIT)
// emit
  NAME(17, 0, 4, 'e', 'm', 'i')
  LINK(18, 13)
  CODE(19, _EMIT)
  CODE(20, _EXIT)
// dup
  NAME(21, 0, 3, 'd', 'u', 'p')
  LINK(22, 17)
  CODE(23, _DUP)
  CODE(24, _EXIT)
// drop
  NAME(25, 0, 4, 'd', 'r', 'o')
  LINK(26, 21)
  CODE(27, _DROP)
  CODE(28, _EXIT)
// swap
  NAME(29, 0, 4, 's', 'w', 'a')
  LINK(30, 25)
  CODE(31, _SWAP)
  CODE(32, _EXIT)
// over
  NAME(33, 0, 4, 'o', 'v', 'e')
  LINK(34, 29)
  CODE(35, _OVER)
  CODE(36, _EXIT)
// @
  NAME(37, 0, 1, '@', 0, 0)
  LINK(38, 33)
  CODE(39, _FETCH)
  CODE(40, _EXIT)
// !
  NAME(41, 0, 1, '!', 0, 0)
  LINK(42, 37)
  CODE(43, _STORE)
  CODE(44, _EXIT)
// ,
  NAME(45, 0, 1, ',', 0, 0)
  LINK(46, 41)
  CODE(47, _COMMA)
  CODE(48, _EXIT)
// +
  NAME(49, 0, 1, '+', 0, 0)
  LINK(50, 45)
  CODE(51, _PLUS)
  CODE(52, _EXIT)
// -
  NAME(53, 0, 1, '-', 0, 0)
  LINK(54, 49)
  CODE(55, _MINUS)
  CODE(56, _EXIT)
// and
  NAME(57, 0, 3, 'a', 'n', 'd')
  LINK(58, 53)
  CODE(59, _AND)
  CODE(60, _EXIT)
// or 
  NAME(61, 0, 2, 'o', 'r', 0)
  LINK(62, 57)
  CODE(63, _OR)
  CODE(64, _EXIT)
// xor 
  NAME(65, 0, 3, 'x', 'o', 'r')
  LINK(66, 61)
  CODE(67, _XOR)
  CODE(68, _EXIT)
// invert 
  NAME(69, 0, 6, 'i', 'n', 'v')
  LINK(70, 65)
  CODE(71, _INVERT)
  CODE(72, _EXIT)
// abs
  NAME(73, 0, 3, 'a', 'b', 's')
  LINK(74, 69)
  CODE(75, _ABS)
  CODE(76, _EXIT)
// negate
  NAME(77, 0, 6, 'n', 'e', 'g')
  LINK(78, 73)
  CODE(79, _NEGATE)
  CODE(80, _EXIT)
// 2*
  NAME(81, 0, 2, '2', '*', 0)
  LINK(82, 77)
  CODE(83, _TWOSTAR)
  CODE(84, _EXIT)
// 2/
  NAME(85, 0, 2, '2', '/', 0)
  LINK(86, 81)
  CODE(87, _TWOSLASH)
  CODE(88, _EXIT)
// *
  NAME(89, 0, 1, '*', 0, 0)
  LINK(90, 85)
  CODE(91, _TIMES)
  CODE(92, _EXIT)
// /
  NAME(93, 0, 1, '/', 0, 0)
  LINK(94, 89)
  CODE(95, _DIVIDE)
  CODE(96, _EXIT)
// mod
  NAME(97, 0, 3, 'm', 'o', 'd')
  LINK(98, 93)
  CODE(99, _MOD)
  CODE(100, _EXIT)
// */
  NAME(101, 0, 2, '*', '/', 0)
  LINK(102, 97)
  CODE(103, _STARSLASH)
  CODE(104, _EXIT)
// 0=
  NAME(105, 0, 2, '0', '=', 0)
  LINK(106, 101)
  CODE(107, _ZEROEQUAL)
  CODE(108, _EXIT)
// 0<
  NAME(109, 0, 2, '0', '<', 0)
  LINK(110, 105)
  CODE(111, _ZEROLESS)
  CODE(112, _EXIT)
// space
  NAME(113, 0, 5, 's', 'p', 'a')
  LINK(114, 109)
  CODE(115, _SPACE)
  CODE(116, _EXIT)
// cr
  NAME(117, 0, 2, 'c', 'r', 0)
  LINK(118, 113)
  CODE(119, _CR)
  CODE(120, _EXIT)
// .
  NAME(121, 0, 1, '.', 0, 0)
  LINK(122, 117)
  CODE(123, _DOT)
  CODE(124, _EXIT)
// h.
  NAME(125, 0, 2, 'h', '.', 0)
  LINK(126, 121)
  CODE(127, _HDOT)
  CODE(128, _EXIT)
// .s
  NAME(129, 0, 2, '.', 's', 0)
  LINK(130, 125)
  CODE(131, _DOTS)
  CODE(132, _EXIT)
// words
  NAME(133, 0, 5, 'w', 'o', 'r')
  LINK(134, 129)
  CODE(135, _WORDS)
  CODE(136, _EXIT)
// find
  NAME(137, 0, 4, 'f', 'i', 'n')
  LINK(138, 133)
  CODE(139, _FIND)
  CODE(140, _EXIT)
// execute
  NAME(141, 0, 7, 'e', 'x', 'e')
  LINK(142, 137)
  CODE(143, _EXECUTE)
  CODE(144, _EXIT)
// word
  NAME(145, 0, 4, 'w', 'o', 'r')
  LINK(146, 141)
  CODE(147, _WORD)
  CODE(148, _EXIT)
// ?dup
  NAME(149, 0, 4, 'w', 'o', 'r')
  LINK(150, 145)
  CODE(151, _WORD)
  CODE(152, _EXIT)
// quit
  NAME(153, 0, 4, 'q', 'u', 'i')
  LINK(154, 149)
  CODE(155, _INITR)
  CODE(156, _WORD)
  CODE(157, _FIND)
  CODE(158, _QDUP)
  CODE(159, _0BRANCH)
  CODE(160, 165)
  CODE(161, _EXECUTE)
  CODE(162, _OK)
  CODE(163, _BRANCH)
  CODE(164, 156) // continue quit loop
  CODE(165, _LIT)
  CODE(166, '?')
  CODE(167, _EMIT)
  CODE(168, _INITS)
  CODE(169, _BRANCH)
  CODE(170, 155)
// abort 
  NAME(171, 0, 5,'a', 'b', 'o')
  LINK(172, 153)
  CODE(173, _INITS)
  CODE(174, _BRANCH)
  CODE(175, 155) // abort part of quit
// d# 
  NAME(176, 0, 2,'d', '#', 0)
  LINK(177, 171)
  CODE(178, _DNUM)
  CODE(179, _EXIT)

  D = 176; // latest word
  H = 180; // top of dictionary

  I = 173; // initialize instruction pointer

  Serial.begin (9600);
  delay (1000);
  Serial.println ("myForth Arm");
//  _words ();
}

// the loop function runs over and over again forever
void loop() {
  W = memory [I++];
  if (W < 0) {  // primitives are inverted, therefore negative
    primitive [~W] (); // execute primitive
  } else { // high level words are just addresses
    memory [--R] = I; // nest
    I = W; // into a high level word
  }
//  delay (100);
}

