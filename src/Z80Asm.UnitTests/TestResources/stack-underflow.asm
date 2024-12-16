; This code causes a stack underflow by removing the return address
; from the stack before returning from the interrupt handler.
RAM_END: equ 0x2FFF

    ld sp, RAM_END
    ld hl, 0x0000
    push hl
    pop af
    pop de
    ret

