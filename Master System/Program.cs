using Master_System.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_System
{
	class Program
	{
		static void Main(string[] args)
		{
			var z = new Z80();
			z.LoadApplication("zexall.sms");
			while(true)
			{
				z.Cycle();
			}
		}
	}
}
