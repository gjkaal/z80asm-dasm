include "macro.asm"

; library for string formatting and manipulation

; Convert a byte to a string
; Input: A = byte to convert
; Output: HL = pointer to memory containing the string

; base for ascii digits is 48
; base for asci characters is 65
; base for ascii lower case characters is 97


convert_byte_to_string:
    push AF
    M_SLA
    
    call convert_nibble_to_ascii
    ld (HL), A
    inc HL

    pop AF
    call convert_nibble_to_ascii
    ld (HL), A
    inc HL
    ret

convert_nibble_to_ascii:
    and 0x0F
    cp 10
    jr C, convert_to_ascii_digit

    add A, 55
    ret
convert_to_ascii_digit:
    add A, 48
    ret