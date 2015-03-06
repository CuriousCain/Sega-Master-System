using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Master_System.Hardware;

namespace Master_System_Tests
{
	[TestClass]
	public class Z80Test
	{
		private Z80 z80;

		[TestInitialize]
		public void TestInitialize()
		{
			z80 = new Z80();

			z80.A = 0x01;
			z80.B = 0x02;
			z80.C = 0x03;
			z80.D = 0x04;
			z80.E = 0x05;
			z80.F = 0x06;
			z80.H = 0x07;
			z80.L = 0x08;
		}

		[TestMethod]
		public void TestDualRegister()
		{
			var upper = (z80.DualRegister(z80.A, z80.F) & 0xFF00) >> 8;
			var lower = (z80.DualRegister(z80.A, z80.F) & 0x00FF);

			Assert.AreEqual(0x01, upper);
			Assert.AreEqual(0x06, lower);
		}

		[TestMethod]
		public void TestSetAF()
		{
			z80.SetAF(0x45F8);

			Assert.AreEqual(0x45, z80.A);
			Assert.AreEqual(0xF8, z80.F);
		}

		[TestMethod]
		public void TestSetBC()
		{
			z80.SetBC(0x45F8);

			Assert.AreEqual(0x45, z80.B);
			Assert.AreEqual(0xF8, z80.C);
		}

		[TestMethod]
		public void TestSetDE()
		{
			z80.SetDE(0x45F8);

			Assert.AreEqual(0x45, z80.D);
			Assert.AreEqual(0xF8, z80.E);
		}

		[TestMethod]
		public void TestSetHL()
		{
			z80.SetHL(0x45F8);

			Assert.AreEqual(0x45, z80.H);
			Assert.AreEqual(0xF8, z80.L);
		}

		[TestMethod]
		public void TestSetAF1()
		{
			z80.SetAF1(0x45F8);

			Assert.AreEqual(0x45, z80.A1);
			Assert.AreEqual(0xF8, z80.F1);
		}

		[TestMethod]
		public void TestSetBC1()
		{
			z80.SetBC1(0x45F8);

			Assert.AreEqual(0x45, z80.B1);
			Assert.AreEqual(0xF8, z80.C1);
		}

		[TestMethod]
		public void TestSetDE1()
		{
			z80.SetDE1(0x45F8);

			Assert.AreEqual(0x45, z80.D1);
			Assert.AreEqual(0xF8, z80.E1);
		}

		[TestMethod]
		public void TestSetHL1()
		{
			z80.SetHL1(0x45F8);

			Assert.AreEqual(0x45, z80.H1);
			Assert.AreEqual(0xF8, z80.L1);
		}

		[TestMethod]
		public void TestExecute()
		{
			Action a = () => { z80.A = 0x0F; };
			z80.Execute(a);

			Assert.AreEqual(0x0F, z80.A);
		}
	}
}
