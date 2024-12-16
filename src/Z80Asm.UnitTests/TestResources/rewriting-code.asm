; This program contains code that writes to the memory
; space where the code is stored.

ld a, 0x04      ; ld 4 into register A 
ld (0x0000), a  ; ld A into memory location 0x0000
ld A, (0x0000)  ; ld A from memory location 0x0000
ret             ; return from subroutine