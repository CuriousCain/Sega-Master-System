using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_System.Hardware
{
	public class Z80
	{
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

		public Z80(int ramSize = 8192)
		{
			ram = new byte[ramSize];
		}

		public void Cycle()
		{
			Fetch();
			DecodeAndExecute();
		}

		public void Fetch()
		{
			opcode = ram[PC];
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
			}
		}
    }
}
