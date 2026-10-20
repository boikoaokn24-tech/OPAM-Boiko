using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace lab04
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            int n = 7; // Номер у списку

            Console.WriteLine("=== ЛАБОРАТОРНА РОБОТА №4 ===");
            Console.WriteLine($"N = {n}, K = 1, a = 1, b = 3, c = 2\n");

            // --- ОСНОВНИЙ СПИСОК (20 випадкових чисел у діапазоні [-9; 9]) ---
            List<int> mainList = GenerateMainList(n, 20);
            Console.WriteLine("Основний список:");
            PrintList(mainList);
            Console.WriteLine();

            // ================= ЗАВДАННЯ 1 =================
            // a = 1: залишаємо елементи, кратні (K % 4 + 2) = 3
            Console.WriteLine("--- ЗАВДАННЯ 1: Побудова за критерієм (кратні 3) ---");
            List<int> filteredList = FilterByCriteria(mainList);
            Console.WriteLine("Результат побудови:");
            PrintList(filteredList);
            Console.WriteLine($"Count: {filteredList.Count}");
            Console.WriteLine($"Вхідний список залишився незмінним (довжина: {mainList.Count})\n");

            // ================= ЗАВДАННЯ 2 =================
            // c = 2: вилучаємо від'ємні елементи
            Console.WriteLine("--- ЗАВДАННЯ 2: Коректне вилучення (від'ємні числа) ---");
            DemonstrateRemoval(mainList);
            Console.WriteLine();

            // ================= ЗАВДАННЯ 3 =================
            Console.WriteLine("--- ЗАВДАННЯ 3: Злиття двох упорядкованих списків ---");
            // Довжина 1: 10 + 7 % 5 = 12, Gen(7)
            // Довжина 2: 6 + 7 % 4 = 9, Gen(107)
            List<int> list1 = GenerateSortedList(12, 7, 1, 10);
            List<int> list2 = GenerateSortedList(9, 107, 1, 10);

            Console.WriteLine("Список 1:");
            PrintList(list1);
            Console.WriteLine("Список 2:");
            PrintList(list2);

            List<int> mergedList = MergeSortedLists(list1, list2);
            Console.WriteLine("Злийований список:");
            PrintList(mergedList);

            bool isSorted = CheckIsSorted(mergedList);
            Console.WriteLine($"Перевірка впорядкованості (очікувано True): {isSorted}\n");

            // ================= ЗАВДАННЯ 4 =================
            // b = 3: розділення на парні та непарні індекси
            Console.WriteLine("--- ЗАВДАННЯ 4: Розділення списку за індексами ---");
            var (evenIndexList, oddIndexList) = SplitByIndex(mainList);
            Console.WriteLine("Елементи на парних індексах:");
            PrintList(evenIndexList);
            Console.WriteLine("Елементи на непарних індексах:");
            PrintList(oddIndexList);

            bool countsMatch = (evenIndexList.Count + oddIndexList.Count == mainList.Count);
            Console.WriteLine($"Перевірка суми Count (очікувано True): {countsMatch}\n");

            // ================= КРАЙОВІ ВИПАДКИ =================
            Console.WriteLine("--- КРАЙОВІ ВИПАДКИ (Тестування) ---");
            RunEdgeCases();

            // ================= ЗАВДАННЯ 5 =================
            Console.WriteLine("\n--- ЗАВДАННЯ 5: Замір часу виконання операцій ---");
            RunPerformanceBenchmarks();
        }

        // --- ДОПОВМІЖНІ МЕТОДИ ---
        static List<int> GenerateMainList(int seed, int count)
        {
            Random rnd = new Random(seed);
            List<int> list = new List<int>();
            for (int i = 0; i < count; i++)
            {
                list.Add(rnd.Next(-9, 10)); // Діапазон [-9; 9]
            }
            return list;
        }

        static void PrintList<T>(List<T> list)
        {
            Console.WriteLine("[" + string.Join(", ", list) + "]");
        }

        // ЗАВДАННЯ 1: a = 1 (кратні 3)
        static List<int> FilterByCriteria(List<int> source)
        {
            List<int> result = new List<int>();
            foreach (int item in source)
            {
                if (item != 0 && item % 3 == 0) // Кратні 3
                {
                    result.Add(item);
                }
            }
            return result;
        }

        // ЗАВДАННЯ 2: Вилучення від'ємних елементів трьома способами
        static void DemonstrateRemoval(List<int> original)
        {
            // 1. Хибний варіант (цикл від початку з RemoveAt)
            List<int> list1 = new List<int>(original);
            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i] < 0)
                {
                    list1.RemoveAt(i); // Помилка: зміщення індексів призводить до пропуску сусідніх елементів
                }
            }
            Console.WriteLine("1. Хибний варіант (з початку):");
            PrintList(list1);
            Console.WriteLine("   -> Пропускає елементи, які стають на місце щойно видалених.");

            // 2. Цикл з кінця
            List<int> list2 = new List<int>(original);
            for (int i = list2.Count - 1; i >= 0; i--)
            {
                if (list2[i] < 0)
                {
                    list2.RemoveAt(i);
                }
            }
            Console.WriteLine("2. Правильний варіант (з кінця):");
            PrintList(list2);

            // 3. Новий список
            List<int> list3 = new List<int>();
            foreach (int item in original)
            {
                if (item >= 0)
                {
                    list3.Add(item);
                }
            }
            Console.WriteLine("3. Правильний варіант (новий список):");
            PrintList(list3);

            bool resultsMatch = ListsAreEqual(list2, list3);
            Console.WriteLine($"Збіг результатів варіантів 2 і 3 (очікувано True): {resultsMatch}");
        }

        static bool ListsAreEqual(List<int> a, List<int> b)
        {
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        // ЗАВДАННЯ 3: Генерація впорядкованого списку
        static List<int> GenerateSortedList(int length, int seed, int minIncr, int maxIncr)
        {
            Random rnd = new Random(seed);
            List<int> list = new List<int>();
            if (length <= 0) return list;

            int current = rnd.Next(1, 10);
            list.Add(current);

            for (int i = 1; i < length; i++)
            {
                current += rnd.Next(minIncr, maxIncr);
                list.Add(current);
            }
            return list;
        }

        // ЗАВДАННЯ 3: Злиття за один прохід двома індексами
        static List<int> MergeSortedLists(List<int> list1, List<int> list2)
        {
            List<int> result = new List<int>(list1.Count + list2.Count);
            int i = 0, j = 0;

            while (i < list1.Count && j < list2.Count)
            {
                if (list1[i] <= list2[j])
                {
                    result.Add(list1[i]);
                    i++;
                }
                else
                {
                    result.Add(list2[j]);
                    j++;
                }
            }

            // Дописуємо хвіст першого списку, якщо він залишився
            while (i < list1.Count)
            {
                result.Add(list1[i]);
                i++;
            }

            // Дописуємо хвіст другого списку, якщо він залишився
            while (j < list2.Count)
            {
                result.Add(list2[j]);
                j++;
            }

            return result;
        }

        static bool CheckIsSorted(List<int> list)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i] > list[i + 1]) return false;
            }
            return true;
        }

        // ЗАВДАННЯ 4: Розділення за індексами (b = 3)
        static (List<int> evenIndex, List<int> oddIndex) SplitByIndex(List<int> source)
        {
            List<int> even = new List<int>();
            List<int> odd = new List<int>();

            for (int i = 0; i < source.Count; i++)
            {
                if (i % 2 == 0) even.Add(source[i]);
                else odd.Add(source[i]);
            }
            return (even, odd);
        }

        // --- КРАЙОВІ ВИПАДКИ ---
        static void RunEdgeCases()
        {
            List<int> emptyList = new List<int>();
            List<int> singleList = new List<int> { 6 };

            // Тест 1: Порожній список
            var r1 = FilterByCriteria(emptyList);
            var (e1, o1) = SplitByIndex(emptyList);
            Console.WriteLine($"Порожній список -> Завд.1 Count: {r1.Count}, Завд.4 Сума Count: {e1.Count + o1.Count}");

            // Тест 2: Список з одного елемента
            var r2 = FilterByCriteria(singleList);
            var (e2, o2) = SplitByIndex(singleList);
            Console.WriteLine($"Список з 1 елемента -> Завд.1 Count: {r2.Count}, Завд.4 Сума Count: {e2.Count + o2.Count}");

            // Тест 3: Злиття, де один список порожній
            var mergedEdge = MergeSortedLists(new List<int> { 1, 3, 5 }, new List<int>());
            Console.WriteLine($"Злиття з порожнім списком (довжина): {mergedEdge.Count} (Очікувано 3)");
        }

        // --- ЗАВДАННЯ 5: Замір часу ---
        static void RunPerformanceBenchmarks()
        {
            int iterations = 100_000;
            Stopwatch sw = new Stopwatch();

            // Прогрівальний прогін (1000 елементів)
            List<int> warmUp = new List<int>();
            for (int i = 0; i < 1000; i++) warmUp.Add(i);

            // 1. Add без місткості
            sw.Start();
            List<int> list1 = new List<int>();
            for (int i = 0; i < iterations; i++) list1.Add(i);
            sw.Stop();
            double timeAddDefault = sw.ElapsedMilliseconds;

            // 2. Add з заданою місткостю
            sw.Restart();
            List<int> list2 = new List<int>(iterations);
            for (int i = 0; i < iterations; i++) list2.Add(i);
            sw.Stop();
            double timeAddCapacity = sw.ElapsedMilliseconds;

            // 3. Insert(0, x)
            sw.Restart();
            List<int> list3 = new List<int>();
            for (int i = 0; i < iterations; i++) list3.Insert(0, i);
            sw.Stop();
            double timeInsert = sw.ElapsedMilliseconds;

            // 4. Contains 1000 разів у списку з 100 000 елементів
            List<int> searchList = new List<int>(iterations);
            for (int i = 0; i < iterations; i++) searchList.Add(i);

            sw.Restart();
            for (int i = 0; i < 1000; i++)
            {
                searchList.Contains(-99999); // Значення, якого немає
            }
            sw.Stop();
            double timeContains = sw.ElapsedMilliseconds;

            Console.WriteLine($"| Що міряємо | Час, мс |");
            Console.WriteLine($"| --- | --- |");
            Console.WriteLine($"| 100 000 × Add(x), без місткості | {timeAddDefault:F1} |");
            Console.WriteLine($"| 100 000 × Add(x), з new List<int>(100000) | {timeAddCapacity:F1} |");
            Console.WriteLine($"| 100 000 × Insert(0, x) | {timeInsert:F1} |");
            Console.WriteLine($"| 1 000 × Contains (немає) у 100 000 елементів | {timeContains:F1} |");
        }
    }
}