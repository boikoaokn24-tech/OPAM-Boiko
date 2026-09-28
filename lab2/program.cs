using System;

class Program
{
    static void Main()
    {
        int n = 7, k = 19;
        Console.WriteLine($"=== Лабораторна робота 2 (N={n}, K={k}, a=1, b=3, c=2) ===\n");

        int[] arr = GenerateArray(n, 18, -3, 3);
        Console.Write("Масив: "); PrintArray(arr);

        // Завдання 1
        Console.WriteLine("\n[Завдання 1] Агрегація:");
        Task1_Aggregation(arr);

        // Завдання 2 (критерій: кратні 5)
        Console.WriteLine("\n[Завдання 2] Фільтрація:");
        int divisor = (k % 4) + 2;
        int[] filtered = Task2_Filter(arr, x => x != 0 && x % divisor == 0);
        PrintArray(filtered);

        // Завдання 3
        Console.WriteLine("\n[Завдання 3] Найдовша серія:");
        Task3_LongestSeries(arr);

        // Завдання 4
        Console.WriteLine("\n[Завдання 4] Зсув вправо на місці:");
        Task4_ShiftRightInPlace(arr);
        PrintArray(arr);

        // Завдання 5
        Console.WriteLine("\n[Завдання 5] Матриця 6x3:");
        Task5_MatrixProcessing(n, 6, 3, -3, 3);
    }

    static int[] GenerateArray(int seed, int len, int min, int max)
    {
        Random rnd = new Random(seed);
        int[] res = new int[len];
        for (int i = 0; i < len; i++) res[i] = rnd.Next(min, max + 1);
        return res;
    }

    static void PrintArray(int[] arr) => 
        Console.WriteLine(arr == null || arr.Length == 0 ? "[порожньо]" : string.Join(", ", arr));

    static void Task1_Aggregation(int[] arr)
    {
        if (arr == null || arr.Length == 0) { Console.WriteLine("Дані відсутні."); return; }

        long sum = 0;
        int min = arr[0], minIdx = 0, max = arr[0], maxIdx = 0, zeros = 0;

        for (int i = 0; i < arr.Length; i++)
        {
            sum += arr[i];
            if (arr[i] < min) { min = arr[i]; minIdx = i; }
            if (arr[i] > max) { max = arr[i]; maxIdx = i; }
            if (arr[i] == 0) zeros++;
        }

        Console.WriteLine($"Сума: {sum}, Середнє: {(double)sum / arr.Length:F2}");
        Console.WriteLine($"Мін: {min} (індекс {minIdx}), Макс: {max} (індекс {maxIdx}), Нулів: {zeros}");
    }

    static int[] Task2_Filter(int[] arr, Func<int, bool> match)
    {
        if (arr == null || arr.Length == 0) return new int[0];

        int count = 0;
        foreach (var x in arr) if (match(x)) count++;

        int[] result = new int[count];
        for (int i = 0, idx = 0; i < arr.Length; i++)
            if (match(arr[i])) result[idx++] = arr[i];

        return result;
    }

    static void Task3_LongestSeries(int[] arr)
    {
        if (arr == null || arr.Length == 0) { Console.WriteLine("Дані відсутні."); return; }

        int bVal = arr[0], bLen = 1, bStart = 0;
        int cVal = arr[0], cLen = 1, cStart = 0;

        for (int i = 1; i < arr.Length; i++)
        {
            if (arr[i] == cVal)
            {
                cLen++;
            }
            else
            {
                if (cLen > bLen) { bLen = cLen; bVal = cVal; bStart = cStart; }
                cVal = arr[i]; cLen = 1; cStart = i;
            }
        }
        if (cLen > bLen) { bLen = cLen; bVal = cVal; bStart = cStart; }

        Console.WriteLine($"Значення: {bVal}, Довжина: {bLen}, Початок: {bStart}");
    }

    static void Task4_ShiftRightInPlace(int[] arr)
    {
        if (arr == null || arr.Length <= 1) return;

        int last = arr[^1];
        Array.Copy(arr, 0, arr, 1, arr.Length - 1);
        arr[0] = last;
    }

    static void Task5_MatrixProcessing(int seed, int rows, int cols, int min, int max)
    {
        Random rnd = new Random(seed);
        int[,] mat = new int[rows, cols];
        int maxRowSum = int.MinValue, maxRowIdx = 0;

        for (int i = 0; i < rows; i++)
        {
            int rowSum = 0;
            for (int j = 0; j < cols; j++)
            {
                mat[i, j] = rnd.Next(min, max + 1);
                rowSum += mat[i, j];
                Console.Write($"{mat[i, j],4}");
            }
            Console.WriteLine($" | Сума: {rowSum}");
            if (rowSum > maxRowSum) { maxRowSum = rowSum; maxRowIdx = i; }
        }

        Console.Write("Максимуми стовпців: ");
        for (int j = 0; j < cols; j++)
        {
            int colMax = mat[0, j];
            for (int i = 1; i < rows; i++) if (mat[i, j] > colMax) colMax = mat[i, j];
            Console.Write($"{colMax} ");
        }
        Console.WriteLine($"\nНайбільша сума у рядку №{maxRowIdx} ({maxRowSum})");
    }
}