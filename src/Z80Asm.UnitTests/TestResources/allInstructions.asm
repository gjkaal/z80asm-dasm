imm_word: equ 0x1234
imm_byte: equ 0x23
imm_offset: equ 0x45

ORG 0x1000

ADC A,imm_byte
ADC A,(HL)
ADC A,(IX+imm_offset)
ADC A,(IY+imm_offset)
ADC A,A
ADC A,B
ADC A,C
ADC A,D
ADC A,E
ADC A,H
ADC A,L
ADC HL,BC
ADC HL,DE
ADC HL,HL
ADC HL,SP
ADD A,imm_byte
ADD A,(HL)
ADD A,(IX+imm_offset)
ADD A,(IY+imm_offset)
ADD A,A
ADD A,B
ADD A,C
ADD A,D
ADD A,E
ADD A,H
ADD A,L
some_label:
ADD HL,BC
ADD HL,DE
ADD HL,HL
ADD HL,SP
ADD IX,BC
ADD IX,DE
ADD IX,IX
ADD IX,SP
ADD IY,BC
ADD IY,DE
ADD IY,IY
ADD IY,SP
AND imm_byte
AND (HL)
AND (IX+imm_offset)
AND (IY+imm_offset)
AND A
AND B
AND C
AND D
AND E
AND H
AND L
BIT 0,(HL)
BIT 1,(HL)
BIT 2,(HL)
BIT 3,(HL)
BIT 4,(HL)
BIT 5,(HL)
BIT 6,(HL)
BIT 7,(HL)
BIT 0,(IX+imm_offset)
BIT 1,(IX+imm_offset)
BIT 2,(IX+imm_offset)
BIT 3,(IX+imm_offset)
BIT 4,(IX+imm_offset)
BIT 5,(IX+imm_offset)
BIT 6,(IX+imm_offset)
BIT 7,(IX+imm_offset)
BIT 0,(IY+imm_offset)
BIT 1,(IY+imm_offset)
BIT 2,(IY+imm_offset)
BIT 3,(IY+imm_offset)
BIT 4,(IY+imm_offset)
BIT 5,(IY+imm_offset)
BIT 6,(IY+imm_offset)
BIT 7,(IY+imm_offset)
BIT 0,A
BIT 1,A
BIT 2,A
BIT 3,A
BIT 4,A
BIT 5,A
BIT 6,A
BIT 7,A
BIT 0,B
BIT 1,B
BIT 2,B
BIT 3,B
BIT 4,B
BIT 5,B
BIT 6,B
BIT 7,B
BIT 0,C
BIT 1,C
BIT 2,C
BIT 3,C
BIT 4,C
BIT 5,C
BIT 6,C
BIT 7,C
BIT 0,D
BIT 1,D
BIT 2,D
BIT 3,D
BIT 4,D
BIT 5,D
BIT 6,D
BIT 7,D
BIT 0,E
BIT 1,E
BIT 2,E
BIT 3,E
BIT 4,E
BIT 5,E
BIT 6,E
BIT 7,E
BIT 0,H
BIT 1,H
BIT 2,H
BIT 3,H
BIT 4,H
BIT 5,H
BIT 6,H
BIT 7,H
BIT 0,L
BIT 1,L
BIT 2,L
BIT 3,L
BIT 4,L
BIT 5,L
BIT 6,L
BIT 7,L
CALL imm_word
CALL C,imm_word
CALL M,imm_word
CALL NC,imm_word
CALL NZ,imm_word
CALL P,imm_word
CALL PE,imm_word
CALL PO,imm_word
CALL Z,imm_word
CCF
CP imm_byte
CP (HL)
CP (IX+imm_offset)
CP (IY+imm_offset)
CP A
CP B
CP C
CP D
CP E
CP H
CP L
CPD
CPDR
CPI
CPIR
CPL
DAA
DEC (HL)
DEC (IX+imm_offset)
DEC (IY+imm_offset)
DEC A
DEC B
DEC BC
DEC C
DEC D
DEC DE
DEC E
DEC H
DEC HL
DEC IX
DEC IY
DEC L
DEC SP
DI
DJNZ $
EI
EX (SP),HL
EX (SP),IX
EX (SP),IY
EX AF,AF'
EX DE,HL
EXX
HALT
IM 0
IM 1
IM 2
IN A,(imm_byte)
IN A,(C)
IN B,(C)
IN C,(C)
IN D,(C)
IN E,(C)
IN H,(C)
IN L,(C)
;IN_F (C)
INC (HL)
INC (IX+imm_offset)
INC (IY+imm_offset)
INC A
INC B
INC BC
INC C
INC D
INC DE
INC E
INC H
INC HL
INC IX
INC IY
INC L
INC SP
IND
INDR
INI
INIR
JP (HL)
JP (IX)
JP (IY)
JP some_label
JP C,imm_word
JP M,imm_word
JP NC,imm_word
JP NZ,imm_word
JP P,imm_word
JP PE,imm_word
JP PO,imm_word
JP Z,imm_word
JR $
JR C,$
JR NC,$
JR NZ,$
JR Z,$
LD (imm_word),A
LD (imm_word),BC
LD (imm_word),DE
LD (imm_word),HL
LD (imm_word),IX
LD (imm_word),IY
LD (imm_word),SP
LD (BC),A
LD (DE),A
LD (HL),imm_byte
LD (HL),A
LD (HL),B
LD (HL),C
LD (HL),D
LD (HL),E
LD (HL),H
LD (HL),L
LD (IX+imm_offset),imm_byte
LD (IX+imm_offset),A
LD (IX+imm_offset),B
LD (IX+imm_offset),C
LD (IX+imm_offset),D
LD (IX+imm_offset),E
LD (IX+imm_offset),H
LD (IX+imm_offset),L
LD (IY+imm_offset),imm_byte
LD (IY+imm_offset),A
LD (IY+imm_offset),B
LD (IY+imm_offset),C
LD (IY+imm_offset),D
LD (IY+imm_offset),E
LD (IY+imm_offset),H
LD (IY+imm_offset),L
LD A,imm_byte
LD A,(imm_word)
LD A,(BC)
LD A,(DE)
LD A,(HL)
LD A,(IX+imm_offset)
LD A,(IY+imm_offset)
LD A,A
LD A,B
LD A,C
LD A,D
LD A,E
LD A,H
LD A,I
LD A,L
LD A,R
LD B,imm_byte
LD B,(HL)
LD B,(IX+imm_offset)
LD B,(IY+imm_offset)
LD B,A
LD B,B
LD B,C
LD B,D
LD B,E
LD B,H
LD B,L
LD BC,(imm_word)
LD BC,imm_word
LD C,imm_byte
LD C,(HL)
LD C,(IX+imm_offset)
LD C,(IY+imm_offset)
LD C,A
LD C,B
LD C,C
LD C,D
LD C,E
LD C,H
LD C,L
LD D,imm_byte
LD D,(HL)
LD D,(IX+imm_offset)
LD D,(IY+imm_offset)
LD D,A
LD D,B
LD D,C
LD D,D
LD D,E
LD D,H
LD D,L
LD DE,(imm_word)
LD DE,imm_word
LD E,imm_byte
LD E,(HL)
LD E,(IX+imm_offset)
LD E,(IY+imm_offset)
LD E,A
LD E,B
LD E,C
LD E,D
LD E,E
LD E,H
LD E,L
LD H,imm_byte
LD H,(HL)
LD H,(IX+imm_offset)
LD H,(IY+imm_offset)
LD H,A
LD H,B
LD H,C
LD H,D
LD H,E
LD H,H
LD H,L
LD HL,(imm_word)
LD HL,imm_word
LD I,A
LD IX,(imm_word)
LD IX,imm_word
LD IY,(imm_word)
LD IY,imm_word
LD L,imm_byte
LD L,(HL)
LD L,(IX+imm_offset)
LD L,(IY+imm_offset)
LD L,A
LD L,B
LD L,C
LD L,D
LD L,E
LD L,H
LD L,L
LD SP,(imm_word)
LD SP,imm_word
LD SP,HL
LD SP,IX
LD SP,IY
;LD_R_A
LDD
LDDR
LDI
LDIR
NEG
NOP
OR imm_byte
OR (HL)
OR (IX+imm_offset)
OR (IY+imm_offset)
OR A
OR B
OR C
OR D
OR E
OR H
OR L
OTDR
OTIR
OUT (imm_byte),A
OUT (C),0
OUT (C),A
OUT (C),B
OUT (C),C
OUT (C),D
OUT (C),E
OUT (C),H
OUT (C),L
OUTD
OUTI
POP AF
POP BC
POP DE
POP HL
POP IX
POP IY
PUSH AF
PUSH BC
PUSH DE
PUSH HL
PUSH IX
PUSH IY
RES 0,(HL)
RES 1,(HL)
RES 2,(HL)
RES 3,(HL)
RES 4,(HL)
RES 5,(HL)
RES 6,(HL)
RES 7,(HL)
RES 0,(IX+imm_offset)
RES 1,(IX+imm_offset)
RES 2,(IX+imm_offset)
RES 3,(IX+imm_offset)
RES 4,(IX+imm_offset)
RES 5,(IX+imm_offset)
RES 6,(IX+imm_offset)
RES 7,(IX+imm_offset)
RES 0,(IY+imm_offset)
RES 1,(IY+imm_offset)
RES 2,(IY+imm_offset)
RES 3,(IY+imm_offset)
RES 4,(IY+imm_offset)
RES 5,(IY+imm_offset)
RES 6,(IY+imm_offset)
RES 7,(IY+imm_offset)
RES 0,A
RES 1,A
RES 2,A
RES 3,A
RES 4,A
RES 5,A
RES 6,A
RES 7,A
RES 0,B
RES 1,B
RES 2,B
RES 3,B
RES 4,B
RES 5,B
RES 6,B
RES 7,B
RES 0,C
RES 1,C
RES 2,C
RES 3,C
RES 4,C
RES 5,C
RES 6,C
RES 7,C
RES 0,D
RES 1,D
RES 2,D
RES 3,D
RES 4,D
RES 5,D
RES 6,D
RES 7,D
RES 0,E
RES 1,E
RES 2,E
RES 3,E
RES 4,E
RES 5,E
RES 6,E
RES 7,E
RES 0,H
RES 1,H
RES 2,H
RES 3,H
RES 4,H
RES 5,H
RES 6,H
RES 7,H
RES 0,L
RES 1,L
RES 2,L
RES 3,L
RES 4,L
RES 5,L
RES 6,L
RES 7,L
RET
RET C
RET M
RET NC
RET NZ
RET P
RET PE
RET PO
RET Z
RETI
RETN
RL (HL)
RL (IX+imm_offset)
RL (IY+imm_offset)
RL A
RL B
RL C
RL D
RL E
RL H
RL L
RLA
RLC (HL)
RLC (IX+imm_offset)
RLC (IY+imm_offset)
RLC A
RLC B
RLC C
RLC D
RLC E
RLC H
RLC L
RLCA
RLD
RR (HL)
RR (IX+imm_offset)
RR (IY+imm_offset)
RR A
RR B
RR C
RR D
RR E
RR H
RR L
RRA
RRC (HL)
RRC (IX+imm_offset)
RRC (IY+imm_offset)
RRC A
RRC B
RRC C
RRC D
RRC E
RRC H
RRC L
RRCA
RRD
RST 0x00
RST 0x08
RST 0x10
RST 0x18
RST 0x20
RST 0x28
RST 0x30
RST 0x38
SBC A,imm_byte
SBC A,(HL)
SBC A,(IX+imm_offset)
SBC A,(IY+imm_offset)
SBC A,A
SBC A,B
SBC A,C
SBC A,D
SBC A,E
SBC A,H
SBC A,L
SBC HL,BC
SBC HL,DE
SBC HL,HL
SBC HL,SP
SCF
SET 0,(HL)
SET 1,(HL)
SET 2,(HL)
SET 3,(HL)
SET 4,(HL)
SET 5,(HL)
SET 6,(HL)
SET 7,(HL)
SET 0,(IX+imm_offset)
SET 1,(IX+imm_offset)
SET 2,(IX+imm_offset)
SET 3,(IX+imm_offset)
SET 4,(IX+imm_offset)
SET 5,(IX+imm_offset)
SET 6,(IX+imm_offset)
SET 7,(IX+imm_offset)
SET 0,(IY+imm_offset)
SET 1,(IY+imm_offset)
SET 2,(IY+imm_offset)
SET 3,(IY+imm_offset)
SET 4,(IY+imm_offset)
SET 5,(IY+imm_offset)
SET 6,(IY+imm_offset)
SET 7,(IY+imm_offset)
SET 0,A
SET 1,A
SET 2,A
SET 3,A
SET 4,A
SET 5,A
SET 6,A
SET 7,A
SET 0,B
SET 1,B
SET 2,B
SET 3,B
SET 4,B
SET 5,B
SET 6,B
SET 7,B
SET 0,C
SET 1,C
SET 2,C
SET 3,C
SET 4,C
SET 5,C
SET 6,C
SET 7,C
SET 0,D
SET 1,D
SET 2,D
SET 3,D
SET 4,D
SET 5,D
SET 6,D
SET 7,D
SET 0,E
SET 1,E
SET 2,E
SET 3,E
SET 4,E
SET 5,E
SET 6,E
SET 7,E
SET 0,H
SET 1,H
SET 2,H
SET 3,H
SET 4,H
SET 5,H
SET 6,H
SET 7,H
SET 0,L
SET 1,L
SET 2,L
SET 3,L
SET 4,L
SET 5,L
SET 6,L
SET 7,L
SLA (HL)
SLA (IX+imm_offset)
SLA (IY+imm_offset)
SLA A
SLA B
SLA C
SLA D
SLA E
SLA H
SLA L
;SLL (HL)
;SLL (IX+imm_offset)
;SLL (IY+imm_offset)
;SLL A
;SLL B
;SLL C
;SLL D
;SLL E
;SLL H
;SLL L
SRA (HL)
SRA (IX+imm_offset)
SRA (IY+imm_offset)
SRA A
SRA B
SRA C
SRA D
SRA E
SRA H
SRA L
SRL (HL)
SRL (IX+imm_offset)
SRL (IY+imm_offset)
SRL A
SRL B
SRL C
SRL D
SRL E
SRL H
SRL L
SUB imm_byte
SUB (HL)
SUB (IX+imm_offset)
SUB (IY+imm_offset)
SUB A
SUB A,(HL)
SUB A,A
SUB A,B
SUB A,C
SUB A,D
SUB A,E
SUB A,H
SUB A,L
SUB B
SUB C
SUB D
SUB E
SUB H
SUB L
XOR imm_byte
XOR (HL)
XOR (IX+imm_offset)
XOR (IY+imm_offset)
XOR A
XOR (HL)
XOR A
XOR B
XOR C
XOR D
XOR E
XOR H
XOR L
XOR B
XOR C
XOR D
XOR E
XOR H
XOR L
