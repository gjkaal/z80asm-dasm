﻿// AUTOGENERATED CODE
//
// Do not make changes directly to this (.cs) file.
// Change "LD (aa),rr        .tt" instead.

namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The LD (aa),HL instruction.
        /// </summary>
        private byte LD_aa_HL()
        {
            var address = (ushort)FetchWord();
            FetchFinished();

            WriteShortToMemory(address, Registers.HL);

            return 16;
        }

        /// <summary>
        /// The LD (aa),DE instruction.
        /// </summary>
        private byte LD_aa_DE()
        {
            var address = (ushort)FetchWord();
            FetchFinished();

            WriteShortToMemory(address, Registers.DE);

            return 20;
        }

        /// <summary>
        /// The LD (aa),BC instruction.
        /// </summary>
        private byte LD_aa_BC()
        {
            var address = (ushort)FetchWord();
            FetchFinished();

            WriteShortToMemory(address, Registers.BC);

            return 20;
        }

        /// <summary>
        /// The LD (aa),SP instruction.
        /// </summary>
        private byte LD_aa_SP()
        {
            var address = (ushort)FetchWord();
            FetchFinished();

            WriteShortToMemory(address, Registers.SP);

            return 20;
        }

        /// <summary>
        /// The LD (aa),IX instruction.
        /// </summary>
        private byte LD_aa_IX()
        {
            var address = (ushort)FetchWord();
            FetchFinished();

            WriteShortToMemory(address, Registers.IX);

            return 20;
        }

        /// <summary>
        /// The LD (aa),IY instruction.
        /// </summary>
        private byte LD_aa_IY()
        {
            var address = (ushort)FetchWord();
            FetchFinished();

            WriteShortToMemory(address, Registers.IY);

            return 20;
        }
    }
}