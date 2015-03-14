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

		byte CFlag { get { return (byte)(F & 0x01); } }
		byte NFlag { get { return (byte)(F & 0x02); } }
		byte HFlag { get { return (byte)(F & 0x10); } }
		byte ZFlag { get { return (byte)(F & 0x40); } }
		byte SFlag { get { return (byte)(F & 0x80); } }
		byte PVFlag { get { return (byte)(F & 0x04); } }

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
			IncR();
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
					A = (byte)((A << 1) | CFlag);
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
					A = (byte)((A >> 1) | (CFlag << 7));
					F = (byte)((F & ~1) | rrca);
					PC += 1;
					break;

				case 0x20: //If Z flag is 0, nn is added to PC
					if (ZFlag == 0)
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

					if (NFlag != 0)
					{
						if ((HFlag != 0) || ((A & 0x0F) > 9))
						{
							r -= 6;
						}
						if ((CFlag != 0) || ((A > 0x99))) {
							r -= 0x60;
						}
					} else
					{
						if ((HFlag != 0) || ((A & 0x0F) > 9))
						{
							r += 6;
						}
						if ((CFlag != 0) || ((A > 0x99))) {
							r += 0x60;
						}
					}

					F = (byte)((F & 3) | Tables.DAA_TABLE[r] | ((A > 0x99) ? 1 : 0) | ((A ^ r) & 0x10));
					A = r;
					PC += 1;
					break;

				case 0x28: //If Z is 1, nn is added to PC
					if (ZFlag == 1)
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
					if (CFlag == 0)
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
					if (CFlag == 1)
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

				case 0x70: //Load B into [HL]
					ram[DualRegister(H, L)] = B;
					PC += 1;
					break;

				case 0x71: //Load C into [HL]
					ram[DualRegister(H, L)] = C;
					PC += 1;
					break;

				case 0x72: //Load D into [HL]
					ram[DualRegister(H, L)] = D;
					PC += 1;
					break;

				case 0x73: //Load E into [HL]
					ram[DualRegister(H, L)] = E;
					PC += 1;
					break;

				case 0x74: //Load H into [HL]
					ram[DualRegister(H, L)] = H;
					PC += 1;
					break;

				case 0x75: //Load L into [HL]
					ram[DualRegister(H, L)] = L;
					PC += 1;
					break;

				case 0x76: //Halt until interrupt or reset. NOTE: SMS doesn't use this, replace if using Z80 in another emulator
					while(I == 0)
						Console.WriteLine("HALT: NOP");
					PC += 1;
					break;

				case 0x77: //Load A into [HL]
					ram[DualRegister(H, L)] = A;
					PC += 1;
					break;

				case 0x78: //Load B into A
					A = B;
					PC += 1;
					break;

				case 0x79: //Load C into A
					A = C;
					PC += 1;
					break;

				case 0x7A: //Load D into A
					A = D;
					PC += 1;
					break;

				case 0x7B: //Load E into A
					A = E;
					PC += 1;
					break;

				case 0x7C: //Load H into A
					A = H;
					PC += 1;
					break;

				case 0x7D: //Load L into A
					A = L;
					PC += 1;
					break;

				case 0x7E: //Load [HL] into A
					A = ram[DualRegister(H, L)];
					PC += 1;
					break;

				case 0x7F: //Load A into A
					Console.WriteLine("Load A into A");
					PC += 1;
					break;

				case 0x80: //Add B to A
					A += B;
					PC += 1;
					break;

				case 0x81: //Add C to A
					A += C;
					PC += 1;
					break;

				case 0x82: //Add D to A
					A += D;
					PC += 1;
					break;

				case 0x83: //Add E to A
					A += E;
					PC += 1;
					break;

				case 0x84: //Add H to A
					A += H;
					PC += 1;
					break;

				case 0x85: //Add L to A
					A += L;
					PC += 1;
					break;

				case 0x86: //Add [HL] to A
					A += ram[DualRegister(H, L)];
					PC += 1;
					break;

				case 0x87: //Add A to A
					A += A;
					PC += 1;
					break;

				case 0x88: //Add Carry Flag and B to A
					A += (byte)(B + CFlag);
					PC += 1;
					break;

				case 0x89: //Add Carry Flag and C to A
					A += (byte)(C + CFlag);
					PC += 1;
					break;

				case 0x8A: //Add Carry Flag and D to A
					A += (byte)(D + CFlag);
					PC += 1;
					break;

				case 0x8B: //Add Carry Flag and E to A
					A += (byte)(E + CFlag);
					PC += 1;
					break;

				case 0x8C: //Add Carry Flag and H to A
					A += (byte)(H + CFlag);
					PC += 1;
					break;

				case 0x8D: //Add Carry Flag and L to A
					A += (byte)(L + CFlag);
					PC += 1;
					break;

				case 0x8E: //Add Carry Flag and [HL] to A
					A += (byte)(ram[DualRegister(H, L)] + CFlag);
					PC += 1;
					break;

				case 0x8F: //Add Carry Flag and A to A
					A += (byte)(A + CFlag);
					PC += 1;
					break;

				case 0x90: //Sub B from A
					A -= B;
					PC += 1;
					break;

				case 0x91: //Sub C from A
					A -= C;
					PC += 1;
					break;

				case 0x92: //Sub D from A
					A -= D;
					PC += 1;
					break;

				case 0x93: //Sub E from A
					A -= E;
					PC += 1;
					break;

				case 0x94: //Sub H from A
					A -= H;
					PC += 1;
					break;

				case 0x95: //Sub L from A
					A -= L;
					PC += 1;
					break;

				case 0x96: //Sub [HL] from A
					A -= ram[DualRegister(H, L)];
					PC += 1;
					break;

				case 0x97: //Sub A from A
					A -= A;
					PC += 1;
					break;

				case 0x98: //Sub B and Carry Flag from A
					A -= (byte)(B + CFlag);
					PC += 1;
					break;

				case 0x99: //Sub C and Carry Flag from A
					A -= (byte)(C + CFlag);
					PC += 1;
					break;

				case 0x9A: //Sub D and Carry Flag from A
					A -= (byte)(C + CFlag);
					PC += 1;
					break;

				case 0x9B: //Sub E and Carry Flag from A
					A -= (byte)(E + CFlag);
					PC += 1;
					break;

				case 0x9C: //Sub H and Carry Flag from A
					A -= (byte)(E + CFlag);
					PC += 1;
					break;

				case 0x9D: //Sub L and Carry Flag from A
					A -= (byte)(L + CFlag);
					PC += 1;
					break;

				case 0x9E: //Sub [HL] and Carry Flag from A
					A -= (byte)(ram[DualRegister(H, L)] + CFlag);
					PC += 1;
					break;

				case 0x9F: //Sub A and Carry Flag from A
					A -= (byte)(A + CFlag);
					PC += 1;
					break;

				case 0xA0: //AND A with B
					A &= B;
					AndFlags();
					PC += 1;
					break;

				case 0xA1: //And A with C
					A &= C;
					AndFlags();
					PC += 1;
					break;

				case 0xA2: //And A with D
					A &= D;
					AndFlags();
					PC += 1;
					break;

				case 0xA3: //And A with E
					A &= E;
					AndFlags();
					PC += 1;
					break;

				case 0xA4: //And A with H
					A &= H;
					AndFlags();
					PC += 1;
					break;

				case 0xA5: //And A with L
					A &= L;
					AndFlags();
					PC += 1;
					break;

				case 0xA6: //And A with [HL]
					A &= ram[DualRegister(H, L)];
					AndFlags();
					PC += 1;
					break;

				case 0xA7: //And A with A
					A &= A;
					AndFlags();
					PC += 1;
					break;

				case 0xA8: //Xor A with B
					A ^= B;
					XorFlags();
					PC += 1;
					break;

				case 0xA9: //Xor A with C
					A ^= C;
					XorFlags();
					PC += 1;
					break;

				case 0xAA: //Xor A with D
					A ^= D;
					XorFlags();
					PC += 1;
					break;

				case 0xAB: //Xor A with E
					A ^= E;
					XorFlags();
					PC += 1;
					break;

				case 0xAC: //Xor A with H
					A ^= H;
					XorFlags();
					PC += 1;
					break;

				case 0xAD: //Xor A with L
					A ^= L;
					XorFlags();
					PC += 1;
					break;

				case 0xAE: //Xor A with [HL]
					A ^= ram[DualRegister(H, L)];
					XorFlags();
					PC += 1;
					break;

				case 0xAF: //Xor A with A
					A ^= A;
					XorFlags();
					PC += 1;
					break;
            }
		}

		public void AndFlags()
		{
			F |= 0x10; //Set HFlag

			F = (byte)(F & ~0x01); //Reset CFlag
			F = (byte)(F & ~0x02); //Reset NFlag
		}

		public void XorFlags()
		{
			F = (byte)(F & ~0x10); //Reset HFlag
			F = (byte)(F & ~0x01); //Reset CFlag
			F = (byte)(F & ~0x02); //Reset NFlag
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
