namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The IN A,(n) instruction.
        /// </summary>
        byte IN_A_n()
        {
            var portNumber = processorAgent.FetchNextOpcode();
            FetchFinished();

            Registers.A = processorAgent.ReadFromPort(portNumber);

            return 11;
        }
    }
}
