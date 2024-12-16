using Konamiman.Z80dotNet.Z80EventArgs;
using Microsoft.Win32;

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
                    aF = aF.SetHighByte(value);
                    OnRegisterChanged("A", (aF & 0xFF00) >> 8);
                }
            }
        }

        public byte F
        {
            get => aF.GetLowByte();
        }

        public byte B
        {
            get => bC.GetHighByte();
            set
            {
                lock (sync)
                    bC = bC.SetHighByte(value);
                OnRegisterChanged("B", (bC & 0xFF00) >> 8);
            }
        }

        public byte C
        {
            get => bC.GetLowByte();
            set
            {
                lock (sync) bC = bC.SetLowByte(value);
                OnRegisterChanged("C", bC & 0xFF);
            }
        }

        public byte D
        {
            get => dE.GetHighByte();
            set
            {
                lock (sync) dE = dE.SetHighByte(value);
                OnRegisterChanged("D", (dE & 0xFF00) >> 8);
            }
        }

        public byte E
        {
            get => dE.GetLowByte();
            set
            {
                lock (sync) dE = dE.SetLowByte(value);
                OnRegisterChanged("E", dE & 0xFF);
            }
        }

        public byte H
        {
            get => hL.GetHighByte();
            set
            {
                lock (sync) hL = hL.SetHighByte(value);
                OnRegisterChanged("H", (hL & 0xFF00) >> 8);
            }
        }

        public byte L
        {
            get => hL.GetLowByte();
            set
            {
                lock (sync) hL = hL.SetLowByte(value);
                OnRegisterChanged("L", hL & 0xFF);
            }
        }

        public Bit CF
        {
            get => aF.GetBit(0);
            set
            {
                lock (sync)
                    aF = aF.SetBit(0, value);
                OnRegisterChanged("F", aF & 0xFF);
            }
        }

        public void ChangeFlags(byte value)
        {
            lock (sync)
            {
                aF = (short)((aF & 0xFF00) | value);
            }
            OnRegisterChanged("F", aF & 0xFF);
        }

        public void SetFlags3and5From(byte value)
        {
            lock (sync)
            {
                const int Flags_3_5 = 0x0028;
                var flags = AF;
                aF = (short)((flags & ~Flags_3_5) | (value & Flags_3_5));
            }
            OnRegisterChanged("F", aF & 0xFF);
        }

        public Bit NF
        {
            get => aF.GetBit(1);
            set
            {
                lock (sync)
                    aF = aF.SetBit(1, value);
                OnRegisterChanged("F", aF & 0xFF);
            }
        }

        public Bit PF
        {
            get => AF.GetBit(2);
            set
            {
                lock (sync)
                    aF = aF.SetBit(2, value);
                OnRegisterChanged("F", aF & 0xFF);
            }
        }

        public Bit Flag3
        {
            get => aF.GetBit(3);
            set
            {
                lock (sync)
                    aF = aF.SetBit(3, value);
                OnRegisterChanged("F", aF & 0xFF);
            }
        }

        public Bit HF
        {
            get => aF.GetBit(4);
            set
            {
                lock (sync)
                    aF = aF.SetBit(4, value);
                OnRegisterChanged("F", aF & 0xFF);
            }
        }

        public Bit Flag5
        {
            get => aF.GetBit(5);
            set
            {
                lock (sync)
                    aF = aF.SetBit(5, value);
                OnRegisterChanged("F", aF & 0xFF);
            }
        }

        public Bit ZF
        {
            get => aF.GetBit(6);
            set
            {
                lock (sync) aF = aF.SetBit(6, value);
                OnRegisterChanged("F", aF & 0xFF);
            }
        }

        public Bit SF
        {
            get => aF.GetBit(7);
            set
            {
                lock (sync) aF = aF.SetBit(7, value);
                OnRegisterChanged("F", aF & 0xFF);
            }
        }

        public EventHandler<RegisterChangedEventArgs>? RegisterChanged;

        private void OnRegisterChanged(string registerName, int newValue)
        {
            RegisterChanged?.Invoke(this, new RegisterChangedEventArgs(registerName, (short)newValue, null));
        }
    }
}