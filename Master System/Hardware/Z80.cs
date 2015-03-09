using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_System.Hardware
{
	public class Z80
	{
		//NOTE: Z80 is Little Endian -> Low bytes come before High bytes, in memory

		byte opcode;

		ushort PC;

		ushort SP;

		ushort IX, IY;

		public byte A, B, C, D, E, F, H, L;
		public byte A1, B1, C1, D1, E1, F1, H1, L1;

		byte I;

		byte R;

		byte[] ram;

		#region RegisterGetSet
		
		public ushort DualRegister(byte upperRegister, byte lowerRegister)
		{
			return (ushort)((upperRegister << 8) | lowerRegister);
		}

		public void SetAF(ushort nnnn)
		{
			A = (byte)((nnnn & 0xFF00) >> 8);
			F = (byte)(nnnn & 0x00FF);
		}

		public void SetBC(ushort nnnn)
		{
			B = (byte)((nnnn & 0xFF00) >> 8);
			C = (byte)(nnnn & 0x00FF);
		}

		public void SetDE(ushort nnnn)
		{
			D = (byte)((nnnn & 0xFF00) >> 8);
			E = (byte)(nnnn & 0x00FF);
		}

		public void SetHL(ushort nnnn)
		{
			H = (byte)((nnnn & 0xFF00) >> 8);
			L = (byte)(nnnn & 0x00FF);
		}

		public void SetAF1(ushort nnnn)
		{
			A1 = (byte)((nnnn & 0xFF00) >> 8);
			F1 = (byte)(nnnn & 0x00FF);
		}

		public void SetBC1(ushort nnnn)
		{
			B1 = (byte)((nnnn & 0xFF00) >> 8);
			C1 = (byte)(nnnn & 0x00FF);
		}

		public void SetDE1(ushort nnnn)
		{
			D1 = (byte)((nnnn & 0xFF00) >> 8);
			E1 = (byte)(nnnn & 0x00FF);
		}

		public void SetHL1(ushort nnnn)
		{
			H1 = (byte)((nnnn & 0xFF00) >> 8);
			L1 = (byte)(nnnn & 0x00FF);
		}

		#endregion

		public Z80(int ramSize = 81920)
		{
			ram = new byte[ramSize];
		}

		public void LoadApplication(string path)
		{
			//Appstartint
			var app = File.ReadAllBytes(path);
			
			for (var i = 0; i < app.Length; ++ i)
			{
				ram[i] = app[i];
			}
		}

		public void Cycle()
		{
			Fetch();
			DecodeAndExecute();
			R += 1;
		}

		public void Fetch()
		{
			opcode = ram[PC];
			Console.WriteLine(opcode.ToString("X2"));
			Console.ReadKey();
		}

		public void DecodeAndExecute()
		{
			switch (opcode)
			{
				case 0x00: //nop
					Console.WriteLine("NOP");
					PC += 1;
					break;

				case 0x01: //Load nn nn into BC
					C = ram[PC + 1];
					B = ram[PC + 2];
					PC += 3;
					break;

				case 0x02: //Load A into [BC]
					ram[DualRegister(B, C)] = A;
					PC += 1;
					break;

				case 0x03: //Add 1 to BC
					SetBC((ushort)(DualRegister(B, C) + 1));
					PC += 1;
					break;

				case 0x04: //Add 1 to B
					B += 1;
					PC += 1;
					break;

				case 0x05: //Sub 1 from B
					B -= 1;
					PC += 1;
					break;

				case 0x06: //Load nn into B
					B = ram[PC + 1];
					PC += 2;
					break;

				case 0x07: //Rotate A left 1 bit. Original bit 7 (bit 0 after rotation) is put in the Carry Flag
					A = (byte)((A << 1) | (A >> 7));
					F = (byte)((F & ~1) | (A & 0x01));
					PC += 1;
					break;

				case 0x08: //Swap AF with AF1
					var temp = DualRegister(A, F);
					SetAF(DualRegister(A1, F1));
					SetAF1(temp);
					PC += 1;
					break;

				case 0x09: //Add BC to HL
					SetHL((ushort)(DualRegister(H, L) + DualRegister(B, C)));
					PC += 1;
					break;

				case 0x0A: //Load [BC] into A
					A = ram[DualRegister(B, C)];
					PC += 1;
					break;

				case 0x0B: //Sub 1 from BC
					SetBC((ushort)(DualRegister(B, C) - 1));
					PC += 1;
					break;

				case 0x0C: //Add 1 to C
					C += 1;
					PC += 1;
					break;

				case 0x0D: //Sub 1 from C
					C -= 1;
					PC += 1;
					break;

				case 0x0E: //Load nn into C
					C = ram[PC + 1];
					PC += 2;
					break;

				case 0x0F: //Rotate A right 1 bit. Original bit 0 (bit 7 after rotation) is put in the Carry Flag
					F = (byte)((F & ~1) | (A & 0x01));
					A = (byte)((A >> 1) | (A << 7));
					PC += 1;
					break;

				case 0x10: //Sub 1 from B. Add nn to PC
					B -= 1;
					if (ram[PC + 1] != 0)
						PC += ram[PC + 1];
					else
						PC += 2;
					break;

				case 0x11: //Load nn nn into DE
					E = ram[PC + 1];
					D = ram[PC + 2];
					PC += 3;
					break;

				case 0x12: //Load A into [DE]
					ram[DualRegister(D, E)] = A;
					PC += 1;
					break;

				case 0x13: //Add 1 to DE
					SetDE((ushort)(DualRegister(D, E) + 1));
					PC += 1;
					break;

				case 0x14: //Add 1 to D
					D += 1;
					PC += 1;
					break;

				case 0x15: //Sub 1 from D
					D -= 1;
					PC += 1;
					break;

				case 0x16: //Load nn into D
					D = ram[PC + 1];
					PC += 2;
					break;

				case 0x17: //Rotate A left 1 bit. Bit 7 is copied to Carry Flag and previous Carry Flag is copied to bit 0
					var tmp = A >> 7;
					A = (byte)((A << 1) | (F & 0x01));
					F = (byte)((F & ~1) | tmp);
					PC += 1;
					break;

				case 0x18: //Add nn to PC
					PC += ram[PC + 1];
					break;

				case 0x19: //Add DE to HL
					SetHL((ushort)(DualRegister(D, E) + DualRegister(H, L)));
					PC += 1;
					break;

				case 0x1A: //Load [DE] into A
					A = ram[DualRegister(D, E)];
					PC += 1;
					break;

				case 0x1B: //Sub 1 from DE
					SetDE((ushort)(DualRegister(D, E) - 1));
					PC += 1;
					break;

				case 0x1C: //Add 1 to E
					E += 1;
					PC += 1;
					break;

				case 0x1D: //Sub 1 from E
					E -= 1;
					PC += 1;
					break;

				case 0x1E: //Load nn into C
					C = ram[PC + 1];
					PC += 2;
					break;

				case 0x1F: //Rotate A right 1 bit. Bit 0 is copied to Carry Flag and previous Carry Flag is copied to bit 7
					var rrca = A << 7;
					A = (byte)((A >> 1) | ((F & 0x01) << 7));
					F = (byte)((F & ~1) | rrca);
					PC += 1;
					break;

				case 0x20: //If Z flag is 0, nn is added to PC
					if ((F & 0x40) == 0)
						PC += ram[PC + 1];
					else
						PC += 2;
					break;

				case 0x21: //Load nn nn into HL
					L = ram[PC + 1];
					H = ram[PC + 2];
					PC += 3;
					break;

				case 0x22: //Store HL in [nn nn]
					ram[ram[PC + 1]] = L;
					ram[ram[PC + 2]] = H;
					PC += 3;
					break;

				case 0x23: //Add 1 to HL
					SetHL((ushort)(DualRegister(H, L) + 1));
					PC += 1;
					break;

				case 0x24: //Add 1 to H
					H += 1;
					PC += 1;
					break;

				case 0x25: //Sub 1 from H
					H -= 1;
					PC += 1;
					break;

				case 0x26: //Load nn into H
					H = ram[PC + 1];
					PC += 2;
					break;

				case 0x27: //DAA - Adjust for BCD operations - from MSX forums http://www.msx.org/forum/semi-msx-talk/emulation/z80-daa
					var r = A;
					var flagH = F & 0x10;
					var flagC = F & 0x01;

					if ((F & 0x02) != 0)
					{
						if ((flagH != 0) || ((A & 0x0F) > 9))
						{
							r -= 6;
						}
						if ((flagC != 0) || ((A > 0x99))) {
							r -= 0x60;
						}
					} else
					{
						if ((flagH != 0) || ((A & 0x0F) > 9))
						{
							r += 6;
						}
						if ((flagC != 0) || ((A > 0x99))) {
							r += 0x60;
						}
					}

					F = (byte)((F & 3) | Tables.DAA_TABLE[r] | ((A > 0x99) ? 1 : 0) | ((A ^ r) & 0x10));
					A = r;
					PC += 1;
					break;

				case 0x28: //If Z is 0, nn is added to PC
					if ((F & 0x40) == 1)
						PC += ram[PC + 1];
					else
						PC += 2;
					break;

				case 0x29: //Add HL to HL
					SetHL((ushort)(DualRegister(H, L) + DualRegister(H, L)));
					PC += 1;
					break;

				case 0x2A: //Load [nn nn] into HL
					H = ram[ram[PC + 2]];
					L = ram[ram[PC + 1]];
					PC += 3;
					break;

				case 0x2B: //Sub 1 from HL
					SetHL((ushort)(DualRegister(H, L) - 1));
					PC += 1;
					break;

				case 0x2C: //Add 1 to L
					L += 1;
					PC += 1;
					break;

				case 0x2D: //Sub 1 from L
					L -= 1;
					PC += 1;
					break;

				case 0x2E: //Load nn into L
					L = ram[PC + 1];
					PC += 2;
					break;

				case 0x2F: //Invert A
					A = (byte) ~A;
					PC += 1;
					break;

				case 0x30: //If C flag is 0, nn is added to PC
					if ((F & 0x01) == 0)
						PC += ram[PC + 1];
					else
						PC += 2;
					break;

				case 0x31: //Load nn nn into SP
					SP = DualRegister(ram[PC + 2], ram[PC + 2]);
					PC += 3;
					break;

				case 0x32: //Store A in [nn nn]
					ram[DualRegister(ram[PC + 2], ram[PC + 1])] = A;
					PC += 3;
					break;

				case 0x33: //Add 1 to SP
					SP += 1;
					PC += 1;
					break;

				case 0x34: //Add 1 to [HL]
					ram[DualRegister(H, L)] += 1;
					PC += 1;
					break;

				case 0x35: //Sub 1 from [HL]
					ram[DualRegister(H, L)] -= 1;
					PC += 1;
					break;

				case 0x36: //Load nn into [HL]
					ram[DualRegister(H, L)] = ram[PC + 1];
					PC += 2;
					break;

				case 0x37: //Set Carry Flag
					F |= 0x01;
					PC += 1;
					break;

				case 0x38://If Carry Flag is 1, add nn to PC
					if ((F & 0x01) == 1)
						PC += ram[PC + 1];
					else
						PC += 2;
					break;

				case 0x39: //Add SP to HL. NOTE: Possibly Add HL to HL. opcode list used, lists this in its description, but I believe it to be incorrect
					SetHL((ushort)(DualRegister(H, L) + SP));
					PC += 1;
					break;

				case 0x3A: //Load [nn nn] into A
					A = ram[DualRegister(ram[PC + 2], ram[PC + 1])];
					PC += 3;
					break;

				case 0x3B: //Sub 1 from SP
					SP -= 1;
					PC += 1;
					break;

				case 0x3C: //Add 1 to A
					A += 1;
					PC += 1;
					break;

				case 0x3D: //Sub 1 from A
					A -= 1;
					PC += 1;
					break;

				case 0x3E: //Load nn into A
					A = ram[PC + 1];
					PC += 2;
					break;

				case 0x3F: //Invert Carry Flag
					F = (byte)(F ^ 0x01);
					PC += 1;
					break;

				case 0x40: //Load B into B
					Console.WriteLine("Load B into B");
					PC += 1;
					break;

				case 0x41: //Load C into B
					B = C;
					PC += 1;
					break;

				case 0x42: //Load D into B
					B = D;
					PC += 1;
					break;

				case 0x43: //Load E into B
					B = E;
					PC += 1;
					break;

				case 0x44: //Load H into B
					B = H;
					PC += 1;
					break;

				case 0x45: //Load L into B
					B = L;
					PC += 1;
					break;

				case 0x46: //Load [HL] into B
					B = ram[DualRegister(H, L)];
					PC += 1;
					break;

				case 0x47: //Load A into B
					B = A;
					PC += 1;
					break;

				case 0x48: //Load B into C
					C = B;
					PC += 1;
					break;

				case 0x49: //Load C into C
					Console.WriteLine("Load C into C");
					PC += 1;
					break;

				case 0x4A: //Load D into C
					C = D;
					PC += 1;
					break;

				case 0x4B: //Load E into C
					C = E;
					PC += 1;
					break;

				case 0x4C: //Load H into C
					C = H;
					PC += 1;
					break;

				case 0x4D: //Load L into C
					C = L;
					PC += 1;
					break;

				case 0x4E: //Load [HL] into C
					C = ram[DualRegister(H, L)];
					PC += 1;
					break;

				case 0x4F: //Load A into C
					C = A;
					PC += 1;
					break;

				case 0x50: //Load B into D
					D = B;
					PC += 1;
					break;

				case 0x51: //Load C into D
					D = C;
					PC += 1;
					break;

				case 0x52: //Load D into D
					Console.WriteLine("Load D into D");
					PC += 1;
					break;

				case 0x53: //Load E into D
					D = E;
					PC += 1;
					break;

				case 0x54: //Load H into D
					D = H;
					PC += 1;
					break;

				case 0x55: //Load L into D
					D = L;
					PC += 1;
					break;

				case 0x56: //Load [HL] into D
					D = ram[DualRegister(H, L)];
					PC += 1;
					break;

				case 0x57: //Load A into D
					D = A;
					PC += 1;
					break;

				case 0x58: //Load B into E
					E = B;
					PC += 1;
					break;

				case 0x59: //Load C into E
					E = C;
					PC += 1;
					break;

				case 0x5A: //Load D into E
					E = D;
					PC += 1;
					break;

				case 0x5B: //Load E into E
					Console.WriteLine("Load E into E");
					PC += 1;
					break;

				case 0x5C: //Load H into E
					E = H;
					PC += 1;
					break;

				case 0x5D: //Load L into E
					E = L;
					PC += 1;
					break;

				case 0x5E: //Load [HL] into E
					E = ram[DualRegister(H, L)];
					PC += 1;
					break;

				case 0x5F: //Load A into E
					E = A;
					PC += 1;
					break;

				case 0x60: //Load B into H
					H = B;
					PC += 1;
					break;

				case 0x61: //Load C into H
					H = C;
					PC += 1;
					break;

				case 0x62: //Load D into H
					H = D;
					PC += 1;
					break;

				case 0x63: //Load E into H
					H = E;
					PC += 1;
					break;

				case 0x64: //Load H into H
					Console.WriteLine("Load H into H");
					PC += 1;
					break;

				case 0x65: //Load L into H
					H = L;
					PC += 1;
					break;

				case 0x66: //Load [HL] into H
					H = ram[DualRegister(H, L)];
					PC += 1;
					break;

				case 0x67: //Load A into H
					H = A;
					PC += 1;
					break;

				case 0x68: //Load B into L
					L = B;
					PC += 1;
					break;

				case 0x69: //Load C into L
					L = C;
					PC += 1;
					break;

				case 0x6A: //Load D into L
					L = D;
					PC += 1;
					break;

				case 0x6B: //Load E into L
					L = E;
					PC += 1;
					break;

				case 0x6C: //Load H into L
					L = H;
					PC += 1;
					break;

				case 0x6D: //Load L into L
					Console.WriteLine("Load L into L");
					PC += 1;
					break;

				case 0x6E: //Load [HL] into L
					L = ram[DualRegister(H, L)];
					PC += 1;
					break;

				case 0x6F: //Load A into L
					L = A;
					PC += 1;
					break;

            }
		}

		public void IncR()
		{
			R += 1;
			
			if ((R & 0x7F) == 127)
			{
				R = (byte)(R & ~0x7F); //Clear lower 6 bits (zero indexed)
			}
		}
    }
}
