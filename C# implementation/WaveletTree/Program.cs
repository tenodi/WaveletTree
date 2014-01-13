using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
namespace Wavelet
{
	class Program
	{
		static void Main(string[] args)
		{             


			try
			{
				Console.Write("\nMemory used on start od program: " + GC.GetTotalMemory(false) + "bytes\n");
				Console.WriteLine("\n\n" + args[0]);
				Console.WriteLine("-------------");
				Stopwatch stopWatch = new Stopwatch();

				stopWatch.Start();
				new WaveletTree(args);
				stopWatch.Stop();
				Console.WriteLine("Total time elapsed: " + stopWatch.Elapsed.TotalMilliseconds +" ms\n");
				Console.Write("Memory used: " + GC.GetTotalMemory(false) + "bytes\n");
			}
			catch (Exception e)
			{
				Console.Write(e.Message);
			}


		}
	}
}
