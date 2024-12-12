; Generic macros for extensing the assembly code

; Shift left byte in A 4 bits
M_SLA MACRO
    rla
    rla
    rla
    rla
    and 0xF0
ENDM

; Shift right byte in A 4 bits
M_SRB MACRO
    rra
    rra
    rra
    rra
    and 0x0F
ENDM