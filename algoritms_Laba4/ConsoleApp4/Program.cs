using System;
using System.Collections.Generic;

namespace HashTable
{
    // Класс хеш-таблицы для цепочек переполнения(hash-хеш-код, chain-цепочка переполнения для конкретного хеш-кода)
    class ChainCell
    {
        public int hash;
        public List<int> chain = new List<int>(0);

        public ChainCell(int hash)
        {
            this.hash = hash;
        }
    }

    // Класс хеш-таблицы для линейного зондирования и двойного хеширования (hash-хеш-код, key-ключ конкретного хеш-кода, если равен -1, значит не заполнен)
    class Cell
    {
        public int hash;
        public int key = -1;

        public Cell(int hash)
        {
            this.hash = hash;
        }
    }

    class Program
    {
        // функция, объединяющая все ключи в один string(для цепочек переполнения)
        static string ChainToString(List<int> list)
        {
            string listStr = string.Empty;

            foreach (int key in list)
            {
                listStr += $"{key} ";
            }

            return listStr;
        }

        //хеш-функция(согласно методу умножения)
        static int HashFunction(int key, double constant, int tableSize)
        {
            double kAFractionPart = key * constant - Math.Floor(key * constant);
            return (int)(kAFractionPart * tableSize);
        }

        public static bool IsCoprime(int a, int b)
        {
            return a == b
                   ? a == 1
                   : a > b
                        ? IsCoprime(a - b, b)
                        : IsCoprime(b - a, a);
        }

        //вторая хеш-функция(для двойного хеширования)
        //Возвращает значения:
        //-не равные 0
        //-независимые от первой хеш-функции
        //-взаимно простые с величиной хеш-таблицы
        static int SecondHashFunction(int key, int tableSize)
        {
            int hashCode = (key % (tableSize - 1)) + 1;
            while(!IsCoprime(hashCode,tableSize))
                hashCode++;
            return hashCode;

        }

        // конечная функция двойного хеширования
        static int DoubledHashFunction(int key, double constant, int tableSize, int i)
        {
            
            return (HashFunction(key, constant, tableSize) + i * SecondHashFunction(key, tableSize)) % tableSize;
        }

        // Возращает максимальную длину цепочки коллизии в хеш-таблице для цепочек переполнения
        static int MaxCollision(ChainCell[] ocHashTable)
        {

            int maxCollision = 0;

            foreach (ChainCell cell in ocHashTable)
            {
                if (cell.chain.Count > maxCollision)
                {
                    maxCollision = cell.chain.Count;
                }
            }

            return maxCollision;
        }

        //Возвращает int массив размера size с рандомными значениями от 1 до keyValueRange
        static int[] GetRandomKeysArray(int size, int keyValueRange)
        {
            int[] keysArray = new int[size];
            Random rnd = new Random();
            
            for (int i = 0; i < size; i++)
            {
                keysArray[i] = rnd.Next(1, keyValueRange);
            }

            return keysArray;
        }

        static void Main(string[] args)
        {
            double knutConstant = (Math.Sqrt(5) - 1) * 0.5;
            
            int keysAmount = 15;
            int keyValueRange = 100;
            int tableSize = 10;

            int[] keysArray = GetRandomKeysArray(keysAmount, keyValueRange);
            
            // цепочки переполнения

            ChainCell[] ocHashTable = new ChainCell[tableSize];

            for (int i = 0; i < tableSize; i++)
            {
                ocHashTable[i] = new ChainCell(i);
            }

            foreach (int key in keysArray)
            {
                int hashValue = HashFunction(key, knutConstant, tableSize);

                ocHashTable[hashValue].chain.Add(key);
            }
            
            Console.WriteLine("Цепочка переполнения:");

            foreach (ChainCell cell in ocHashTable)
            {
                Console.WriteLine(cell.hash + ": " + ChainToString(cell.chain));
            }
            
            //моя константа
            double myConstant = 0.83;

            // число P в условии
            int repetitions = 1000;
            int knutConstantIsBetterCounter = 0;
            int myConstantIsBetterCounter = 0;

            for (int p = 0; p < repetitions; p++)
            {
                int[] keysArr = GetRandomKeysArray(keysAmount, keyValueRange);
                
                // максимальная коллизия с константой Кнута
                
                ChainCell[] knutConstantHashTable = new ChainCell[tableSize];

                for (int i = 0; i < tableSize; i++)
                {
                    knutConstantHashTable[i] = new ChainCell(i);
                }

                foreach (int key in keysArr)
                {
                    int hashValue = HashFunction(key, knutConstant, tableSize);

                    knutConstantHashTable[hashValue].chain.Add(key);
                }
                
                int knutConstantMaxCollision = MaxCollision(knutConstantHashTable);
                
                // Максимальная коллизия с моей константой
                
                ChainCell[] myConstantHashTable = new ChainCell[tableSize];

                for (int i = 0; i < tableSize; i++)
                {
                    myConstantHashTable[i] = new ChainCell(i);
                }

                foreach (int key in keysArr)
                {
                    int hashValue = HashFunction(key, myConstant, tableSize);

                    myConstantHashTable[hashValue].chain.Add(key);
                }
                
                int myConstantMaxCollision = MaxCollision(myConstantHashTable);
                
                // Вывод

                if (knutConstantMaxCollision <= myConstantMaxCollision)
                {
                    knutConstantIsBetterCounter++;
                }
                else
                {
                    myConstantIsBetterCounter++;
                }
            }

            Console.WriteLine($"Моя константа против константы Кнута: {myConstantIsBetterCounter} - {knutConstantIsBetterCounter}");

            keysAmount = 1000;
            keyValueRange = 1000;
            tableSize = 1550;

            keysArray = GetRandomKeysArray(keysAmount, keyValueRange);

            // линейное зондирование

            Cell[] lpHashTable = new Cell[tableSize];
            
            for (int i = 0; i < tableSize; i++)
            {
                lpHashTable[i] = new Cell(i);
            }
            
            foreach (int key in keysArray)
            {
                int hashValue = HashFunction(key, knutConstant, tableSize);

                while (lpHashTable[hashValue % tableSize].key != -1)
                {
                    hashValue++;
                }

                lpHashTable[hashValue % tableSize].key = key;
            }

            Console.WriteLine("Линейное зондирование:");
            
            foreach (Cell cell in lpHashTable)
            {
                Console.WriteLine($"{cell.hash}: " + (cell.key == -1 ? "" : $"{cell.key}"));
            }
            
            // двойное хеширование
            
            Cell[] dhHashTable = new Cell[tableSize];
            
            for (int i = 0; i < tableSize; i++)
            {
                dhHashTable[i] = new Cell(i);
            }
            
            foreach (int key in keysArray)
            {
                int i = 0;
                int hashValue = DoubledHashFunction(key, knutConstant, tableSize, i);

                while (dhHashTable[hashValue].key != -1)
                {
                    i += 1;
                    hashValue = DoubledHashFunction(key, knutConstant, tableSize, i);

                }

                dhHashTable[hashValue].key = key;
            }
            
            Console.WriteLine("Двойное хеширование:");
            
            foreach (Cell cell in dhHashTable)
            {
                Console.WriteLine($"{cell.hash}: " + (cell.key == -1 ? "" : $"{cell.key}"));
            }
        }
    }
}