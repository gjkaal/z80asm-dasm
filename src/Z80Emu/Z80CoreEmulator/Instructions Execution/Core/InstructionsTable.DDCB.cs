﻿using System;

namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        private Func<byte, byte>[] DDCB_InstructionExecutors = [];

        private void Initialize_DDCB_InstructionsTable()
        {
            DDCB_InstructionExecutors =
            [
                RLC_aIX_plus_n_and_load_B,    //00
                RLC_aIX_plus_n_and_load_C,    //01
                RLC_aIX_plus_n_and_load_D,    //02
                RLC_aIX_plus_n_and_load_E,    //03
                RLC_aIX_plus_n_and_load_H,    //04
                RLC_aIX_plus_n_and_load_L,    //05
                RLC_aIX_plus_n,    //06
                RLC_aIX_plus_n_and_load_A,    //07
                RRC_aIX_plus_n_and_load_B,    //08
                RRC_aIX_plus_n_and_load_C,    //09
                RRC_aIX_plus_n_and_load_D,    //0A
                RRC_aIX_plus_n_and_load_E,    //0B
                RRC_aIX_plus_n_and_load_H,    //0C
                RRC_aIX_plus_n_and_load_L,    //0D
                RRC_aIX_plus_n,    //0E
                RRC_aIX_plus_n_and_load_A,    //0F
                RL_aIX_plus_n_and_load_B,    //10
                RL_aIX_plus_n_and_load_C,    //11
                RL_aIX_plus_n_and_load_D,    //12
                RL_aIX_plus_n_and_load_E,    //13
                RL_aIX_plus_n_and_load_H,    //14
                RL_aIX_plus_n_and_load_L,    //15
                RL_aIX_plus_n,    //16
                RL_aIX_plus_n_and_load_A,    //17
                RR_aIX_plus_n_and_load_B,    //18
                RR_aIX_plus_n_and_load_C,    //19
                RR_aIX_plus_n_and_load_D,    //1A
                RR_aIX_plus_n_and_load_E,    //1B
                RR_aIX_plus_n_and_load_H,    //1C
                RR_aIX_plus_n_and_load_L,    //1D
                RR_aIX_plus_n,    //1E
                RR_aIX_plus_n_and_load_A,    //1F
                SLA_aIX_plus_n_and_load_B,    //20
                SLA_aIX_plus_n_and_load_C,    //21
                SLA_aIX_plus_n_and_load_D,    //22
                SLA_aIX_plus_n_and_load_E,    //23
                SLA_aIX_plus_n_and_load_H,    //24
                SLA_aIX_plus_n_and_load_L,    //25
                SLA_aIX_plus_n,    //26
                SLA_aIX_plus_n_and_load_A,    //27
                SRA_aIX_plus_n_and_load_B,    //28
                SRA_aIX_plus_n_and_load_C,    //29
                SRA_aIX_plus_n_and_load_D,    //2A
                SRA_aIX_plus_n_and_load_E,    //2B
                SRA_aIX_plus_n_and_load_H,    //2C
                SRA_aIX_plus_n_and_load_L,    //2D
                SRA_aIX_plus_n,    //2E
                SRA_aIX_plus_n_and_load_A,    //2F
                SLL_aIX_plus_n_and_load_B,    //30
                SLL_aIX_plus_n_and_load_C,    //31
                SLL_aIX_plus_n_and_load_D,    //32
                SLL_aIX_plus_n_and_load_E,    //33
                SLL_aIX_plus_n_and_load_H,    //34
                SLL_aIX_plus_n_and_load_L,    //35
                SLL_aIX_plus_n,    //36
                SLL_aIX_plus_n_and_load_A,    //37
                SRL_aIX_plus_n_and_load_B,    //38
                SRL_aIX_plus_n_and_load_C,    //39
                SRL_aIX_plus_n_and_load_D,    //3A
                SRL_aIX_plus_n_and_load_E,    //3B
                SRL_aIX_plus_n_and_load_H,    //3C
                SRL_aIX_plus_n_and_load_L,    //3D
                SRL_aIX_plus_n,    //3E
                SRL_aIX_plus_n_and_load_A,    //3F
                BIT_0_aIX_plus_n,    //40
                BIT_0_aIX_plus_n,    //41
                BIT_0_aIX_plus_n,    //42
                BIT_0_aIX_plus_n,    //43
                BIT_0_aIX_plus_n,    //44
                BIT_0_aIX_plus_n,    //45
                BIT_0_aIX_plus_n,    //46
                BIT_0_aIX_plus_n,    //47
                BIT_1_aIX_plus_n,    //48
                BIT_1_aIX_plus_n,    //49
                BIT_1_aIX_plus_n,    //4A
                BIT_1_aIX_plus_n,    //4B
                BIT_1_aIX_plus_n,    //4C
                BIT_1_aIX_plus_n,    //4D
                BIT_1_aIX_plus_n,    //4E
                BIT_1_aIX_plus_n,    //4F
                BIT_2_aIX_plus_n,    //50
                BIT_2_aIX_plus_n,    //51
                BIT_2_aIX_plus_n,    //52
                BIT_2_aIX_plus_n,    //53
                BIT_2_aIX_plus_n,    //54
                BIT_2_aIX_plus_n,    //55
                BIT_2_aIX_plus_n,    //56
                BIT_2_aIX_plus_n,    //57
                BIT_3_aIX_plus_n,    //58
                BIT_3_aIX_plus_n,    //59
                BIT_3_aIX_plus_n,    //5A
                BIT_3_aIX_plus_n,    //5B
                BIT_3_aIX_plus_n,    //5C
                BIT_3_aIX_plus_n,    //5D
                BIT_3_aIX_plus_n,    //5E
                BIT_3_aIX_plus_n,    //5F
                BIT_4_aIX_plus_n,    //60
                BIT_4_aIX_plus_n,    //61
                BIT_4_aIX_plus_n,    //62
                BIT_4_aIX_plus_n,    //63
                BIT_4_aIX_plus_n,    //64
                BIT_4_aIX_plus_n,    //65
                BIT_4_aIX_plus_n,    //66
                BIT_4_aIX_plus_n,    //67
                BIT_5_aIX_plus_n,    //68
                BIT_5_aIX_plus_n,    //69
                BIT_5_aIX_plus_n,    //6A
                BIT_5_aIX_plus_n,    //6B
                BIT_5_aIX_plus_n,    //6C
                BIT_5_aIX_plus_n,    //6D
                BIT_5_aIX_plus_n,    //6E
                BIT_5_aIX_plus_n,    //6F
                BIT_6_aIX_plus_n,    //70
                BIT_6_aIX_plus_n,    //71
                BIT_6_aIX_plus_n,    //72
                BIT_6_aIX_plus_n,    //73
                BIT_6_aIX_plus_n,    //74
                BIT_6_aIX_plus_n,    //75
                BIT_6_aIX_plus_n,    //76
                BIT_6_aIX_plus_n,    //77
                BIT_7_aIX_plus_n,    //78
                BIT_7_aIX_plus_n,    //79
                BIT_7_aIX_plus_n,    //7A
                BIT_7_aIX_plus_n,    //7B
                BIT_7_aIX_plus_n,    //7C
                BIT_7_aIX_plus_n,    //7D
                BIT_7_aIX_plus_n,    //7E
                BIT_7_aIX_plus_n,    //7F
                RES_0_aIX_plus_n_and_load_B,    //80
                RES_0_aIX_plus_n_and_load_C,    //81
                RES_0_aIX_plus_n_and_load_D,    //82
                RES_0_aIX_plus_n_and_load_E,    //83
                RES_0_aIX_plus_n_and_load_H,    //84
                RES_0_aIX_plus_n_and_load_L,    //85
                RES_0_aIX_plus_n,    //86
                RES_0_aIX_plus_n_and_load_A,    //87
                RES_1_aIX_plus_n_and_load_B,    //88
                RES_1_aIX_plus_n_and_load_C,    //89
                RES_1_aIX_plus_n_and_load_D,    //8A
                RES_1_aIX_plus_n_and_load_E,    //8B
                RES_1_aIX_plus_n_and_load_H,    //8C
                RES_1_aIX_plus_n_and_load_L,    //8D
                RES_1_aIX_plus_n,    //8E
                RES_1_aIX_plus_n_and_load_A,    //8F
                RES_2_aIX_plus_n_and_load_B,    //90
                RES_2_aIX_plus_n_and_load_C,    //91
                RES_2_aIX_plus_n_and_load_D,    //92
                RES_2_aIX_plus_n_and_load_E,    //93
                RES_2_aIX_plus_n_and_load_H,    //94
                RES_2_aIX_plus_n_and_load_L,    //95
                RES_2_aIX_plus_n,    //96
                RES_2_aIX_plus_n_and_load_A,    //97
                RES_3_aIX_plus_n_and_load_B,    //98
                RES_3_aIX_plus_n_and_load_C,    //99
                RES_3_aIX_plus_n_and_load_D,    //9A
                RES_3_aIX_plus_n_and_load_E,    //9B
                RES_3_aIX_plus_n_and_load_H,    //9C
                RES_3_aIX_plus_n_and_load_L,    //9D
                RES_3_aIX_plus_n,    //9E
                RES_3_aIX_plus_n_and_load_A,    //9F
                RES_4_aIX_plus_n_and_load_B,    //A0
                RES_4_aIX_plus_n_and_load_C,    //A1
                RES_4_aIX_plus_n_and_load_D,    //A2
                RES_4_aIX_plus_n_and_load_E,    //A3
                RES_4_aIX_plus_n_and_load_H,    //A4
                RES_4_aIX_plus_n_and_load_L,    //A5
                RES_4_aIX_plus_n,    //A6
                RES_4_aIX_plus_n_and_load_A,    //A7
                RES_5_aIX_plus_n_and_load_B,    //A8
                RES_5_aIX_plus_n_and_load_C,    //A9
                RES_5_aIX_plus_n_and_load_D,    //AA
                RES_5_aIX_plus_n_and_load_E,    //AB
                RES_5_aIX_plus_n_and_load_H,    //AC
                RES_5_aIX_plus_n_and_load_L,    //AD
                RES_5_aIX_plus_n,    //AE
                RES_5_aIX_plus_n_and_load_A,    //AF
                RES_6_aIX_plus_n_and_load_B,    //B0
                RES_6_aIX_plus_n_and_load_C,    //B1
                RES_6_aIX_plus_n_and_load_D,    //B2
                RES_6_aIX_plus_n_and_load_E,    //B3
                RES_6_aIX_plus_n_and_load_H,    //B4
                RES_6_aIX_plus_n_and_load_L,    //B5
                RES_6_aIX_plus_n,    //B6
                RES_6_aIX_plus_n_and_load_A,    //B7
                RES_7_aIX_plus_n_and_load_B,    //B8
                RES_7_aIX_plus_n_and_load_C,    //B9
                RES_7_aIX_plus_n_and_load_D,    //BA
                RES_7_aIX_plus_n_and_load_E,    //BB
                RES_7_aIX_plus_n_and_load_H,    //BC
                RES_7_aIX_plus_n_and_load_L,    //BD
                RES_7_aIX_plus_n,    //BE
                RES_7_aIX_plus_n_and_load_A,    //BF
                SET_0_aIX_plus_n_and_load_B,    //C0
                SET_0_aIX_plus_n_and_load_C,    //C1
                SET_0_aIX_plus_n_and_load_D,    //C2
                SET_0_aIX_plus_n_and_load_E,    //C3
                SET_0_aIX_plus_n_and_load_H,    //C4
                SET_0_aIX_plus_n_and_load_L,    //C5
                SET_0_aIX_plus_n,    //C6
                SET_0_aIX_plus_n_and_load_A,    //C7
                SET_1_aIX_plus_n_and_load_B,    //C8
                SET_1_aIX_plus_n_and_load_C,    //C9
                SET_1_aIX_plus_n_and_load_D,    //CA
                SET_1_aIX_plus_n_and_load_E,    //CB
                SET_1_aIX_plus_n_and_load_H,    //CC
                SET_1_aIX_plus_n_and_load_L,    //CD
                SET_1_aIX_plus_n,    //CE
                SET_1_aIX_plus_n_and_load_A,    //CF
                SET_2_aIX_plus_n_and_load_B,    //D0
                SET_2_aIX_plus_n_and_load_C,    //D1
                SET_2_aIX_plus_n_and_load_D,    //D2
                SET_2_aIX_plus_n_and_load_E,    //D3
                SET_2_aIX_plus_n_and_load_H,    //D4
                SET_2_aIX_plus_n_and_load_L,    //D5
                SET_2_aIX_plus_n,    //D6
                SET_2_aIX_plus_n_and_load_A,    //D7
                SET_3_aIX_plus_n_and_load_B,    //D8
                SET_3_aIX_plus_n_and_load_C,    //D9
                SET_3_aIX_plus_n_and_load_D,    //DA
                SET_3_aIX_plus_n_and_load_E,    //DB
                SET_3_aIX_plus_n_and_load_H,    //DC
                SET_3_aIX_plus_n_and_load_L,    //DD
                SET_3_aIX_plus_n,    //DE
                SET_3_aIX_plus_n_and_load_A,    //DF
                SET_4_aIX_plus_n_and_load_B,    //E0
                SET_4_aIX_plus_n_and_load_C,    //E1
                SET_4_aIX_plus_n_and_load_D,    //E2
                SET_4_aIX_plus_n_and_load_E,    //E3
                SET_4_aIX_plus_n_and_load_H,    //E4
                SET_4_aIX_plus_n_and_load_L,    //E5
                SET_4_aIX_plus_n,    //E6
                SET_4_aIX_plus_n_and_load_A,    //E7
                SET_5_aIX_plus_n_and_load_B,    //E8
                SET_5_aIX_plus_n_and_load_C,    //E9
                SET_5_aIX_plus_n_and_load_D,    //EA
                SET_5_aIX_plus_n_and_load_E,    //EB
                SET_5_aIX_plus_n_and_load_H,    //EC
                SET_5_aIX_plus_n_and_load_L,    //ED
                SET_5_aIX_plus_n,    //EE
                SET_5_aIX_plus_n_and_load_A,    //EF
                SET_6_aIX_plus_n_and_load_B,    //F0
                SET_6_aIX_plus_n_and_load_C,    //F1
                SET_6_aIX_plus_n_and_load_D,    //F2
                SET_6_aIX_plus_n_and_load_E,    //F3
                SET_6_aIX_plus_n_and_load_H,    //F4
                SET_6_aIX_plus_n_and_load_L,    //F5
                SET_6_aIX_plus_n,    //F6
                SET_6_aIX_plus_n_and_load_A,    //F7
                SET_7_aIX_plus_n_and_load_B,    //F8
                SET_7_aIX_plus_n_and_load_C,    //F9
                SET_7_aIX_plus_n_and_load_D,    //FA
                SET_7_aIX_plus_n_and_load_E,    //FB
                SET_7_aIX_plus_n_and_load_H,    //FC
                SET_7_aIX_plus_n_and_load_L,    //FD
                SET_7_aIX_plus_n,    //FE
                SET_7_aIX_plus_n_and_load_A,    //FF
            ];
        }
    }
}
