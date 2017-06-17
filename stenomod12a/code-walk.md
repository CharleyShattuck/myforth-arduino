# A Code Walk Through the Stenomod Firmware

## Overview

Stenomod is a relatively inexpensive amateur steno machine. It's an Arduino application, but using a personal version of Forth instead C++ IDE. The purpose of this walk through is to give you just enough background to understand the firmware source code in the file "main.fs". You won't learn to use Forth for general purpose programming here, just enough to know how the stenomod works.

## What Can a Computer Actually Do?

When you learn about machine languages you see that a general purpose computer really only knows how to do a very limited number of things, called instructions. First there is memory, and memory can be accessed in terms of addresses. Some "memory" is called "registers" and may not be in the same address space as the rest of memory. Instructions tend to work on registers. Data may be stored in memory but has to be moved into a register to be acted upon by an instruction. Please bear with me here, this is an approximation, not exactly true for all computers, but good enough for this discussion.

### Move Instructions

Every computer has instructions for moving data from an address in memory to a register, and from a register to some address in memory. In Forth there is a register called T, or TOS, or Top of Stack. In Forth T is pretty much where all the action takes place. Data can be moved into T from a memory address with the "@" instruction. Data can be moved from T to an address in memory with the "!". "@" is pronounced "fetch", and "!" is pronounced "store". The single character names were chosen because these instruction are used all the time. This makes them easier to type. After a short time you should just be saying "store" in your mind when you see "!", and "fetch" when you see "@".

I've glossed over something about the T register though. It's called the "Top of Stack" because that's what it is, the top of a data stack. When you fetch something from memory to T, the address is already on the stack, that is, in T. Internally the address is popped from the data stack into an address register, then data is fetched from that address and pushed onto the data stack. Likewise, to store data from stack (from now on I'll just say "stack" when I mean "data stack") the data should be in T. The destination address is pushed onto the stack, into T. The data is now the second item on the stack. Internally that address is popped into the address register. The data, which has now moved into T, is popped and moved into memory at the address  contained in the address register.

That's a lot of info that you may not even need to know, but I think it helps to make things more real if you know a little bit of what's going on behind the scenes. You can easily take a look at the source code for myforth to see how all this happens.

## A Little More about Forth Now

You actually know quite a bit about Forth already, @ and ! are maybe the most important operators to know about. By the way, operators in Forth are called "words". Words are the Forth name for functions or procedures too. Here's another detail. The Forth machine model (think of Forth as a virtual machine) has a linear address space. Forth programs and data both go into the "dictionary" which builds up from low addresses to higher addresses. New definitions will be placed into memory at "here", the next available location. If you want to create a data structure you just "create" a name for the next available location and "allot" some space. It's super simple and there is no heap, no reclaiming of chunks.

The stenomod firmware exists in a virtual address space in the dictionary. This virtual space is created in the file "compiler.fs" with the line "create target-image target-size allot", target-size having already been defined as however many bytes there are in the target machine,  which is an Arduino AVR328p processor. Think of this virtual address space as the target, and the rest of memory as the host.

Now that we have a virtual memory space for stenomod's firmware, we need some convenient words to use to access it. We have @ and ! for Forth's main address space. Let's define @-t and !-t for the virtual address space. Think of "-t" to mean "there" or in the "target". These virtual operators just internally add an offset to the address on the stack to fetch or store in the  target space instead of the host space. This way you can pretend you're reading and writing the actual target instead some data structure in the host memory.

There is a Forth primitive called "," which pops a number from the stack and stores it in the dictionary at the next available spot. Then "here" is updated to point just after that spot. The word is pronounced "comma". This is the most basic word in a Forth target assembler, which happens to be what we're building here. If you know exactly what you want in your target dictionary in terms of the lowest level machine codes then you could just "comma" them in now and be done with it. Since that's not very productive nor fun, we'll add a few more words to make things easier.

## What else can a computer do, besides fetch and store memory?

Well, computers do arithmetic and logic. These operations are done to the one or two items on top of the stack. "+" (pronounced "plus") pops two numbers off the stack, adds them together, and pushes the result back onto the stack. This is the pattern of a binary operator. Other binary operators are "and", "or", and "xor". A unary operator such as "invert" pops the top of stack, inverts all bits, and pushes the result back onto the stack. (Be aware that actual code for such Forth words will be written in whatever way is most efficient, and may not actually pop and push the stack. This is mostly just a mental model).

The other important thing a computer can do is to choose a different path of execution based on the result of a test. In other words "if X then Y else Z". In Forth the word "if" branches based on the value it finds in T. This value probably gets there as the result of some test. Generally speaking, the value is consumed, or dropped before the branch is taken. Let's see if this is enough info about Forth to get us started.

## Now to Walk Through Some Code

Open the file "main.fs" and start reading. The first character in the first line is "\", which means comment to end of line. It's like // in C++ but was invented before we knew about //. Another comment operator is "(", which comments until a closing ")" is encountered. Finally, a way of commenting a range of lines is to start one with "0 [if]" and later on close it with "[then]". [if] and [then] are used for conditional compilation. "main.fs" is nothing but comments until line 52.

Now is the time to learn about defining words.




