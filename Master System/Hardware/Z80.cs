using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_System.Hardware
{
	public class Z80
	{
		ushort opcode;

		ushort PC;

		ushort SP;

		ushort IX, IY;

		public byte A, B, C, D, E, F, H, L;
		public byte A1, B1, C1, D1, E1, F1, H1, L1;

		byte I;

		byte R;

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
	}
}
