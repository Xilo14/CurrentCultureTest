using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CurrentCultureTest
{
    class Program
    {
        static ConcurrentDictionary<CultureInfo, int> counts1 = new();
        static ConcurrentDictionary<int, int> threads1 = new();
        static ConcurrentDictionary<CultureInfo, int> counts2 = new();
        static ConcurrentDictionary<int, int> threads2 = new();
        static void Main(string[] args)
        {
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var tasks = cultures.Select(c => Work(c));

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine($"Total amount of cultures: {cultures.Count()}");
            Console.WriteLine($"Total amount pairs in 'counts1': {counts1.Count()}");
            Console.WriteLine($"Total amount pairs in 'threads1': {threads1.Count()}");
            Console.WriteLine($"Total amount pairs in 'counts2': {counts2.Count()}");
            Console.WriteLine($"Total amount pairs in 'threads2': {threads2.Count()}");

            // Total amount of cultures: 813
            // Total amount pairs in 'counts1': 813
            // Total amount pairs in 'threads1': 13
            // Total amount pairs in 'counts2': 813
            // Total amount pairs in 'threads2': 13
            Console.ReadLine();
        }
        async static Task Work(CultureInfo culture)
        {
            CultureInfo.CurrentUICulture = culture;
            await Task.Delay(1000);
            counts1.AddOrUpdate(CultureInfo.CurrentUICulture, 1, (_, ind) => ++ind);
            threads1.AddOrUpdate(Thread.CurrentThread.ManagedThreadId, 1, (_, ind) => ++ind);
            await AfterWork().ConfigureAwait(false);
        }
        async static Task AfterWork()
        {
            await Task.Delay(100);
            counts2.AddOrUpdate(CultureInfo.CurrentUICulture, 1, (_, ind) => ++ind);
            threads2.AddOrUpdate(Thread.CurrentThread.ManagedThreadId, 1, (_, ind) => ++ind);
        }
    }
}
