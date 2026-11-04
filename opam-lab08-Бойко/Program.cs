using System;

namespace opam_lab08_Бойко
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            int N = 2;
            int K = 1;
            int a = N % 3; // 2
            int b = N % 4; // 2
            int c = N % 5; // 2

            Console.WriteLine($"=== ЛАБОРАТОРНА РОБОТА №8 ===");
            Console.WriteLine($"Параметри: N = {N}, K = {K}, a = {a}, b = {b}, c = {c}\n");

            // Компаратор порівняння лише за Score
            Func<Student, Student, int> cmp = (s1, s2) => s1.Score.CompareTo(s2.Score);

            // Генерація даних
            int size = 100000;
            var rawStudents = new Student[size];
            var rnd = new Random(N);

            string[] names = { "Артьом", "Бойко", "Мельник", "Ковальчук", "Шевченко", "Бондаренко" };
            string[] groups = { "КН-3/1", "КН-3/2", "ІПЗ-3/1" };

            for (int i = 0; i < size; i++)
            {
                rawStudents[i] = new Student(
                    names[rnd.Next(names.Length)],
                    groups[rnd.Next(groups.Length)],
                    rnd.Next(0, 101)
                );
            }

            // Копія та сортування масиву тим самим компаратором!
            var sortedStudents = new Student[size];
            Array.Copy(rawStudents, sortedStudents, size);
            Array.Sort(sortedStudents, (x, y) => cmp(x, y));

            // Контрольні об'єкти
            var targetX = new Student("TargetX", "КН-3/1", K + 50); // Score = 51
            var targetFirst = sortedStudents[0];
            var targetNotFound = new Student("NotFound", "КН-3/1", 101); // Score = 101

            #region Завдання 1: Лінійний пошук
            Console.WriteLine("--- ЗАВДАННЯ 1: Лінійний пошук ---");
            int idx1 = Search.LinearSearch(sortedStudents, targetX, cmp, out int comp1);
            Console.WriteLine($"1. X (Score=51): Індекс = {idx1}, Порівнянь = {comp1}");

            int idx2 = Search.LinearSearch(sortedStudents, targetFirst, cmp, out int comp2);
            Console.WriteLine($"2. Перший елемент: Індекс = {idx2}, Порівнянь = {comp2}");

            int idx3 = Search.LinearSearch(sortedStudents, targetNotFound, cmp, out int comp3);
            Console.WriteLine($"3. Відсутній (Score=101): Індекс = {idx3}, Порівнянь = {comp3}\n");
            #endregion

            #region Завдання 2: Бінарний пошук і поломки
            Console.WriteLine("--- ЗАВДАННЯ 2: Бінарний пошук і 4 поломки ---");
            int bIdx1 = Search.BinarySearch(sortedStudents, targetX, cmp, out int bComp1);
            Console.WriteLine($"Правильний бінарний: X (Score=51): Індекс = {bIdx1}, Порівнянь = {bComp1}");

            int bIdx2 = Search.BinarySearch(sortedStudents, targetFirst, cmp, out int bComp2);
            Console.WriteLine($"Правильний бінарний: Перший елемент: Індекс = {bIdx2}, Порівнянь = {bComp2}");

            int bIdx3 = Search.BinarySearch(sortedStudents, targetNotFound, cmp, out int bComp3);
            Console.WriteLine($"Правильний бінарний: Відсутній: Індекс = {bIdx3}, Порівнянь = {bComp3}");

            // Поломка 1
            int br1Idx = BrokenSearch.Broken1_Condition(sortedStudents, targetFirst, cmp, out _);
            Console.WriteLine($"[Поломка 1] targetFirst (lo < hi): Повернув {br1Idx}");

            // Поломка 2
            var smallArr = new Student[] { new Student("A", "G", 10), new Student("B", "G", 20) };
            int br2Idx = BrokenSearch.Broken2_NoIncrement(smallArr, new Student("C", "G", 20), cmp, out int br2Comp);
            Console.WriteLine($"[Поломка 2] (lo = mid): Цикл не завершився і був зупинений захистом на {br2Comp} порівнянні.");

            // Поломка 3
            Console.WriteLine($"[Поломка 3] (lo+hi)/2: На масиві {size} працює ідеально, оскільки 100 000 < int.MaxValue / 2.");

            // Поломка 4
            var sampleTargets = new Student[100];
            for (int i = 0; i < 100; i++) sampleTargets[i] = rawStudents[rnd.Next(size)];
            int failedCount = BrokenSearch.TestUnsortedArray(rawStudents, sampleTargets, cmp);
            Console.WriteLine($"[Поломка 4] Невідсортований масив: з 100 наявних елементів НЕ знайдено {failedCount}.\n");
            #endregion

            #region Завдання 3: Пошук межі (FindBound)
            Console.WriteLine("--- ЗАВДАННЯ 3: Пошук межі (FindBound) ---");
            // Знаходження меж для X (Score == 51)
            int lbX = Search.FindBound(sortedStudents, s => s.Score >= 51, out int lbComp);
            int ubX = Search.FindBound(sortedStudents, s => s.Score > 51, out int ubComp);
            int countX = ubX - lbX;

            Console.WriteLine($"Нижня межа (Score >= 51): Індекс = {lbX}, Порівнянь = {lbComp}");
            Console.WriteLine($"Верхня межа (Score > 51): Індекс = {ubX}, Порівнянь = {ubComp}");
            Console.WriteLine($"Кількість елементів із Score == 51: {countX}");

            // Ваш персональний запит (b = 2): індекс першого елемента з Score >= 51
            Console.WriteLine($"[Ваш запит b=2] Індекс першого елемента з Score >= 51: {lbX} (Порівнянь: {lbComp})");

            // Перевірка неузгодженої умови: першого студента з кратною 7 оцінкою
            int invalidBound = Search.FindBound(sortedStudents, s => s.Score % 7 == 0, out _);
            Console.WriteLine($"[Неузгоджена умова] Перший студент із балом % 7 == 0: індекс {invalidBound} (НЕКОРЕКТНО, бо умова не монотона!)\n");
            #endregion

            #region Завдання 4: Потрійний пошук (c = 2)
            Console.WriteLine("--- ЗАВДАННЯ 4: Потрійний пошук (Ternary Search) ---");
            int tIdx1 = Search.TernarySearch(sortedStudents, targetX, cmp, out int tComp1);
            Console.WriteLine($"1. X (Score=51): Індекс = {tIdx1}, Порівнянь = {tComp1}");
            int tIdx2 = Search.TernarySearch(sortedStudents, targetFirst, cmp, out int tComp2);
            Console.WriteLine($"2. Перший елемент: Індекс = {tIdx2}, Порівнянь = {tComp2}");
            int tIdx3 = Search.TernarySearch(sortedStudents, targetNotFound, cmp, out int tComp3);
            Console.WriteLine($"3. Відсутній: Індекс = {tIdx3}, Порівнянь = {tComp3}\n");
            #endregion

            #region Завдання 5: Вимірювання
            Benchmark.RunComparisonsTable(sortedStudents, cmp);
            Benchmark.RunBreakEvenPoint(rawStudents, cmp);
            #endregion
        }
    }
}