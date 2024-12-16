namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The OUT (n),A instruction.
        /// </summary>
        byte OUT_n_A()
        {
            var portNumber = processorAgent.FetchNextOpcode();
            FetchFinished();

            processorAgent.WriteToPort(portNumber, Registers.A);

            return 11;
        }
    }
}
