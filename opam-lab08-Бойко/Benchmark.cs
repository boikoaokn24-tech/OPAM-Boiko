using System;
using System.Diagnostics;

namespace opam_lab08_Бойко
{
    public static class Benchmark
    {
        public static void RunComparisonsTable(Student[] sortedStudents, Func<Student, Student, int> cmp)
        {
            int[] sizes = { 1000, 10000, 100000 };
            Console.WriteLine("\n--- ЗАВДАННЯ 5 (Частина 1: Таблиця порівнянь) ---");
            Console.WriteLine("n\t| Лінійний (пор.)\t| Бінарний (пор.)\t| Відношення");
            Console.WriteLine(new string('-', 60));

            var rnd = new Random(2);
            foreach (int n in sizes)
            {
                var subArray = new Student[n];
                Array.Copy(sortedStudents, subArray, n);

                long linearTotalComp = 0;
                long binaryTotalComp = 0;

                for (int q = 0; q < 100; q++)
                {
                    // Виправляємо вибір випадкового елемента в межах поточного розміру n
                    Student target = subArray[rnd.Next(n)];
                    
                    Search.LinearSearch(subArray, target, cmp, out int lComp);
                    Search.BinarySearch(subArray, target, cmp, out int bComp);

                    linearTotalComp += lComp;
                    binaryTotalComp += bComp;
                }

                double avgLinear = (double)linearTotalComp / 100;
                double avgBinary = (double)binaryTotalComp / 100;
                double ratio = avgBinary > 0 ? avgLinear / avgBinary : 1.0;

                Console.WriteLine($"{n}\t| {avgLinear:F1}\t\t| {avgBinary:F1}\t\t| {ratio:F2}");
            }
        }

        public static void RunBreakEvenPoint(Student[] rawArray, Func<Student, Student, int> cmp)
        {
            Console.WriteLine("\n--- ЗАВДАННЯ 5 (Частина 2: Точка окупності сортування) ---");
            Console.WriteLine("q\t| Лінійно, мс\t| Сортування + бінарно, мс\t| Хто швидший");
            Console.WriteLine(new string('-', 65));

            int[] queries = { 1, 5, 10, 20, 30, 50, 100 };
            var rnd = new Random(2);

            // Прогрівочний запуск (JIT warm-up) для стабілізації вимірювань
            var warmupArray = new Student[rawArray.Length];
            Array.Copy(rawArray, warmupArray, rawArray.Length);
            Array.Sort(warmupArray, (x, y) => cmp(x, y));
            Search.BinarySearch(warmupArray, warmupArray[0], cmp, out _);

            foreach (int q in queries)
            {
                var targets = new Student[q];
                for (int i = 0; i < q; i++)
                {
                    targets[i] = rawArray[rnd.Next(rawArray.Length)];
                }

                // 1. Вимірювання лінійного пошуку (3 заміри, беремо найменший)
                double minLinearMs = double.MaxValue;
                for (int run = 0; run < 3; run++)
                {
                    var testArray = new Student[rawArray.Length];
                    Array.Copy(rawArray, testArray, rawArray.Length);

                    var sw = Stopwatch.StartNew();
                    for (int i = 0; i < q; i++)
                    {
                        Search.LinearSearch(testArray, targets[i], cmp, out _);
                    }
                    sw.Stop();
                    if (sw.Elapsed.TotalMilliseconds < minLinearMs) 
                        minLinearMs = sw.Elapsed.TotalMilliseconds;
                }

                // 2. Вимірювання (Сортування + бінарний пошук)
                double minSortBinaryMs = double.MaxValue;
                for (int run = 0; run < 3; run++)
                {
                    var testArray = new Student[rawArray.Length];
                    Array.Copy(rawArray, testArray, rawArray.Length);

                    var sw = Stopwatch.StartNew();
                    Array.Sort(testArray, (x, y) => cmp(x, y));
                    for (int i = 0; i < q; i++)
                    {
                        Search.BinarySearch(testArray, targets[i], cmp, out _);
                    }
                    sw.Stop();
                    if (sw.Elapsed.TotalMilliseconds < minSortBinaryMs) 
                        minSortBinaryMs = sw.Elapsed.TotalMilliseconds;
                }

                string winner = minSortBinaryMs < minLinearMs ? "Сортування + бінарно" : "Лінійний";
                Console.WriteLine($"{q}\t| {minLinearMs:F2} мс\t| {minSortBinaryMs:F2} мс\t\t| {winner}");
            }
        }
    }
}