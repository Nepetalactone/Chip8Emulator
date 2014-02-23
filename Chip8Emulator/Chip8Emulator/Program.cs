using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Emulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Emulator emu = new Emulator();
            emu.loadGame(@"I:\Documents\Chip8Emulator\Chip8Emulator\Chip8Emulator\ROM\PONG");
            Console.ReadKey();
        }
    }
}
