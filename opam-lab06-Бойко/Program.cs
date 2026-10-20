#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace opam_lab06_Бойко
{
    class Student
    {
        public string Name;
        public string Group;
        public int Score;

        public Student(string name, string group, int score)
        {
            Name = name;
            Group = group;
            Score = score;
        }
    }

    class Program
    {
        // Варіант: N = 2, K = 1, a = 2, b = 2, c = 2
        const int N = 2;
        const int K = 1;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== ЛАБОРАТОРНА РОБОТА №6 ===");
            Console.WriteLine($"Параметри: N = {N}, K = {K}, a = {N % 3}, b = {N % 4}, c = {N % 5}\n");

            RunTask1();
            Console.WriteLine(new string('-', 50));

            RunTask2();
            Console.WriteLine(new string('-', 50));

            RunTask3();
            Console.WriteLine(new string('-', 50));

            RunTask4();
            Console.WriteLine(new string('-', 50));

            RunTask5();
            Console.WriteLine(new string('-', 50));

            RunEdgeCases();
        }

        #region Завдання 1: Підрахунок частот довжин слів (a = 2)
        static void RunTask1()
        {
            Console.WriteLine("--- Завдання 1: Підрахунок частот довжин слів ---");
            string text = "Алгоритм описує послідовність дій. Алгоритм можна записати словами, а можна записати кодом. Кодом алгоритм стає тоді, коли його виконує машина, а не людина. Одна задача — багато алгоритмів.";
            
            Dictionary<int, int> wordLengthFrequencies = CountWordLengths(text);

            int threshold = (K % 3) + 2; // (1 % 3) + 2 = 3
            Console.WriteLine($"Поріг частоти (>= K % 3 + 2): {threshold}");
            Console.WriteLine("Довжина слова | Частота");
            Console.WriteLine("-----------------------");

            foreach (KeyValuePair<int, int> pair in wordLengthFrequencies)
            {
                if (pair.Value >= threshold)
                {
                    Console.WriteLine($"{pair.Key,13} | {pair.Value}");
                }
            }

            Console.WriteLine($"Загальна кількість різних довжин слів: {wordLengthFrequencies.Count}");
        }

        public static Dictionary<int, int> CountWordLengths(string text)
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            int currentLength = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                if (char.IsLetterOrDigit(ch))
                {
                    currentLength++;
                }
                else
                {
                    if (currentLength > 0)
                    {
                        AddOrIncrement(dict, currentLength);
                        currentLength = 0;
                    }
                }
            }

            if (currentLength > 0)
            {
                AddOrIncrement(dict, currentLength);
            }

            return dict;
        }

        private static void AddOrIncrement(Dictionary<int, int> dict, int length)
        {
            if (dict.TryGetValue(length, out int count))
            {
                dict[length] = count + 1;
            }
            else
            {
                dict[length] = 1;
            }
        }
        #endregion

        #region Завдання 2: Групування студентів за діапазоном балів (b = 2)
        static void RunTask2()
        {
            Console.WriteLine("--- Завдання 2: Групування за діапазоном балів ---");
            List<Student> students = GetSampleStudents();

            Dictionary<string, List<Student>> grouped = GroupStudentsByScoreRange(students);

            foreach (KeyValuePair<string, List<Student>> pair in grouped)
            {
                double sum = 0;
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    sum += pair.Value[i].Score;
                }
                double avg = pair.Value.Count > 0 ? sum / pair.Value.Count : 0;

                Console.WriteLine($"Діапазон: {pair.Key,-10} | Кількість: {pair.Value.Count,2} | Середній бал: {avg:F2}");
            }
        }

        public static Dictionary<string, List<Student>> GroupStudentsByScoreRange(List<Student> students)
        {
            Dictionary<string, List<Student>> result = new Dictionary<string, List<Student>>();
            string[] ranges = { "0–59", "60–74", "75–89", "90–100" };

            for (int i = 0; i < ranges.Length; i++)
            {
                result[ranges[i]] = new List<Student>();
            }

            for (int i = 0; i < students.Count; i++)
            {
                Student s = students[i];
                string rangeKey = GetScoreRange(s.Score);

                if (result.TryGetValue(rangeKey, out List<Student>? list) && list != null)
                {
                    list.Add(s);
                }
            }

            return result;
        }

        private static string GetScoreRange(int score)
        {
            if (score <= 59) return "0–59";
            if (score <= 74) return "60–74";
            if (score <= 89) return "75–89";
            return "90–100";
        }

        static List<Student> GetSampleStudents()
        {
            return new List<Student>
            {
                new Student("Коваленко", "КН-3/1", 88),
                new Student("Мельник", "КН-3/2", 64),
                new Student("Іщенко", "КН-3/1", 93),
                new Student("Бондар", "КН-3/1", 71),
                new Student("Савченко", "КН-3/2", 55),
                new Student("Ткачук", "КН-3/2", 90),
                new Student("Кравець", "КН-3/1", 76),
                new Student("Іванів", "КН-3/1", 48),
                new Student("Мороз", "КН-3/2", 82),
                new Student("Шевчук", "КН-3/1", 67),
                new Student("Бойко", "КН-3/2", 95),
                new Student("Кушнір", "КН-3/2", 73)
            };
        }
        #endregion

        #region Завдання 3: Вилучення дублікатів
        static void RunTask3()
        {
            Console.WriteLine("--- Завдання 3: Вилучення дублікатів ---");
            Random rng = new Random(N);
            int maxVal = (K % 10) + 6; // (1 % 10) + 6 = 7
            List<int> numbers = new List<int>();

            for (int i = 0; i < 25; i++)
            {
                numbers.Add(rng.Next(1, maxVal + 1));
            }

            Console.Write("Оригінальний список (25 елементів): ");
            PrintList(numbers);

            // 1. Без збереження порядку (HashSet конструктор)
            HashSet<int> unorderedSet = new HashSet<int>(numbers);
            Console.Write("Спосіб 1 (HashSet конструктор, без збереження порядку): ");
            PrintCollection(unorderedSet);

            // 2. Зі збереженням порядку
            List<int> orderedList = RemoveDuplicatesPreservingOrder(numbers);
            Console.Write("Спосіб 2 (Зі збереженням порядку): ");
            PrintList(orderedList);
        }

        public static List<int> RemoveDuplicatesPreservingOrder(List<int> input)
        {
            HashSet<int> seen = new HashSet<int>();
            List<int> result = new List<int>();

            for (int i = 0; i < input.Count; i++)
            {
                if (seen.Add(input[i]))
                {
                    result.Add(input[i]);
                }
            }

            return result;
        }
        #endregion

        #region Завдання 4: Елементи, що є лише в одному з наборів (c = 2)
        static void RunTask4()
        {
            Console.WriteLine("--- Завдання 4: Симетрична різниця (елементи лише в одному з наборів) ---");
            int maxRange = 20 + K; // 21
            List<int> setA_list = GenerateRandomList(30, maxRange, N);
            List<int> setB_list = GenerateRandomList(30, maxRange, N + 100);

            // Спосіб 1: Через HashSet
            List<int> resultHashSet = SymmetricDifferenceHashSet(setA_list, setB_list);
            
            // Спосіб 2: Вручну (циклами по списках)
            List<int> resultManual = SymmetricDifferenceManual(setA_list, setB_list);

            resultHashSet.Sort();
            resultManual.Sort();

            Console.Write("Результат HashSet: ");
            PrintList(resultHashSet);

            Console.Write("Результат Вручну:  ");
            PrintList(resultManual);

            bool isMatches = ListsAreEqual(resultHashSet, resultManual);
            Console.WriteLine($"Результати двох способів збігаються: {isMatches}");
        }

        public static List<int> SymmetricDifferenceHashSet(List<int> listA, List<int> listB)
        {
            HashSet<int> setA = new HashSet<int>(listA);
            HashSet<int> setB = new HashSet<int>(listB);

            HashSet<int> copyA = new HashSet<int>(setA);
            copyA.SymmetricExceptWith(setB);

            return new List<int>(copyA);
        }

        public static List<int> SymmetricDifferenceManual(List<int> listA, List<int> listB)
        {
            List<int> result = new List<int>();

            for (int i = 0; i < listA.Count; i++)
            {
                int val = listA[i];
                if (!ListContains(listB, val) && !ListContains(result, val))
                {
                    result.Add(val);
                }
            }

            for (int i = 0; i < listB.Count; i++)
            {
                int val = listB[i];
                if (!ListContains(listA, val) && !ListContains(result, val))
                {
                    result.Add(val);
                }
            }

            return result;
        }

        private static bool ListContains(List<int> list, int value)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == value) return true;
            }
            return false;
        }

        private static bool ListsAreEqual(List<int> a, List<int> b)
        {
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        private static List<int> GenerateRandomList(int count, int maxVal, int seed)
        {
            Random rng = new Random(seed);
            List<int> list = new List<int>();
            for (int i = 0; i < count; i++)
            {
                list.Add(rng.Next(1, maxVal + 1));
            }
            return list;
        }
        #endregion

        #region Завдання 5: Замір виграшу (Stopwatch)
        static void RunTask5()
        {
            Console.WriteLine("--- Завдання 5: Замір виграшу продуктивності ---");
            Warmup();

            MeasureContains10k();
            MeasureWordFrequencies50k();
            MeasureRemoveDuplicates20k();
            MeasureIntersect5k();
        }

        static void Warmup()
        {
            List<int> dummy = new List<int>() { 1, 2, 3 };
            HashSet<int> dummySet = new HashSet<int>(dummy);
            dummySet.Contains(2);
        }

        static void MeasureContains10k()
        {
            int size = 100000;
            int queries = 10000;
            List<int> list = new List<int>();
            HashSet<int> set = new HashSet<int>();
            for (int i = 0; i < size; i++)
            {
                list.Add(i);
                set.Add(i);
            }

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < queries; i++)
            {
                ListContains(list, -1);
            }
            sw.Stop();
            double listTime = sw.Elapsed.TotalMilliseconds;

            sw.Restart();
            for (int i = 0; i < queries; i++)
            {
                set.Contains(-1);
            }
            sw.Stop();
            double setTime = sw.Elapsed.TotalMilliseconds;

            PrintBenchRow("10 000 перевірок (100k елементів)", listTime, setTime);
        }

        static void MeasureWordFrequencies50k()
        {
            int wordCount = 50000;
            List<string> words = new List<string>();
            Random rng = new Random(42);
            for (int i = 0; i < wordCount; i++)
            {
                words.Add("word" + rng.Next(1, 1000));
            }

            Stopwatch sw = Stopwatch.StartNew();
            List<string> keysList = new List<string>();
            List<int> valuesList = new List<int>();
            for (int i = 0; i < words.Count; i++)
            {
                string w = words[i];
                int idx = -1;
                for (int j = 0; j < keysList.Count; j++)
                {
                    if (keysList[j] == w) { idx = j; break; }
                }
                if (idx != -1) valuesList[idx]++;
                else { keysList.Add(w); valuesList.Add(1); }
            }
            sw.Stop();
            double listTime = sw.Elapsed.TotalMilliseconds;

            sw.Restart();
            Dictionary<string, int> dict = new Dictionary<string, int>();
            for (int i = 0; i < words.Count; i++)
            {
                string w = words[i];
                if (dict.TryGetValue(w, out int count)) dict[w] = count + 1;
                else dict[w] = 1;
            }
            sw.Stop();
            double dictTime = sw.Elapsed.TotalMilliseconds;

            PrintBenchRow("Підрахунок частот 50 000 слів", listTime, dictTime);
        }

        static void MeasureRemoveDuplicates20k()
        {
            int count = 20000;
            List<int> numbers = new List<int>();
            Random rng = new Random(42);
            for (int i = 0; i < count; i++) numbers.Add(rng.Next(1, 5000));

            Stopwatch sw = Stopwatch.StartNew();
            List<int> listResult = new List<int>();
            for (int i = 0; i < numbers.Count; i++)
            {
                if (!ListContains(listResult, numbers[i])) listResult.Add(numbers[i]);
            }
            sw.Stop();
            double listTime = sw.Elapsed.TotalMilliseconds;

            sw.Restart();
            HashSet<int> set = new HashSet<int>(numbers);
            sw.Stop();
            double setTime = sw.Elapsed.TotalMilliseconds;

            PrintBenchRow("Вилучення дублікатів (20k чисел)", listTime, setTime);
        }

        static void MeasureIntersect5k()
        {
            int count = 5000;
            List<int> listA = GenerateRandomList(count, 10000, 1);
            List<int> listB = GenerateRandomList(count, 10000, 2);

            Stopwatch sw = Stopwatch.StartNew();
            List<int> intersectList = new List<int>();
            for (int i = 0; i < listA.Count; i++)
            {
                int val = listA[i];
                if (ListContains(listB, val) && !ListContains(intersectList, val))
                {
                    intersectList.Add(val);
                }
            }
            sw.Stop();
            double listTime = sw.Elapsed.TotalMilliseconds;

            sw.Restart();
            HashSet<int> setA = new HashSet<int>(listA);
            HashSet<int> setB = new HashSet<int>(listB);
            setA.IntersectWith(setB);
            sw.Stop();
            double setTime = sw.Elapsed.TotalMilliseconds;

            PrintBenchRow("Спільні елементи (2 набори по 5k)", listTime, setTime);
        }

        static void PrintBenchRow(string title, double listTimeMs, double hashTimeMs)
        {
            double ratio = hashTimeMs > 0 ? listTimeMs / hashTimeMs : listTimeMs;
            Console.WriteLine($"{title,-35} | Список: {listTimeMs,8:F2} мс | Хеш-структура: {hashTimeMs,6:F2} мс | Виграш: у {ratio,6:F1} разів");
        }
        #endregion

        #region Крайові випадки (Edge Cases)
        static void RunEdgeCases()
        {
            Console.WriteLine("--- Перевірка крайових випадків ---");

            var res1 = CountWordLengths("");
            Console.WriteLine($"1. Порожній текст -> Кількість елементів: {res1.Count} (Очікується 0)");

            var res2 = CountWordLengths("a bb ccc dddd");
            Console.WriteLine($"2. Усі слова різні -> Кількість довжин: {res2.Count} (Очікується 4)");

            List<Student> sameScoreStudents = new List<Student>
            {
                new Student("Студент1", "КН-1", 95),
                new Student("Студент2", "КН-1", 92)
            };
            var res3 = GroupStudentsByScoreRange(sameScoreStudents);
            Console.WriteLine($"3. Усі в одній групі балів -> Група '90–100': {res3["90–100"].Count} чол.");

            List<int> noDups = new List<int> { 1, 2, 3, 4, 5 };
            var res4 = RemoveDuplicatesPreservingOrder(noDups);
            Console.WriteLine($"4. Без дублікатів -> Розмір: {res4.Count} (Очікується 5)");

            List<int> setA = new List<int> { 1, 2 };
            List<int> setB = new List<int> { 3, 4 };
            var res5 = SymmetricDifferenceHashSet(setA, setB);
            Console.WriteLine($"5. Набори не перетинаються -> Розмір симетричної різниці: {res5.Count} (Очікується 4)");

            var res6 = SymmetricDifferenceHashSet(setA, setA);
            Console.WriteLine($"6. Набори однакові -> Розмір симетричної різниці: {res6.Count} (Очікується 0)");
        }
        #endregion

        #region Допоміжні методи
        static void PrintList(List<int> list)
        {
            Console.WriteLine(string.Join(", ", list));
        }

        static void PrintCollection(IEnumerable<int> col)
        {
            Console.WriteLine(string.Join(", ", col));
        }
        #endregion
    }
}