using System;
using System.IO;
using System.Collections.Generic;

namespace Lab03
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("N = 7, K = 1, a = 1, b = 3, c = 2\n");

            // --- Завдання 1: Обхід каталогів ---
            Console.WriteLine("=== Завдання 1: Обхід каталогів ===");
            Console.Write("Введіть шлях до папки: ");
            string path = Console.ReadLine();
            if (Directory.Exists(path))
            {
                var (count, size) = Task1_CountCsFiles(path);
                Console.WriteLine($"Файлів .cs: {count}, Сумарний розмір: {size} байт");
            }
            else
            {
                Console.WriteLine("Помилка: Папка не існує!");
            }

            // --- Завдання 2: Поділ навпіл ---
            Console.WriteLine("\n=== Завдання 2: Поділ масиву навпіл ===");
            int[] arr = GenerateArray(7, 16, -20, 20);
            Console.Write("Масив: ");
            PrintArray(arr);

            int threshold = 1; // K % 10 = 1 % 10 = 1
            int maxDepth = 0;
            int countGreater = Task2_CountGreater(arr, 0, arr.Length - 1, threshold, 0, ref maxDepth);
            Console.WriteLine($"Кількість елементів більших за {threshold}: {countGreater}");
            Console.WriteLine($"Максимальна глибина рекурсії: {maxDepth} (теоретичний log2(16) = 4)");

            // --- Завдання 3: Перебір із поверненням (Сходинки) ---
            Console.WriteLine("\n=== Завдання 3: Способи піднятися на сходинки (M = 6) ===");
            int M = 1 % 8 + 5; // 6 сходинок
            List<int> currentPath = new List<int>();
            int totalWays = 0;
            Task3_Stairs(M, currentPath, ref totalWays);
            Console.WriteLine($"Загальна кількість способів: {totalWays}");

            // --- Завдання 4: Трасування ---
            Console.WriteLine("\n=== Завдання 4: Трасування на 4 елементах ===");
            int[] smallArr = GenerateArray(7, 4, -20, 20);
            Console.Write("Малий масив: "); PrintArray(smallArr);
            int traceMaxDepth = 0;
            Task4_Trace(smallArr, 0, smallArr.Length - 1, 0, ref traceMaxDepth);

            // --- Завдання 5: Ітеративна версія ---
            Console.WriteLine("\n=== Завдання 5: Ітеративна версія завдання 2 ===");
            int iterativeResult = Task5_Iterative(arr, threshold);
            Console.WriteLine($"Результат циклом: {iterativeResult} (Рекурсія дала: {countGreater})");
        }

        // Допоміжні методи для генерації
        static int[] GenerateArray(int seed, int len, int min, int max)
        {
            Random rnd = new Random(seed);
            int[] res = new int[len];
            for (int i = 0; i < len; i++) res[i] = rnd.Next(min, max + 1);
            return res;
        }

        static void PrintArray(int[] arr) => Console.WriteLine(string.Join(", ", arr));

        // Завдання 1
        static (int count, long size) Task1_CountCsFiles(string dirPath)
        {
            int count = 0;
            long size = 0;
            try
            {
                // Базовий випадок: якщо немає доступних файлів або підпапок, цикл не виконається, повернемо 0, 0.
                foreach (string file in Directory.GetFiles(dirPath))
                {
                    if (Path.GetExtension(file).Equals(".cs", StringComparison.OrdinalIgnoreCase))
                    {
                        count++;
                        size += new FileInfo(file).Length;
                    }
                }

                foreach (string subDir in Directory.GetDirectories(dirPath))
                {
                    var (subCount, subSize) = Task1_CountCsFiles(subDir);
                    count += subCount;
                    size += subSize;
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Пропускаємо системні/закриті папки
            }
            return (count, size);
        }

        // Завдання 2
        static int Task2_CountGreater(int[] a, int lo, int hi, int threshold, int currentDepth, ref int maxDepth)
        {
            if (currentDepth > maxDepth) maxDepth = currentDepth;

            // Базовий випадок: залишився один елемент
            if (lo == hi)
            {
                return a[lo] > threshold ? 1 : 0;
            }

            int mid = (lo + hi) / 2;
            int leftCount = Task2_CountGreater(a, lo, mid, threshold, currentDepth + 1, ref maxDepth);
            int rightCount = Task2_CountGreater(a, mid + 1, hi, threshold, currentDepth + 1, ref maxDepth);

            return leftCount + rightCount;
        }

        // Завдання 3
        static void Task3_Stairs(int target, List<int> path, ref int waysCount)
        {
            int currentSum = 0;
            foreach (var step in path) currentSum += step;

            // Базовий випадок: дійшли точно до цілі
            if (currentSum == target)
            {
                Console.WriteLine(string.Join(" + ", path));
                waysCount++;
                return;
            }

            // Якщо перейшли ціль — зупиняємо гілку
            if (currentSum > target) return;

            // Пробуємо крок 1, потім крок 2
            for (int step = 1; step <= 2; step++)
            {
                path.Add(step);                    // Зробити вибір
                Task3_Stairs(target, path, ref waysCount); // Піти вглиб
                path.RemoveAt(path.Count - 1);     // Скасувати вибір (повернення)
            }
        }

        // Завдання 4
        static int Task4_Trace(int[] a, int lo, int hi, int depth, ref int maxDepth)
        {
            string indent = new string(' ', depth * 2);
            Console.WriteLine($"{indent}enter Count({lo}..{hi})");

            if (lo == hi)
            {
                int res = a[lo] > 1 ? 1 : 0;
                Console.WriteLine($"{indent}  base({lo}) -> {res}");
                Console.WriteLine($"{indent}exit  Count({lo}..{hi}) -> {res}");
                return res;
            }

            int mid = (lo + hi) / 2;
            int left = Task4_Trace(a, lo, mid, depth + 1, ref maxDepth);
            int right = Task4_Trace(a, mid + 1, hi, depth + 1, ref maxDepth);
            int total = left + right;

            Console.WriteLine($"{indent}exit  Count({lo}..{hi}) -> {total}");
            return total;
        }

        // Завдання 5
        static int Task5_Iterative(int[] arr, int threshold)
        {
            int count = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] > threshold) count++;
            }
            return count;
        }
    }
}