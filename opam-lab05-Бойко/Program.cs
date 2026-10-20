using System;
using System.Collections.Generic;

namespace opam_lab05_Бойко
{
    // --- ЗАВДАННЯ 4: Власний стек поверх масиву для цілих чисел ---
    public class IntStack
    {
        private int[] _array;
        private int _top;

        public IntStack(int initialCapacity)
        {
            if (initialCapacity < 1) initialCapacity = 4;
            _array = new int[initialCapacity];
            _top = 0;
        }

        public int Count => _top;

        public int Capacity => _array.Length;

        public void Push(int item)
        {
            if (_top == _array.Length)
            {
                int newCapacity = _array.Length * 2;
                int[] newArray = new int[newCapacity];
                Array.Copy(_array, newArray, _array.Length);
                _array = newArray;
            }
            _array[_top++] = item;
        }

        public int Pop()
        {
            if (_top == 0)
            {
                throw new InvalidOperationException("Стек порожній.");
            }
            return _array[--_top];
        }

        public int Peek()
        {
            if (_top == 0)
            {
                throw new InvalidOperationException("Стек порожній.");
            }
            return _array[_top - 1];
        }

        public bool IsEmpty => _top == 0;
    }

    // --- Спеціалізований стек символів на основі IntStack для Завдання 4 (перевірка дужок) ---
    public class CharStack
    {
        private IntStack _intStack;

        public CharStack(int capacity)
        {
            _intStack = new IntStack(capacity);
        }

        public int Count => _intStack.Count;
        public int Capacity => _intStack.Capacity;

        public void Push(char c) => _intStack.Push((int)c);
        public char Pop() => (char)_intStack.Pop();
        public char Peek() => (char)_intStack.Peek();
        public bool IsEmpty => _intStack.IsEmpty;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            int n = 7;
            int k = 1;

            Console.WriteLine("=== ЛАБОРАТОРНА РОБОТА №5 ===");
            Console.WriteLine($"N = {n}, K = {k}, a = 1, b = 3, c = 2\n");

            // ================= ЗАВДАННЯ 1 =================
            Console.WriteLine("--- ЗАВДАННЯ 1: Збалансованість дужок ---");
            RunTask1Tests();

            // ================= ЗАВДАННЯ 2 =================
            Console.WriteLine("\n--- ЗАВДАННЯ 2: Зворотний польський запис (ЗПЗ) ---");
            RunTask2Tests(k);

            // ================= ЗАВДАННЯ 3 =================
            Console.WriteLine("\n--- ЗАВДАННЯ 3: Модель обслуговування (Дві черги) ---");
            RunTask3Simulation(n, k);

            // ================= ЗАВДАННЯ 4 =================
            Console.WriteLine("\n--- ЗАВДАННЯ 4: Власний стек та його перевірка ---");
            RunTask4Tests(k);

            // ================= КРАЙОВІ ВИПАДКИ =================
            Console.WriteLine("\n--- КРАЙОВІ ВИПАДКИ ---");
            RunEdgeCases();
        }

        // ================= МЕТОДИ ЗАВДАННЯ 1 =================
        static (bool isValid, int errorIndex) CheckBrackets(string s, int aVariant)
        {
            CharStack stack = new CharStack(4);

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                if (c == '(' || c == '[' || c == '{' || (aVariant == 1 && c == '<'))
                {
                    stack.Push(c);
                }
                else if (c == ')' || c == ']' || c == '}' || (aVariant == 1 && c == '>'))
                {
                    if (stack.IsEmpty) return (false, i); // Закривальна при порожньому стеку

                    char top = stack.Pop();
                    if (!IsMatchingPair(top, c)) return (false, i);
                }
            }

