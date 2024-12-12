; ZC-Kernel
; Kernel for the ZC-80 computer
; Version 1.0

; This is the main kernel file for the ZC-80 computer. It contains the
; initialization code, the main loop, and the interrupt service routine.
; It contains main routines for the following:
; - Keyboard input using serial port (RS-232)
; - Terminal output using serial port (RS-232)
; - Standard I/O functions

ORG 0xE000


; Constants
; ROM locations
SYS_ROM EQU 0xE000
SYS_CORE EQU 0x0000

; Memory locations
SYS_MEM EQU 0x8000

