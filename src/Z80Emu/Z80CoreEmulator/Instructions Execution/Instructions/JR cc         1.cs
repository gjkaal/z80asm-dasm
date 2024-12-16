﻿// AUTOGENERATED CODE
//
// Do not make changes directly to this (.cs) file.
// Change "JR cc         .tt" instead.

namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The JR C,d instruction.
        /// </summary>
        private byte JR_C_d()
        {
            var offset = processorAgent.FetchNextOpcode();
            FetchFinished();

            if (Registers.CF == 0)
                return 7;

            Registers.PC = (ushort)(Registers.PC + (SByte)offset);
            return 12;
        }

        /// <summary>
        /// The JR NC,d instruction.
        /// </summary>
        private byte JR_NC_d()
        {
            var offset = processorAgent.FetchNextOpcode();
            FetchFinished();

            if (Registers.CF == 1)
                return 7;

            Registers.PC = (ushort)(Registers.PC + (SByte)offset);
            return 12;
        }

        /// <summary>
        /// The JR Z,d instruction.
        /// </summary>
        private byte JR_Z_d()
        {
            var offset = processorAgent.FetchNextOpcode();
            FetchFinished();

            if (Registers.ZF == 0)
                return 7;

            Registers.PC = (ushort)(Registers.PC + (SByte)offset);
            return 12;
        }

        /// <summary>
        /// The JR NZ,d instruction.
        /// </summary>
        private byte JR_NZ_d()
        {
            var offset = processorAgent.FetchNextOpcode();
            FetchFinished();

            if (Registers.ZF == 1)
                return 7;

            Registers.PC = (ushort)(Registers.PC + (SByte)offset);
            return 12;
        }
    }
}