            if (!stack.IsEmpty) return (false, s.Length); // Непорожній стек після кінця
            return (true, -1);
        }

        static bool IsMatchingPair(char open, char close)
        {
            return (open == '(' && close == ')') ||
                   (open == '[' && close == ']') ||
                   (open == '{' && close == '}') ||
                   (open == '<' && close == '>');
        }

        static void RunTask1Tests()
        {
            string[] tests = { "(a[b]{c})", "([)]", ")(", "((", "", "( ( (" };
            foreach (var t in tests)
            {
                var res = CheckBrackets(t, 1);
                Console.WriteLine($"Рядок: \"{t}\" -> Збалансований: {res.isValid}, Індекс помилки: {res.errorIndex}");
            }
            // Свідомо незбалансований рядок для K = 1 (рівно 1 відкривальна '(' і 0 закривальних)
            string customBad = "(";
            var resCustom = CheckBrackets(customBad, 1);
            Console.WriteLine($"Власний незбалансований (\"(\"): Збалансований: {resCustom.isValid}, Індекс помилки: {resCustom.errorIndex}");
        }

        // ================= МЕТОДИ ЗАВДАННЯ 2 =================
        static void RunTask2Tests(int k)
        {
            string[] expressions = {
                "3 4 + 2 *",
                "5 3 -",
                "8 2 /",
                "2 3 4 * +",
                "5 0 /",
                "3 4 min",
                "5 2 max",
                "unknownTok 2 +", // невідомий токен
                "3 4 + 5",       // забагато чисел
                "3 +"            // забракло чисел
            };

            foreach (var expr in expressions)
            {
                EvaluateRPN(expr);
            }

            // Власний вираз у ЗПЗ, результат якого дорівнює K (1), з трьома операціями: "5 3 min 2 - 1 +" -> мін(5,3) = 3; 3 - 2 = 1; 1 + 1 = 2 (помилка, треба K=1).
            // Спробуємо інший: "5 3 min 4 - 2 +" -> min(5,3)=3; 3-4 = -1; -1 + 2 = 1. Операцій: min, -, + (тут 3 операції!).
            string myExpr = "5 3 min 4 - 2 +";
            Console.WriteLine($"Власний вираз на K={k}: \"{myExpr}\"");
            EvaluateRPN(myExpr);
        }

        static void EvaluateRPN(string expr)
        {
            Stack<double> stack = new Stack<double>();
            string[] tokens = expr.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var token in tokens)
            {
                if (double.TryParse(token, out double num))
                {
                    stack.Push(num);
                }
                else
                {
                    // Операції (+, -, *, /, %, min, max)
                    if (token == "+" || token == "-" || token == "*" || token == "/" || token == "%" || token == "min" || token == "max")
                    {
                        if (token == "min" || token == "max")
                        {
                            if (stack.Count < 2)
                            {
                                Console.WriteLine($"[Помилка у виразі \"{expr}\"]: замало операндів для операції {token}.");
                                return;
                            }
                            double b = stack.Pop();
                            double a = stack.Pop();
                            if (token == "min") stack.Push(Math.Min(a, b));
                            else stack.Push(Math.Max(a, b));
                        }
                        else
                        {
                            if (stack.Count < 2)
                            {
                                Console.WriteLine($"[Помилка у виразі \"{expr}\"]: замало операндів для операції {token}.");
                                return;
                            }
                            double b = stack.Pop();
                            double a = stack.Pop();

                            if (token == "+") stack.Push(a + b);
                            else if (token == "-") stack.Push(a - b);
                            else if (token == "*") stack.Push(a * b);
                            else if (token == "/")
                            {
                                if (b == 0)
                                {
                                    Console.WriteLine($"[Помилка у виразі \"{expr}\"]: ділення на нуль!");
                                    return;
                                }
                                stack.Push(a / b);
                            }
                            else if (token == "%") stack.Push(a % b);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[Помилка у виразі \"{expr}\"]: невідомий токен \"{token}\".");
                        return;
                    }
                }
            }

            if (stack.Count == 1)
            {
                Console.WriteLine($"Результат виразу \"{expr}\" = {stack.Pop()}");
            }
            else if (stack.Count > 1)
            {
                Console.WriteLine($"[Помилка у виразі \"{expr}\"]: забагато чисел у стеку (залишилось {stack.Count}).");
            }
            else
            {
                Console.WriteLine($"[Помилка у виразі \"{expr}\"]: порожній стек результатів.");
            }
        }

        // ================= МЕТОДИ ЗАВДАННЯ 3 =================
        class Request
        {
            public int Id { get; set; }
            public int WorkUnits { get; set; }
        }

        static void RunTask3Simulation(int n, int k)
        {
            int totalRequests = 8 + (k % 5);
            Random rnd = new Random(n);

            Queue<Request> normalQueue = new Queue<Request>();
            Queue<Request> priorityQueue = new Queue<Request>();

            for (int i = 1; i <= totalRequests; i++)
            {
                int weight = rnd.Next(1, 10);
                Request req = new Request { Id = i, WorkUnits = weight };

                // c = 2: кожна третя заявка потрапляє в пріоритетну
                if (i % 3 == 0)
                {
                    priorityQueue.Enqueue(req);
                }
                else
                {
                    normalQueue.Enqueue(req);
                }
            }

            int currentTime = 0;
            int maxQueueLength = Math.Max(normalQueue.Count + priorityQueue.Count, 0);

            Console.WriteLine($"Загальна кількість заявок: {totalRequests}");

            // Обслуговуємо спочатку пріоритетну чергу
            while (priorityQueue.Count > 0 || normalQueue.Count > 0)
            {
                int currentQLen = normalQueue.Count + priorityQueue.Count;
                if (currentQLen > maxQueueLength) maxQueueLength = currentQLen;

                Request currentReq;
                if (priorityQueue.Count > 0)
                {
                    currentReq = priorityQueue.Dequeue();
                }
                else
                {
                    currentReq = normalQueue.Dequeue();
                }

                int startTime = currentTime;
                int finishTime = startTime + currentReq.WorkUnits;
                currentTime = finishTime;

                Console.WriteLine($"Заявка #{currentReq.Id} (вага: {currentReq.WorkUnits}): початок = {startTime} хв, завершення = {finishTime} хв");
            }

            Console.WriteLine($"Максимальна довжина черги за час моделювання: {maxQueueLength}");
        }

        // ================= МЕТОДИ ЗАВДАННЯ 4 =================
        static void RunTask4Tests(int k)
        {
            // Початкова місткість: K % 4 + 2 = 1 % 4 + 2 = 3
            int initialCap = (k % 4) + 2;
            IntStack stack = new IntStack(initialCap);

            // Кількість викликів Push: K % 4 + 3 = 1 % 4 + 3 = 4 виклики
            int pushCount = (k % 4) + 3;

            Console.WriteLine($"Початкова місткість власного стеку: {initialCap}");
            for (int i = 1; i <= pushCount; i++)
            {
                stack.Push(i * 10);
                Console.WriteLine($"Після Push({i * 10}) -> Count = {stack.Count}, Capacity = {stack.Capacity}");
            }
        }

        // ================= КРАЙОВІ ВИПАДКИ =================
        static void RunEdgeCases()
        {
            // 1. Порожній рядок (Завдання 1)
            var resEmpty = CheckBrackets("", 1);
            Console.WriteLine($"Порожній рядок (Завд. 1) -> Збалансований: {resEmpty.isValid}");

            // 2. Вираз з одного числа (Завдання 2)
            EvaluateRPN("42");

            // 3. Ділення на нуль (Завдання 2)
            EvaluateRPN("10 0 /");

            // 4. Pop на порожньому стеку (Завдання 4)
            try
            {
                IntStack emptyStack = new IntStack(2);
                emptyStack.Pop();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Pop на порожньому стеку (Завд. 4) перехоплено виняток: {ex.Message}");
            }
        }
    }
}