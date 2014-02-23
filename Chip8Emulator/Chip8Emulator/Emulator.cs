using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;

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

            byte[] loadedProgramBytes = null;
            try
            {
                loadedProgramBytes = File.ReadAllBytes(path);
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }

            /*using (FileStream fs = File.OpenRead(path))
            {
                Console.WriteLine(fs.Length);
                byte[] buffer = new byte[246];
                UTF8Encoding enc = new UTF8Encoding();
                fs.Read(buffer, 0, buffer.Length); */

                int i = 0;
                while (i < loadedProgramBytes.Length)
                {
                    byte first = loadedProgramBytes[i];
                    byte second = loadedProgramBytes[++i];
                    ushort opcode = (ushort)((first) << 8 | (second));
                    //int sopcode = opcode & 0xF000;

                    Console.WriteLine(opcode.ToString("X2"));

                    i++;

                }
            //}
        }

        public void startGame()
        {
            while(true)
            {

            }
        }

        public void emulateCycle()
        {
        }

        public void drawFlag()
        {
        }

        public void setKeys()
        {
        }
    }
}
