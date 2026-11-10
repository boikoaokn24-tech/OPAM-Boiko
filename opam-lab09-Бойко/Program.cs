using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Lab09
{
    // Модель даних для перевірки стабільності
    public class Student
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public int Score { get; set; }

        public Student(string name, string group, int score)
        {
            Name = name;
            Group = group;
            Score = score;
        }

        public override string ToString() => $"{Name} ({Group}): {Score}";
    }

    public static class Program
    {
        // Константи варіанта (N = 7, K = 19 - Перша літера імені 'О')
        public const int N = 7;
        public const int K = 19;
        public const int A = N % 3; // 1 -> Insertion Sort
        public const int B = N % 4; // 3 -> QuickSort (Median of Three)
        public const int C = N % 5; // 2 -> Hybrid Sort

        public static void Main(string[] args)
        {
            Console.WriteLine($"=== N = {N}, K = {K}, a = {A}, b = {B}, c = {C} ===");
            Console.WriteLine();

            // 1. Трасування масиву для Завдання 1
            RunTracingTask1();

            // 2. Завдання 4: Таблиця порівнянь і переставлянь
            RunTask4Table();

            // 3. Завдання 3: Порівняння порогів гібридного сортування
            RunTask3HybridThresholds();

            // 4. Завдання 5: Заміри часу проти Array.Sort
            RunTask5TimeBenchmarks();

            // 5. Завдання 5: Експеримент на стабільність
            RunTask5StabilityCheck();

            // 6. Крайові випадки (Edge Cases)
            RunEdgeCases();
        }

        #region --- 1. Алгоритми сортування (Generic, з лічильниками) ---

        // Завдання 1: Сортування вставками
        public static void InsertionSort<T>(T[] a, Func<T, T, int> cmp, out int comparisons, out int moves)
        {
            comparisons = 0;
            moves = 0;

            for (int i = 1; i < a.Length; i++)
            {
                T key = a[i];
                int j = i - 1;

                while (j >= 0)
                {
                    comparisons++;
                    if (cmp(a[j], key) > 0)
                    {
                        a[j + 1] = a[j];
                        moves++;
                        j--;
                    }
                    else
                    {
                        break;
                    }
                }
                a[j + 1] = key;
                moves++;
            }
        }

        // Завдання 2: QuickSort з медіаною трьох
        public static void QuickSortMedianOfThree<T>(T[] a, Func<T, T, int> cmp, out int comparisons, out int moves)
        {
            comparisons = 0;
            moves = 0;
            QuickSortRec(a, 0, a.Length - 1, cmp, ref comparisons, ref moves);
        }

        private static void QuickSortRec<T>(T[] a, int low, int high, Func<T, T, int> cmp, ref int comparisons, ref int moves)
        {
            if (low < high)
            {
                if (high - low == 1)
                {
                    comparisons++;
                    if (cmp(a[low], a[high]) > 0)
                    {
                        Swap(a, low, high, ref moves);
                    }
                    return;
                }

                int pivotIndex = PartitionMedianOfThree(a, low, high, cmp, ref comparisons, ref moves);
                QuickSortRec(a, low, pivotIndex - 1, cmp, ref comparisons, ref moves);
                QuickSortRec(a, pivotIndex + 1, high, cmp, ref comparisons, ref moves);
            }
        }

        private static int PartitionMedianOfThree<T>(T[] a, int low, int high, Func<T, T, int> cmp, ref int comparisons, ref int moves)
        {
            int mid = low + (high - low) / 2;

            if (Compare(a[low], a[mid], cmp, ref comparisons) > 0) Swap(a, low, mid, ref moves);
            if (Compare(a[low], a[high], cmp, ref comparisons) > 0) Swap(a, low, high, ref moves);
            if (Compare(a[mid], a[high], cmp, ref comparisons) > 0) Swap(a, mid, high, ref moves);

            Swap(a, mid, high - 1, ref moves);
            T pivot = a[high - 1];

            int i = low;
            int j = high - 1;

            while (true)
            {
                while (Compare(a[++i], pivot, cmp, ref comparisons) < 0) { }
                while (Compare(a[--j], pivot, cmp, ref comparisons) > 0) { }

                if (i >= j) break;
                Swap(a, i, j, ref moves);
            }

            Swap(a, i, high - 1, ref moves);
            return i;
        }

        // Завдання 3: Гібридне сортування (QuickSort + InsertionSort за порогом)
        public static void HybridSort<T>(T[] a, int threshold, Func<T, T, int> cmp, out int comparisons, out int moves)
        {
            comparisons = 0;
            moves = 0;
            HybridSortRec(a, 0, a.Length - 1, threshold, cmp, ref comparisons, ref moves);
        }

        private static void HybridSortRec<T>(T[] a, int low, int high, int threshold, Func<T, T, int> cmp, ref int comparisons, ref int moves)
        {
            if (high - low + 1 <= threshold)
            {
                InsertionSortSubarray(a, low, high, cmp, ref comparisons, ref moves);
            }
            else if (low < high)
            {
                int pivotIndex = PartitionMedianOfThree(a, low, high, cmp, ref comparisons, ref moves);
                HybridSortRec(a, low, pivotIndex - 1, threshold, cmp, ref comparisons, ref moves);
                HybridSortRec(a, pivotIndex + 1, high, threshold, cmp, ref comparisons, ref moves);
            }
        }

        private static void InsertionSortSubarray<T>(T[] a, int low, int high, Func<T, T, int> cmp, ref int comparisons, ref int moves)
        {
            for (int i = low + 1; i <= high; i++)
            {
                T key = a[i];
                int j = i - 1;
                while (j >= low)
                {
                    comparisons++;
                    if (cmp(a[j], key) > 0)
                    {
                        a[j + 1] = a[j];
                        moves++;
                        j--;
                    }
                    else break;
                }
                a[j + 1] = key;
                moves++;
            }
        }

        private static int Compare<T>(T x, T y, Func<T, T, int> cmp, ref int comparisons)
        {
            comparisons++;
            return cmp(x, y);
        }

        private static void Swap<T>(T[] a, int i, int j, ref int moves)
        {
            if (i == j) return;
            T temp = a[i];
            a[i] = a[j];
            a[j] = temp;
            moves += 2;
        }

        #endregion

        #region --- 2. Генерація даних та перевірки ---

        public static int[] GenerateArray(int size)
        {
            Random rnd = new Random(N);
            int[] arr = new int[size];
            int maxVal = 20 * K;
            for (int i = 0; i < size; i++)
            {
                arr[i] = rnd.Next(0, maxVal + 1);
            }
            return arr;
        }

        public static int[] GetNearlySorted(int[] sortedArr)
        {
            int[] arr = (int[])sortedArr.Clone();
            Random rnd = new Random(N);
            for (int i = 0; i < K; i++)
            {
                int idx1 = rnd.Next(0, arr.Length);
                int idx2 = rnd.Next(0, arr.Length);
                int temp = arr[idx1];
                arr[idx1] = arr[idx2];
                arr[idx2] = temp;
            }
            return arr;
        }

        public static bool IsSorted<T>(T[] array, Func<T, T, int> cmp)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                if (cmp(array[i], array[i + 1]) > 0)
                    return false;
            }
            return true;
        }

        // Обмеження where T : notnull виправляє CS8714
        public static bool IsPermutationOf<T>(T[] original, T[] sorted) where T : notnull
        {
            if (original.Length != sorted.Length) return false;
            var dict = new Dictionary<T, int>();
            foreach (var item in original)
            {
                if (dict.ContainsKey(item)) dict[item]++;
                else dict[item] = 1;
            }
            foreach (var item in sorted)
            {
                if (!dict.ContainsKey(item) || dict[item] == 0) return false;
                dict[item]--;
            }
            return true;
        }

        #endregion

        #region --- 3. Тестові прогони та таблиці ---

        private static void RunTracingTask1()
        {
            Console.WriteLine("--- Завдання 1: Трасування масиву ---");
            int len = (K % 5) + 6;
            int[] baseArr = GenerateArray(len);
            Console.WriteLine($"Початковий масив ({len} ел.): {string.Join(", ", baseArr)}");

            int comp, moves;
            int[] arrCopy = (int[])baseArr.Clone();
            InsertionSort(arrCopy, (x, y) => x.CompareTo(y), out comp, out moves);
            Console.WriteLine($"Відсортований: {string.Join(", ", arrCopy)}");
            Console.WriteLine($"Порівнянь: {comp}, Переставлянь: {moves}");
            Console.WriteLine();
        }

        private static void RunTask4Table()
        {
            Console.WriteLine("--- Завдання 4: Порівняння / Переставляння ---");
            int[] sizes = { 100, 1000, 10000 };
            Func<int, int, int> cmp = (x, y) => x.CompareTo(y);

            foreach (int size in sizes)
            {
                Console.WriteLine($"\n[ Розмір N = {size} ]");
                var randomArr = GenerateArray(size);
                var sortedArr = (int[])randomArr.Clone();
                Array.Sort(sortedArr);
                var reversedArr = sortedArr.Reverse().ToArray();
                var nearlyArr = GetNearlySorted(sortedArr);

                var inputs = new Dictionary<string, int[]>()
                {
                    { "Випадковий", randomArr },
                    { "Відсортований", sortedArr },
                    { "Навпаки", reversedArr },
                    { "Майже відсорт.", nearlyArr }
                };

                Console.WriteLine("{0,-15} | {1,-18} | {2,-18} | {3,-18}", "Алгоритм", "Вхід", "Порівняння", "Переставляння");
                Console.WriteLine(new string('-', 75));

                foreach (var input in inputs)
                {
                    int[] a1 = (int[])input.Value.Clone();
                    InsertionSort(a1, cmp, out int c1, out int m1);
                    Console.WriteLine("{0,-15} | {1,-18} | {2,-18} | {3,-18}", "Insertion", input.Key, c1, m1);

                    int[] a2 = (int[])input.Value.Clone();
                    QuickSortMedianOfThree(a2, cmp, out int c2, out int m2);
                    Console.WriteLine("{0,-15} | {1,-18} | {2,-18} | {3,-18}", "Quick (Med3)", input.Key, c2, m2);

                    int[] a3 = (int[])input.Value.Clone();
                    HybridSort(a3, 16, cmp, out int c3, out int m3);
                    Console.WriteLine("{0,-15} | {1,-18} | {2,-18} | {3,-18}", "Hybrid (T=16)", input.Key, c3, m3);

                    Console.WriteLine(new string('-', 75));
                }
            }
            Console.WriteLine();
        }

        private static void RunTask3HybridThresholds()
        {
            Console.WriteLine("--- Завдання 3: Визначення кращого порогу Гібрида ---");
            int[] thresholds = { 1, 4, 16, 2 * K + 3 };
            int size = 100000;
            var data = GenerateArray(size);
            Func<int, int, int> cmp = (x, y) => x.CompareTo(y);

            foreach (int t in thresholds)
            {
                long bestTime = long.MaxValue;
                for (int run = 0; run < 3; run++)
                {
                    int[] copy = (int[])data.Clone();
                    var sw = Stopwatch.StartNew();
                    HybridSort(copy, t, cmp, out _, out _);
                    sw.Stop();
                    if (sw.ElapsedMilliseconds < bestTime) bestTime = sw.ElapsedMilliseconds;
                }
                Console.WriteLine($"Поріг {t,2}: {bestTime} ms");
            }
            Console.WriteLine();
        }

        private static void RunTask5TimeBenchmarks()
        {
            Console.WriteLine("--- Завдання 5: Час виконання (мс) ---");
            int[] sizes = { 1000, 10000, 100000 };
            Func<int, int, int> cmp = (x, y) => x.CompareTo(y);

            Console.WriteLine("{0,-10} | {1,-12} | {2,-15} | {3,-15} | {4,-12}", "N", "Insertion", "Quick (Med3)", "Hybrid (16)", "Array.Sort");
            Console.WriteLine(new string('-', 75));

            foreach (int size in sizes)
            {
                var data = GenerateArray(size);

                long tIns = -1;
                if (size <= 10000)
                {
                    tIns = MeasureBestTime(() => {
                        int[] copy = (int[])data.Clone();
                        InsertionSort(copy, cmp, out _, out _);
                    });
                }

                long tQuick = MeasureBestTime(() => {
                    int[] copy = (int[])data.Clone();
                    QuickSortMedianOfThree(copy, cmp, out _, out _);
                });

                long tHyb = MeasureBestTime(() => {
                    int[] copy = (int[])data.Clone();
                    HybridSort(copy, 16, cmp, out _, out _);
                });

                long tArray = MeasureBestTime(() => {
                    int[] copy = (int[])data.Clone();
                    Array.Sort(copy);
                });

                string sIns = tIns == -1 ? ">10000 ms" : $"{tIns} ms";
                Console.WriteLine("{0,-10} | {1,-12} | {2,-15} | {3,-15} | {4,-12}", size, sIns, $"{tQuick} ms", $"{tHyb} ms", $"{tArray} ms");
            }
            Console.WriteLine();
        }

        private static long MeasureBestTime(Action action)
        {
            action();

            long minMs = long.MaxValue;
            for (int i = 0; i < 3; i++)
            {
                var sw = Stopwatch.StartNew();
                action();
                sw.Stop();
                if (sw.ElapsedMilliseconds < minMs) minMs = sw.ElapsedMilliseconds;
            }
            return minMs;
        }

        private static void RunTask5StabilityCheck()
        {
            Console.WriteLine("--- Завдання 5: Перевірка Стабільності ---");

            Student[] students = new Student[]
            {
                new Student("Андрієнко", "301", 85),
                new Student("Бойко", "301", 95),
                new Student("Василенко", "301", 85),
                new Student("Грищенко", "301", 95),
                new Student("Демченко", "301", 70)
            };

            Func<Student, Student, int> cmpScore = (s1, s2) => s1.Score.CompareTo(s2.Score);

            Student[] s1 = (Student[])students.Clone();
            InsertionSort(s1, cmpScore, out _, out _);
            Console.WriteLine($"Insertion Sort (Очікувано: Стабільне) -> {FormatStudentList(s1)}");

            Student[] s2 = (Student[])students.Clone();
            QuickSortMedianOfThree(s2, cmpScore, out _, out _);
            Console.WriteLine($"QuickSort (Очікувано: Нестабільне)   -> {FormatStudentList(s2)}");

            Student[] s3 = (Student[])students.Clone();
            Array.Sort(s3, (x, y) => x.Score.CompareTo(y.Score));
            Console.WriteLine($"Array.Sort (Очікувано: Нестабільне)  -> {FormatStudentList(s3)}");

            var s4 = students.OrderBy(x => x.Score).ToArray();
            Console.WriteLine($"OrderBy    (Очікувано: Стабільне)    -> {FormatStudentList(s4)}");
            Console.WriteLine();
        }

        private static string FormatStudentList(Student[] list)
        {
            return string.Join(" | ", list.Select(s => $"{s.Name}:{s.Score}"));
        }

        private static void RunEdgeCases()
        {
            Console.WriteLine("--- Перевірка крайових випадків (Edge Cases) ---");
            Func<int, int, int> cmp = (x, y) => x.CompareTo(y);

            var cases = new Dictionary<string, int[]>()
            {
                { "Порожній масив", new int[] { } },
                { "Один елемент", new int[] { 42 } },
                { "Два елементи в порядку", new int[] { 10, 20 } },
                { "Два елементи навпаки", new int[] { 20, 10 } },
                { "Усі елементи однакові", new int[] { 5, 5, 5, 5, 5 } },
                { "Два значення (попівмасиву)", new int[] { 1, 1, 1, 2, 2, 2 } }
            };

            foreach (var item in cases)
            {
                int[] orig = item.Value;
                int[] sorted = (int[])orig.Clone();

                QuickSortMedianOfThree(sorted, cmp, out _, out _);

                bool isS = IsSorted(sorted, cmp);
                bool isP = IsPermutationOf(orig, sorted);

                Console.WriteLine($"[{item.Key,-28}] -> IsSorted: {isS,-5} | IsPermutation: {isP,-5}");
            }
        }

        #endregion
    }
}