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
        String[] program2;  //contains the program as opcodes

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
            I = 0;
            PC = 0;
            stackPointer = 0;
            delayTimer = 0;
            soundTimer = 0;
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

            program2 = new String[loadedProgramBytes.Length / 2];

            int y = 0;

            for (int i = 0; i < loadedProgramBytes.Length; i = i + 2)
            {
                program2[y] = convertBytesToOpcode(loadedProgramBytes[i], loadedProgramBytes[i + 1]);
            }
        }

        public string convertBytesToOpcode(byte first, byte second)
        {
            return first.ToString("X2") + second.ToString("X2");
        }

        public void startGame()
        {
            while (true)
            {
                emulateCycle(program2[PC]);
            }
        }

        public void emulateCycle(String opcode)
        {
            if (opcode.StartsWith("0"))
            {
                if (opcode == "00E0")
                {
                    //Clears the screen
                }
                else if (opcode == "00EE")
                {
                    //Returns from a subroutine
                }
                else
                {
                    //0NNN Calls RCA 1802 program at address NNN
                }
            }
            else if (opcode.StartsWith("1"))
            {
                //1NNN Jumps to address NNN
                PC = ushort.Parse(opcode.Substring(1));
            }
            else if (opcode.StartsWith("2"))
            {
                //2NNN Calls subroutine at NNN
            }
            else if (opcode.StartsWith("3"))
            {
                //3XNN Skips the next instruction if VX equals NN
                if (v[opcode[1]].Equals(Byte.Parse(opcode.Substring(2))))
                {
                    PC += 1;
                }
            }
            else if (opcode.StartsWith("4"))
            {
                //4XNN	Skips the next instruction if VX doesn't equal NN
                if (!v[opcode[1]].Equals(Byte.Parse(opcode.Substring(2))))
                {
                    PC += 1;
                }
            }
            else if (opcode.StartsWith("5"))
            {
                //5XY0	Skips the next instruction if VX equals VY
                if (v[opcode[1]].Equals(v[opcode[2]]))
                {
                    PC += 1;
                }
            }
            else if (opcode.StartsWith("6"))
            {
                //6XNN	Sets VX to NN
                v[opcode[1]] = byte.Parse(opcode.Substring(2));
            }
            else if (opcode.StartsWith("7"))
            {
                //7XNN	Adds NN to VX
                v[opcode[1]] += byte.Parse(opcode.Substring(2));
            }
            else if (opcode.StartsWith("8"))
            {
                if (opcode.EndsWith("0"))
                {
                    //8XY0	Sets VX to the value of VY
                    v[opcode[1]] = v[opcode[2]];
                }
                else if (opcode.EndsWith("1"))
                {
                    //8XY1	Sets VX to VX or VY
                    v[opcode[1]] = (byte)(v[opcode[1]] | v[opcode[2]]);
                }
                else if (opcode.EndsWith("2"))
                {
                    //8XY2	Sets VX to VX and VY
                    v[opcode[1]] = (byte)(v[opcode[1]] & v[opcode[2]]);
                }
                else if (opcode.EndsWith("3"))
                {
                    //8XY3	Sets VX to VX xor VY
                    v[opcode[1]] = (byte)(v[opcode[1]] ^ v[opcode[2]]);
                }
                else if (opcode.EndsWith("4"))
                {
                    //8XY4	Adds VY to VX. VF is set to 1 when there's a carry, and to 0 when there isn't
                    byte temp = (byte)(v[opcode[1]] + v[opcode[2]]);
                    if (temp < v[opcode[1]])
                    {
                        v[15] = 1;
                    }
                }
                else if (opcode.EndsWith("5"))
                {
                    //8XY5	VY is subtracted from VX. VF is set to 0 when there's a borrow, and 1 when there isn't
                    byte temp = (byte)(v[opcode[1]] - v[opcode[2]]);
                    if (temp > v[opcode[1]])
                    {
                        v[15] = 0;
                    }
                }
                else if (opcode.EndsWith("6"))
                {
                    //8XY6	Shifts VX right by one. VF is set to the value of the least significant bit of VX before the shift
                    v[15] = (byte)(v[opcode[1]] & 0x0000001);
                    v[opcode[1]] = (byte)(v[opcode[1]] >> 1);
                }
                else if (opcode.EndsWith("7"))
                {
                    //8XY7	Sets VX to VY minus VX. VF is set to 0 when there's a borrow, and 1 when there isn't
                    byte temp = (byte)(v[opcode[2]] - v[opcode[1]]);
                    if (temp > v[opcode[2]])
                    {
                        v[15] = 0;
                    }
                    v[opcode[1]] = temp;
                }
                else if (opcode.EndsWith("E"))
                {
                    //8XYE	Shifts VX left by one. VF is set to the value of the most significant bit of VX before the shift
                    v[15] = (byte)(v[opcode[1]] & 0x10000000);
                    v[opcode[1]] = (byte)(v[opcode[1]] << 1);
                }
            }
            else if (opcode.StartsWith("9"))
            {
                //9XY0	Skips the next instruction if VX doesn't equal VY
                if (v[opcode[1]].Equals(v[opcode[2]]))
                {
                    PC += 1;
                }
            }
            else if (opcode.StartsWith("A"))
            {
                //ANNN	Sets I to the address NNN
                I = ushort.Parse(opcode.Substring(1));
            }
            else if (opcode.StartsWith("B"))
            {
                //BNNN	Jumps to the address NNN plus V0
                PC = (ushort)(ushort.Parse(opcode.Substring(1)) + (ushort)v[0]);
            }
            else if (opcode.StartsWith("C"))
            {
                //CXNN	Sets VX to a random number and NN
                Random rng = new Random();
                byte temp = (byte)rng.Next(0, 255);
                v[opcode[1]] = (byte)(temp & byte.Parse(opcode.Substring(2)));
            }
            else if (opcode.StartsWith("D"))
            {
                //Draw a sprite at position VX, VY with N bytes of sprite data starting at the address stored in I
                //Set VF to 01 if any set pixels are changed to unset, and 00 otherwise
            }
            else if (opcode.StartsWith("E"))
            {
                if (opcode.EndsWith("9E"))
                {
                    //EX9E	Skips the next instruction if the key stored in VX is pressed
                }
                else if (opcode.EndsWith("A1"))
                {
                    //Skips the next instruction if the key stored in VX isn't pressed
                }
            }
            else if (opcode.StartsWith("F"))
            {
                if (opcode.EndsWith("07"))
                {
                    //FX07	Store the current value of the delay timer in register VX
                    v[opcode[1]] = delayTimer;
                }
                else if (opcode.EndsWith("0A"))
                {
                    //FX0A	Wait for a keypress and store the result in register VX
                }
                else if (opcode.EndsWith("15"))
                {
                    //FX15	Set the delay timer to the value of register VX
                    delayTimer = v[opcode[1]];
                }
                else if (opcode.EndsWith("18"))
                {
                    soundTimer = v[opcode[1]];
                }
                else if (opcode.EndsWith("1E"))
                {
                    //FX1E	Add the value stored in register VX to register I
                    I += v[opcode[1]];
                }
                else if (opcode.EndsWith("29"))
                {
                }
                else if (opcode.EndsWith("33"))
                {
                }
                else if (opcode.EndsWith("55"))
                {
                }
                else if (opcode.EndsWith("65"))
                {
                }
            }



            /*  0NNN	Calls RCA 1802 program at address NNN.
                00E0	Clears the screen.
                00EE	Returns from a subroutine.
                1NNN	Jumps to address NNN.
                2NNN	Calls subroutine at NNN.
                3XNN	Skips the next instruction if VX equals NN.
                4XNN	Skips the next instruction if VX doesn't equal NN.
                5XY0	Skips the next instruction if VX equals VY.
                6XNN	Sets VX to NN.
                7XNN	Adds NN to VX.
                8XY0	Sets VX to the value of VY.
                8XY1	Sets VX to VX or VY.
                8XY2	Sets VX to VX and VY.
                8XY3	Sets VX to VX xor VY.
                8XY4	Adds VY to VX. VF is set to 1 when there's a carry, and to 0 when there isn't.
                8XY5	VY is subtracted from VX. VF is set to 0 when there's a borrow, and 1 when there isn't.
                8XY6	Shifts VX right by one. VF is set to the value of the least significant bit of VX before the shift.[2]
                8XY7	Sets VX to VY minus VX. VF is set to 0 when there's a borrow, and 1 when there isn't.
                8XYE	Shifts VX left by one. VF is set to the value of the most significant bit of VX before the shift.[2]
                9XY0	Skips the next instruction if VX doesn't equal VY.
                ANNN	Sets I to the address NNN.
                BNNN	Jumps to the address NNN plus V0.
                CXNN	Sets VX to a random number and NN.
                DXYN	Sprites stored in memory at location in index register (I), maximum 8bits wide. Wraps around the screen. If when drawn, clears a pixel, register VF is set to 1 otherwise it is zero. All drawing is XOR drawing (e.g. it toggles the screen pixels)
                EX9E	Skips the next instruction if the key stored in VX is pressed.
                EXA1	Skips the next instruction if the key stored in VX isn't pressed.
                FX07	Sets VX to the value of the delay timer.
                FX0A	A key press is awaited, and then stored in VX.
                FX15	Sets the delay timer to VX.
                FX18	Sets the sound timer to VX.
                FX1E	Adds VX to I.[3]
                FX29	Sets I to the location of the sprite for the character in VX. Characters 0-F (in hexadecimal) are represented by a 4x5 font.
                FX33	Stores the Binary-coded decimal representation of VX, with the most significant of three digits at the address in I, the middle digit at I plus 1, and the least significant digit at I plus 2. (In other words, take the decimal representation of VX, place the hundreds digit in memory at location in I, the tens digit at location I+1, and the ones digit at location I+2.)
                FX55	Stores V0 to VX in memory starting at address I.[4]
                FX65	Fills V0 to VX with values from memory starting at address I*/
        }

        public void drawFlag()
        {
        }

        public void setKeys()
        {
        }

        //hide in shame until I find a better solution
        private int convertHexToInt(char hex)
        {
            switch (char.ToUpper(hex))
            {
                case 'A':
                    return 10;
                case 'B':
                    return 11;
                case 'C':
                    return 12;
                case 'D':
                    return 13;
                case 'E':
                    return 14;
                case 'F':
                    return 15;
                default:
                    return (int)hex;
            }
        }
    }
}
