using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabWork_2
{
    internal class Program
    {
        //Левый сын, правый брат (таблица, массив)
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            int n = int.TryParse(Console.ReadLine(), out int temp) ? temp : 10;
            Console.WriteLine($"N = {n}");

            MyTree myTree = new MyTree(n);
            Random random = new Random();
            var numbers = Enumerable.Range(1, n).OrderBy(i => random.Next()).ToArray();
            foreach (var number in numbers)
                myTree.AddRandom(number);

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Дерево А");
            Console.ForegroundColor = ConsoleColor.Gray;
            TreePrinter.PrintMyTree(myTree);

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            PreOrder(myTree);
            InOrder(myTree);
            Console.ForegroundColor = ConsoleColor.Gray;

            MyTree deleteTree = new MyTree(n / 2);
            numbers = numbers.OrderBy(i => random.Next()).TakeWhile((_, idx) => idx < numbers.Length / 2).ToArray();
            foreach (var number in numbers)
                deleteTree.AddRandom(number);

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Дерево B");
            Console.ForegroundColor = ConsoleColor.Gray;
            TreePrinter.PrintMyTree(deleteTree);

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            PreOrder(deleteTree);
            InOrder(deleteTree);
            Console.ForegroundColor = ConsoleColor.Gray;

            myTree.Deletion(deleteTree);
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("А = A ⋂ B");
            Console.ForegroundColor = ConsoleColor.Gray;
            TreePrinter.PrintMyTree(myTree);
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            PreOrder(myTree);
            InOrder(myTree);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ReadLine();
        }

        public class MyTree
        {
            public readonly int[,] Table; // leftmost_child, key, right_sibling ---- idx = pointer ---- 0 = null
            private int _addIdx; // индекс массива, в который будет добавлен новый элемент
            public int TopIdx; // индекс корня дерева
            private readonly int _treeSize;

            public MyTree(int n)
            {
                _addIdx = 1;
                _treeSize = n;
                Table = new int[n + 1, 3];
            }

            // обычная вставка
            public void Add(int value)
            {
                AddOnIdx(TopIdx, value);
                _addIdx++;

                void AddOnIdx(int index, int key)
                {
                    // если дерево пустое
                    if (_addIdx == 1)
                    {
                        Table[_addIdx, 1] = key;
                        TopIdx = 1;
                        return;
                    }

                    // проверка на входной элемент
                    if (Table[index, 1] == key)
                    {
                        throw new Exception("бинарное дерево уже содержит такой элемент");
                    }

                    // если элемент является листом
                    if (Table[index, 0] == 0)
                    {
                        Table[index, 0] = _addIdx;
                        Table[_addIdx, 1] = key;
                        return;
                    }

                    // если вставляемый элемент меньше
                    if (Table[index, 1] > key)
                    {
                        int childIdx = Table[index, 0];
                        //если ребенок меньше родителя
                        if (Table[childIdx, 1] < Table[index, 1])
                        {
                            AddOnIdx(childIdx, key);
                        }
                        else // если ребенок больше родителя
                        {
                            Table[index, 0] = _addIdx;
                            Table[_addIdx, 2] = childIdx;
                            Table[_addIdx, 1] = key;
                        }
                    }

                    // если вставляемый элемент больше
                    if (Table[index, 1] < key)
                    {
                        int childIdx = Table[index, 0];
                        // если ребенок меньше родителя
                        if (Table[childIdx, 1] < Table[index, 1])
                        {
                            int broIdx = Table[childIdx, 2];
                            // если есть брат
                            if (broIdx != 0)
                                AddOnIdx(broIdx, key);
                            else // если справа пусто
                            {
                                Table[childIdx, 2] = _addIdx;
                                Table[_addIdx, 1] = key;
                            }
                        }
                        else // если ребенок больше родителя
                        {
                            AddOnIdx(childIdx, key);
                            // table[index, 0] = addIdx;
                            // table[addIdx, 2] = childIdx;
                            // table[addIdx, 1] = key;
                        }
                    }

                }
            }

            // рандомизированная вставка
            public void AddRandom(int value)
            {
                Add(value);
                int random = new Random().Next(1, _addIdx) % (_addIdx - 1);
                if (random == 0)
                    InsertRoot(_addIdx - 1, TopIdx);
            }

            private bool Contains(int value) //1+1+14log_2(n)+1 = 3 + 14log_2(n)
            {
                int index = TopIdx; //1
                bool result = false; //1
                Search(); //14log_2(n)
                return result; //1

                void Search() // (1+1+3+1+1+1+1+3+1+1)log_2(n) = 14log_2(n)
                {
                    int childIdx = Table[index, 0]; // 1
                    int broIdx = Table[childIdx, 2]; // 1
                    if (broIdx == index || childIdx == index) // 3
                        return; // 1

                    if (value == Table[index, 1]) // 1
                    {
                        result = true; // 1
                        return; // 1
                    }

                    if (Table[index, 1] < value && broIdx != 0) // 3
                        index = broIdx; // 1
                    else
                        index = childIdx; // 1

                    Search();
                }
            }

            public void Deletion(MyTree tree) //  1 + 28n + 38n(log_2(n)) 
            {
                for (int i = 1;
                    i <= _treeSize;
                    i++) //1+n+n(1+(3+14log_2(n))+(24+24log_2(n))) = 1 + 28n + 38n(log_2(n)) 
                {
                    int value = Table[i, 1]; // 1
                    if (!tree.Contains(value)) //3 + 14log_2(n)
                    {
                        Delete(value); // 24 + 24log_2(n)
                    }
                }

                void Delete(int value) //(5+14log_2(n))+(5+10log_2(n))+1+1+1+1+1+1+1+1+1+1+1+1+1+1 = 24 + 24log_2(n)
                {
                    int deletingIdx = GetIdx(value); // 5 + 14log_2(n)
                    int parentIdx = GetParentIdx(deletingIdx); // 5 + 10log_2(n)
                    int leftChildIdx = Table[deletingIdx, 0]; // 1
                    int rightBroIdx = Table[leftChildIdx, 2]; // 1

                    // если удаляемая вершина является листом
                    if (leftChildIdx == 0) // 1
                    {
                        // если удаляемая вершина слева от родителя
                        if (Table[parentIdx, 0] == deletingIdx) // 1
                        {
                            Table[parentIdx, 0] = 0; // 1
                            // если остался правый брат
                            int deletingBroIdx = Table[deletingIdx, 2]; // 1
                            if (deletingBroIdx != 0) // 1
                            {
                                Table[parentIdx, 0] = deletingBroIdx; // 1
                            }
                        }
                        else // если удаляемая вершина справа от родителя
                        {
                            int parentLeftChildIdx = Table[parentIdx, 0]; // 1
                            Table[parentLeftChildIdx, 2] = 0; // 1
                        }

                        Table[deletingIdx, 0] = 0; // 1
                        Table[deletingIdx, 1] = 0; // 1
                        Table[deletingIdx, 2] = 0; // 1
                        return; // 1
                    }

                    // если у вершины один дочерний узел
                    if (rightBroIdx == 0)
                    {
                        // если удаляемая вершина младшая или единственная у родителя
                        if (Table[parentIdx, 0] == deletingIdx || parentIdx == 0)
                        {
                            if (parentIdx == 0)
                                TopIdx = leftChildIdx;
                            else
                                Table[parentIdx, 0] = leftChildIdx;
                            Table[leftChildIdx, 2] = Table[deletingIdx, 2];
                        }
                        else // если удаляемая вершина старшая и не единственная у родителя
                        {
                            int parentsChildIdx = Table[parentIdx, 0];
                            Table[parentsChildIdx, 2] = leftChildIdx;
                        }

                        Table[deletingIdx, 0] = 0;
                        Table[deletingIdx, 1] = 0;
                        Table[deletingIdx, 2] = 0;
                        return;
                    }

                    // если у вершины два дочерних узла
                    int nextIdx = FindNext(deletingIdx);
                    int nextValue = Table[nextIdx, 1];
                    Delete(nextValue);
                    Table[deletingIdx, 1] = nextValue;

                }

                int FindNext(int startIdx)
                {
                    int leftChildIdx = Table[startIdx, 0];
                    int rightChildIdx = Table[leftChildIdx, 2];
                    // если левый сын больше родителя (то он правый)
                    if (Table[leftChildIdx, 1] > Table[startIdx, 1])
                        rightChildIdx = leftChildIdx;

                    // находим минимальное справа
                    int minIdx = Table[rightChildIdx, 0];
                    while (Table[minIdx, 1] < Table[rightChildIdx, 1] && minIdx != 0)
                    {
                        rightChildIdx = minIdx;
                        minIdx = Table[rightChildIdx, 0];
                    }

                    return rightChildIdx;
                }
            }

//             public void Deletion(MyTree tree)
//             {
//                 Search(tree.topIdx);
//
//                 void Search(int value)
//                 {
//                     if (value == 0)
//                         return;
//                     if (!tree.Contains(table[value, 1]))
//                     {
//                         Delete(value);
//                         Console.WriteLine($"Удаление --- {table[value, 1]}");
//                         PrintMyTree(this);
//                     }
//                     Search(table[value, 0]);
//                     Search(table[value, 2]);
//                 }
//
//                 void Delete(int index)
//                 {
// //                     int parentIdx = GetParentIdx(index);
// //                     int childIdx = table[index, 0];
// //                     // если это лист
// //                     if (childIdx == 0)
// //                     {
// //                         // если лист слева
// //                         if (table[parentIdx, 1] > table[index, 1] || table[index, 2] == 0)
// //                             table[parentIdx, 0] = table[index, 2];
// //                         else
// //                             table[parentIdx, 2] = 0;
// //                         return;
// //                     }
// //
// //                     // один дочерний узел
// //                     if (table[childIdx, 2] == 0)
// //                     {
// //                         // если узел слева или единственный
// //                         if (table[parentIdx, 1] > table[index, 1] || table[index, 2] == 0)
// //                         {
// //                             table[parentIdx, 0] = childIdx;
// //                         }
// //                         else
// //                             table[parentIdx, 2] = childIdx;
// //
// //                         table[childIdx, 2] = table[index, 2];
// //                         return;
// //                     }
// //
// // ////////////////////////////////////////////////////////
// //                     // два дочерних узла
// //                     int changeIdx = FindNext(index);
// //                     int clearIdx = GetParentIdx(changeIdx);
// //                     if (table[changeIdx, 2] == 0)
// //                     {
// //                         if (clearIdx == index)
// //                             table[table[clearIdx, 0], 2] = 0;
// //                         else
// //                             table[clearIdx, 0] = 0;
// //                     }
// //                     else
// //                         table[clearIdx, 0] = table[changeIdx, 2];
// //                     table[index, 1] = table[changeIdx, 1];
//                 }
//
//                 // Если у узла есть правое поддерево, то следующий за ним элемент будет минимальным элементом в этом поддереве.
//                 int FindNext(int index)
//                 {
//                     int leftChildIdx = table[index, 0];
//                     int rightChildIdx = table[leftChildIdx, 2];
//                     // если левый сын больше родителя (то он правый)
//                     if (table[leftChildIdx, 1] > table[index, 1])
//                         rightChildIdx = leftChildIdx;
//
//                     // находим минимальное справа
//                     int minIdx = table[rightChildIdx, 0];
//                     while (table[minIdx, 1] < table[rightChildIdx, 1] && minIdx != 0)
//                     {
//                         rightChildIdx = minIdx;
//                         minIdx = table[rightChildIdx, 0];
//                     }
//
//                     return rightChildIdx;
//
//                 }
//             }

            private int GetIdx(int value) //1+2+1+14log_2(n)+1 = 5 + 14log_2(n)
            {
                int checkingIdx = TopIdx; // 1
                if (value == Table[TopIdx, 1]) // 2
                    return TopIdx; // 1
                Check(); //14log_2(n)
                return checkingIdx; // 1

                void Check() // (1+1+1+1+1+3+1+1+3+1)log_2(n) = 14log_2(n)
                {
                    int childIdx = Table[checkingIdx, 0]; // 1
                    int broIdx = Table[childIdx, 2]; // 1
                    int checkingValue = Table[checkingIdx, 1]; // 1

                    if (value == checkingValue) // 1
                        return; // 1

                    if (value < Table[checkingIdx, 1] || Table[childIdx, 2] == 0) // 3
                        checkingIdx = childIdx; // 1
                    else
                        checkingIdx = broIdx; // 1

                    if (childIdx == 0 && broIdx == 0) // 3
                    {
                        return; // 1
                    }

                    Check();
                }
            }

            private int GetParentIdx(int myIdx) // 1+1+1+1+10log_2(n)+1 = 5 + 10log_2(n)
            {
                int checkingIdx = TopIdx; // 1
                int myValue = Table[myIdx, 1]; // 1
                if (myIdx == TopIdx) // 1
                    return 0; // 1
                CheckParent(); // 10log_2(n)
                return checkingIdx; // 1

                void CheckParent() //(1+1+3+1+3+1+1)log_2(n) = 10log_2(n)
                {
                    int childIdx = Table[checkingIdx, 0]; // 1
                    int broIdx = Table[childIdx, 2]; // 1
                    if (broIdx == myIdx || childIdx == myIdx) // 3
                        return; // 1

                    if (Table[checkingIdx, 1] < myValue && broIdx != 0) // 3
                        checkingIdx = broIdx; // 1
                    else
                        checkingIdx = childIdx; // 1

                    CheckParent();
                }
            }

            private void InsertRoot(int myIdx, int upperIdx)
            {
                //PrintMyTree(this, 0, 10);
                while (myIdx != upperIdx)
                {
                    int parentIdx;
                    if (Table[upperIdx, 0] == myIdx || Table[upperIdx, 2] == myIdx)
                        parentIdx = upperIdx;
                    else
                        parentIdx = GetParentIdx(myIdx);
                    if (Table[parentIdx, 1] > Table[myIdx, 1])
                        RotateRight(myIdx, parentIdx);
                    else
                        RotateLeft(myIdx, parentIdx);
                    myIdx = parentIdx;

                    //PrintMyTree(this, 0, 10);
                }
            }

            private void RotateLeft(int myIdx, int parentIdx)
            {
                //Console.WriteLine("RotateLeft");

                int[] q = {Table[myIdx, 0], Table[myIdx, 1], Table[myIdx, 2]};
                int[] p = {Table[parentIdx, 0], Table[parentIdx, 1], Table[parentIdx, 2]};

                int aIdx = p[0];
                int bIdx = q[0];
                int cIdx = Table[bIdx, 2];

                // проверяем A
                if (Table[aIdx, 2] == 0)
                {
                    aIdx = 0;
                }

                // если B справа, то меняем индексы B и C
                if (Table[bIdx, 1] > q[1])
                {
                    cIdx = cIdx + bIdx;
                    bIdx = cIdx - bIdx;
                    cIdx = cIdx - bIdx;
                }
                else
                {
                    Table[bIdx, 2] = 0;
                }

                q[0] = myIdx;
                q[2] = p[2];
                p[2] = cIdx;

                if (aIdx == 0)
                {
                    p[0] = bIdx;
                }
                else
                {
                    Table[aIdx, 2] = bIdx;
                }

                for (int i = 0; i < 3; i++)
                {
                    Table[myIdx, i] = p[i];
                    Table[parentIdx, i] = q[i];
                }
            }

            private void RotateRight(int myIdx, int parentIdx)
            {
                //Console.WriteLine("RotateRight");

                int[] p = {Table[myIdx, 0], Table[myIdx, 1], Table[myIdx, 2]};
                int[] q = {Table[parentIdx, 0], Table[parentIdx, 1], Table[parentIdx, 2]};

                int aIdx = p[0];
                int bIdx = Table[aIdx, 2];
                int cIdx = Table[myIdx, 2];
                // если А справа, то меняем индексы А и В
                if (Table[aIdx, 1] > p[1])
                {
                    aIdx = aIdx + bIdx;
                    bIdx = aIdx - bIdx;
                    aIdx = aIdx - bIdx;
                }

                // указываем на Q (изменится потом)
                if (aIdx == 0)
                    p[0] = myIdx;
                else
                    Table[aIdx, 2] = myIdx;

                // указываем на B и C
                if (bIdx == 0)
                    q[0] = cIdx;
                else
                {
                    q[0] = bIdx;
                    Table[bIdx, 2] = cIdx;
                }

                p[2] = q[2];
                q[2] = 0;
                for (int i = 0; i < 3; i++)
                {
                    Table[myIdx, i] = q[i];
                    Table[parentIdx, i] = p[i];
                }
            }
        }

        /// <summary>
        /// прямой обход бинарного дерева
        /// </summary>
        /// <param name="tree"></param>
        private static void PreOrder(MyTree tree)
        {
            // Проверяем, не является ли текущий узел пустым или null.
            // Показываем поле данных корня (или текущего узла).
            // Обходим левое поддерево рекурсивно, вызвав функцию прямого обхода.
            // Обходим правое поддерево рекурсивно, вызвав функцию прямого обхода.

            List<int> output = new List<int>();
            Search(tree.TopIdx);
            Console.WriteLine("Прямой обход");
            Console.WriteLine(string.Join(", ", output));

            void Search(int index)
            {
                if (index == 0)
                    return;
                output.Add(tree.Table[index, 1]);
                Search(tree.Table[index, 0]);
                Search(tree.Table[index, 2]);
            }
        }

        /// <summary>
        /// симметричный (центрированный) обход бинарного дерева
        /// </summary>
        /// <param name="tree"></param>
        private static void InOrder(MyTree tree)
        {
            // Проверяем, не является ли текущий узел пустым или null.
            // Обходим левое поддерево рекурсивно, вызвав функцию центрированного обхода.
            // Показываем поле данных корня (или текущего узла).
            // Обходим правое поддерево рекурсивно, вызвав функцию центрированного обхода.

            List<int> output = new List<int>();
            Search(tree.TopIdx);
            Console.WriteLine("Симметричный обход");
            Console.WriteLine(string.Join(", ", output));

            void Search(int index)
            {
                if (index == 0)
                    return;
                int leftIdx = tree.Table[index, 0];
                int rightIdx = tree.Table[leftIdx, 2];
                if (tree.Table[leftIdx, 1] > tree.Table[index, 1])
                {
                    leftIdx = leftIdx ^ rightIdx;
                    rightIdx = leftIdx ^ rightIdx;
                    leftIdx = leftIdx ^ rightIdx;
                }

                Search(leftIdx);
                output.Add(tree.Table[index, 1]);
                Search(rightIdx);
            }
        }

    }
}