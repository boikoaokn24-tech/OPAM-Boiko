using System;

namespace Lab1
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            
            Console.WriteLine("=== Завдання 1: Три числа за спаданням ===");
            Console.Write("Число 1: "); int.TryParse(Console.ReadLine(), out int num1);
            Console.Write("Число 2: "); int.TryParse(Console.ReadLine(), out int num2);
            Console.Write("Число 3: "); int.TryParse(Console.ReadLine(), out int num3);

            int max, mid, min;
            if (num1 >= num2 && num1 >= num3)
            {
                max = num1;
                if (num2 >= num3) { mid = num2; min = num3; }
                else { mid = num3; min = num2; }
            }
            else if (num2 >= num1 && num2 >= num3)
            {
                max = num2;
                if (num1 >= num3) { mid = num1; min = num3; }
                else { mid = num3; min = num1; }
            }
            else
            {
                max = num3;
                if (num1 >= num2) { mid = num1; min = num2; }
                else { mid = num2; min = num1; }
            }

            int uniqueCount = (num1 == num2 && num2 == num3) ? 1 : (num1 == num2 || num1 == num3 || num2 == num3) ? 2 : 3;
            Console.WriteLine($"За спаданням: {max}, {mid}, {min} | Унікальних значень: {uniqueCount}\n");


            
            Console.WriteLine("=== Завдання 2: Аналіз цифр числа ===");
            Console.Write("Введіть натуральне число: ");
            int.TryParse(Console.ReadLine(), out int number);

            if (number <= 0)
            {
                Console.WriteLine("Помилка: Число має бути натуральним!");
            }
            else
            {
                int temp = number;
                long totalSum = 0;
                int zeroCount = 0;
                bool hasMatching = false;
                long matchingProduct = 1;

                while (temp > 0)
                {
                    int digit = temp % 10;
                    totalSum += digit;
                    if (digit == 0) zeroCount++;

                    // Критерій: непарні та < K (K = 1)
                    if (digit % 2 != 0 && digit < 1)
                    {
                        hasMatching = true;
                        matchingProduct *= digit;
                    }
                    temp /= 10;
                }

                Console.WriteLine($"Сума цифр: {totalSum} | Нулів: {zeroCount}");
                if (hasMatching)
                    Console.WriteLine($"Добуток непарних цифр (< 1): {matchingProduct}");
                else
                    Console.WriteLine("Цифр, що відповідають критерію (непарні та < 1), не знайдено.");
            }


            
            Console.WriteLine("\n=== Завдання 4: Порівняння НСД ===");
            int N = 2;
            int A = 1000000 + N; // 1000002
            int B = 999999 - N;  // 999997

            int gcd1 = 1, iter1 = 0;
            for (int i = Math.Min(A, B); i >= 1; i--)
            {
                iter1++;
                if (A % i == 0 && B % i == 0) { gcd1 = i; break; }
            }

            int a = A, b = B, iter2 = 0;
            while (b != 0) { iter2++; int rem = a % b; a = b; b = rem; }

            Console.WriteLine($"A={A}, B={B}");
            Console.WriteLine($"Перебор: НСД = {gcd1} (ітерацій: {iter1})");
            Console.WriteLine($"Евклід:  НСД = {a} (ітерацій: {iter2})");
            Console.WriteLine($"Висновок: Евклід швидший у {iter1 / iter2} разів завдяки діленню з остачею.");
        }
    }
}