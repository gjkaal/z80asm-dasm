using Microsoft.Win32;

namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        private const int HF_NF_reset = 0xED;
        private const int CF_set = 1;

        /// <summary>
        /// The SCF instruction.
        /// </summary>
        private byte SCF()
        {
            FetchFinished();

            var flags = Registers.F;
            var newValue = (byte)((flags & HF_NF_reset) | CF_set);
            Registers.ChangeFlags(newValue);
            Registers.SetFlags3and5From(Registers.A);

            return 4;
        }
    }
}
