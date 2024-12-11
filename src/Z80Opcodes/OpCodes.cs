using System.Collections.ObjectModel;

namespace Z80Mnemonics;

public static class OpCodes
{
    /**/
    public static ReadOnlyCollection<OpCode> DasmBase =
        new ReadOnlyCollection<OpCode>(
        [
            new( "NOP"               ,  4 ,  0 ), /* 00 */
			new( "LD BC,@"           , 10 ,  0 ), /* 01 */
			new( "LD (BC),A"         ,  7 ,  0 ), /* 02 */
			new( "INC BC"            ,  6 ,  0 ), /* 03 */
			new( "INC B"             ,  4 ,  0 ), /* 04 */
			new( "DEC B"             ,  4 ,  0 ), /* 05 */
			new( "LD B,#"            ,  7 ,  0 ), /* 06 */
			new( "RLCA"              ,  4 ,  0 ), /* 07 */
			new( "EX AF,AF'"         ,  4 ,  0 ), /* 08 */
			new( "ADD HL,BC"         , 11 ,  0 ), /* 09 */
			new( "LD A,(BC)"         ,  7 ,  0 ), /* 0A */
			new( "DEC BC"            ,  6 ,  0 ), /* 0B */
			new( "INC C"             ,  4 ,  0 ), /* 0C */
			new( "DEC C"             ,  4 ,  0 ), /* 0D */
			new( "LD C,#"            ,  7 ,  0 ), /* 0E */
			new( "RRCA"              ,  4 ,  0 ), /* 0F */
			new( "DJNZ %"            ,  8 , 13 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* 10 */
			new( "LD DE,@"           , 10 ,  0 ), /* 11 */
			new( "LD (DE),A"         ,  7 ,  0 ), /* 12 */
			new( "INC DE"            ,  6 ,  0 ), /* 13 */
			new( "INC D"             ,  4 ,  0 ), /* 14 */
			new( "DEC D"             ,  4 ,  0 ), /* 15 */
			new( "LD D,#"            ,  7 ,  0 ), /* 16 */
			new( "RLA"               ,  4 ,  0 ), /* 17 */
			new( "JR %"              , 12 ,  0, OpCodeFlags.Jumps ), /* 18 */
			new( "ADD HL,DE"         , 11 ,  0 ), /* 19 */
			new( "LD A,(DE)"         ,  7 ,  0 ), /* 1A */
			new( "DEC DE"            ,  6 ,  0 ), /* 1B */
			new( "INC E"             ,  4 ,  0 ), /* 1C */
			new( "DEC E"             ,  4 ,  0 ), /* 1D */
			new( "LD E,#"            ,  7 ,  0 ), /* 1E */
			new( "RRA"               ,  4 ,  0 ), /* 1F */
			new( "JR NZ,%"           ,  7 , 12 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* 20 */
			new( "LD HL,@"           , 10 ,  0 ), /* 21 */
			new( "LD (@),HL"         , 16 ,  0 ), /* 22 */
			new( "INC HL"            ,  6 ,  0 ), /* 23 */
			new( "INC H"             ,  4 ,  0 ), /* 24 */
			new( "DEC H"             ,  4 ,  0 ), /* 25 */
			new( "LD H,#"            ,  7 ,  0 ), /* 26 */
			new( "DAA"               ,  4 ,  0 ), /* 27 */
			new( "JR Z,%"            ,  7 , 12 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* 28 */
			new( "ADD HL,HL"         , 11 ,  0 ), /* 29 */
			new( "LD HL,(@)"         , 16 ,  0 ), /* 2A */
			new( "DEC HL"            ,  6 ,  0 ), /* 2B */
			new( "INC L"             ,  4 ,  0 ), /* 2C */
			new( "DEC L"             ,  4 ,  0 ), /* 2D */
			new( "LD L,#"            ,  7 ,  0 ), /* 2E */
			new( "CPL"               ,  4 ,  0 ), /* 2F */
			new( "JR NC,%"           ,  7 , 12 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* 30 */
			new( "LD SP,@"           , 10 ,  0 ), /* 31 */
			new( "LD (@),A"          , 13 ,  0 ), /* 32 */
			new( "INC SP"            ,  6 ,  0 ), /* 33 */
			new( "INC (HL)"          , 11 ,  0 ), /* 34 */
			new( "DEC (HL)"          , 11 ,  0 ), /* 35 */
			new( "LD (HL),#"         , 10 ,  0 ), /* 36 */
			new( "SCF"               ,  4 ,  0 ), /* 37 */
			new( "JR C,%"            ,  7 , 12 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* 38 */
			new( "ADD HL,SP"         , 11 ,  0 ), /* 39 */
			new( "LD A,(@)"          , 13 ,  0 ), /* 3A */
			new( "DEC SP"            ,  6 ,  0 ), /* 3B */
			new( "INC A"             ,  4 ,  0 ), /* 3C */
			new( "DEC A"             ,  4 ,  0 ), /* 3D */
			new( "LD A,#"            ,  7 ,  0 ), /* 3E */
			new( "CCF"               ,  4 ,  0 ), /* 3F */
			new( "LD B,B"            ,  4 ,  0 ), /* 40 */
			new( "LD B,C"            ,  4 ,  0 ), /* 41 */
			new( "LD B,D"            ,  4 ,  0 ), /* 42 */
			new( "LD B,E"            ,  4 ,  0 ), /* 43 */
			new( "LD B,H"            ,  4 ,  0 ), /* 44 */
			new( "LD B,L"            ,  4 ,  0 ), /* 45 */
			new( "LD B,(HL)"         ,  7 ,  0 ), /* 46 */
			new( "LD B,A"            ,  4 ,  0 ), /* 47 */
			new( "LD C,B"            ,  4 ,  0 ), /* 48 */
			new( "LD C,C"            ,  4 ,  0 ), /* 49 */
			new( "LD C,D"            ,  4 ,  0 ), /* 4A */
			new( "LD C,E"            ,  4 ,  0 ), /* 4B */
			new( "LD C,H"            ,  4 ,  0 ), /* 4C */
			new( "LD C,L"            ,  4 ,  0 ), /* 4D */
			new( "LD C,(HL)"         ,  7 ,  0 ), /* 4E */
			new( "LD C,A"            ,  4 ,  0 ), /* 4F */
			new( "LD D,B"            ,  4 ,  0 ), /* 50 */
			new( "LD D,C"            ,  4 ,  0 ), /* 51 */
			new( "LD D,D"            ,  4 ,  0 ), /* 52 */
			new( "LD D,E"            ,  4 ,  0 ), /* 53 */
			new( "LD D,H"            ,  4 ,  0 ), /* 54 */
			new( "LD D,L"            ,  4 ,  0 ), /* 55 */
			new( "LD D,(HL)"         ,  7 ,  0 ), /* 56 */
			new( "LD D,A"            ,  4 ,  0 ), /* 57 */
			new( "LD E,B"            ,  4 ,  0 ), /* 58 */
			new( "LD E,C"            ,  4 ,  0 ), /* 59 */
			new( "LD E,D"            ,  4 ,  0 ), /* 5A */
			new( "LD E,E"            ,  4 ,  0 ), /* 5B */
			new( "LD E,H"            ,  4 ,  0 ), /* 5C */
			new( "LD E,L"            ,  4 ,  0 ), /* 5D */
			new( "LD E,(HL)"         ,  7 ,  0 ), /* 5E */
			new( "LD E,A"            ,  4 ,  0 ), /* 5F */
			new( "LD H,B"            ,  4 ,  0 ), /* 60 */
			new( "LD H,C"            ,  4 ,  0 ), /* 61 */
			new( "LD H,D"            ,  4 ,  0 ), /* 62 */
			new( "LD H,E"            ,  4 ,  0 ), /* 63 */
			new( "LD H,H"            ,  4 ,  0 ), /* 64 */
			new( "LD H,L"            ,  4 ,  0 ), /* 65 */
			new( "LD H,(HL)"         ,  7 ,  0 ), /* 66 */
			new( "LD H,A"            ,  4 ,  0 ), /* 67 */
			new( "LD L,B"            ,  4 ,  0 ), /* 68 */
			new( "LD L,C"            ,  4 ,  0 ), /* 69 */
			new( "LD L,D"            ,  4 ,  0 ), /* 6A */
			new( "LD L,E"            ,  4 ,  0 ), /* 6B */
			new( "LD L,H"            ,  4 ,  0 ), /* 6C */
			new( "LD L,L"            ,  4 ,  0 ), /* 6D */
			new( "LD L,(HL)"         ,  7 ,  0 ), /* 6E */
			new( "LD L,A"            ,  4 ,  0 ), /* 6F */
			new( "LD (HL),B"         ,  7 ,  0 ), /* 70 */
			new( "LD (HL),C"         ,  7 ,  0 ), /* 71 */
			new( "LD (HL),D"         ,  7 ,  0 ), /* 72 */
			new( "LD (HL),E"         ,  7 ,  0 ), /* 73 */
			new( "LD (HL),H"         ,  7 ,  0 ), /* 74 */
			new( "LD (HL),L"         ,  7 ,  0 ), /* 75 */
			new( "HALT"              ,  4 ,  0 ), /* 76 */
			new( "LD (HL),A"         ,  7 ,  0 ), /* 77 */
			new( "LD A,B"            ,  4 ,  0 ), /* 78 */
			new( "LD A,C"            ,  4 ,  0 ), /* 79 */
			new( "LD A,D"            ,  4 ,  0 ), /* 7A */
			new( "LD A,E"            ,  4 ,  0 ), /* 7B */
			new( "LD A,H"            ,  4 ,  0 ), /* 7C */
			new( "LD A,L"            ,  4 ,  0 ), /* 7D */
			new( "LD A,(HL)"         ,  7 ,  0 ), /* 7E */
			new( "LD A,A"            ,  4 ,  0 ), /* 7F */
			new( "ADD A,B"           ,  4 ,  0 ), /* 80 */
			new( "ADD A,C"           ,  4 ,  0 ), /* 81 */
			new( "ADD A,D"           ,  4 ,  0 ), /* 82 */
			new( "ADD A,E"           ,  4 ,  0 ), /* 83 */
			new( "ADD A,H"           ,  4 ,  0 ), /* 84 */
			new( "ADD A,L"           ,  4 ,  0 ), /* 85 */
			new( "ADD A,(HL)"        ,  7 ,  0 ), /* 86 */
			new( "ADD A,A"           ,  4 ,  0 ), /* 87 */
			new( "ADC A,B"           ,  4 ,  0 ), /* 88 */
			new( "ADC A,C"           ,  4 ,  0 ), /* 89 */
			new( "ADC A,D"           ,  4 ,  0 ), /* 8A */
			new( "ADC A,E"           ,  4 ,  0 ), /* 8B */
			new( "ADC A,H"           ,  4 ,  0 ), /* 8C */
			new( "ADC A,L"           ,  4 ,  0 ), /* 8D */
			new( "ADC A,(HL)"        ,  7 ,  0 ), /* 8E */
			new( "ADC A,A"           ,  4 ,  0 ), /* 8F */
			new( "SUB B"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues ), /* 90 */
			new( "SUB C"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues ), /* 91 */
			new( "SUB D"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues ), /* 92 */
			new( "SUB E"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues ), /* 93 */
			new( "SUB H"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues ), /* 94 */
			new( "SUB L"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues ), /* 95 */
			new( "SUB (HL)"          ,  7 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues ), /* 96 */
			new( "SUB A"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues ), /* 97 */
			new( "SBC A,B"           ,  4 ,  0 ), /* 98 */
			new( "SBC A,C"           ,  4 ,  0 ), /* 99 */
			new( "SBC A,D"           ,  4 ,  0 ), /* 9A */
			new( "SBC A,E"           ,  4 ,  0 ), /* 9B */
			new( "SBC A,H"           ,  4 ,  0 ), /* 9C */
			new( "SBC A,L"           ,  4 ,  0 ), /* 9D */
			new( "SBC A,(HL)"        ,  7 ,  0 ), /* 9E */
			new( "SBC A,A"           ,  4 ,  0 ), /* 9F */
			new( "AND B"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* A0 */
			new( "AND C"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* A1 */
			new( "AND D"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* A2 */
			new( "AND E"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* A3 */
			new( "AND H"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* A4 */
			new( "AND L"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* A5 */
			new( "AND (HL)"          ,  7 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* A6 */
			new( "AND A"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* A7 */
			new( "XOR B"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* A8 */
			new( "XOR C"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* A9 */
			new( "XOR D"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* AA */
			new( "XOR E"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* AB */
			new( "XOR H"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* AC */
			new( "XOR L"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* AD */
			new( "XOR (HL)"          ,  7 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* AE */
			new( "XOR A"             ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* AF */
			new( "OR B"              ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* B0 */
			new( "OR C"              ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* B1 */
			new( "OR D"              ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* B2 */
			new( "OR E"              ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* B3 */
			new( "OR H"              ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* B4 */
			new( "OR L"              ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* B5 */
			new( "OR (HL)"           ,  7 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* B6 */
			new( "OR A"              ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* B7 */
			new( "CP B"              ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* B8 */
			new( "CP C"              ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* B9 */
			new( "CP D"              ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* BA */
			new( "CP E"              ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* BB */
			new( "CP H"              ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* BC */
			new( "CP L"              ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* BD */
			new( "CP (HL)"           ,  7 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* BE */
			new( "CP A"              ,  4 ,  0, OpCodeFlags.ImplicitA | OpCodeFlags.Continues  ), /* BF */
			new( "RET NZ"            ,  5 , 11 , OpCodeFlags.Returns | OpCodeFlags.Continues), /* C0 */
			new( "POP BC"            , 10 ,  0 ), /* C1 */
			new( "JP NZ,@"           , 10 ,  0 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* C2 */
			new( "JP @"              , 10 ,  0 , OpCodeFlags.Jumps), /* C3 */
			new( "CALL NZ,@"         , 10 , 17 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* C4 */
			new( "PUSH BC"           , 11 ,  0 ), /* C5 */
			new( "ADD A,#"           ,  7 ,  0 ), /* C6 */
			new( "RST 0x00"          , 11 ,  0 , OpCodeFlags.Restarts | OpCodeFlags.Continues), /* C7 */
			new( "RET Z"             ,  5 , 11 , OpCodeFlags.Returns | OpCodeFlags.Continues), /* C8 */
			new( "RET"               , 10 ,  0 , OpCodeFlags.Returns ), /* C9 */
			new( "JP Z,@"            , 10 ,  0 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* CA */
			new( "shift CB"          ,  4 ,  0 ), /* CB */
			new( "CALL Z,@"          , 10 , 17 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* CC */
			new( "CALL @"            , 17 ,  0 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* CD */
			new( "ADC A,#"           ,  7 ,  0 ), /* CE */
			new( "RST 0x08"          , 11 ,  0 , OpCodeFlags.Restarts | OpCodeFlags.Continues), /* CF */
			new( "RET NC"            ,  5 , 11 , OpCodeFlags.Returns | OpCodeFlags.Continues), /* D0 */
			new( "POP DE"            , 10 ,  0 ), /* D1 */
			new( "JP NC,@"           , 10 ,  0 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* D2 */
			new( "OUT (#),A"         , 11 ,  0 ), /* D3 */
			new( "CALL NC,@"         , 10 , 17 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* D4 */
			new( "PUSH DE"           , 11 ,  0 ), /* D5 */
			new( "SUB #"             ,  7 ,  0 ), /* D6 */
			new( "RST 0x10"          , 11 ,  0 , OpCodeFlags.Restarts | OpCodeFlags.Continues), /* D7 */
			new( "RET C"             ,  5 , 11 , OpCodeFlags.Returns | OpCodeFlags.Continues), /* D8 */
			new( "EXX"               ,  4 ,  0 ), /* D9 */
			new( "JP C,@"            , 10 ,  0 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* DA */
			new( "IN A,(#)"          , 11 ,  0 ), /* DB */
			new( "CALL C,@"          , 10 , 17 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* DC */
			new( "shift DD"          ,  0 ,  0 ), /* DD */
			new( "SBC A,#"           ,  7 ,  0 ), /* DE */
			new( "RST 0x18"          , 11 ,  0 , OpCodeFlags.Restarts | OpCodeFlags.Continues), /* DF */
			new( "RET PO"            ,  5 , 11 , OpCodeFlags.Returns | OpCodeFlags.Continues), /* E0 */
			new( "POP HL"            , 10 ,  0 ), /* E1 */
			new( "JP PO,@"           , 10 ,  0 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* E2 */
			new( "EX (SP),HL"        , 19 ,  0 ), /* E3 */
			new( "CALL PO,@"         , 10 , 17 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* E4 */
			new( "PUSH HL"           , 11 ,  0 ), /* E5 */
			new( "AND #"             ,  7 ,  0 ), /* E6 */
			new( "RST 0x20"          , 11 ,  0 , OpCodeFlags.Restarts | OpCodeFlags.Continues), /* E7 */
			new( "RET PE"            ,  5 , 11 , OpCodeFlags.Returns | OpCodeFlags.Continues), /* E8 */
			new( "JP (HL)"             ,  4 ,  0 , OpCodeFlags.Jumps), /* E9 */
			new( "JP PE,@"           , 10 ,  0 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* EA */
			new( "EX DE,HL"          ,  4 ,  0 ), /* EB */
			new( "CALL PE,@"         , 10 , 17 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* EC */
			new( "shift ED"          ,  0 ,  0 ), /* ED */
			new( "XOR #"             ,  7 ,  0 ), /* EE */
			new( "RST 0x28"          , 11 ,  0 , OpCodeFlags.Restarts | OpCodeFlags.Continues), /* EF */
			new( "RET P"             ,  5 , 11 , OpCodeFlags.Returns | OpCodeFlags.Continues), /* F0 */
			new( "POP AF"            , 10 ,  0 ), /* F1 */
			new( "JP P,@"            , 10 ,  0 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* F2 */
			new( "DI"                ,  4 ,  0 ), /* F3 */
			new( "CALL P,@"          , 10 , 17 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* F4 */
			new( "PUSH AF"           , 11 ,  0 ), /* F5 */
			new( "OR #"              ,  7 ,  0 ), /* F6 */
			new( "RST 0x30"          , 11 ,  0 , OpCodeFlags.Restarts | OpCodeFlags.Continues), /* F7 */
			new( "RET M"             ,  5 , 11 , OpCodeFlags.Returns | OpCodeFlags.Continues), /* F8 */
			new( "LD SP,HL"          ,  6 ,  0 ), /* F9 */
			new( "JP M,@"            , 10 ,  0 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* FA */
			new( "EI"                ,  4 ,  0 ), /* FB */
			new( "CALL M,@"          , 10 , 17 , OpCodeFlags.Jumps | OpCodeFlags.Continues), /* FC */
			new( "shift FD"          ,  4 ,  0 ), /* FD */
			new( "CP #"              ,  7 ,  0 ), /* FE */
			new( "RST 0x38"          , 11 ,  0 , OpCodeFlags.Restarts | OpCodeFlags.Continues), /* FF */
    ]);

    public static ReadOnlyCollection<OpCode> DasmCB = new ReadOnlyCollection<OpCode>(
    [   new( "RLC B"             ,  8 ,  0 ), /* 00 */
			new( "RLC C"             ,  8 ,  0 ), /* 01 */
			new( "RLC D"             ,  8 ,  0 ), /* 02 */
			new( "RLC E"             ,  8 ,  0 ), /* 03 */
			new( "RLC H"             ,  8 ,  0 ), /* 04 */
			new( "RLC L"             ,  8 ,  0 ), /* 05 */
			new( "RLC (HL)"          , 15 ,  0 ), /* 06 */
			new( "RLC A"             ,  8 ,  0 ), /* 07 */
			new( "RRC B"             ,  8 ,  0 ), /* 08 */
			new( "RRC C"             ,  8 ,  0 ), /* 09 */
			new( "RRC D"             ,  8 ,  0 ), /* 0A */
			new( "RRC E"             ,  8 ,  0 ), /* 0B */
			new( "RRC H"             ,  8 ,  0 ), /* 0C */
			new( "RRC L"             ,  8 ,  0 ), /* 0D */
			new( "RRC (HL)"          , 15 ,  0 ), /* 0E */
			new( "RRC A"             ,  8 ,  0 ), /* 0F */
			new( "RL B"              ,  8 ,  0 ), /* 10 */
			new( "RL C"              ,  8 ,  0 ), /* 11 */
			new( "RL D"              ,  8 ,  0 ), /* 12 */
			new( "RL E"              ,  8 ,  0 ), /* 13 */
			new( "RL H"              ,  8 ,  0 ), /* 14 */
			new( "RL L"              ,  8 ,  0 ), /* 15 */
			new( "RL (HL)"           , 15 ,  0 ), /* 16 */
			new( "RL A"              ,  8 ,  0 ), /* 17 */
			new( "RR B"              ,  8 ,  0 ), /* 18 */
			new( "RR C"              ,  8 ,  0 ), /* 19 */
			new( "RR D"              ,  8 ,  0 ), /* 1A */
			new( "RR E"              ,  8 ,  0 ), /* 1B */
			new( "RR H"              ,  8 ,  0 ), /* 1C */
			new( "RR L"              ,  8 ,  0 ), /* 1D */
			new( "RR (HL)"           , 15 ,  0 ), /* 1E */
			new( "RR A"              ,  8 ,  0 ), /* 1F */
			new( "SLA B"             ,  8 ,  0 ), /* 20 */
			new( "SLA C"             ,  8 ,  0 ), /* 21 */
			new( "SLA D"             ,  8 ,  0 ), /* 22 */
			new( "SLA E"             ,  8 ,  0 ), /* 23 */
			new( "SLA H"             ,  8 ,  0 ), /* 24 */
			new( "SLA L"             ,  8 ,  0 ), /* 25 */
			new( "SLA (HL)"          , 15 ,  0 ), /* 26 */
			new( "SLA A"             ,  8 ,  0 ), /* 27 */
			new( "SRA B"             ,  8 ,  0 ), /* 28 */
			new( "SRA C"             ,  8 ,  0 ), /* 29 */
			new( "SRA D"             ,  8 ,  0 ), /* 2A */
			new( "SRA E"             ,  8 ,  0 ), /* 2B */
			new( "SRA H"             ,  8 ,  0 ), /* 2C */
			new( "SRA L"             ,  8 ,  0 ), /* 2D */
			new( "SRA (HL)"          , 15 ,  0 ), /* 2E */
			new( "SRA A"             ,  8 ,  0 ), /* 2F */
			new( "SLL B"             ,  8 ,  0 ), /* 30 */
			new( "SLL C"             ,  8 ,  0 ), /* 31 */
			new( "SLL D"             ,  8 ,  0 ), /* 32 */
			new( "SLL E"             ,  8 ,  0 ), /* 33 */
			new( "SLL H"             ,  8 ,  0 ), /* 34 */
			new( "SLL L"             ,  8 ,  0 ), /* 35 */
			new( "SLL (HL)"          , 15 ,  0 ), /* 36 */
			new( "SLL A"             ,  8 ,  0 ), /* 37 */
			new( "SRL B"             ,  8 ,  0 ), /* 38 */
			new( "SRL C"             ,  8 ,  0 ), /* 39 */
			new( "SRL D"             ,  8 ,  0 ), /* 3A */
			new( "SRL E"             ,  8 ,  0 ), /* 3B */
			new( "SRL H"             ,  8 ,  0 ), /* 3C */
			new( "SRL L"             ,  8 ,  0 ), /* 3D */
			new( "SRL (HL)"          , 15 ,  0 ), /* 3E */
			new( "SRL A"             ,  8 ,  0 ), /* 3F */
			new( "BIT 0,B"           ,  8 ,  0 ), /* 40 */
			new( "BIT 0,C"           ,  8 ,  0 ), /* 41 */
			new( "BIT 0,D"           ,  8 ,  0 ), /* 42 */
			new( "BIT 0,E"           ,  8 ,  0 ), /* 43 */
			new( "BIT 0,H"           ,  8 ,  0 ), /* 44 */
			new( "BIT 0,L"           ,  8 ,  0 ), /* 45 */
			new( "BIT 0,(HL)"        , 12 ,  0 ), /* 46 */
			new( "BIT 0,A"           ,  8 ,  0 ), /* 47 */
			new( "BIT 1,B"           ,  8 ,  0 ), /* 48 */
			new( "BIT 1,C"           ,  8 ,  0 ), /* 49 */
			new( "BIT 1,D"           ,  8 ,  0 ), /* 4A */
			new( "BIT 1,E"           ,  8 ,  0 ), /* 4B */
			new( "BIT 1,H"           ,  8 ,  0 ), /* 4C */
			new( "BIT 1,L"           ,  8 ,  0 ), /* 4D */
			new( "BIT 1,(HL)"        , 12 ,  0 ), /* 4E */
			new( "BIT 1,A"           ,  8 ,  0 ), /* 4F */
			new( "BIT 2,B"           ,  8 ,  0 ), /* 50 */
			new( "BIT 2,C"           ,  8 ,  0 ), /* 51 */
			new( "BIT 2,D"           ,  8 ,  0 ), /* 52 */
			new( "BIT 2,E"           ,  8 ,  0 ), /* 53 */
			new( "BIT 2,H"           ,  8 ,  0 ), /* 54 */
			new( "BIT 2,L"           ,  8 ,  0 ), /* 55 */
			new( "BIT 2,(HL)"        , 12 ,  0 ), /* 56 */
			new( "BIT 2,A"           ,  8 ,  0 ), /* 57 */
			new( "BIT 3,B"           ,  8 ,  0 ), /* 58 */
			new( "BIT 3,C"           ,  8 ,  0 ), /* 59 */
			new( "BIT 3,D"           ,  8 ,  0 ), /* 5A */
			new( "BIT 3,E"           ,  8 ,  0 ), /* 5B */
			new( "BIT 3,H"           ,  8 ,  0 ), /* 5C */
			new( "BIT 3,L"           ,  8 ,  0 ), /* 5D */
			new( "BIT 3,(HL)"        , 12 ,  0 ), /* 5E */
			new( "BIT 3,A"           ,  8 ,  0 ), /* 5F */
			new( "BIT 4,B"           ,  8 ,  0 ), /* 60 */
			new( "BIT 4,C"           ,  8 ,  0 ), /* 61 */
			new( "BIT 4,D"           ,  8 ,  0 ), /* 62 */
			new( "BIT 4,E"           ,  8 ,  0 ), /* 63 */
			new( "BIT 4,H"           ,  8 ,  0 ), /* 64 */
			new( "BIT 4,L"           ,  8 ,  0 ), /* 65 */
			new( "BIT 4,(HL)"        , 12 ,  0 ), /* 66 */
			new( "BIT 4,A"           ,  8 ,  0 ), /* 67 */
			new( "BIT 5,B"           ,  8 ,  0 ), /* 68 */
			new( "BIT 5,C"           ,  8 ,  0 ), /* 69 */
			new( "BIT 5,D"           ,  8 ,  0 ), /* 6A */
			new( "BIT 5,E"           ,  8 ,  0 ), /* 6B */
			new( "BIT 5,H"           ,  8 ,  0 ), /* 6C */
			new( "BIT 5,L"           ,  8 ,  0 ), /* 6D */
			new( "BIT 5,(HL)"        , 12 ,  0 ), /* 6E */
			new( "BIT 5,A"           ,  8 ,  0 ), /* 6F */
			new( "BIT 6,B"           ,  8 ,  0 ), /* 70 */
			new( "BIT 6,C"           ,  8 ,  0 ), /* 71 */
			new( "BIT 6,D"           ,  8 ,  0 ), /* 72 */
			new( "BIT 6,E"           ,  8 ,  0 ), /* 73 */
			new( "BIT 6,H"           ,  8 ,  0 ), /* 74 */
			new( "BIT 6,L"           ,  8 ,  0 ), /* 75 */
			new( "BIT 6,(HL)"        , 12 ,  0 ), /* 76 */
			new( "BIT 6,A"           ,  8 ,  0 ), /* 77 */
			new( "BIT 7,B"           ,  8 ,  0 ), /* 78 */
			new( "BIT 7,C"           ,  8 ,  0 ), /* 79 */
			new( "BIT 7,D"           ,  8 ,  0 ), /* 7A */
			new( "BIT 7,E"           ,  8 ,  0 ), /* 7B */
			new( "BIT 7,H"           ,  8 ,  0 ), /* 7C */
			new( "BIT 7,L"           ,  8 ,  0 ), /* 7D */
			new( "BIT 7,(HL)"        , 12 ,  0 ), /* 7E */
			new( "BIT 7,A"           ,  8 ,  0 ), /* 7F */
			new( "RES 0,B"           ,  8 ,  0 ), /* 80 */
			new( "RES 0,C"           ,  8 ,  0 ), /* 81 */
			new( "RES 0,D"           ,  8 ,  0 ), /* 82 */
			new( "RES 0,E"           ,  8 ,  0 ), /* 83 */
			new( "RES 0,H"           ,  8 ,  0 ), /* 84 */
			new( "RES 0,L"           ,  8 ,  0 ), /* 85 */
			new( "RES 0,(HL)"        , 15 ,  0 ), /* 86 */
			new( "RES 0,A"           ,  8 ,  0 ), /* 87 */
			new( "RES 1,B"           ,  8 ,  0 ), /* 88 */
			new( "RES 1,C"           ,  8 ,  0 ), /* 89 */
			new( "RES 1,D"           ,  8 ,  0 ), /* 8A */
			new( "RES 1,E"           ,  8 ,  0 ), /* 8B */
			new( "RES 1,H"           ,  8 ,  0 ), /* 8C */
			new( "RES 1,L"           ,  8 ,  0 ), /* 8D */
			new( "RES 1,(HL)"        , 15 ,  0 ), /* 8E */
			new( "RES 1,A"           ,  8 ,  0 ), /* 8F */
			new( "RES 2,B"           ,  8 ,  0 ), /* 90 */
			new( "RES 2,C"           ,  8 ,  0 ), /* 91 */
			new( "RES 2,D"           ,  8 ,  0 ), /* 92 */
			new( "RES 2,E"           ,  8 ,  0 ), /* 93 */
			new( "RES 2,H"           ,  8 ,  0 ), /* 94 */
			new( "RES 2,L"           ,  8 ,  0 ), /* 95 */
			new( "RES 2,(HL)"        , 15 ,  0 ), /* 96 */
			new( "RES 2,A"           ,  8 ,  0 ), /* 97 */
			new( "RES 3,B"           ,  8 ,  0 ), /* 98 */
			new( "RES 3,C"           ,  8 ,  0 ), /* 99 */
			new( "RES 3,D"           ,  8 ,  0 ), /* 9A */
			new( "RES 3,E"           ,  8 ,  0 ), /* 9B */
			new( "RES 3,H"           ,  8 ,  0 ), /* 9C */
			new( "RES 3,L"           ,  8 ,  0 ), /* 9D */
			new( "RES 3,(HL)"        , 15 ,  0 ), /* 9E */
			new( "RES 3,A"           ,  8 ,  0 ), /* 9F */
			new( "RES 4,B"           ,  8 ,  0 ), /* A0 */
			new( "RES 4,C"           ,  8 ,  0 ), /* A1 */
			new( "RES 4,D"           ,  8 ,  0 ), /* A2 */
			new( "RES 4,E"           ,  8 ,  0 ), /* A3 */
			new( "RES 4,H"           ,  8 ,  0 ), /* A4 */
			new( "RES 4,L"           ,  8 ,  0 ), /* A5 */
			new( "RES 4,(HL)"        , 15 ,  0 ), /* A6 */
			new( "RES 4,A"           ,  8 ,  0 ), /* A7 */
			new( "RES 5,B"           ,  8 ,  0 ), /* A8 */
			new( "RES 5,C"           ,  8 ,  0 ), /* A9 */
			new( "RES 5,D"           ,  8 ,  0 ), /* AA */
			new( "RES 5,E"           ,  8 ,  0 ), /* AB */
			new( "RES 5,H"           ,  8 ,  0 ), /* AC */
			new( "RES 5,L"           ,  8 ,  0 ), /* AD */
			new( "RES 5,(HL)"        , 15 ,  0 ), /* AE */
			new( "RES 5,A"           ,  8 ,  0 ), /* AF */
			new( "RES 6,B"           ,  8 ,  0 ), /* B0 */
			new( "RES 6,C"           ,  8 ,  0 ), /* B1 */
			new( "RES 6,D"           ,  8 ,  0 ), /* B2 */
			new( "RES 6,E"           ,  8 ,  0 ), /* B3 */
			new( "RES 6,H"           ,  8 ,  0 ), /* B4 */
			new( "RES 6,L"           ,  8 ,  0 ), /* B5 */
			new( "RES 6,(HL)"        , 15 ,  0 ), /* B6 */
			new( "RES 6,A"           ,  8 ,  0 ), /* B7 */
			new( "RES 7,B"           ,  8 ,  0 ), /* B8 */
			new( "RES 7,C"           ,  8 ,  0 ), /* B9 */
			new( "RES 7,D"           ,  8 ,  0 ), /* BA */
			new( "RES 7,E"           ,  8 ,  0 ), /* BB */
			new( "RES 7,H"           ,  8 ,  0 ), /* BC */
			new( "RES 7,L"           ,  8 ,  0 ), /* BD */
			new( "RES 7,(HL)"        , 15 ,  0 ), /* BE */
			new( "RES 7,A"           ,  8 ,  0 ), /* BF */
			new( "SET 0,B"           ,  8 ,  0 ), /* C0 */
			new( "SET 0,C"           ,  8 ,  0 ), /* C1 */
			new( "SET 0,D"           ,  8 ,  0 ), /* C2 */
			new( "SET 0,E"           ,  8 ,  0 ), /* C3 */
			new( "SET 0,H"           ,  8 ,  0 ), /* C4 */
			new( "SET 0,L"           ,  8 ,  0 ), /* C5 */
			new( "SET 0,(HL)"        , 15 ,  0 ), /* C6 */
			new( "SET 0,A"           ,  8 ,  0 ), /* C7 */
			new( "SET 1,B"           ,  8 ,  0 ), /* C8 */
			new( "SET 1,C"           ,  8 ,  0 ), /* C9 */
			new( "SET 1,D"           ,  8 ,  0 ), /* CA */
			new( "SET 1,E"           ,  8 ,  0 ), /* CB */
			new( "SET 1,H"           ,  8 ,  0 ), /* CC */
			new( "SET 1,L"           ,  8 ,  0 ), /* CD */
			new( "SET 1,(HL)"        , 15 ,  0 ), /* CE */
			new( "SET 1,A"           ,  8 ,  0 ), /* CF */
			new( "SET 2,B"           ,  8 ,  0 ), /* D0 */
			new( "SET 2,C"           ,  8 ,  0 ), /* D1 */
			new( "SET 2,D"           ,  8 ,  0 ), /* D2 */
			new( "SET 2,E"           ,  8 ,  0 ), /* D3 */
			new( "SET 2,H"           ,  8 ,  0 ), /* D4 */
			new( "SET 2,L"           ,  8 ,  0 ), /* D5 */
			new( "SET 2,(HL)"        , 15 ,  0 ), /* D6 */
			new( "SET 2,A"           ,  8 ,  0 ), /* D7 */
			new( "SET 3,B"           ,  8 ,  0 ), /* D8 */
			new( "SET 3,C"           ,  8 ,  0 ), /* D9 */
			new( "SET 3,D"           ,  8 ,  0 ), /* DA */
			new( "SET 3,E"           ,  8 ,  0 ), /* DB */
			new( "SET 3,H"           ,  8 ,  0 ), /* DC */
			new( "SET 3,L"           ,  8 ,  0 ), /* DD */
			new( "SET 3,(HL)"        , 15 ,  0 ), /* DE */
			new( "SET 3,A"           ,  8 ,  0 ), /* DF */
			new( "SET 4,B"           ,  8 ,  0 ), /* E0 */
			new( "SET 4,C"           ,  8 ,  0 ), /* E1 */
			new( "SET 4,D"           ,  8 ,  0 ), /* E2 */
			new( "SET 4,E"           ,  8 ,  0 ), /* E3 */
			new( "SET 4,H"           ,  8 ,  0 ), /* E4 */
			new( "SET 4,L"           ,  8 ,  0 ), /* E5 */
			new( "SET 4,(HL)"        , 15 ,  0 ), /* E6 */
			new( "SET 4,A"           ,  8 ,  0 ), /* E7 */
			new( "SET 5,B"           ,  8 ,  0 ), /* E8 */
			new( "SET 5,C"           ,  8 ,  0 ), /* E9 */
			new( "SET 5,D"           ,  8 ,  0 ), /* EA */
			new( "SET 5,E"           ,  8 ,  0 ), /* EB */
			new( "SET 5,H"           ,  8 ,  0 ), /* EC */
			new( "SET 5,L"           ,  8 ,  0 ), /* ED */
			new( "SET 5,(HL)"        , 15 ,  0 ), /* EE */
			new( "SET 5,A"           ,  8 ,  0 ), /* EF */
			new( "SET 6,B"           ,  8 ,  0 ), /* F0 */
			new( "SET 6,C"           ,  8 ,  0 ), /* F1 */
			new( "SET 6,D"           ,  8 ,  0 ), /* F2 */
			new( "SET 6,E"           ,  8 ,  0 ), /* F3 */
			new( "SET 6,H"           ,  8 ,  0 ), /* F4 */
			new( "SET 6,L"           ,  8 ,  0 ), /* F5 */
			new( "SET 6,(HL)"        , 15 ,  0 ), /* F6 */
			new( "SET 6,A"           ,  8 ,  0 ), /* F7 */
			new( "SET 7,B"           ,  8 ,  0 ), /* F8 */
			new( "SET 7,C"           ,  8 ,  0 ), /* F9 */
			new( "SET 7,D"           ,  8 ,  0 ), /* FA */
			new( "SET 7,E"           ,  8 ,  0 ), /* FB */
			new( "SET 7,H"           ,  8 ,  0 ), /* FC */
			new( "SET 7,L"           ,  8 ,  0 ), /* FD */
			new( "SET 7,(HL)"        , 15 ,  0 ), /* FE */
			new( "SET 7,A"           ,  8 ,  0 ), /* FF */
	]);

    public static ReadOnlyCollection<OpCode> DasmDD = new ReadOnlyCollection<OpCode>(
    [
        new( null                ,  0 ,  0 ), /* 00 */
			new( null                ,  0 ,  0 ), /* 01 */
			new( null                ,  0 ,  0 ), /* 02 */
			new( null                ,  0 ,  0 ), /* 03 */
			new( null                ,  0 ,  0 ), /* 04 */
			new( null                ,  0 ,  0 ), /* 05 */
			new( null                ,  0 ,  0 ), /* 06 */
			new( null                ,  0 ,  0 ), /* 07 */
			new( null                ,  0 ,  0 ), /* 08 */
			new( "ADD IX,BC"         , 15 ,  0 ), /* 09 */
			new( null                ,  0 ,  0 ), /* 0A */
			new( null                ,  0 ,  0 ), /* 0B */
			new( null                ,  0 ,  0 ), /* 0C */
			new( null                ,  0 ,  0 ), /* 0D */
			new( null                ,  0 ,  0 ), /* 0E */
			new( null                ,  0 ,  0 ), /* 0F */
			new( null                ,  0 ,  0 ), /* 10 */
			new( null                ,  0 ,  0 ), /* 11 */
			new( null                ,  0 ,  0 ), /* 12 */
			new( null                ,  0 ,  0 ), /* 13 */
			new( null                ,  0 ,  0 ), /* 14 */
			new( null                ,  0 ,  0 ), /* 15 */
			new( null                ,  0 ,  0 ), /* 16 */
			new( null                ,  0 ,  0 ), /* 17 */
			new( null                ,  0 ,  0 ), /* 18 */
			new( "ADD IX,DE"         , 15 ,  0 ), /* 19 */
			new( null                ,  0 ,  0 ), /* 1A */
			new( null                ,  0 ,  0 ), /* 1B */
			new( null                ,  0 ,  0 ), /* 1C */
			new( null                ,  0 ,  0 ), /* 1D */
			new( null                ,  0 ,  0 ), /* 1E */
			new( null                ,  0 ,  0 ), /* 1F */
			new( null                ,  0 ,  0 ), /* 20 */
			new( "LD IX,@"           , 14 ,  0 ), /* 21 */
			new( "LD (@),IX"         , 20 ,  0 ), /* 22 */
			new( "INC IX"            , 10 ,  0 ), /* 23 */
			new( "INC IXH"           ,  8 ,  0 ), /* 24 */
			new( "DEC IXH"           ,  8 ,  0 ), /* 25 */
			new( "LD IXH,#"          , 11 ,  0 ), /* 26 */
			new( null                ,  0 ,  0 ), /* 27 */
			new( null                ,  0 ,  0 ), /* 28 */
			new( "ADD IX,IX"         , 15 ,  0 ), /* 29 */
			new( "LD IX,(@)"         , 20 ,  0 ), /* 2A */
			new( "DEC IX"            , 10 ,  0 ), /* 2B */
			new( "INC IXL"           ,  8 ,  0 ), /* 2C */
			new( "DEC IXL"           ,  8 ,  0 ), /* 2D */
			new( "LD IXL,#"          , 11 ,  0 ), /* 2E */
			new( null                ,  0 ,  0 ), /* 2F */
			new( null                ,  0 ,  0 ), /* 30 */
			new( null                ,  0 ,  0 ), /* 31 */
			new( null                ,  0 ,  0 ), /* 32 */
			new( null                ,  0 ,  0 ), /* 33 */
			new( "INC (IX+$)"        , 23 ,  0 ), /* 34 */
			new( "DEC (IX+$)"        , 23 ,  0 ), /* 35 */
			new( "LD (IX+$),#"       , 19 ,  0 ), /* 36 */
			new( null                ,  0 ,  0 ), /* 37 */
			new( null                ,  0 ,  0 ), /* 38 */
			new( "ADD IX,SP"         , 15 ,  0 ), /* 39 */
			new( null                ,  0 ,  0 ), /* 3A */
			new( null                ,  0 ,  0 ), /* 3B */
			new( null                ,  0 ,  0 ), /* 3C */
			new( null                ,  0 ,  0 ), /* 3D */
			new( null                ,  0 ,  0 ), /* 3E */
			new( null                ,  0 ,  0 ), /* 3F */
			new( null                ,  0 ,  0 ), /* 40 */
			new( null                ,  0 ,  0 ), /* 41 */
			new( null                ,  0 ,  0 ), /* 42 */
			new( null                ,  0 ,  0 ), /* 43 */
			new( "LD B,IXH"          ,  8 ,  0 ), /* 44 */
			new( "LD B,IXL"          ,  8 ,  0 ), /* 45 */
			new( "LD B,(IX+$)"       , 19 ,  0 ), /* 46 */
			new( null                ,  0 ,  0 ), /* 47 */
			new( null                ,  0 ,  0 ), /* 48 */
			new( null                ,  0 ,  0 ), /* 49 */
			new( null                ,  0 ,  0 ), /* 4A */
			new( null                ,  0 ,  0 ), /* 4B */
			new( "LD C,IXH"          ,  8 ,  0 ), /* 4C */
			new( "LD C,IXL"          ,  8 ,  0 ), /* 4D */
			new( "LD C,(IX+$)"       , 19 ,  0 ), /* 4E */
			new( null                ,  0 ,  0 ), /* 4F */
			new( null                ,  0 ,  0 ), /* 50 */
			new( null                ,  0 ,  0 ), /* 51 */
			new( null                ,  0 ,  0 ), /* 52 */
			new( null                ,  0 ,  0 ), /* 53 */
			new( "LD D,IXH"          ,  8 ,  0 ), /* 54 */
			new( "LD D,IXL"          ,  8 ,  0 ), /* 55 */
			new( "LD D,(IX+$)"       , 19 ,  0 ), /* 56 */
			new( null                ,  0 ,  0 ), /* 57 */
			new( null                ,  0 ,  0 ), /* 58 */
			new( null                ,  0 ,  0 ), /* 59 */
			new( null                ,  0 ,  0 ), /* 5A */
			new( null                ,  0 ,  0 ), /* 5B */
			new( "LD E,IXH"          ,  8 ,  0 ), /* 5C */
			new( "LD E,IXL"          ,  8 ,  0 ), /* 5D */
			new( "LD E,(IX+$)"       , 19 ,  0 ), /* 5E */
			new( null                ,  0 ,  0 ), /* 5F */
			new( "LD IXH,B"          ,  8 ,  0 ), /* 60 */
			new( "LD IXH,C"          ,  8 ,  0 ), /* 61 */
			new( "LD IXH,D"          ,  8 ,  0 ), /* 62 */
			new( "LD IXH,E"          ,  8 ,  0 ), /* 63 */
			new( "LD IXH,IXH"        ,  8 ,  0 ), /* 64 */
			new( "LD IXH,IXL"        ,  8 ,  0 ), /* 65 */
			new( "LD H,(IX+$)"       , 19 ,  0 ), /* 66 */
			new( "LD IXH,A"          ,  8 ,  0 ), /* 67 */
			new( "LD IXL,B"          ,  8 ,  0 ), /* 68 */
			new( "LD IXL,C"          ,  8 ,  0 ), /* 69 */
			new( "LD IXL,D"          ,  8 ,  0 ), /* 6A */
			new( "LD IXL,E"          ,  8 ,  0 ), /* 6B */
			new( "LD IXL,IXH"        ,  8 ,  0 ), /* 6C */
			new( "LD IXL,IXL"        ,  8 ,  0 ), /* 6D */
			new( "LD L,(IX+$)"       , 19 ,  0 ), /* 6E */
			new( "LD IXL,A"          ,  8 ,  0 ), /* 6F */
			new( "LD (IX+$),B"       , 19 ,  0 ), /* 70 */
			new( "LD (IX+$),C"       , 19 ,  0 ), /* 71 */
			new( "LD (IX+$),D"       , 19 ,  0 ), /* 72 */
			new( "LD (IX+$),E"       , 19 ,  0 ), /* 73 */
			new( "LD (IX+$),H"       , 19 ,  0 ), /* 74 */
			new( "LD (IX+$),L"       , 19 ,  0 ), /* 75 */
			new( null                ,  0 ,  0 ), /* 76 */
			new( "LD (IX+$),A"       , 19 ,  0 ), /* 77 */
			new( null                ,  0 ,  0 ), /* 78 */
			new( null                ,  0 ,  0 ), /* 79 */
			new( null                ,  0 ,  0 ), /* 7A */
			new( null                ,  0 ,  0 ), /* 7B */
			new( "LD A,IXH"          ,  8 ,  0 ), /* 7C */
			new( "LD A,IXL"          ,  8 ,  0 ), /* 7D */
			new( "LD A,(IX+$)"       , 19 ,  0 ), /* 7E */
			new( null                ,  0 ,  0 ), /* 7F */
			new( null                ,  0 ,  0 ), /* 80 */
			new( null                ,  0 ,  0 ), /* 81 */
			new( null                ,  0 ,  0 ), /* 82 */
			new( null                ,  0 ,  0 ), /* 83 */
			new( "ADD A,IXH"         ,  8 ,  0 ), /* 84 */
			new( "ADD A,IXL"         ,  8 ,  0 ), /* 85 */
			new( "ADD A,(IX+$)"      , 19 ,  0 ), /* 86 */
			new( null                ,  0 ,  0 ), /* 87 */
			new( null                ,  0 ,  0 ), /* 88 */
			new( null                ,  0 ,  0 ), /* 89 */
			new( null                ,  0 ,  0 ), /* 8A */
			new( null                ,  0 ,  0 ), /* 8B */
			new( "ADC A,IXH"         ,  8 ,  0 ), /* 8C */
			new( "ADC A,IXL"         ,  8 ,  0 ), /* 8D */
			new( "ADC A,(IX+$)"      , 19 ,  0 ), /* 8E */
			new( null                ,  0 ,  0 ), /* 8F */
			new( null                ,  0 ,  0 ), /* 90 */
			new( null                ,  0 ,  0 ), /* 91 */
			new( null                ,  0 ,  0 ), /* 92 */
			new( null                ,  0 ,  0 ), /* 93 */
			new( "SUB IXH"           ,  8 ,  0 ), /* 94 */
			new( "SUB IXL"           ,  8 ,  0 ), /* 95 */
			new( "SUB (IX+$)"        , 19 ,  0 ), /* 96 */
			new( null                ,  0 ,  0 ), /* 97 */
			new( null                ,  0 ,  0 ), /* 98 */
			new( null                ,  0 ,  0 ), /* 99 */
			new( null                ,  0 ,  0 ), /* 9A */
			new( null                ,  0 ,  0 ), /* 9B */
			new( "SBC A,IXH"         ,  8 ,  0 ), /* 9C */
			new( "SBC A,IXL"         ,  8 ,  0 ), /* 9D */
			new( "SBC A,(IX+$)"      , 19 ,  0 ), /* 9E */
			new( null                ,  0 ,  0 ), /* 9F */
			new( null                ,  0 ,  0 ), /* A0 */
			new( null                ,  0 ,  0 ), /* A1 */
			new( null                ,  0 ,  0 ), /* A2 */
			new( null                ,  0 ,  0 ), /* A3 */
			new( "AND IXH"           ,  8 ,  0 ), /* A4 */
			new( "AND IXL"           ,  8 ,  0 ), /* A5 */
			new( "AND (IX+$)"        , 19 ,  0 ), /* A6 */
			new( null                ,  0 ,  0 ), /* A7 */
			new( null                ,  0 ,  0 ), /* A8 */
			new( null                ,  0 ,  0 ), /* A9 */
			new( null                ,  0 ,  0 ), /* AA */
			new( null                ,  0 ,  0 ), /* AB */
			new( "XOR IXH"           ,  8 ,  0 ), /* AC */
			new( "XOR IXL"           ,  8 ,  0 ), /* AD */
			new( "XOR (IX+$)"        , 19 ,  0 ), /* AE */
			new( null                ,  0 ,  0 ), /* AF */
			new( null                ,  0 ,  0 ), /* B0 */
			new( null                ,  0 ,  0 ), /* B1 */
			new( null                ,  0 ,  0 ), /* B2 */
			new( null                ,  0 ,  0 ), /* B3 */
			new( "OR IXH"            ,  8 ,  0 ), /* B4 */
			new( "OR IXL"            ,  8 ,  0 ), /* B5 */
			new( "OR (IX+$)"         , 19 ,  0 ), /* B6 */
			new( null                ,  0 ,  0 ), /* B7 */
			new( null                ,  0 ,  0 ), /* B8 */
			new( null                ,  0 ,  0 ), /* B9 */
			new( null                ,  0 ,  0 ), /* BA */
			new( null                ,  0 ,  0 ), /* BB */
			new( "CP IXH"            ,  8 ,  0 ), /* BC */
			new( "CP IXL"            ,  8 ,  0 ), /* BD */
			new( "CP (IX+$)"         , 19 ,  0 ), /* BE */
			new( null                ,  0 ,  0 ), /* BF */
			new( null                ,  0 ,  0 ), /* C0 */
			new( null                ,  0 ,  0 ), /* C1 */
			new( null                ,  0 ,  0 ), /* C2 */
			new( null                ,  0 ,  0 ), /* C3 */
			new( null                ,  0 ,  0 ), /* C4 */
			new( null                ,  0 ,  0 ), /* C5 */
			new( null                ,  0 ,  0 ), /* C6 */
			new( null                ,  0 ,  0 ), /* C7 */
			new( null                ,  0 ,  0 ), /* C8 */
			new( null                ,  0 ,  0 ), /* C9 */
			new( null                ,  0 ,  0 ), /* CA */
			new( "shift CB"          ,  0 ,  0 ), /* CB */
			new( null                ,  0 ,  0 ), /* CC */
			new( null                ,  0 ,  0 ), /* CD */
			new( null                ,  0 ,  0 ), /* CE */
			new( null                ,  0 ,  0 ), /* CF */
			new( null                ,  0 ,  0 ), /* D0 */
			new( null                ,  0 ,  0 ), /* D1 */
			new( null                ,  0 ,  0 ), /* D2 */
			new( null                ,  0 ,  0 ), /* D3 */
			new( null                ,  0 ,  0 ), /* D4 */
			new( null                ,  0 ,  0 ), /* D5 */
			new( null                ,  0 ,  0 ), /* D6 */
			new( null                ,  0 ,  0 ), /* D7 */
			new( null                ,  0 ,  0 ), /* D8 */
			new( null                ,  0 ,  0 ), /* D9 */
			new( null                ,  0 ,  0 ), /* DA */
			new( null                ,  0 ,  0 ), /* DB */
			new( null                ,  0 ,  0 ), /* DC */
			new( "ignore"            ,  4 ,  0 ), /* DD */
			new( null                ,  0 ,  0 ), /* DE */
			new( null                ,  0 ,  0 ), /* DF */
			new( null                ,  0 ,  0 ), /* E0 */
			new( "POP IX"            , 14 ,  0 ), /* E1 */
			new( null                ,  0 ,  0 ), /* E2 */
			new( "EX (SP),IX"        , 23 ,  0 ), /* E3 */
			new( null                ,  0 ,  0 ), /* E4 */
			new( "PUSH IX"           , 15 ,  0 ), /* E5 */
			new( null                ,  0 ,  0 ), /* E6 */
			new( null                ,  0 ,  0 ), /* E7 */
			new( null                ,  0 ,  0 ), /* E8 */
			new( "JP (IX)"             ,  8 ,  0 , OpCodeFlags.Jumps), /* E9 */
			new( null                ,  0 ,  0 ), /* EA */
			new( null                ,  0 ,  0 ), /* EB */
			new( null                ,  0 ,  0 ), /* EC */
			new( null                ,  4 ,  0 ), /* ED */
			new( null                ,  0 ,  0 ), /* EE */
			new( null                ,  0 ,  0 ), /* EF */
			new( null                ,  0 ,  0 ), /* F0 */
			new( null                ,  0 ,  0 ), /* F1 */
			new( null                ,  0 ,  0 ), /* F2 */
			new( null                ,  0 ,  0 ), /* F3 */
			new( null                ,  0 ,  0 ), /* F4 */
			new( null                ,  0 ,  0 ), /* F5 */
			new( null                ,  0 ,  0 ), /* F6 */
			new( null                ,  0 ,  0 ), /* F7 */
			new( null                ,  0 ,  0 ), /* F8 */
			new( "LD SP,IX"          , 10 ,  0 ), /* F9 */
			new( null                ,  0 ,  0 ), /* FA */
			new( null                ,  0 ,  0 ), /* FB */
			new( null                ,  0 ,  0 ), /* FC */
			new( "ignore"            ,  4 ,  0 ), /* FD */
			new( null                ,  0 ,  0 ), /* FE */
			new( null                ,  0 ,  0 ), /* FF */
    ]);

    public static ReadOnlyCollection<OpCode> DasmDDCB = new ReadOnlyCollection<OpCode>(
    [
        new( "LD B,RLC (IX+$)"   , 23 ,  0 ), /* 00 */
			new( "LD C,RLC (IX+$)"   , 23 ,  0 ), /* 01 */
			new( "LD D,RLC (IX+$)"   , 23 ,  0 ), /* 02 */
			new( "LD E,RLC (IX+$)"   , 23 ,  0 ), /* 03 */
			new( "LD H,RLC (IX+$)"   , 23 ,  0 ), /* 04 */
			new( "LD L,RLC (IX+$)"   , 23 ,  0 ), /* 05 */
			new( "RLC (IX+$)"        , 23 ,  0 ), /* 06 */
			new( "LD A,RLC (IX+$)"   , 23 ,  0 ), /* 07 */
			new( "LD B,RRC (IX+$)"   , 23 ,  0 ), /* 08 */
			new( "LD C,RRC (IX+$)"   , 23 ,  0 ), /* 09 */
			new( "LD D,RRC (IX+$)"   , 23 ,  0 ), /* 0A */
			new( "LD E,RRC (IX+$)"   , 23 ,  0 ), /* 0B */
			new( "LD H,RRC (IX+$)"   , 23 ,  0 ), /* 0C */
			new( "LD L,RRC (IX+$)"   , 23 ,  0 ), /* 0D */
			new( "RRC (IX+$)"        , 23 ,  0 ), /* 0E */
			new( "LD A,RRC (IX+$)"   , 23 ,  0 ), /* 0F */
			new( "LD B,RL (IX+$)"    , 23 ,  0 ), /* 10 */
			new( "LD C,RL (IX+$)"    , 23 ,  0 ), /* 11 */
			new( "LD D,RL (IX+$)"    , 23 ,  0 ), /* 12 */
			new( "LD E,RL (IX+$)"    , 23 ,  0 ), /* 13 */
			new( "LD H,RL (IX+$)"    , 23 ,  0 ), /* 14 */
			new( "LD L,RL (IX+$)"    , 23 ,  0 ), /* 15 */
			new( "RL (IX+$)"         , 23 ,  0 ), /* 16 */
			new( "LD A,RL (IX+$)"    , 23 ,  0 ), /* 17 */
			new( "LD B,RR (IX+$)"    , 23 ,  0 ), /* 18 */
			new( "LD C,RR (IX+$)"    , 23 ,  0 ), /* 19 */
			new( "LD D,RR (IX+$)"    , 23 ,  0 ), /* 1A */
			new( "LD E,RR (IX+$)"    , 23 ,  0 ), /* 1B */
			new( "LD H,RR (IX+$)"    , 23 ,  0 ), /* 1C */
			new( "LD L,RR (IX+$)"    , 23 ,  0 ), /* 1D */
			new( "RR (IX+$)"         , 23 ,  0 ), /* 1E */
			new( "LD A,RR (IX+$)"    , 23 ,  0 ), /* 1F */
			new( "LD B,SLA (IX+$)"   , 23 ,  0 ), /* 20 */
			new( "LD C,SLA (IX+$)"   , 23 ,  0 ), /* 21 */
			new( "LD D,SLA (IX+$)"   , 23 ,  0 ), /* 22 */
			new( "LD E,SLA (IX+$)"   , 23 ,  0 ), /* 23 */
			new( "LD H,SLA (IX+$)"   , 23 ,  0 ), /* 24 */
			new( "LD L,SLA (IX+$)"   , 23 ,  0 ), /* 25 */
			new( "SLA (IX+$)"        , 23 ,  0 ), /* 26 */
			new( "LD A,SLA (IX+$)"   , 23 ,  0 ), /* 27 */
			new( "LD B,SRA (IX+$)"   , 23 ,  0 ), /* 28 */
			new( "LD C,SRA (IX+$)"   , 23 ,  0 ), /* 29 */
			new( "LD D,SRA (IX+$)"   , 23 ,  0 ), /* 2A */
			new( "LD E,SRA (IX+$)"   , 23 ,  0 ), /* 2B */
			new( "LD H,SRA (IX+$)"   , 23 ,  0 ), /* 2C */
			new( "LD L,SRA (IX+$)"   , 23 ,  0 ), /* 2D */
			new( "SRA (IX+$)"        , 23 ,  0 ), /* 2E */
			new( "LD A,SRA (IX+$)"   , 23 ,  0 ), /* 2F */
			new( "LD B,SLL (IX+$)"   , 23 ,  0 ), /* 30 */
			new( "LD C,SLL (IX+$)"   , 23 ,  0 ), /* 31 */
			new( "LD D,SLL (IX+$)"   , 23 ,  0 ), /* 32 */
			new( "LD E,SLL (IX+$)"   , 23 ,  0 ), /* 33 */
			new( "LD H,SLL (IX+$)"   , 23 ,  0 ), /* 34 */
			new( "LD L,SLL (IX+$)"   , 23 ,  0 ), /* 35 */
			new( "SLL (IX+$)"        , 23 ,  0 ), /* 36 */
			new( "LD A,SLL (IX+$)"   , 23 ,  0 ), /* 37 */
			new( "LD B,SRL (IX+$)"   , 23 ,  0 ), /* 38 */
			new( "LD C,SRL (IX+$)"   , 23 ,  0 ), /* 39 */
			new( "LD D,SRL (IX+$)"   , 23 ,  0 ), /* 3A */
			new( "LD E,SRL (IX+$)"   , 23 ,  0 ), /* 3B */
			new( "LD H,SRL (IX+$)"   , 23 ,  0 ), /* 3C */
			new( "LD L,SRL (IX+$)"   , 23 ,  0 ), /* 3D */
			new( "SRL (IX+$)"        , 23 ,  0 ), /* 3E */
			new( "LD A,SRL (IX+$)"   , 23 ,  0 ), /* 3F */
			new( "BIT 0,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues), /* 40 */
			new( "BIT 0,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 41 */
			new( "BIT 0,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 42 */
			new( "BIT 0,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 43 */
			new( "BIT 0,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 44 */
			new( "BIT 0,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 45 */
			new( "BIT 0,(IX+$)"      , 20 ,  0 ), /* 46 */
			new( "BIT 0,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 47 */
			new( "BIT 1,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 48 */
			new( "BIT 1,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 49 */
			new( "BIT 1,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 4A */
			new( "BIT 1,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 4B */
			new( "BIT 1,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 4C */
			new( "BIT 1,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 4D */
			new( "BIT 1,(IX+$)"      , 20 ,  0 ), /* 4E */
			new( "BIT 1,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 4F */
			new( "BIT 2,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 50 */
			new( "BIT 2,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 51 */
			new( "BIT 2,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 52 */
			new( "BIT 2,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 53 */
			new( "BIT 2,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 54 */
			new( "BIT 2,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 55 */
			new( "BIT 2,(IX+$)"      , 20 ,  0 ), /* 56 */
			new( "BIT 2,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 57 */
			new( "BIT 3,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 58 */
			new( "BIT 3,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 59 */
			new( "BIT 3,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 5A */
			new( "BIT 3,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 5B */
			new( "BIT 3,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 5C */
			new( "BIT 3,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 5D */
			new( "BIT 3,(IX+$)"      , 20 ,  0 ), /* 5E */
			new( "BIT 3,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 5F */
			new( "BIT 4,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 60 */
			new( "BIT 4,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 61 */
			new( "BIT 4,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 62 */
			new( "BIT 4,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 63 */
			new( "BIT 4,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 64 */
			new( "BIT 4,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 65 */
			new( "BIT 4,(IX+$)"      , 20 ,  0 ), /* 66 */
			new( "BIT 4,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 67 */
			new( "BIT 5,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 68 */
			new( "BIT 5,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 69 */
			new( "BIT 5,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 6A */
			new( "BIT 5,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 6B */
			new( "BIT 5,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 6C */
			new( "BIT 5,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 6D */
			new( "BIT 5,(IX+$)"      , 20 ,  0 ), /* 6E */
			new( "BIT 5,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 6F */
			new( "BIT 6,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 70 */
			new( "BIT 6,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 71 */
			new( "BIT 6,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 72 */
			new( "BIT 6,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 73 */
			new( "BIT 6,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 74 */
			new( "BIT 6,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 75 */
			new( "BIT 6,(IX+$)"      , 20 ,  0 ), /* 76 */
			new( "BIT 6,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 77 */
			new( "BIT 7,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 78 */
			new( "BIT 7,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 79 */
			new( "BIT 7,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 7A */
			new( "BIT 7,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 7B */
			new( "BIT 7,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 7C */
			new( "BIT 7,(IX+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 7D */
			new( "BIT 7,(IX+$)"      , 20 ,  0 ), /* 7E */
			new( "BIT 7,(IX+$)"      , 20 ,  0 ), /* 7F */
			new( "LD B,RES 0,(IX+$)" , 23 ,  0 ), /* 80 */
			new( "LD C,RES 0,(IX+$)" , 23 ,  0 ), /* 81 */
			new( "LD D,RES 0,(IX+$)" , 23 ,  0 ), /* 82 */
			new( "LD E,RES 0,(IX+$)" , 23 ,  0 ), /* 83 */
			new( "LD H,RES 0,(IX+$)" , 23 ,  0 ), /* 84 */
			new( "LD L,RES 0,(IX+$)" , 23 ,  0 ), /* 85 */
			new( "RES 0,(IX+$)"      , 23 ,  0 ), /* 86 */
			new( "LD A,RES 0,(IX+$)" , 23 ,  0 ), /* 87 */
			new( "LD B,RES 1,(IX+$)" , 23 ,  0 ), /* 88 */
			new( "LD C,RES 1,(IX+$)" , 23 ,  0 ), /* 89 */
			new( "LD D,RES 1,(IX+$)" , 23 ,  0 ), /* 8A */
			new( "LD E,RES 1,(IX+$)" , 23 ,  0 ), /* 8B */
			new( "LD H,RES 1,(IX+$)" , 23 ,  0 ), /* 8C */
			new( "LD L,RES 1,(IX+$)" , 23 ,  0 ), /* 8D */
			new( "RES 1,(IX+$)"      , 23 ,  0 ), /* 8E */
			new( "LD A,RES 1,(IX+$)" , 23 ,  0 ), /* 8F */
			new( "LD B,RES 2,(IX+$)" , 23 ,  0 ), /* 90 */
			new( "LD C,RES 2,(IX+$)" , 23 ,  0 ), /* 91 */
			new( "LD D,RES 2,(IX+$)" , 23 ,  0 ), /* 92 */
			new( "LD E,RES 2,(IX+$)" , 23 ,  0 ), /* 93 */
			new( "LD H,RES 2,(IX+$)" , 23 ,  0 ), /* 94 */
			new( "LD L,RES 2,(IX+$)" , 23 ,  0 ), /* 95 */
			new( "RES 2,(IX+$)"      , 23 ,  0 ), /* 96 */
			new( "LD A,RES 2,(IX+$)" , 23 ,  0 ), /* 97 */
			new( "LD B,RES 3,(IX+$)" , 23 ,  0 ), /* 98 */
			new( "LD C,RES 3,(IX+$)" , 23 ,  0 ), /* 99 */
			new( "LD D,RES 3,(IX+$)" , 23 ,  0 ), /* 9A */
			new( "LD E,RES 3,(IX+$)" , 23 ,  0 ), /* 9B */
			new( "LD H,RES 3,(IX+$)" , 23 ,  0 ), /* 9C */
			new( "LD L,RES 3,(IX+$)" , 23 ,  0 ), /* 9D */
			new( "RES 3,(IX+$)"      , 23 ,  0 ), /* 9E */
			new( "LD A,RES 3,(IX+$)" , 23 ,  0 ), /* 9F */
			new( "LD B,RES 4,(IX+$)" , 23 ,  0 ), /* A0 */
			new( "LD C,RES 4,(IX+$)" , 23 ,  0 ), /* A1 */
			new( "LD D,RES 4,(IX+$)" , 23 ,  0 ), /* A2 */
			new( "LD E,RES 4,(IX+$)" , 23 ,  0 ), /* A3 */
			new( "LD H,RES 4,(IX+$)" , 23 ,  0 ), /* A4 */
			new( "LD L,RES 4,(IX+$)" , 23 ,  0 ), /* A5 */
			new( "RES 4,(IX+$)"      , 23 ,  0 ), /* A6 */
			new( "LD A,RES 4,(IX+$)" , 23 ,  0 ), /* A7 */
			new( "LD B,RES 5,(IX+$)" , 23 ,  0 ), /* A8 */
			new( "LD C,RES 5,(IX+$)" , 23 ,  0 ), /* A9 */
			new( "LD D,RES 5,(IX+$)" , 23 ,  0 ), /* AA */
			new( "LD E,RES 5,(IX+$)" , 23 ,  0 ), /* AB */
			new( "LD H,RES 5,(IX+$)" , 23 ,  0 ), /* AC */
			new( "LD L,RES 5,(IX+$)" , 23 ,  0 ), /* AD */
			new( "RES 5,(IX+$)"      , 23 ,  0 ), /* AE */
			new( "LD A,RES 5,(IX+$)" , 23 ,  0 ), /* AF */
			new( "LD B,RES 6,(IX+$)" , 23 ,  0 ), /* B0 */
			new( "LD C,RES 6,(IX+$)" , 23 ,  0 ), /* B1 */
			new( "LD D,RES 6,(IX+$)" , 23 ,  0 ), /* B2 */
			new( "LD E,RES 6,(IX+$)" , 23 ,  0 ), /* B3 */
			new( "LD H,RES 6,(IX+$)" , 23 ,  0 ), /* B4 */
			new( "LD L,RES 6,(IX+$)" , 23 ,  0 ), /* B5 */
			new( "RES 6,(IX+$)"      , 23 ,  0 ), /* B6 */
			new( "LD A,RES 6,(IX+$)" , 23 ,  0 ), /* B7 */
			new( "LD B,RES 7,(IX+$)" , 23 ,  0 ), /* B8 */
			new( "LD C,RES 7,(IX+$)" , 23 ,  0 ), /* B9 */
			new( "LD D,RES 7,(IX+$)" , 23 ,  0 ), /* BA */
			new( "LD E,RES 7,(IX+$)" , 23 ,  0 ), /* BB */
			new( "LD H,RES 7,(IX+$)" , 23 ,  0 ), /* BC */
			new( "LD L,RES 7,(IX+$)" , 23 ,  0 ), /* BD */
			new( "RES 7,(IX+$)"      , 23 ,  0 ), /* BE */
			new( "LD A,RES 7,(IX+$)" , 23 ,  0 ), /* BF */
			new( "LD B,SET 0,(IX+$)" , 23 ,  0 ), /* C0 */
			new( "LD C,SET 0,(IX+$)" , 23 ,  0 ), /* C1 */
			new( "LD D,SET 0,(IX+$)" , 23 ,  0 ), /* C2 */
			new( "LD E,SET 0,(IX+$)" , 23 ,  0 ), /* C3 */
			new( "LD H,SET 0,(IX+$)" , 23 ,  0 ), /* C4 */
			new( "LD L,SET 0,(IX+$)" , 23 ,  0 ), /* C5 */
			new( "SET 0,(IX+$)"      , 23 ,  0 ), /* C6 */
			new( "LD A,SET 0,(IX+$)" , 23 ,  0 ), /* C7 */
			new( "LD B,SET 1,(IX+$)" , 23 ,  0 ), /* C8 */
			new( "LD C,SET 1,(IX+$)" , 23 ,  0 ), /* C9 */
			new( "LD D,SET 1,(IX+$)" , 23 ,  0 ), /* CA */
			new( "LD E,SET 1,(IX+$)" , 23 ,  0 ), /* CB */
			new( "LD H,SET 1,(IX+$)" , 23 ,  0 ), /* CC */
			new( "LD L,SET 1,(IX+$)" , 23 ,  0 ), /* CD */
			new( "SET 1,(IX+$)"      , 23 ,  0 ), /* CE */
			new( "LD A,SET 1,(IX+$)" , 23 ,  0 ), /* CF */
			new( "LD B,SET 2,(IX+$)" , 23 ,  0 ), /* D0 */
			new( "LD C,SET 2,(IX+$)" , 23 ,  0 ), /* D1 */
			new( "LD D,SET 2,(IX+$)" , 23 ,  0 ), /* D2 */
			new( "LD E,SET 2,(IX+$)" , 23 ,  0 ), /* D3 */
			new( "LD H,SET 2,(IX+$)" , 23 ,  0 ), /* D4 */
			new( "LD L,SET 2,(IX+$)" , 23 ,  0 ), /* D5 */
			new( "SET 2,(IX+$)"      , 23 ,  0 ), /* D6 */
			new( "LD A,SET 2,(IX+$)" , 23 ,  0 ), /* D7 */
			new( "LD B,SET 3,(IX+$)" , 23 ,  0 ), /* D8 */
			new( "LD C,SET 3,(IX+$)" , 23 ,  0 ), /* D9 */
			new( "LD D,SET 3,(IX+$)" , 23 ,  0 ), /* DA */
			new( "LD E,SET 3,(IX+$)" , 23 ,  0 ), /* DB */
			new( "LD H,SET 3,(IX+$)" , 23 ,  0 ), /* DC */
			new( "LD L,SET 3,(IX+$)" , 23 ,  0 ), /* DD */
			new( "SET 3,(IX+$)"      , 23 ,  0 ), /* DE */
			new( "LD A,SET 3,(IX+$)" , 23 ,  0 ), /* DF */
			new( "LD B,SET 4,(IX+$)" , 23 ,  0 ), /* E0 */
			new( "LD C,SET 4,(IX+$)" , 23 ,  0 ), /* E1 */
			new( "LD D,SET 4,(IX+$)" , 23 ,  0 ), /* E2 */
			new( "LD E,SET 4,(IX+$)" , 23 ,  0 ), /* E3 */
			new( "LD H,SET 4,(IX+$)" , 23 ,  0 ), /* E4 */
			new( "LD L,SET 4,(IX+$)" , 23 ,  0 ), /* E5 */
			new( "SET 4,(IX+$)"      , 23 ,  0 ), /* E6 */
			new( "LD A,SET 4,(IX+$)" , 23 ,  0 ), /* E7 */
			new( "LD B,SET 5,(IX+$)" , 23 ,  0 ), /* E8 */
			new( "LD C,SET 5,(IX+$)" , 23 ,  0 ), /* E9 */
			new( "LD D,SET 5,(IX+$)" , 23 ,  0 ), /* EA */
			new( "LD E,SET 5,(IX+$)" , 23 ,  0 ), /* EB */
			new( "LD H,SET 5,(IX+$)" , 23 ,  0 ), /* EC */
			new( "LD L,SET 5,(IX+$)" , 23 ,  0 ), /* ED */
			new( "SET 5,(IX+$)"      , 23 ,  0 ), /* EE */
			new( "LD A,SET 5,(IX+$)" , 23 ,  0 ), /* EF */
			new( "LD B,SET 6,(IX+$)" , 23 ,  0 ), /* F0 */
			new( "LD C,SET 6,(IX+$)" , 23 ,  0 ), /* F1 */
			new( "LD D,SET 6,(IX+$)" , 23 ,  0 ), /* F2 */
			new( "LD E,SET 6,(IX+$)" , 23 ,  0 ), /* F3 */
			new( "LD H,SET 6,(IX+$)" , 23 ,  0 ), /* F4 */
			new( "LD L,SET 6,(IX+$)" , 23 ,  0 ), /* F5 */
			new( "SET 6,(IX+$)"      , 23 ,  0 ), /* F6 */
			new( "LD A,SET 6,(IX+$)" , 23 ,  0 ), /* F7 */
			new( "LD B,SET 7,(IX+$)" , 23 ,  0 ), /* F8 */
			new( "LD C,SET 7,(IX+$)" , 23 ,  0 ), /* F9 */
			new( "LD D,SET 7,(IX+$)" , 23 ,  0 ), /* FA */
			new( "LD E,SET 7,(IX+$)" , 23 ,  0 ), /* FB */
			new( "LD H,SET 7,(IX+$)" , 23 ,  0 ), /* FC */
			new( "LD L,SET 7,(IX+$)" , 23 ,  0 ), /* FD */
			new( "SET 7,(IX+$)"      , 23 ,  0 ), /* FE */
			new( "LD A,SET 7,(IX+$)" , 23 ,  0 ), /* FF */
        ]);

    public static ReadOnlyCollection<OpCode> DasmED = new ReadOnlyCollection<OpCode>(
                    [   new( null                ,  0 ,  0 ), /* 00 */
			new( null                ,  0 ,  0 ), /* 01 */
			new( null                ,  0 ,  0 ), /* 02 */
			new( null                ,  0 ,  0 ), /* 03 */
			new( null                ,  0 ,  0 ), /* 04 */
			new( null                ,  0 ,  0 ), /* 05 */
			new( null                ,  0 ,  0 ), /* 06 */
			new( null                ,  0 ,  0 ), /* 07 */
			new( null                ,  0 ,  0 ), /* 08 */
			new( null                ,  0 ,  0 ), /* 09 */
			new( null                ,  0 ,  0 ), /* 0A */
			new( null                ,  0 ,  0 ), /* 0B */
			new( null                ,  0 ,  0 ), /* 0C */
			new( null                ,  0 ,  0 ), /* 0D */
			new( null                ,  0 ,  0 ), /* 0E */
			new( null                ,  0 ,  0 ), /* 0F */
			new( null                ,  0 ,  0 ), /* 10 */
			new( null                ,  0 ,  0 ), /* 11 */
			new( null                ,  0 ,  0 ), /* 12 */
			new( null                ,  0 ,  0 ), /* 13 */
			new( null                ,  0 ,  0 ), /* 14 */
			new( null                ,  0 ,  0 ), /* 15 */
			new( null                ,  0 ,  0 ), /* 16 */
			new( null                ,  0 ,  0 ), /* 17 */
			new( null                ,  0 ,  0 ), /* 18 */
			new( null                ,  0 ,  0 ), /* 19 */
			new( null                ,  0 ,  0 ), /* 1A */
			new( null                ,  0 ,  0 ), /* 1B */
			new( null                ,  0 ,  0 ), /* 1C */
			new( null                ,  0 ,  0 ), /* 1D */
			new( null                ,  0 ,  0 ), /* 1E */
			new( null                ,  0 ,  0 ), /* 1F */
			new( null                ,  0 ,  0 ), /* 20 */
			new( null                ,  0 ,  0 ), /* 21 */
			new( null                ,  0 ,  0 ), /* 22 */
			new( null                ,  0 ,  0 ), /* 23 */
			new( null                ,  0 ,  0 ), /* 24 */
			new( null                ,  0 ,  0 ), /* 25 */
			new( null                ,  0 ,  0 ), /* 26 */
			new( null                ,  0 ,  0 ), /* 27 */
			new( null                ,  0 ,  0 ), /* 28 */
			new( null                ,  0 ,  0 ), /* 29 */
			new( null                ,  0 ,  0 ), /* 2A */
			new( null                ,  0 ,  0 ), /* 2B */
			new( null                ,  0 ,  0 ), /* 2C */
			new( null                ,  0 ,  0 ), /* 2D */
			new( null                ,  0 ,  0 ), /* 2E */
			new( null                ,  0 ,  0 ), /* 2F */
			new( null                ,  0 ,  0 ), /* 30 */
			new( null                ,  0 ,  0 ), /* 31 */
			new( null                ,  0 ,  0 ), /* 32 */
			new( null                ,  0 ,  0 ), /* 33 */
			new( null                ,  0 ,  0 ), /* 34 */
			new( null                ,  0 ,  0 ), /* 35 */
			new( null                ,  0 ,  0 ), /* 36 */
			new( null                ,  0 ,  0 ), /* 37 */
			new( null                ,  0 ,  0 ), /* 38 */
			new( null                ,  0 ,  0 ), /* 39 */
			new( null                ,  0 ,  0 ), /* 3A */
			new( null                ,  0 ,  0 ), /* 3B */
			new( null                ,  0 ,  0 ), /* 3C */
			new( null                ,  0 ,  0 ), /* 3D */
			new( null                ,  0 ,  0 ), /* 3E */
			new( null                ,  0 ,  0 ), /* 3F */
			new( "IN B,(C)"          , 12 ,  0 ), /* 40 */
			new( "OUT (C),B"         , 12 ,  0 ), /* 41 */
			new( "SBC HL,BC"         , 15 ,  0 ), /* 42 */
			new( "LD (@),BC"         , 20 ,  0 ), /* 43 */
			new( "NEG"               ,  8 ,  0 ), /* 44 */
			new( "RETN"              , 14 ,  0 , OpCodeFlags.Returns), /* 45 */
			new( "IM 0"              ,  8 ,  0 ), /* 46 */
			new( "LD I,A"            ,  9 ,  0 ), /* 47 */
			new( "IN C,(C)"          , 12 ,  0 ), /* 48 */
			new( "OUT (C),C"         , 12 ,  0 ), /* 49 */
			new( "ADC HL,BC"         , 15 ,  0 ), /* 4A */
			new( "LD BC,(@)"         , 20 ,  0 ), /* 4B */
			new( "NEG"               ,  8 ,  0 ), /* 4C */
			new( "RETI"              , 14 ,  0 , OpCodeFlags.Returns), /* 4D */
			new( "IM 0"              ,  8 ,  0 ), /* 4E */
			new( "LD_R_A"            ,  9 ,  0 ), /* 4F */
			new( "IN D,(C)"          , 12 ,  0 ), /* 50 */
			new( "OUT (C),D"         , 12 ,  0 ), /* 51 */
			new( "SBC HL,DE"         , 15 ,  0 ), /* 52 */
			new( "LD (@),DE"         , 20 ,  0 ), /* 53 */
			new( "NEG"               ,  8 ,  0 ), /* 54 */
			new( "RETN"              , 14 ,  0 , OpCodeFlags.Returns), /* 55 */
			new( "IM 1"              ,  8 ,  0 ), /* 56 */
			new( "LD A,I"            ,  9 ,  0 ), /* 57 */
			new( "IN E,(C)"          , 12 ,  0 ), /* 58 */
			new( "OUT (C),E"         , 12 ,  0 ), /* 59 */
			new( "ADC HL,DE"         , 15 ,  0 ), /* 5A */
			new( "LD DE,(@)"         , 20 ,  0 ), /* 5B */
			new( "NEG"               ,  8 ,  0 ), /* 5C */
			new( "RETI"              , 14 ,  0 , OpCodeFlags.Returns), /* 5D */
			new( "IM 2"              ,  8 ,  0 ), /* 5E */
			new( "LD A,R"            ,  9 ,  0 ), /* 5F */
			new( "IN H,(C)"          , 12 ,  0 ), /* 60 */
			new( "OUT (C),H"         , 12 ,  0 ), /* 61 */
			new( "SBC HL,HL"         , 15 ,  0 ), /* 62 */
			new( "LD (@),HL"         , 20 ,  0 ), /* 63 */
			new( "NEG"               ,  8 ,  0 ), /* 64 */
			new( "RETN"              , 14 ,  0 , OpCodeFlags.Returns), /* 65 */
			new( "IM 0"              ,  8 ,  0 ), /* 66 */
			new( "RRD"               , 18 ,  0 ), /* 67 */
			new( "IN L,(C)"          , 12 ,  0 ), /* 68 */
			new( "OUT (C),L"         , 12 ,  0 ), /* 69 */
			new( "ADC HL,HL"         , 15 ,  0 ), /* 6A */
			new( "LD HL,(@)"         , 20 ,  0 ), /* 6B */
			new( "NEG"               ,  8 ,  0 ), /* 6C */
			new( "RETI"              , 14 ,  0 , OpCodeFlags.Returns ), /* 6D */
			new( "IM 0"              ,  8 ,  0 ), /* 6E */
			new( "RLD"               , 18 ,  0 ), /* 6F */
			new( "IN_F (C)"          , 12 ,  0 ), /* 70 */
			new( "OUT (C),0"         , 12 ,  0 ), /* 71 */
			new( "SBC HL,SP"         , 15 ,  0 ), /* 72 */
			new( "LD (@),SP"         , 20 ,  0 ), /* 73 */
			new( "NEG"               ,  8 ,  0 ), /* 74 */
			new( "RETN"              , 14 ,  0 , OpCodeFlags.Returns ), /* 75 */
			new( "IM 1"              ,  8 ,  0 ), /* 76 */
			new( null                ,  0 ,  0 ), /* 77 */
			new( "IN A,(C)"          , 12 ,  0 ), /* 78 */
			new( "OUT (C),A"         , 12 ,  0 ), /* 79 */
			new( "ADC HL,SP"         , 15 ,  0 ), /* 7A */
			new( "LD SP,(@)"         , 20 ,  0 ), /* 7B */
			new( "NEG"               ,  8 ,  0 ), /* 7C */
			new( "RETI"              , 14 ,  0 , OpCodeFlags.Returns ), /* 7D */
			new( "IM 2"              ,  8 ,  0 ), /* 7E */
			new( null                ,  0 ,  0 ), /* 7F */
			new( null                ,  0 ,  0 ), /* 80 */
			new( null                ,  0 ,  0 ), /* 81 */
			new( null                ,  0 ,  0 ), /* 82 */
			new( null                ,  0 ,  0 ), /* 83 */
			new( null                ,  0 ,  0 ), /* 84 */
			new( null                ,  0 ,  0 ), /* 85 */
			new( null                ,  0 ,  0 ), /* 86 */
			new( null                ,  0 ,  0 ), /* 87 */
			new( null                ,  0 ,  0 ), /* 88 */
			new( null                ,  0 ,  0 ), /* 89 */
			new( null                ,  0 ,  0 ), /* 8A */
			new( null                ,  0 ,  0 ), /* 8B */
			new( null                ,  0 ,  0 ), /* 8C */
			new( null                ,  0 ,  0 ), /* 8D */
			new( null                ,  0 ,  0 ), /* 8E */
			new( null                ,  0 ,  0 ), /* 8F */
			new( null                ,  0 ,  0 ), /* 90 */
			new( null                ,  0 ,  0 ), /* 91 */
			new( null                ,  0 ,  0 ), /* 92 */
			new( null                ,  0 ,  0 ), /* 93 */
			new( null                ,  0 ,  0 ), /* 94 */
			new( null                ,  0 ,  0 ), /* 95 */
			new( null                ,  0 ,  0 ), /* 96 */
			new( null                ,  0 ,  0 ), /* 97 */
			new( null                ,  0 ,  0 ), /* 98 */
			new( null                ,  0 ,  0 ), /* 99 */
			new( null                ,  0 ,  0 ), /* 9A */
			new( null                ,  0 ,  0 ), /* 9B */
			new( null                ,  0 ,  0 ), /* 9C */
			new( null                ,  0 ,  0 ), /* 9D */
			new( null                ,  0 ,  0 ), /* 9E */
			new( null                ,  0 ,  0 ), /* 9F */
			new( "LDI"               , 16 ,  0 ), /* A0 */
			new( "CPI"               , 16 ,  0 ), /* A1 */
			new( "INI"               , 16 ,  0 ), /* A2 */
			new( "OUTI"              , 16 ,  0 ), /* A3 */
			new( null                ,  0 ,  0 ), /* A4 */
			new( null                ,  0 ,  0 ), /* A5 */
			new( null                ,  0 ,  0 ), /* A6 */
			new( null                ,  0 ,  0 ), /* A7 */
			new( "LDD"               , 16 ,  0 ), /* A8 */
			new( "CPD"               , 16 ,  0 ), /* A9 */
			new( "IND"               , 16 ,  0 ), /* AA */
			new( "OUTD"              , 16 ,  0 ), /* AB */
			new( null                ,  0 ,  0 ), /* AC */
			new( null                ,  0 ,  0 ), /* AD */
			new( null                ,  0 ,  0 ), /* AE */
			new( null                ,  0 ,  0 ), /* AF */
			new( "LDIR"              , 16 , 21 ), /* B0 */
			new( "CPIR"              , 16 , 21 ), /* B1 */
			new( "INIR"              , 16 , 21 ), /* B2 */
			new( "OTIR"              , 16 , 21 ), /* B3 */
			new( null                ,  0 ,  0 ), /* B4 */
			new( null                ,  0 ,  0 ), /* B5 */
			new( null                ,  0 ,  0 ), /* B6 */
			new( null                ,  0 ,  0 ), /* B7 */
			new( "LDDR"              , 16 , 21 ), /* B8 */
			new( "CPDR"              , 16 , 21 ), /* B9 */
			new( "INDR"              , 16 , 21 ), /* BA */
			new( "OTDR"              , 16 , 21 ), /* BB */
			new( null                ,  0 ,  0 ), /* BC */
			new( null                ,  0 ,  0 ), /* BD */
			new( null                ,  0 ,  0 ), /* BE */
			new( null                ,  0 ,  0 ), /* BF */
			new( null                ,  0 ,  0 ), /* C0 */
			new( null                ,  0 ,  0 ), /* C1 */
			new( null                ,  0 ,  0 ), /* C2 */
			new( null                ,  0 ,  0 ), /* C3 */
			new( null                ,  0 ,  0 ), /* C4 */
			new( null                ,  0 ,  0 ), /* C5 */
			new( null                ,  0 ,  0 ), /* C6 */
			new( null                ,  0 ,  0 ), /* C7 */
			new( null                ,  0 ,  0 ), /* C8 */
			new( null                ,  0 ,  0 ), /* C9 */
			new( null                ,  0 ,  0 ), /* CA */
			new( null                ,  0 ,  0 ), /* CB */
			new( null                ,  0 ,  0 ), /* CC */
			new( null                ,  0 ,  0 ), /* CD */
			new( null                ,  0 ,  0 ), /* CE */
			new( null                ,  0 ,  0 ), /* CF */
			new( null                ,  0 ,  0 ), /* D0 */
			new( null                ,  0 ,  0 ), /* D1 */
			new( null                ,  0 ,  0 ), /* D2 */
			new( null                ,  0 ,  0 ), /* D3 */
			new( null                ,  0 ,  0 ), /* D4 */
			new( null                ,  0 ,  0 ), /* D5 */
			new( null                ,  0 ,  0 ), /* D6 */
			new( null                ,  0 ,  0 ), /* D7 */
			new( null                ,  0 ,  0 ), /* D8 */
			new( null                ,  0 ,  0 ), /* D9 */
			new( null                ,  0 ,  0 ), /* DA */
			new( null                ,  0 ,  0 ), /* DB */
			new( null                ,  0 ,  0 ), /* DC */
			new( null                ,  0 ,  0 ), /* DD */
			new( null                ,  0 ,  0 ), /* DE */
			new( null                ,  0 ,  0 ), /* DF */
			new( null                ,  0 ,  0 ), /* E0 */
			new( null                ,  0 ,  0 ), /* E1 */
			new( null                ,  0 ,  0 ), /* E2 */
			new( null                ,  0 ,  0 ), /* E3 */
			new( null                ,  0 ,  0 ), /* E4 */
			new( null                ,  0 ,  0 ), /* E5 */
			new( null                ,  0 ,  0 ), /* E6 */
			new( null                ,  0 ,  0 ), /* E7 */
			new( null                ,  0 ,  0 ), /* E8 */
			new( null                ,  0 ,  0 ), /* E9 */
			new( null                ,  0 ,  0 ), /* EA */
			new( null                ,  0 ,  0 ), /* EB */
			new( null                ,  0 ,  0 ), /* EC */
			new( null                ,  0 ,  0 ), /* ED */
			new( null                ,  0 ,  0 ), /* EE */
			new( null                ,  0 ,  0 ), /* EF */
			new( null                ,  0 ,  0 ), /* F0 */
			new( null                ,  0 ,  0 ), /* F1 */
			new( null                ,  0 ,  0 ), /* F2 */
			new( null                ,  0 ,  0 ), /* F3 */
			new( null                ,  0 ,  0 ), /* F4 */
			new( null                ,  0 ,  0 ), /* F5 */
			new( null                ,  0 ,  0 ), /* F6 */
			new( null                ,  0 ,  0 ), /* F7 */
			new( null                ,  0 ,  0 ), /* F8 */
			new( null                ,  0 ,  0 ), /* F9 */
			new( null                ,  0 ,  0 ), /* FA */
			new( null                ,  0 ,  0 ), /* FB */
			new( null                ,  0 ,  0 ), /* FC */
			new( null                ,  0 ,  0 ), /* FD */
			new( null                ,  0 ,  0 ), /* FE */
			new( null                ,  0 ,  0 ), /* FF */

        ]);

    /**/
    /**/

    public static ReadOnlyCollection<OpCode> DasmFD = new ReadOnlyCollection<OpCode>(
    [
        new( null                ,  0 ,  0 ), /* 00 */
			new( null                ,  0 ,  0 ), /* 01 */
			new( null                ,  0 ,  0 ), /* 02 */
			new( null                ,  0 ,  0 ), /* 03 */
			new( null                ,  0 ,  0 ), /* 04 */
			new( null                ,  0 ,  0 ), /* 05 */
			new( null                ,  0 ,  0 ), /* 06 */
			new( null                ,  0 ,  0 ), /* 07 */
			new( null                ,  0 ,  0 ), /* 08 */
			new( "ADD IY,BC"         , 15 ,  0 ), /* 09 */
			new( null                ,  0 ,  0 ), /* 0A */
			new( null                ,  0 ,  0 ), /* 0B */
			new( null                ,  0 ,  0 ), /* 0C */
			new( null                ,  0 ,  0 ), /* 0D */
			new( null                ,  0 ,  0 ), /* 0E */
			new( null                ,  0 ,  0 ), /* 0F */
			new( null                ,  0 ,  0 ), /* 10 */
			new( null                ,  0 ,  0 ), /* 11 */
			new( null                ,  0 ,  0 ), /* 12 */
			new( null                ,  0 ,  0 ), /* 13 */
			new( null                ,  0 ,  0 ), /* 14 */
			new( null                ,  0 ,  0 ), /* 15 */
			new( null                ,  0 ,  0 ), /* 16 */
			new( null                ,  0 ,  0 ), /* 17 */
			new( null                ,  0 ,  0 ), /* 18 */
			new( "ADD IY,DE"         , 15 ,  0 ), /* 19 */
			new( null                ,  0 ,  0 ), /* 1A */
			new( null                ,  0 ,  0 ), /* 1B */
			new( null                ,  0 ,  0 ), /* 1C */
			new( null                ,  0 ,  0 ), /* 1D */
			new( null                ,  0 ,  0 ), /* 1E */
			new( null                ,  0 ,  0 ), /* 1F */
			new( null                ,  0 ,  0 ), /* 20 */
			new( "LD IY,@"           , 14 ,  0 ), /* 21 */
			new( "LD (@),IY"         , 20 ,  0 ), /* 22 */
			new( "INC IY"            , 10 ,  0 ), /* 23 */
			new( "INC IYH"           ,  8 ,  0 ), /* 24 */
			new( "DEC IYH"           ,  8 ,  0 ), /* 25 */
			new( "LD IYH,#"          , 11 ,  0 ), /* 26 */
			new( null                ,  0 ,  0 ), /* 27 */
			new( null                ,  0 ,  0 ), /* 28 */
			new( "ADD IY,IY"         , 15 ,  0 ), /* 29 */
			new( "LD IY,(@)"         , 20 ,  0 ), /* 2A */
			new( "DEC IY"            , 10 ,  0 ), /* 2B */
			new( "INC IYL"           ,  8 ,  0 ), /* 2C */
			new( "DEC IYL"           ,  8 ,  0 ), /* 2D */
			new( "LD IYL,#"          , 11 ,  0 ), /* 2E */
			new( null                ,  0 ,  0 ), /* 2F */
			new( null                ,  0 ,  0 ), /* 30 */
			new( null                ,  0 ,  0 ), /* 31 */
			new( null                ,  0 ,  0 ), /* 32 */
			new( null                ,  0 ,  0 ), /* 33 */
			new( "INC (IY+$)"        , 23 ,  0 ), /* 34 */
			new( "DEC (IY+$)"        , 23 ,  0 ), /* 35 */
			new( "LD (IY+$),#"       , 19 ,  0 ), /* 36 */
			new( null                ,  0 ,  0 ), /* 37 */
			new( null                ,  0 ,  0 ), /* 38 */
			new( "ADD IY,SP"         , 15 ,  0 ), /* 39 */
			new( null                ,  0 ,  0 ), /* 3A */
			new( null                ,  0 ,  0 ), /* 3B */
			new( null                ,  0 ,  0 ), /* 3C */
			new( null                ,  0 ,  0 ), /* 3D */
			new( null                ,  0 ,  0 ), /* 3E */
			new( null                ,  0 ,  0 ), /* 3F */
			new( null                ,  0 ,  0 ), /* 40 */
			new( null                ,  0 ,  0 ), /* 41 */
			new( null                ,  0 ,  0 ), /* 42 */
			new( null                ,  0 ,  0 ), /* 43 */
			new( "LD B,IYH"          ,  8 ,  0 ), /* 44 */
			new( "LD B,IYL"          ,  8 ,  0 ), /* 45 */
			new( "LD B,(IY+$)"       , 19 ,  0 ), /* 46 */
			new( null                ,  0 ,  0 ), /* 47 */
			new( null                ,  0 ,  0 ), /* 48 */
			new( null                ,  0 ,  0 ), /* 49 */
			new( null                ,  0 ,  0 ), /* 4A */
			new( null                ,  0 ,  0 ), /* 4B */
			new( "LD C,IYH"          ,  8 ,  0 ), /* 4C */
			new( "LD C,IYL"          ,  8 ,  0 ), /* 4D */
			new( "LD C,(IY+$)"       , 19 ,  0 ), /* 4E */
			new( null                ,  0 ,  0 ), /* 4F */
			new( null                ,  0 ,  0 ), /* 50 */
			new( null                ,  0 ,  0 ), /* 51 */
			new( null                ,  0 ,  0 ), /* 52 */
			new( null                ,  0 ,  0 ), /* 53 */
			new( "LD D,IYH"          ,  8 ,  0 ), /* 54 */
			new( "LD D,IYL"          ,  8 ,  0 ), /* 55 */
			new( "LD D,(IY+$)"       , 19 ,  0 ), /* 56 */
			new( null                ,  0 ,  0 ), /* 57 */
			new( null                ,  0 ,  0 ), /* 58 */
			new( null                ,  0 ,  0 ), /* 59 */
			new( null                ,  0 ,  0 ), /* 5A */
			new( null                ,  0 ,  0 ), /* 5B */
			new( "LD E,IYH"          ,  8 ,  0 ), /* 5C */
			new( "LD E,IYL"          ,  8 ,  0 ), /* 5D */
			new( "LD E,(IY+$)"       , 19 ,  0 ), /* 5E */
			new( null                ,  0 ,  0 ), /* 5F */
			new( "LD IYH,B"          ,  8 ,  0 ), /* 60 */
			new( "LD IYH,C"          ,  8 ,  0 ), /* 61 */
			new( "LD IYH,D"          ,  8 ,  0 ), /* 62 */
			new( "LD IYH,E"          ,  8 ,  0 ), /* 63 */
			new( "LD IYH,IYH"        ,  8 ,  0 ), /* 64 */
			new( "LD IYH,IYL"        ,  8 ,  0 ), /* 65 */
			new( "LD H,(IY+$)"       , 19 ,  0 ), /* 66 */
			new( "LD IYH,A"          ,  8 ,  0 ), /* 67 */
			new( "LD IYL,B"          ,  8 ,  0 ), /* 68 */
			new( "LD IYL,C"          ,  8 ,  0 ), /* 69 */
			new( "LD IYL,D"          ,  8 ,  0 ), /* 6A */
			new( "LD IYL,E"          ,  8 ,  0 ), /* 6B */
			new( "LD IYL,IYH"        ,  8 ,  0 ), /* 6C */
			new( "LD IYL,IYL"        ,  8 ,  0 ), /* 6D */
			new( "LD L,(IY+$)"       , 19 ,  0 ), /* 6E */
			new( "LD IYL,A"          ,  8 ,  0 ), /* 6F */
			new( "LD (IY+$),B"       , 19 ,  0 ), /* 70 */
			new( "LD (IY+$),C"       , 19 ,  0 ), /* 71 */
			new( "LD (IY+$),D"       , 19 ,  0 ), /* 72 */
			new( "LD (IY+$),E"       , 19 ,  0 ), /* 73 */
			new( "LD (IY+$),H"       , 19 ,  0 ), /* 74 */
			new( "LD (IY+$),L"       , 19 ,  0 ), /* 75 */
			new( null                ,  0 ,  0 ), /* 76 */
			new( "LD (IY+$),A"       , 19 ,  0 ), /* 77 */
			new( null                ,  0 ,  0 ), /* 78 */
			new( null                ,  0 ,  0 ), /* 79 */
			new( null                ,  0 ,  0 ), /* 7A */
			new( null                ,  0 ,  0 ), /* 7B */
			new( "LD A,IYH"          ,  8 ,  0 ), /* 7C */
			new( "LD A,IYL"          ,  8 ,  0 ), /* 7D */
			new( "LD A,(IY+$)"       , 19 ,  0 ), /* 7E */
			new( null                ,  0 ,  0 ), /* 7F */
			new( null                ,  0 ,  0 ), /* 80 */
			new( null                ,  0 ,  0 ), /* 81 */
			new( null                ,  0 ,  0 ), /* 82 */
			new( null                ,  0 ,  0 ), /* 83 */
			new( "ADD A,IYH"         ,  8 ,  0 ), /* 84 */
			new( "ADD A,IYL"         ,  8 ,  0 ), /* 85 */
			new( "ADD A,(IY+$)"      , 19 ,  0 ), /* 86 */
			new( null                ,  0 ,  0 ), /* 87 */
			new( null                ,  0 ,  0 ), /* 88 */
			new( null                ,  0 ,  0 ), /* 89 */
			new( null                ,  0 ,  0 ), /* 8A */
			new( null                ,  0 ,  0 ), /* 8B */
			new( "ADC A,IYH"         ,  8 ,  0 ), /* 8C */
			new( "ADC A,IYL"         ,  8 ,  0 ), /* 8D */
			new( "ADC A,(IY+$)"      , 19 ,  0 ), /* 8E */
			new( null                ,  0 ,  0 ), /* 8F */
			new( null                ,  0 ,  0 ), /* 90 */
			new( null                ,  0 ,  0 ), /* 91 */
			new( null                ,  0 ,  0 ), /* 92 */
			new( null                ,  0 ,  0 ), /* 93 */
			new( "SUB IYH"           ,  8 ,  0 ), /* 94 */
			new( "SUB IYL"           ,  8 ,  0 ), /* 95 */
			new( "SUB (IY+$)"        , 19 ,  0 ), /* 96 */
			new( null                ,  0 ,  0 ), /* 97 */
			new( null                ,  0 ,  0 ), /* 98 */
			new( null                ,  0 ,  0 ), /* 99 */
			new( null                ,  0 ,  0 ), /* 9A */
			new( null                ,  0 ,  0 ), /* 9B */
			new( "SBC A,IYH"         ,  8 ,  0 ), /* 9C */
			new( "SBC A,IYL"         ,  8 ,  0 ), /* 9D */
			new( "SBC A,(IY+$)"      , 19 ,  0 ), /* 9E */
			new( null                ,  0 ,  0 ), /* 9F */
			new( null                ,  0 ,  0 ), /* A0 */
			new( null                ,  0 ,  0 ), /* A1 */
			new( null                ,  0 ,  0 ), /* A2 */
			new( null                ,  0 ,  0 ), /* A3 */
			new( "AND IYH"           ,  8 ,  0 ), /* A4 */
			new( "AND IYL"           ,  8 ,  0 ), /* A5 */
			new( "AND (IY+$)"        , 19 ,  0 ), /* A6 */
			new( null                ,  0 ,  0 ), /* A7 */
			new( null                ,  0 ,  0 ), /* A8 */
			new( null                ,  0 ,  0 ), /* A9 */
			new( null                ,  0 ,  0 ), /* AA */
			new( null                ,  0 ,  0 ), /* AB */
			new( "XOR IYH"           ,  8 ,  0 ), /* AC */
			new( "XOR IYL"           ,  8 ,  0 ), /* AD */
			new( "XOR (IY+$)"        , 19 ,  0 ), /* AE */
			new( null                ,  0 ,  0 ), /* AF */
			new( null                ,  0 ,  0 ), /* B0 */
			new( null                ,  0 ,  0 ), /* B1 */
			new( null                ,  0 ,  0 ), /* B2 */
			new( null                ,  0 ,  0 ), /* B3 */
			new( "OR IYH"            ,  8 ,  0 ), /* B4 */
			new( "OR IYL"            ,  8 ,  0 ), /* B5 */
			new( "OR (IY+$)"         , 19 ,  0 ), /* B6 */
			new( null                ,  0 ,  0 ), /* B7 */
			new( null                ,  0 ,  0 ), /* B8 */
			new( null                ,  0 ,  0 ), /* B9 */
			new( null                ,  0 ,  0 ), /* BA */
			new( null                ,  0 ,  0 ), /* BB */
			new( "CP IYH"            ,  8 ,  0 ), /* BC */
			new( "CP IYL"            ,  8 ,  0 ), /* BD */
			new( "CP (IY+$)"         , 19 ,  0 ), /* BE */
			new( null                ,  0 ,  0 ), /* BF */
			new( null                ,  0 ,  0 ), /* C0 */
			new( null                ,  0 ,  0 ), /* C1 */
			new( null                ,  0 ,  0 ), /* C2 */
			new( null                ,  0 ,  0 ), /* C3 */
			new( null                ,  0 ,  0 ), /* C4 */
			new( null                ,  0 ,  0 ), /* C5 */
			new( null                ,  0 ,  0 ), /* C6 */
			new( null                ,  0 ,  0 ), /* C7 */
			new( null                ,  0 ,  0 ), /* C8 */
			new( null                ,  0 ,  0 ), /* C9 */
			new( null                ,  0 ,  0 ), /* CA */
			new( "shift CB"          ,  0 ,  0 ), /* CB */
			new( null                ,  0 ,  0 ), /* CC */
			new( null                ,  0 ,  0 ), /* CD */
			new( null                ,  0 ,  0 ), /* CE */
			new( null                ,  0 ,  0 ), /* CF */
			new( null                ,  0 ,  0 ), /* D0 */
			new( null                ,  0 ,  0 ), /* D1 */
			new( null                ,  0 ,  0 ), /* D2 */
			new( null                ,  0 ,  0 ), /* D3 */
			new( null                ,  0 ,  0 ), /* D4 */
			new( null                ,  0 ,  0 ), /* D5 */
			new( null                ,  0 ,  0 ), /* D6 */
			new( null                ,  0 ,  0 ), /* D7 */
			new( null                ,  0 ,  0 ), /* D8 */
			new( null                ,  0 ,  0 ), /* D9 */
			new( null                ,  0 ,  0 ), /* DA */
			new( null                ,  0 ,  0 ), /* DB */
			new( null                ,  0 ,  0 ), /* DC */
			new( "ignore"            ,  4 ,  0 ), /* DD */
			new( null                ,  0 ,  0 ), /* DE */
			new( null                ,  0 ,  0 ), /* DF */
			new( null                ,  0 ,  0 ), /* E0 */
			new( "POP IY"            , 14 ,  0 ), /* E1 */
			new( null                ,  0 ,  0 ), /* E2 */
			new( "EX (SP),IY"        , 23 ,  0 ), /* E3 */
			new( null                ,  0 ,  0 ), /* E4 */
			new( "PUSH IY"           , 15 ,  0 ), /* E5 */
			new( null                ,  0 ,  0 ), /* E6 */
			new( null                ,  0 ,  0 ), /* E7 */
			new( null                ,  0 ,  0 ), /* E8 */
			new( "JP (IY)"             ,  8 ,  0 , OpCodeFlags.Jumps ), /* E9 */
			new( null                ,  0 ,  0 ), /* EA */
			new( null                ,  0 ,  0 ), /* EB */
			new( null                ,  0 ,  0 ), /* EC */
			new( null                ,  4 ,  0 ), /* ED */
			new( null                ,  0 ,  0 ), /* EE */
			new( null                ,  0 ,  0 ), /* EF */
			new( null                ,  0 ,  0 ), /* F0 */
			new( null                ,  0 ,  0 ), /* F1 */
			new( null                ,  0 ,  0 ), /* F2 */
			new( null                ,  0 ,  0 ), /* F3 */
			new( null                ,  0 ,  0 ), /* F4 */
			new( null                ,  0 ,  0 ), /* F5 */
			new( null                ,  0 ,  0 ), /* F6 */
			new( null                ,  0 ,  0 ), /* F7 */
			new( null                ,  0 ,  0 ), /* F8 */
			new( "LD SP,IY"          , 10 ,  0 ), /* F9 */
			new( null                ,  0 ,  0 ), /* FA */
			new( null                ,  0 ,  0 ), /* FB */
			new( null                ,  0 ,  0 ), /* FC */
			new( "ignore"            ,  4 ,  0 ), /* FD */
			new( null                ,  0 ,  0 ), /* FE */
			new( null                ,  0 ,  0 ), /* FF */
		]);

    /**/
    /**/

    public static ReadOnlyCollection<OpCode> DasmFDCB = new ReadOnlyCollection<OpCode>(
    [
        new( "LD B,RLC (IY+$)"   , 23 ,  0 ), /* 00 */
			new( "LD C,RLC (IY+$)"   , 23 ,  0 ), /* 01 */
			new( "LD D,RLC (IY+$)"   , 23 ,  0 ), /* 02 */
			new( "LD E,RLC (IY+$)"   , 23 ,  0 ), /* 03 */
			new( "LD H,RLC (IY+$)"   , 23 ,  0 ), /* 04 */
			new( "LD L,RLC (IY+$)"   , 23 ,  0 ), /* 05 */
			new( "RLC (IY+$)"        , 23 ,  0 ), /* 06 */
			new( "LD A,RLC (IY+$)"   , 23 ,  0 ), /* 07 */
			new( "LD B,RRC (IY+$)"   , 23 ,  0 ), /* 08 */
			new( "LD C,RRC (IY+$)"   , 23 ,  0 ), /* 09 */
			new( "LD D,RRC (IY+$)"   , 23 ,  0 ), /* 0A */
			new( "LD E,RRC (IY+$)"   , 23 ,  0 ), /* 0B */
			new( "LD H,RRC (IY+$)"   , 23 ,  0 ), /* 0C */
			new( "LD L,RRC (IY+$)"   , 23 ,  0 ), /* 0D */
			new( "RRC (IY+$)"        , 23 ,  0 ), /* 0E */
			new( "LD A,RRC (IY+$)"   , 23 ,  0 ), /* 0F */
			new( "LD B,RL (IY+$)"    , 23 ,  0 ), /* 10 */
			new( "LD C,RL (IY+$)"    , 23 ,  0 ), /* 11 */
			new( "LD D,RL (IY+$)"    , 23 ,  0 ), /* 12 */
			new( "LD E,RL (IY+$)"    , 23 ,  0 ), /* 13 */
			new( "LD H,RL (IY+$)"    , 23 ,  0 ), /* 14 */
			new( "LD L,RL (IY+$)"    , 23 ,  0 ), /* 15 */
			new( "RL (IY+$)"         , 23 ,  0 ), /* 16 */
			new( "LD A,RL (IY+$)"    , 23 ,  0 ), /* 17 */
			new( "LD B,RR (IY+$)"    , 23 ,  0 ), /* 18 */
			new( "LD C,RR (IY+$)"    , 23 ,  0 ), /* 19 */
			new( "LD D,RR (IY+$)"    , 23 ,  0 ), /* 1A */
			new( "LD E,RR (IY+$)"    , 23 ,  0 ), /* 1B */
			new( "LD H,RR (IY+$)"    , 23 ,  0 ), /* 1C */
			new( "LD L,RR (IY+$)"    , 23 ,  0 ), /* 1D */
			new( "RR (IY+$)"         , 23 ,  0 ), /* 1E */
			new( "LD A,RR (IY+$)"    , 23 ,  0 ), /* 1F */
			new( "LD B,SLA (IY+$)"   , 23 ,  0 ), /* 20 */
			new( "LD C,SLA (IY+$)"   , 23 ,  0 ), /* 21 */
			new( "LD D,SLA (IY+$)"   , 23 ,  0 ), /* 22 */
			new( "LD E,SLA (IY+$)"   , 23 ,  0 ), /* 23 */
			new( "LD H,SLA (IY+$)"   , 23 ,  0 ), /* 24 */
			new( "LD L,SLA (IY+$)"   , 23 ,  0 ), /* 25 */
			new( "SLA (IY+$)"        , 23 ,  0 ), /* 26 */
			new( "LD A,SLA (IY+$)"   , 23 ,  0 ), /* 27 */
			new( "LD B,SRA (IY+$)"   , 23 ,  0 ), /* 28 */
			new( "LD C,SRA (IY+$)"   , 23 ,  0 ), /* 29 */
			new( "LD D,SRA (IY+$)"   , 23 ,  0 ), /* 2A */
			new( "LD E,SRA (IY+$)"   , 23 ,  0 ), /* 2B */
			new( "LD H,SRA (IY+$)"   , 23 ,  0 ), /* 2C */
			new( "LD L,SRA (IY+$)"   , 23 ,  0 ), /* 2D */
			new( "SRA (IY+$)"        , 23 ,  0 ), /* 2E */
			new( "LD A,SRA (IY+$)"   , 23 ,  0 ), /* 2F */
			new( "LD B,SLL (IY+$)"   , 23 ,  0 ), /* 30 */
			new( "LD C,SLL (IY+$)"   , 23 ,  0 ), /* 31 */
			new( "LD D,SLL (IY+$)"   , 23 ,  0 ), /* 32 */
			new( "LD E,SLL (IY+$)"   , 23 ,  0 ), /* 33 */
			new( "LD H,SLL (IY+$)"   , 23 ,  0 ), /* 34 */
			new( "LD L,SLL (IY+$)"   , 23 ,  0 ), /* 35 */
			new( "SLL (IY+$)"        , 23 ,  0 ), /* 36 */
			new( "LD A,SLL (IY+$)"   , 23 ,  0 ), /* 37 */
			new( "LD B,SRL (IY+$)"   , 23 ,  0 ), /* 38 */
			new( "LD C,SRL (IY+$)"   , 23 ,  0 ), /* 39 */
			new( "LD D,SRL (IY+$)"   , 23 ,  0 ), /* 3A */
			new( "LD E,SRL (IY+$)"   , 23 ,  0 ), /* 3B */
			new( "LD H,SRL (IY+$)"   , 23 ,  0 ), /* 3C */
			new( "LD L,SRL (IY+$)"   , 23 ,  0 ), /* 3D */
			new( "SRL (IY+$)"        , 23 ,  0 ), /* 3E */
			new( "LD A,SRL (IY+$)"   , 23 ,  0 ), /* 3F */
			new( "BIT 0,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 40 */
			new( "BIT 0,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 41 */
			new( "BIT 0,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 42 */
			new( "BIT 0,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 43 */
			new( "BIT 0,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 44 */
			new( "BIT 0,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 45 */
			new( "BIT 0,(IY+$)"      , 20 ,  0 ), /* 46 */
			new( "BIT 0,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 47 */
			new( "BIT 1,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 48 */
			new( "BIT 1,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 49 */
			new( "BIT 1,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 4A */
			new( "BIT 1,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 4B */
			new( "BIT 1,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 4C */
			new( "BIT 1,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 4D */
			new( "BIT 1,(IY+$)"      , 20 ,  0 ), /* 4E */
			new( "BIT 1,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 4F */
			new( "BIT 2,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 50 */
			new( "BIT 2,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 51 */
			new( "BIT 2,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 52 */
			new( "BIT 2,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 53 */
			new( "BIT 2,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 54 */
			new( "BIT 2,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 55 */
			new( "BIT 2,(IY+$)"      , 20 ,  0 ), /* 56 */
			new( "BIT 2,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 57 */
			new( "BIT 3,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 58 */
			new( "BIT 3,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 59 */
			new( "BIT 3,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 5A */
			new( "BIT 3,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 5B */
			new( "BIT 3,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 5C */
			new( "BIT 3,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 5D */
			new( "BIT 3,(IY+$)"      , 20 ,  0 ), /* 5E */
			new( "BIT 3,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 5F */
			new( "BIT 4,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 60 */
			new( "BIT 4,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 61 */
			new( "BIT 4,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 62 */
			new( "BIT 4,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 63 */
			new( "BIT 4,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 64 */
			new( "BIT 4,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 65 */
			new( "BIT 4,(IY+$)"      , 20 ,  0 ), /* 66 */
			new( "BIT 4,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 67 */
			new( "BIT 5,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 68 */
			new( "BIT 5,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 69 */
			new( "BIT 5,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 6A */
			new( "BIT 5,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 6B */
			new( "BIT 5,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 6C */
			new( "BIT 5,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 6D */
			new( "BIT 5,(IY+$)"      , 20 ,  0 ), /* 6E */
			new( "BIT 5,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 6F */
			new( "BIT 6,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 70 */
			new( "BIT 6,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 71 */
			new( "BIT 6,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 72 */
			new( "BIT 6,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 73 */
			new( "BIT 6,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 74 */
			new( "BIT 6,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 75 */
			new( "BIT 6,(IY+$)"      , 20 ,  0 ), /* 76 */
			new( "BIT 6,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 77 */
			new( "BIT 7,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 78 */
			new( "BIT 7,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 79 */
			new( "BIT 7,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 7A */
			new( "BIT 7,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 7B */
			new( "BIT 7,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 7C */
			new( "BIT 7,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues ), /* 7D */
			new( "BIT 7,(IY+$)"      , 20 ,  0 ), /* 7E */
			new( "BIT 7,(IY+$)"      , 20 ,  0, OpCodeFlags.NoAsm | OpCodeFlags.Continues  ), /* 7F */
			new( "LD B,RES 0,(IY+$)" , 23 ,  0 ), /* 80 */
			new( "LD C,RES 0,(IY+$)" , 23 ,  0 ), /* 81 */
			new( "LD D,RES 0,(IY+$)" , 23 ,  0 ), /* 82 */
			new( "LD E,RES 0,(IY+$)" , 23 ,  0 ), /* 83 */
			new( "LD H,RES 0,(IY+$)" , 23 ,  0 ), /* 84 */
			new( "LD L,RES 0,(IY+$)" , 23 ,  0 ), /* 85 */
			new( "RES 0,(IY+$)"      , 23 ,  0 ), /* 86 */
			new( "LD A,RES 0,(IY+$)" , 23 ,  0 ), /* 87 */
			new( "LD B,RES 1,(IY+$)" , 23 ,  0 ), /* 88 */
			new( "LD C,RES 1,(IY+$)" , 23 ,  0 ), /* 89 */
			new( "LD D,RES 1,(IY+$)" , 23 ,  0 ), /* 8A */
			new( "LD E,RES 1,(IY+$)" , 23 ,  0 ), /* 8B */
			new( "LD H,RES 1,(IY+$)" , 23 ,  0 ), /* 8C */
			new( "LD L,RES 1,(IY+$)" , 23 ,  0 ), /* 8D */
			new( "RES 1,(IY+$)"      , 23 ,  0 ), /* 8E */
			new( "LD A,RES 1,(IY+$)" , 23 ,  0 ), /* 8F */
			new( "LD B,RES 2,(IY+$)" , 23 ,  0 ), /* 90 */
			new( "LD C,RES 2,(IY+$)" , 23 ,  0 ), /* 91 */
			new( "LD D,RES 2,(IY+$)" , 23 ,  0 ), /* 92 */
			new( "LD E,RES 2,(IY+$)" , 23 ,  0 ), /* 93 */
			new( "LD H,RES 2,(IY+$)" , 23 ,  0 ), /* 94 */
			new( "LD L,RES 2,(IY+$)" , 23 ,  0 ), /* 95 */
			new( "RES 2,(IY+$)"      , 23 ,  0 ), /* 96 */
			new( "LD A,RES 2,(IY+$)" , 23 ,  0 ), /* 97 */
			new( "LD B,RES 3,(IY+$)" , 23 ,  0 ), /* 98 */
			new( "LD C,RES 3,(IY+$)" , 23 ,  0 ), /* 99 */
			new( "LD D,RES 3,(IY+$)" , 23 ,  0 ), /* 9A */
			new( "LD E,RES 3,(IY+$)" , 23 ,  0 ), /* 9B */
			new( "LD H,RES 3,(IY+$)" , 23 ,  0 ), /* 9C */
			new( "LD L,RES 3,(IY+$)" , 23 ,  0 ), /* 9D */
			new( "RES 3,(IY+$)"      , 23 ,  0 ), /* 9E */
			new( "LD A,RES 3,(IY+$)" , 23 ,  0 ), /* 9F */
			new( "LD B,RES 4,(IY+$)" , 23 ,  0 ), /* A0 */
			new( "LD C,RES 4,(IY+$)" , 23 ,  0 ), /* A1 */
			new( "LD D,RES 4,(IY+$)" , 23 ,  0 ), /* A2 */
			new( "LD E,RES 4,(IY+$)" , 23 ,  0 ), /* A3 */
			new( "LD H,RES 4,(IY+$)" , 23 ,  0 ), /* A4 */
			new( "LD L,RES 4,(IY+$)" , 23 ,  0 ), /* A5 */
			new( "RES 4,(IY+$)"      , 23 ,  0 ), /* A6 */
			new( "LD A,RES 4,(IY+$)" , 23 ,  0 ), /* A7 */
			new( "LD B,RES 5,(IY+$)" , 23 ,  0 ), /* A8 */
			new( "LD C,RES 5,(IY+$)" , 23 ,  0 ), /* A9 */
			new( "LD D,RES 5,(IY+$)" , 23 ,  0 ), /* AA */
			new( "LD E,RES 5,(IY+$)" , 23 ,  0 ), /* AB */
			new( "LD H,RES 5,(IY+$)" , 23 ,  0 ), /* AC */
			new( "LD L,RES 5,(IY+$)" , 23 ,  0 ), /* AD */
			new( "RES 5,(IY+$)"      , 23 ,  0 ), /* AE */
			new( "LD A,RES 5,(IY+$)" , 23 ,  0 ), /* AF */
			new( "LD B,RES 6,(IY+$)" , 23 ,  0 ), /* B0 */
			new( "LD C,RES 6,(IY+$)" , 23 ,  0 ), /* B1 */
			new( "LD D,RES 6,(IY+$)" , 23 ,  0 ), /* B2 */
			new( "LD E,RES 6,(IY+$)" , 23 ,  0 ), /* B3 */
			new( "LD H,RES 6,(IY+$)" , 23 ,  0 ), /* B4 */
			new( "LD L,RES 6,(IY+$)" , 23 ,  0 ), /* B5 */
			new( "RES 6,(IY+$)"      , 23 ,  0 ), /* B6 */
			new( "LD A,RES 6,(IY+$)" , 23 ,  0 ), /* B7 */
			new( "LD B,RES 7,(IY+$)" , 23 ,  0 ), /* B8 */
			new( "LD C,RES 7,(IY+$)" , 23 ,  0 ), /* B9 */
			new( "LD D,RES 7,(IY+$)" , 23 ,  0 ), /* BA */
			new( "LD E,RES 7,(IY+$)" , 23 ,  0 ), /* BB */
			new( "LD H,RES 7,(IY+$)" , 23 ,  0 ), /* BC */
			new( "LD L,RES 7,(IY+$)" , 23 ,  0 ), /* BD */
			new( "RES 7,(IY+$)"      , 23 ,  0 ), /* BE */
			new( "LD A,RES 7,(IY+$)" , 23 ,  0 ), /* BF */
			new( "LD B,SET 0,(IY+$)" , 23 ,  0 ), /* C0 */
			new( "LD C,SET 0,(IY+$)" , 23 ,  0 ), /* C1 */
			new( "LD D,SET 0,(IY+$)" , 23 ,  0 ), /* C2 */
			new( "LD E,SET 0,(IY+$)" , 23 ,  0 ), /* C3 */
			new( "LD H,SET 0,(IY+$)" , 23 ,  0 ), /* C4 */
			new( "LD L,SET 0,(IY+$)" , 23 ,  0 ), /* C5 */
			new( "SET 0,(IY+$)"      , 23 ,  0 ), /* C6 */
			new( "LD A,SET 0,(IY+$)" , 23 ,  0 ), /* C7 */
			new( "LD B,SET 1,(IY+$)" , 23 ,  0 ), /* C8 */
			new( "LD C,SET 1,(IY+$)" , 23 ,  0 ), /* C9 */
			new( "LD D,SET 1,(IY+$)" , 23 ,  0 ), /* CA */
			new( "LD E,SET 1,(IY+$)" , 23 ,  0 ), /* CB */
			new( "LD H,SET 1,(IY+$)" , 23 ,  0 ), /* CC */
			new( "LD L,SET 1,(IY+$)" , 23 ,  0 ), /* CD */
			new( "SET 1,(IY+$)"      , 23 ,  0 ), /* CE */
			new( "LD A,SET 1,(IY+$)" , 23 ,  0 ), /* CF */
			new( "LD B,SET 2,(IY+$)" , 23 ,  0 ), /* D0 */
			new( "LD C,SET 2,(IY+$)" , 23 ,  0 ), /* D1 */
			new( "LD D,SET 2,(IY+$)" , 23 ,  0 ), /* D2 */
			new( "LD E,SET 2,(IY+$)" , 23 ,  0 ), /* D3 */
			new( "LD H,SET 2,(IY+$)" , 23 ,  0 ), /* D4 */
			new( "LD L,SET 2,(IY+$)" , 23 ,  0 ), /* D5 */
			new( "SET 2,(IY+$)"      , 23 ,  0 ), /* D6 */
			new( "LD A,SET 2,(IY+$)" , 23 ,  0 ), /* D7 */
			new( "LD B,SET 3,(IY+$)" , 23 ,  0 ), /* D8 */
			new( "LD C,SET 3,(IY+$)" , 23 ,  0 ), /* D9 */
			new( "LD D,SET 3,(IY+$)" , 23 ,  0 ), /* DA */
			new( "LD E,SET 3,(IY+$)" , 23 ,  0 ), /* DB */
			new( "LD H,SET 3,(IY+$)" , 23 ,  0 ), /* DC */
			new( "LD L,SET 3,(IY+$)" , 23 ,  0 ), /* DD */
			new( "SET 3,(IY+$)"      , 23 ,  0 ), /* DE */
			new( "LD A,SET 3,(IY+$)" , 23 ,  0 ), /* DF */
			new( "LD B,SET 4,(IY+$)" , 23 ,  0 ), /* E0 */
			new( "LD C,SET 4,(IY+$)" , 23 ,  0 ), /* E1 */
			new( "LD D,SET 4,(IY+$)" , 23 ,  0 ), /* E2 */
			new( "LD E,SET 4,(IY+$)" , 23 ,  0 ), /* E3 */
			new( "LD H,SET 4,(IY+$)" , 23 ,  0 ), /* E4 */
			new( "LD L,SET 4,(IY+$)" , 23 ,  0 ), /* E5 */
			new( "SET 4,(IY+$)"      , 23 ,  0 ), /* E6 */
			new( "LD A,SET 4,(IY+$)" , 23 ,  0 ), /* E7 */
			new( "LD B,SET 5,(IY+$)" , 23 ,  0 ), /* E8 */
			new( "LD C,SET 5,(IY+$)" , 23 ,  0 ), /* E9 */
			new( "LD D,SET 5,(IY+$)" , 23 ,  0 ), /* EA */
			new( "LD E,SET 5,(IY+$)" , 23 ,  0 ), /* EB */
			new( "LD H,SET 5,(IY+$)" , 23 ,  0 ), /* EC */
			new( "LD L,SET 5,(IY+$)" , 23 ,  0 ), /* ED */
			new( "SET 5,(IY+$)"      , 23 ,  0 ), /* EE */
			new( "LD A,SET 5,(IY+$)" , 23 ,  0 ), /* EF */
			new( "LD B,SET 6,(IY+$)" , 23 ,  0 ), /* F0 */
			new( "LD C,SET 6,(IY+$)" , 23 ,  0 ), /* F1 */
			new( "LD D,SET 6,(IY+$)" , 23 ,  0 ), /* F2 */
			new( "LD E,SET 6,(IY+$)" , 23 ,  0 ), /* F3 */
			new( "LD H,SET 6,(IY+$)" , 23 ,  0 ), /* F4 */
			new( "LD L,SET 6,(IY+$)" , 23 ,  0 ), /* F5 */
			new( "SET 6,(IY+$)"      , 23 ,  0 ), /* F6 */
			new( "LD A,SET 6,(IY+$)" , 23 ,  0 ), /* F7 */
			new( "LD B,SET 7,(IY+$)" , 23 ,  0 ), /* F8 */
			new( "LD C,SET 7,(IY+$)" , 23 ,  0 ), /* F9 */
			new( "LD D,SET 7,(IY+$)" , 23 ,  0 ), /* FA */
			new( "LD E,SET 7,(IY+$)" , 23 ,  0 ), /* FB */
			new( "LD H,SET 7,(IY+$)" , 23 ,  0 ), /* FC */
			new( "LD L,SET 7,(IY+$)" , 23 ,  0 ), /* FD */
			new( "SET 7,(IY+$)"      , 23 ,  0 ), /* FE */
			new( "LD A,SET 7,(IY+$)" , 23 ,  0 ), /* FF */
        ]);
}
