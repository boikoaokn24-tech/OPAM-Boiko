using System;
using System.Collections.Generic;

namespace opam_lab07_Бойко
{
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

        public override string ToString()
        {
            return $"{Name} ({Group}) - {Score} балів";
        }
    }

    public static class Algorithms
    {
        public static List<T> Filter<T>(List<T> src, Func<T, bool> keep)
        {
            var result = new List<T>();
            if (src == null) return result;

            foreach (var item in src)
            {
                if (keep(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static List<TOut> Map<TIn, TOut>(List<TIn> src, Func<TIn, TOut> convert)
        {
            var result = new List<TOut>();
            if (src == null) return result;

            foreach (var item in src)
            {
                result.Add(convert(item));
            }
            return result;
        }

        public static TAcc Aggregate<T, TAcc>(List<T> src, TAcc start, Func<TAcc, T, TAcc> step)
        {
            TAcc acc = start;
            if (src == null) return acc;

            foreach (var item in src)
            {
                acc = step(acc, item);
            }
            return acc;
        }

        public static Dictionary<TKey, List<T>> GroupBy<T, TKey>(List<T> src, Func<T, TKey> keyOf) where TKey : notnull
        {
            var dict = new Dictionary<TKey, List<T>>();
            if (src == null) return dict;

            foreach (var item in src)
            {
                TKey key = keyOf(item);
                if (!dict.ContainsKey(key))
                {
                    dict[key] = new List<T>();
                }
                dict[key].Add(item);
            }
            return dict;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            int N = 2;
            int K = 1;

            Console.WriteLine($"=== ЛАБОРАТОРНА РОБОТА №7 (Параметри: N = {N}, K = {K}, a = 2, b = 2, c = 2) ===\n");

            var rnd = new Random(N);
            var numbers = new List<int>();
            for (int i = 0; i < 20; i++)
            {
                numbers.Add(rnd.Next(-K, K + 1));
            }

            var words = new List<string>
            {
                "коледж", "веб-розробка", "C#", "клас-контейнер", "код",
                "IT-сфера", "алгоритм", "делегат", "онлайн-курс", "Unity",
                "дво-вимірний", "лабораторна"
            };

            var students = new List<Student>
            {
                new Student("Артьом", "КН-3/1", 85),
                new Student("Бойко", "КН-3/1", 58),
                new Student("Мельник", "КН-3/2", 92),
                new Student("Ковальчук", "КН-3/1", 74),
                new Student("Шевченко", "КН-3/2", 65),
                new Student("Бондаренко", "ІПЗ-3/1", 45)
            };

            // Завдання 1: Фільтрація (а = 2)
            Console.WriteLine("--- ЗАВДАННЯ 1: Відбір (Filter) ---");
            var filteredNums = Algorithms.Filter(numbers, x => x <= 0);
            Console.WriteLine($"Числа <= 0 (Кількість: {filteredNums.Count}): {string.Join(", ", filteredNums)}");

            var filteredWords = Algorithms.Filter(words, w => w.Contains("-"));
            Console.WriteLine($"Рядки з дефісом (Кількість: {filteredWords.Count}): {string.Join(", ", filteredWords)}");

            var filteredStudents = Algorithms.Filter(students, s => s.Score >= 60 && s.Score <= 89);
            Console.WriteLine($"Студенти з балом 60..89 (Кількість: {filteredStudents.Count}):");
            foreach (var st in filteredStudents) Console.WriteLine($"  - {st}");
            Console.WriteLine();

            // Завдання 2: Перетворення (b = 2)
            Console.WriteLine("--- ЗАВДАННЯ 2: Перетворення (Map) ---");
            var mappedSigns = Algorithms.Map(numbers, x => Math.Sign(x));
            Console.WriteLine($"Знаки чисел: {string.Join(", ", mappedSigns)}");

            var mappedFirstLetters = Algorithms.Map(words, w => string.IsNullOrEmpty(w) ? ' ' : w[0]);
            Console.WriteLine($"Перші літери слів: {string.Join(", ", mappedFirstLetters)}");

            var mappedScores = Algorithms.Map(students, s => s.Score);
            Console.WriteLine($"Бали студентів: {string.Join(", ", mappedScores)}");
            Console.WriteLine();

            // Завдання 3: Агрегація (c = 2)
            Console.WriteLine("--- ЗАВДАННЯ 3: Агрегація (Aggregate) ---");
            int sum = Algorithms.Aggregate(numbers, 0, (acc, x) => acc + x);
            Console.WriteLine($"1. Сума чисел: {sum}");

            long product = Algorithms.Aggregate(numbers, 1L, (acc, x) => acc * x);
            Console.WriteLine($"2. Добуток чисел: {product}");

            int max = numbers.Count > 0 
                ? Algorithms.Aggregate(numbers, numbers[0], (acc, x) => x > acc ? x : acc)
                : 0;
            Console.WriteLine($"3. Максимум чисел: {max}");

            string concat = words.Count > 0
                ? Algorithms.Aggregate(words, "", (acc, w) => string.IsNullOrEmpty(acc) ? w : acc + ", " + w)
                : "";
            Console.WriteLine($"4. Конкатенація слів через кому: {concat}");
            Console.WriteLine();

            // Завдання 4: Групування (GroupBy)
            Console.WriteLine("--- ЗАВДАННЯ 4: Групування (GroupBy) ---");
            var byGroup = Algorithms.GroupBy(students, s => s.Group);
            Console.WriteLine("Студенти за групою:");
            foreach (var kvp in byGroup)
                Console.WriteLine($"  Ключ [{kvp.Key}]: {kvp.Value.Count} елемент(ів)");

            var byFirstLetter = Algorithms.GroupBy(students, s => s.Name[0]);
            Console.WriteLine("Студенти за першою літерою прізвища/імені:");
            foreach (var kvp in byFirstLetter)
                Console.WriteLine($"  Ключ [{kvp.Key}]: {kvp.Value.Count} елемент(ів)");

            var byScoreRange = Algorithms.GroupBy(students, s =>
            {
                if (s.Score < 60) return "0–59";
                if (s.Score <= 74) return "60–74";
                if (s.Score <= 89) return "75–89";
                return "90–100";
            });
            Console.WriteLine("Студенти за діапазоном балів:");
            foreach (var kvp in byScoreRange)
                Console.WriteLine($"  Ключ [{kvp.Key}]: {kvp.Value.Count} елемент(ів)");

            var byWordLength = Algorithms.GroupBy(words, w => w.Length);
            Console.WriteLine("Рядки за довжиною:");
            foreach (var kvp in byWordLength)
                Console.WriteLine($"  Довжина [{kvp.Key}]: {kvp.Value.Count} елемент(ів)");
            Console.WriteLine();

            // Завдання 5: Захоплення змінних (Closures)
            Console.WriteLine("--- ЗАВДАННЯ 5: Захоплення змінних ---");
            int threshold = K;
            Func<int, bool> checkThreshold = x => x > threshold;
            threshold = 1000;
            Console.WriteLine($"1. Змінна після лямбди (5 > threshold де threshold=1000): {checkThreshold(5)}");

            var funcsFor = new List<Func<int>>();
            for (int i = 0; i < 3; i++)
            {
                funcsFor.Add(() => i);
            }
            Console.Write("2. Результат для циклу for: ");
            foreach (var f in funcsFor) Console.Write($"{f()} ");
            Console.WriteLine();

            var funcsForeach = new List<Func<int>>();
            foreach (var item in new int[] { 0, 1, 2 })
            {
                funcsForeach.Add(() => item);
            }
            Console.Write("3. Результат для циклу foreach: ");
            foreach (var f in funcsForeach) Console.Write($"{f()} ");
            Console.WriteLine("\n");
        }
    }
}