; This file contains data that overwrites parts of the code in the
; file, due to an ORG that overlaps with the code.

    org 0x0000

    ld hl, 0x0001   ; Some arbitrary code
    ld bc, 0x0002   ; Some arbitrary code
    ld de, 0x0003   ; Some arbitrary code
    ld a, 0x04      ; Some arbitrary code
    ld (hl), a      ; Some arbitrary code
    ret

    org 0x0008

    ld hl, 0x0005   ; Some arbitrary code at the same position as the previous code
    ret
