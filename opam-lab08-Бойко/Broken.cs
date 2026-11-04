using System;

namespace opam_lab08_Бойко
{
    public static class BrokenSearch
    {
        // Поломка 1: while (lo < hi) замість <=
        public static int Broken1_Condition<T>(T[] a, T target, Func<T, T, int> cmp, out int comparisons)
        {
            comparisons = 0;
            if (a == null || a.Length == 0) return -1;

            int lo = 0;
            int hi = a.Length - 1;

            while (lo < hi)
            {
                int mid = lo + (hi - lo) / 2;
                comparisons++;
                int res = cmp(a[mid], target);

                if (res == 0) return mid;
                if (res < 0) lo = mid + 1;
                else hi = mid - 1;
            }

            return -1;
        }

        // Поломка 2: lo = mid замість mid + 1 (із захистом від нескінченного циклу)
        public static int Broken2_NoIncrement<T>(T[] a, T target, Func<T, T, int> cmp, out int comparisons)
        {
            comparisons = 0;
            if (a == null || a.Length == 0) return -1;

            int lo = 0;
            int hi = a.Length - 1;
            int stepGuard = 0;

            while (lo <= hi)
            {
                stepGuard++;
                if (stepGuard > 1000) break; // Переривання для запобігання зависанню

                int mid = lo + (hi - lo) / 2;
                comparisons++;
                int res = cmp(a[mid], target);

                if (res == 0) return mid;
                if (res < 0) lo = mid; // Поломка!
                else hi = mid - 1;
            }

            return -1;
        }

        // Поломка 3: mid = (lo + hi) / 2 (переповнення int)
        public static int Broken3_Overflow<T>(T[] a, T target, Func<T, T, int> cmp, out int comparisons)
        {
            comparisons = 0;
            if (a == null || a.Length == 0) return -1;

            int lo = 0;
            int hi = a.Length - 1;

            while (lo <= hi)
            {
                int mid = (lo + hi) / 2; // Поломка виникає при lo + hi > int.MaxValue
                comparisons++;
                int res = cmp(a[mid], target);

                if (res == 0) return mid;
                if (res < 0) lo = mid + 1;
                else hi = mid - 1;
            }

            return -1;
        }

        // Поломка 4: Пошук на невідсортованому масиві
        public static int TestUnsortedArray<T>(T[] unsortedArray, T[] sampleTargets, Func<T, T, int> cmp)
        {
            int failedCount = 0;
            foreach (var target in sampleTargets)
            {
                int idx = Search.BinarySearch(unsortedArray, target, cmp, out _);
                if (idx == -1) failedCount++;
            }
            return failedCount;
        }
    }
}