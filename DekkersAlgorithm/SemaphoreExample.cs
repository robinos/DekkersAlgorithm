﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DekkersAlgorithm
{
	public class SemaphoreExample
	{
		private static bool process1WantsResource = false;
		private static bool process2WantsResource = false;

		//Semaphore lås
		private static Semaphore locker = new Semaphore(1, 1);
		
		private static int turn = 1;
		private static int process1Counter = 2;
		private static int process2Counter = 2;

		public SemaphoreExample()
		{
		}

		/// <summary>
		/// Main skapar och startar både trådar.
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			SemaphoreExample program = new SemaphoreExample();

			Thread thread1 = new Thread(new ParameterizedThreadStart(Process1));
			Thread thread2 = new Thread(new ParameterizedThreadStart(Process2));

			thread1.Start("Process 1");
			thread2.Start("Process 2");

			Console.ReadKey();
		}

		/// <summary>
		/// Första processen bara använder resursen under första rundan.
		/// </summary>
		/// <param name="num">Tråd namnet</param>
		public static void Process1(object name)
		{
			while (process1Counter > 0)
			{
				Console.WriteLine("{0}: Gör icke kritiska arbete", name);
				Thread.Sleep(1000);

				process1WantsResource = true;
				Console.WriteLine("{0}: Vill ha resursen", name);

				while (process2WantsResource)
				{
					if(turn == 2)
					{
						process1WantsResource = false;
						Console.WriteLine("{0}: Vill inte ha resursen ändå (process 2s tur)", name);
						Thread.Sleep(1000);
					}
					else
					{
						process1WantsResource = true;
						Console.WriteLine("{0}: Vill ha resursen!", name);
					}
				}

				//Vänta på resursen, fortsätter när man få det
				locker.WaitOne();
				
				Console.WriteLine("{0}: Börjar kritiska arbete", name);
				turn = 2;
				Console.WriteLine("{0}: Sätter runda till 2", name);

				//Släpp det när klar
				locker.Release();
				
				process1WantsResource = false;
				Console.WriteLine("{0}: Vill inte ha resursen längre", name);
				process1Counter--;
			}
		}

		/// <summary>
		/// Andra processen är likt den första men bara använda resursen på andra
		/// rundan.
		/// </summary>
		/// <param name="name">Tråd namnet</param>
		public static void Process2(object name)
		{
			while (process2Counter > 0)
			{
				Console.WriteLine("{0}: Gör icke kritiska arbete", name);
				Thread.Sleep(1000);

				process2WantsResource = true;
				Console.WriteLine("{0}: Vill ha resursen", name);

				while (process1WantsResource)
				{
					if (turn == 1)
					{
						process2WantsResource = false;
						Console.WriteLine("{0}: Vill inte ha resursen ändå (process 1s tur)", name);
						Thread.Sleep(1000);
					}
					else
					{
						process2WantsResource = true;
						Console.WriteLine("{0}: Vill ha resursen!", name);
					}
				}

				//Vänta på resursen, fortsätter när man få det
				locker.WaitOne();

				Console.WriteLine("{0}: Börjar kritiska arbete", name);
				turn = 1;
				Console.WriteLine("{0}: Sätter rundan till 1", name);
				
				//Släpp det när klar
				locker.Release();
				
				process2WantsResource = false;
				Console.WriteLine("{0}: Vill inte ha resursen längre", name);
				process2Counter--;
			}
		}
	}
}
