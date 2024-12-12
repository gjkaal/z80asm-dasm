namespace Z80Mnemonics;

public class OpCode
{
    public string altMnemonic { get; private set; }
    public OpCodeFlags flags { get; private set; }
    public string mnemonic { get; private set; }
    public int t_states { get; private set; }
    public int t_states2 { get; private set; }

    public OpCode(
        string? mnemonic,
        int t_states,
        int t_states2,
        OpCodeFlags flags = OpCodeFlags.Continues
        )
    {
        altMnemonic = string.Empty;
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
                var spacePos = this.mnemonic.IndexOf(' ');
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