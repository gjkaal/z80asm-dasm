﻿using System;
using System.Collections.Generic;

namespace Z80DAsm
{
    public class Proc
    {
        public Proc(Instruction instruction)
        {
            if (instruction.proc != null)
                throw new InvalidOperationException("Instruction already associated with another procedure");

            firstInstruction = instruction;
            firstInstruction.proc = this;
        }

        public Instruction firstInstruction;
        public bool hasLocalReturn;
        public List<int> dependants;
        public int lengthInBytes;
    }
}