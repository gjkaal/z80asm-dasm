﻿// AUTOGENERATED CODE
//
// Do not make changes directly to this (.cs) file.
// Change "RST                   .tt" instead.

namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The RST 00h instruction.
        /// </summary>
        private byte RST_00()
        {
            FetchFinished();

            var valueToPush = (short)Registers.PC;
            var sp = (ushort)(Registers.SP - 1);
            processorAgent.WriteToMemory(sp, valueToPush.GetHighByte());
            sp--;
            processorAgent.WriteToMemory(sp, valueToPush.GetLowByte());
            Registers.DecSp();
            Registers.PC = 0x00;

            return 11;
        }

        /// <summary>
        /// The RST 08h instruction.
        /// </summary>
        private byte RST_08()
        {
            FetchFinished();

            var valueToPush = (short)Registers.PC;
            var sp = (ushort)(Registers.SP - 1);
            processorAgent.WriteToMemory(sp, valueToPush.GetHighByte());
            sp--;
            processorAgent.WriteToMemory(sp, valueToPush.GetLowByte());
            Registers.DecSp();
            Registers.PC = 0x08;

            return 11;
        }

        /// <summary>
        /// The RST 10h instruction.
        /// </summary>
        private byte RST_10()
        {
            FetchFinished();

            var valueToPush = (short)Registers.PC;
            var sp = (ushort)(Registers.SP - 1);
            processorAgent.WriteToMemory(sp, valueToPush.GetHighByte());
            sp--;
            processorAgent.WriteToMemory(sp, valueToPush.GetLowByte());
            Registers.DecSp();
            Registers.PC = 0x10;

            return 11;
        }

        /// <summary>
        /// The RST 18h instruction.
        /// </summary>
        private byte RST_18()
        {
            FetchFinished();

            var valueToPush = (short)Registers.PC;
            var sp = (ushort)(Registers.SP - 1);
            processorAgent.WriteToMemory(sp, valueToPush.GetHighByte());
            sp--;
            processorAgent.WriteToMemory(sp, valueToPush.GetLowByte());
            Registers.DecSp();
            Registers.PC = 0x18;

            return 11;
        }

        /// <summary>
        /// The RST 20h instruction.
        /// </summary>
        private byte RST_20()
        {
            FetchFinished();

            var valueToPush = (short)Registers.PC;
            var sp = (ushort)(Registers.SP - 1);
            processorAgent.WriteToMemory(sp, valueToPush.GetHighByte());
            sp--;
            processorAgent.WriteToMemory(sp, valueToPush.GetLowByte());
            Registers.DecSp();
            Registers.PC = 0x20;

            return 11;
        }

        /// <summary>
        /// The RST 28h instruction.
        /// </summary>
        private byte RST_28()
        {
            FetchFinished();

            var valueToPush = (short)Registers.PC;
            var sp = (ushort)(Registers.SP - 1);
            processorAgent.WriteToMemory(sp, valueToPush.GetHighByte());
            sp--;
            processorAgent.WriteToMemory(sp, valueToPush.GetLowByte());
            Registers.DecSp();
            Registers.PC = 0x28;

            return 11;
        }

        /// <summary>
        /// The RST 30h instruction.
        /// </summary>
        private byte RST_30()
        {
            FetchFinished();

            var valueToPush = (short)Registers.PC;
            var sp = (ushort)(Registers.SP - 1);
            processorAgent.WriteToMemory(sp, valueToPush.GetHighByte());
            sp--;
            processorAgent.WriteToMemory(sp, valueToPush.GetLowByte());
            Registers.DecSp();
            Registers.PC = 0x30;

            return 11;
        }

        /// <summary>
        /// The RST 38h instruction.
        /// </summary>
        private byte RST_38()
        {
            FetchFinished();

            var valueToPush = (short)Registers.PC;
            var sp = (ushort)(Registers.SP - 1);
            processorAgent.WriteToMemory(sp, valueToPush.GetHighByte());
            sp--;
            processorAgent.WriteToMemory(sp, valueToPush.GetLowByte());
            Registers.DecSp();
            Registers.PC = 0x38;

            return 11;
        }
    }
}