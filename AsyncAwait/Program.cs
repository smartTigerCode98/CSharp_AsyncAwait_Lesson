using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Press any key to cancel writing integers...");

			var cts = new CancellationTokenSource();

			Task.Run(() =>
			{
				Console.ReadKey(false);
				cts.Cancel();
			});

			await Task.Run(() => { WriteALotOfIntegers(cts.Token); })
					  .ContinueWith(previousTask =>
					   {
						   if (previousTask.IsCompletedSuccessfully)
						   {
							   Console.WriteLine("Writing done OK!");
						   }
						   else
						   {
							   Console.WriteLine("User interrupted writing");
						   }
					   })
					  .ContinueWith(previousTask =>
					   {
						   Console.WriteLine("Now we can close our application");
					   })
					  .ContinueWith(previousTask =>
					   {
						   Console.WriteLine("Good bye!");
					   });
		}

		private static void WriteALotOfIntegers(CancellationToken cancellationToken)
		{
			var integers = Enumerable.Range(0, 10_000_000_0);

			using (var writer = new StreamWriter("output.data"))
			{
				try
				{
					foreach (int integer in integers)
					{
						cancellationToken.ThrowIfCancellationRequested();
						writer.WriteLine(integer);
					}
				}
				finally
				{
					Console.WriteLine("Finally block");
					writer.Flush();
					writer.Close();
				}
			}
		}
	}
}