namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Default implementation of <see cref="IMainZ80Registers"/>.
    /// </summary>
    public class MainZ80Registers : IMainZ80Registers
    {
        private readonly object sync = new();
        private short aF;
        private short bC;
        private short dE;
        private short hL;

        public short AF
        {
            get => aF;
            set { lock (sync) aF = value; }
        }

        public short BC
        { get => bC; set { lock (sync) bC = value; } }

        public short DE
        { get => dE; set { lock (sync) dE = value; } }

        public short HL
        {
            get => hL; set
            {
                lock (sync) hL = value;
            }
        }

        public byte A
        {
            get => AF.GetHighByte();
            set
            {
                lock (sync)
                {
                    aF = AF.SetHighByte(value);
                }
            }
        }

        public byte F
        {
            get => AF.GetLowByte();
        }

        public void ChangeFlags(byte value)
        {
            lock (sync)
                aF = AF.SetLowByte(value);
        }

        public byte B
        {
            get => bC.GetHighByte();
            set
            {
                lock (sync)
                    bC = BC.SetHighByte(value);
            }
        }

        public byte C
        {
            get => bC.GetLowByte();
            set
            {
                lock (sync) bC = BC.SetLowByte(value);
            }
        }

        public byte D
        {
            get => dE.GetHighByte();
            set
            {
                lock (sync) dE = DE.SetHighByte(value);
            }
        }

        public byte E
        {
            get => dE.GetLowByte();
            set
            {
                lock (sync) dE = DE.SetLowByte(value);
            }
        }

        public byte H
        {
            get => hL.GetHighByte();
            set
            {
                lock (sync) hL = HL.SetHighByte(value);
            }
        }

        public byte L
        {
            get => hL.GetLowByte();
            set { lock (sync) hL = HL.SetLowByte(value); }
        }

        public Bit CF
        {
            get => aF.GetBit(0);
            set
            {
                lock (sync)
                    aF = aF.SetBit(0, value);
            }
        }

        public Bit NF
        {
            get => aF.GetBit(1);
            set
            {
                lock (sync)
                    aF = AF.SetBit(1, value);
            }
        }

        public Bit PF
        {
            get => AF.GetBit(2);
            set
            {
                lock (sync)
                    aF = AF.SetBit(2, value);
            }
        }

        public Bit Flag3
        {
            get => aF.GetBit(3);
            set
            {
                lock (sync)
                    aF = AF.SetBit(3, value);
            }
        }

        public Bit HF
        {
            get => aF.GetBit(4);
            set
            {
                lock (sync)
                    aF = AF.SetBit(4, value);
            }
        }

        public Bit Flag5
        {
            get => aF.GetBit(5);
            set
            {
                lock (sync)
                    aF = AF.SetBit(5, value);
            }
        }

        public Bit ZF
        {
            get => aF.GetBit(6);
            set
            {
                lock (sync) aF = AF.SetBit(6, value);
            }
        }

        public Bit SF
        {
            get => aF.GetBit(7);
            set
            {
                lock (sync) aF = AF.SetBit(7, value);
            }
        }
    }
}