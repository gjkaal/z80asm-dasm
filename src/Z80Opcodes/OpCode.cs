namespace Z80Mnemonics;

public class OpCode
{
    public string altMnemonic;
    public OpCodeFlags flags;
    public string mnemonic;
    public int t_states;
    public int t_states2;

    public OpCode(
        string? mnemonic,
        int t_states,
        int t_states2,
        OpCodeFlags flags = OpCodeFlags.Continues)
    {
        this.altMnemonic = string.Empty;
        this.mnemonic = mnemonic ?? string.Empty;
        this.t_states = t_states;
        this.t_states2 = t_states2;
        this.flags = flags;

        if (mnemonic != null)
        {
            if (mnemonic.Contains("(@)", StringComparison.CurrentCulture))
                this.flags |= OpCodeFlags.RefAddr;

            if (mnemonic.StartsWith("IN ") || mnemonic.StartsWith("OUT "))
                this.flags |= OpCodeFlags.PortRef;

            if (mnemonic.StartsWith("CALL "))
                this.flags |= OpCodeFlags.Call;

            if ((this.flags & OpCodeFlags.ImplicitA) != 0)
            {
                int spacePos = this.mnemonic.IndexOf(' ');
                if (spacePos >= 0)
                {
                    altMnemonic = string.Concat(mnemonic.AsSpan(0, spacePos + 1), "A,", mnemonic.AsSpan(spacePos + 1));
                }
                else
                {
                    altMnemonic = mnemonic + " A";
                }
            }
        }
    }
}