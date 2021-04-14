using System;

namespace ManagedCLR.JustTesting
{
	public static class Class1
	{
		public static int Main()
		{
			return Test(9, 30);
		}

		public static int Wow()
		{
			return 1;
		}

		public static int Return(int val)
		{
			return val + val;
		}

		public static int Test(int x, int y)
		{
			return x > y ? Return(x) : Return(y);
		}
	}
}
