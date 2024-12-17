using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace WordAnalyzerHomework
{
    class Program
    {
        static int total = 0;
        static string tiny = "";
        static string giant = "";
        static double average = 0;
        static List<string> mostRepeated = new List<string>();
        static List<string> leastRepeated = new List<string>();
        static string[] arrayOfWords;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            ReadFile();
            NoThreadsVersion();
            Console.WriteLine();
            ThreadsVersion();
        }

        static void ReadFile()
        {
            char[] delims = { ' ', ',', '.', '?', '!', '_',':', '–', '…', '\n' };
            string filepath = @"../../../Lynn-Flewelling_-_Skritijat_voin_-_11895-b.txt";
            string book = File.ReadAllText(filepath);
            arrayOfWords = book.Split(delims, StringSplitOptions.RemoveEmptyEntries);
        }

        static void NoThreadsVersion()
        {
            Console.WriteLine("БЕЗ НИШКА:");
            var clock = System.Diagnostics.Stopwatch.StartNew();
            total = CountWords(arrayOfWords);
            tiny = MinWord(arrayOfWords);
            giant = MaxWord(arrayOfWords);
            average = AvgLength(arrayOfWords);
            mostRepeated = TopWords(arrayOfWords, 5);
            leastRepeated = RareWords(arrayOfWords, 5);
            clock.Stop();
            DisplayResults(clock.ElapsedMilliseconds);
        }

        static void ThreadsVersion()
        {
            Console.WriteLine("С НИШКА:");
            var timer = System.Diagnostics.Stopwatch.StartNew();
            Thread t1 = new Thread(() => total = CountWords(arrayOfWords));
            Thread t2 = new Thread(() => tiny = MinWord(arrayOfWords));
            Thread t3 = new Thread(() => giant = MaxWord(arrayOfWords));
            Thread t4 = new Thread(() => average = AvgLength(arrayOfWords));
            Thread t5 = new Thread(() => mostRepeated = TopWords(arrayOfWords, 5));
            Thread t6 = new Thread(() => leastRepeated = RareWords(arrayOfWords, 5));

            t1.Start(); t2.Start(); t3.Start();
            t4.Start(); t5.Start(); t6.Start();

            t1.Join(); t2.Join(); t3.Join();
            t4.Join(); t5.Join(); t6.Join();

            timer.Stop();
            DisplayResults(timer.ElapsedMilliseconds);
        }

        static void DisplayResults(long time)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Брой думи: {total}");
            sb.AppendLine($"Най малка дума: {tiny}");
            sb.AppendLine($"Най голяма дума: {giant}");
            sb.AppendLine($"Средна дължина на дума: {average:F2}");
            sb.Append("Най повтаряни: ");
            sb.AppendLine(string.Join(", ", mostRepeated));
            sb.Append("Най редки: ");
            sb.AppendLine(string.Join(", ", leastRepeated));
            sb.AppendLine($"Време: {time} ms");

            Console.WriteLine(sb.ToString());
        }

        static int CountWords(string[] words)
        {
            int c = 0;
            foreach (string w in words)
            {
                string x = w.Trim();
                if (x.Length >= 3) c++;
            }
            return c;
        }

        static string MinWord(string[] words)
        {
            string min = words[0];
            for (int i = 0; i < words.Length; i++)
            {
                string w = words[i].Trim();
                if (w.Length >= 3 && w.Length < min.Length) min = w;
            }
            return min;
        }

        static string MaxWord(string[] words)
        {
            string max = words[0];
            foreach (var w in words)
            {
                string x = w.Trim();
                if (x.Length >= 3 && x.Length > max.Length) max = x;
            }
            return max;
        }

        static double AvgLength(string[] words)
        {
            int t = 0, n = 0;
            for (int i = 0; i < words.Length; i++)
            {
                string w = words[i].Trim();
                if (w.Length >= 3)
                {
                    t += w.Length;
                    n++;
                }
            }
            return n > 0 ? (double)t / n : 0;
        }

        static List<string> TopWords(string[] words, int k)
        {
            Dictionary<string, int> map = new Dictionary<string, int>();
            foreach (var w in words)
            {
                string x = w.Trim();
                if (x.Length >= 3)
                {
                    if (map.ContainsKey(x)) map[x]++;
                    else map[x] = 1;
                }
            }

            List<string> result = new List<string>();
            for (int i = 0; i < k; i++)
            {
                int maxVal = 0; string maxKey = "";
                foreach (var kv in map)
                {
                    if (kv.Value > maxVal)
                    {
                        maxVal = kv.Value;
                        maxKey = kv.Key;
                    }
                }
                if (maxKey != "")
                {
                    result.Add(maxKey);
                    map.Remove(maxKey);
                }
            }
            return result;
        }

        static List<string> RareWords(string[] words, int k)
        {
            Dictionary<string, int> m = new Dictionary<string, int>();
            foreach (var w in words)
            {
                string x = w.Trim();
                if (x.Length >= 3)
                {
                    if (m.ContainsKey(x)) m[x]++;
                    else m[x] = 1;
                }
            }

            List<string> result = new List<string>();
            for (int i = 0; i < k; i++)
            {
                string minKey = "";
                int minVal = int.MaxValue;
                foreach (var kv in m)
                {
                    if (kv.Value < minVal)
                    {
                        minVal = kv.Value;
                        minKey = kv.Key;
                    }
                }
                if (!string.IsNullOrEmpty(minKey))
                {
                    result.Add(minKey);
                    m.Remove(minKey);
                }
            }
            return result;
        }
    }
}
