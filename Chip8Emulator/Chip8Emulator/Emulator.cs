using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Chip8Emulator
{
    class Emulator
    {
        byte[] memory; //4096 bytes
        byte[] v; //CPU register
        byte[,] gfx; //Pixelarray
        byte[] key; //keypad

        ushort opcode; //current opcode
        ushort I; //Index register
        ushort PC; //Program counter
        ushort stackPointer;
        ushort[] stack;

        //Timer delayTimer = new Timer(1 / 60);
        //Timer soundTimer = new Timer(1 / 60);
        byte delayTimer;
        byte soundTimer;

        public Emulator()
        {
            memory = new byte[4096];
            v = new byte[16];
            gfx = new byte[64, 32];
            key = new byte[16];

            stack = new ushort[16];
        }

        public void loadGame(string path)
        {
            throw new NotImplementedException();
        }
    }
}
