namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Represents a full set of Z80 registers. This is the default implementation of
    /// <see cref="IZ80Registers"/>.
    /// </summary>
    public class Z80Registers : MainZ80Registers, IZ80Registers
    {
        private IMainZ80Registers alternate;

        public Z80Registers()
        {
            Alternate = new MainZ80Registers();
        }

        public IMainZ80Registers Alternate
        {
            get => alternate;
            set
            {
                if (value == null)
                {
                    throw new NullReferenceException();
                }
                alternate = value;
            }
        }

        public short IX { get; set; }

        public short IY { get; set; }

        public ushort PC { get; set; }

        public short SP { get; set; }

        public short IR { get; set; }

        public Bit IFF1 { get; set; }

        public Bit IFF2 { get; set; }

        public byte IXH
        {
            get => IX.GetHighByte();
            set => IX = IX.SetHighByte(value);
        }

        public byte IXL
        {
            get => IX.GetLowByte();
            set => IX = IX.SetLowByte(value);
        }

        public byte IYH
        {
            get => IY.GetHighByte();
            set => IY = IY.SetHighByte(value);
        }

        public byte IYL
        {
            get => IY.GetLowByte();
            set => IY = IY.SetLowByte(value);
        }

        public byte I
        {
            get => IR.GetHighByte();
            set => IR = IR.SetHighByte(value);
        }

        public byte R
        {
            get => IR.GetLowByte();
            set => IR = IR.SetLowByte(value);
        }
    }
}