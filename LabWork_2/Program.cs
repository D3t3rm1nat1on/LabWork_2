using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Schema;

namespace LabWork_2
{
    internal class Program
    {
        //Левый сын, правый брат (таблица, массив)
        public static void Main(string[] args)
        {
            int n = int.Parse(Console.ReadLine());
            MyTree myTree = new MyTree(n);
            Random random = new Random();
            var numbers = Enumerable.Range(1, n).OrderBy(i => random.Next()).ToArray();
            int count = 0;
            foreach (var number in numbers)
            {
                myTree.AddRandom(number);
                count++;
            }
            
            // myTree = new MyTree(10);
            // Console.WriteLine("root " + 5);
            // myTree.DebugAddRoot(5);
            // PrintMyTree(myTree);
            // Console.WriteLine(3);
            // myTree.Add(3);
            // PrintMyTree(myTree);
            // Console.WriteLine(10);
            // myTree.Add(10);
            // PrintMyTree(myTree);
            // Console.WriteLine(2);
            // myTree.Add(2);
            // PrintMyTree(myTree);
            // Console.WriteLine(6);
            // myTree.Add(6);
            // PrintMyTree(myTree);
            // Console.WriteLine(8);
            // myTree.Add(8);
            // PrintMyTree(myTree);
            // Console.WriteLine("root " + 4);
            // myTree.DebugAddRoot(4);
            // PrintMyTree(myTree);
            // Console.WriteLine("root " + 9);
            // myTree.DebugAddRoot(9);
            // PrintMyTree(myTree);
            // Console.WriteLine("root " + 1);
            // myTree.DebugAddRoot(1);
            // PrintMyTree(myTree);
            // Console.WriteLine("root " + 7);
            // myTree.DebugAddRoot(7);
            // PrintMyTree(myTree);
            
            Console.WriteLine($"Дерево А --- {myTree.treeSize}");
            //Console.WriteLine(string.Join(" ", numbers));
            PrintMyTree(myTree);
            PreOrder(myTree);
            InOrder(myTree);
            
            
            MyTree deleteTree = new MyTree(n/2);
            numbers = numbers.OrderBy(i => random.Next()).TakeWhile((_, idx) => idx < numbers.Length/2).ToArray();
            count = 0;
            foreach (var number in numbers)
            {
                deleteTree.AddRandom(number);
                count++;
            }

            Console.WriteLine($"Дерево B --- {count}");
            //Console.WriteLine(string.Join(" ", numbers));
            PrintMyTree(deleteTree);
            PreOrder(deleteTree);
            InOrder(deleteTree);
            
            
            myTree.Deletion(deleteTree);
            Console.WriteLine("В итоге осталось:------------------------------------");
            PrintMyTree(myTree);
            PreOrder(myTree);
            InOrder(myTree);
            Console.ReadLine();
        }


        public class MyTree
        {
            public int[,] table; // leftmost_child, key, right_sibling ---- idx = pointer ---- 0 = null
            public int addIdx; // индекс массива, в который будет добавлен новый элемент
            public int topIdx; // индекс корня дерева
            public int treeSize;

            public MyTree(int n)
            {
                addIdx = 1;
                treeSize = n;
                table = new int[n + 1, 3];
            }

            public void Add(int value)
            {
                AddOnIdx(topIdx, value);
                addIdx++;

                void AddOnIdx(int index, int key)
                {
                    // если дерево пустое
                    if (addIdx == 1)
                    {
                        table[addIdx, 1] = key;
                        topIdx = 1;
                        return;
                    }

                    // проверка на входной элемент
                    if (table[index, 1] == key)
                    {
                        throw new Exception("бинарное дерево уже содержит такой элемент");
                    }

                    // если элемент является листом
                    if (table[index, 0] == 0)
                    {
                        table[index, 0] = addIdx;
                        table[addIdx, 1] = key;
                        return;
                    }

                    // если вставляемый элемент меньше
                    if (table[index, 1] > key)
                    {
                        int childIdx = table[index, 0];
                        //если ребенок меньше родителя
                        if (table[childIdx, 1] < table[index, 1])
                        {
                            AddOnIdx(childIdx, key);
                        }
                        else // если ребенок больше родителя
                        {
                            table[index, 0] = addIdx;
                            table[addIdx, 2] = childIdx;
                            table[addIdx, 1] = key;
                        }
                    }

                    // если вставляемый элемент больше
                    if (table[index, 1] < key)
                    {
                        int childIdx = table[index, 0];
                        // если ребенок меньше родителя
                        if (table[childIdx, 1] < table[index, 1])
                        {
                            int broIdx = table[childIdx, 2];
                            // если есть брат
                            if (broIdx != 0)
                                AddOnIdx(broIdx, key);
                            else // если справа пусто
                            {
                                table[childIdx, 2] = addIdx;
                                table[addIdx, 1] = key;
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

            public void AddRandom(int value)
            {
                Add(value);
                int random = new Random().Next(1, addIdx) % (addIdx - 1);
                if (random == 0)
                    InsertRoot(addIdx - 1, topIdx);
            }
            
            // public void DebugAddRoot(int value)
            // {
            //     Add(value);
            //     int random = new Random().Next(1, addIdx) % (addIdx - 1);
            //     if (true)
            //         InsertRoot(addIdx - 1, topIdx);
            // }

            public bool Contains(int value)
            {
                int index = topIdx;
                bool result = false;
                Search();
                return result;

                void Search()
                {
                    int childIdx = table[index, 0];
                    int broIdx = table[childIdx, 2];
                    if (broIdx == index || childIdx == index)
                        return;

                    if (value == table[index, 1])
                    {
                        result = true;
                        return;
                    }
                    
                    if (table[index, 1] < value && broIdx != 0)
                        index = broIdx;
                    else
                        index = childIdx;

                    Search();
                }
            }

            public void Deletion(MyTree tree)
            {
                int[] deleteNumbers = new int[treeSize + 1];
                for (int i = 1; i <= treeSize; i++)
                {
                    deleteNumbers[i] = table[i, 1];
                }
                for (int i = 1; i <= treeSize; i++)
                {
                    int value = deleteNumbers[i];
                    if (!tree.Contains(value))
                    {
                        Delete(value);
                    }
                }

                void Delete(int value)
                {
                    int deletingIdx = GetIdx(value);
                    int parentIdx = GetParentIdx(deletingIdx);
                    int leftChildIdx = table[deletingIdx, 0];
                    int rightBroIdx = table[leftChildIdx, 2];

                    // если удаляемая вершина является листом
                    if (leftChildIdx == 0)
                    {
                        // если удаляемая вершина слева от родителя
                        if (table[parentIdx, 0] == deletingIdx)
                        {
                            table[parentIdx, 0] = 0;
                            // если остался правый брат
                            int deletingBroIdx = table[deletingIdx, 2];
                            if (deletingBroIdx != 0)
                            {
                                table[parentIdx, 0] = deletingBroIdx;
                            }
                        }
                        else // если удаляемая вершина справа от родителя
                        {
                            int parentLeftChildIdx = table[parentIdx, 0];
                            table[parentLeftChildIdx, 2] = 0;
                        }

                        table[deletingIdx, 0] = 0;
                        table[deletingIdx, 1] = 0;
                        table[deletingIdx, 2] = 0;
                        return;
                    }

                    // если у вершины один дочерний узел
                    if (rightBroIdx == 0)
                    {
                        // если удаляемая вершина младшая или единственная у родителя
                        if (table[parentIdx, 0] == deletingIdx || parentIdx == 0)
                        {
                            if (parentIdx == 0)
                                topIdx = leftChildIdx;
                            else
                                table[parentIdx, 0] = leftChildIdx;
                            table[leftChildIdx, 2] = table[deletingIdx, 2];
                        }
                        else // если удаляемая вершина старшая и не единственная у родителя
                        {
                            int parentsChildIdx = table[parentIdx, 0];
                            table[parentsChildIdx, 2] = leftChildIdx;
                        }

                        table[deletingIdx, 0] = 0;
                        table[deletingIdx, 1] = 0;
                        table[deletingIdx, 2] = 0;
                        return;
                    }

                    // если у вершины два дочерних узла
                    int nextIdx = FindNext(deletingIdx);
                    int nextValue = table[nextIdx, 1];
                    Delete(nextValue);
                    table[deletingIdx, 1] = nextValue;

                }

                int FindNext(int startIdx)
                {
                    int leftChildIdx = table[startIdx, 0];
                    int rightChildIdx = table[leftChildIdx, 2];
                    // если левый сын больше родителя (то он правый)
                    if (table[leftChildIdx, 1] > table[startIdx, 1])
                        rightChildIdx = leftChildIdx;

                    // находим минимальное справа
                    int minIdx = table[rightChildIdx, 0];
                    while (table[minIdx, 1] < table[rightChildIdx, 1] && minIdx != 0)
                    {
                        rightChildIdx = minIdx;
                        minIdx = table[rightChildIdx, 0];
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

            public int GetIdx(int value)
            {
                int checkingIdx = topIdx;
                if (value == table[topIdx, 1])
                    return topIdx;
                Check();
                return checkingIdx;

                void Check()
                {
                    int childIdx = table[checkingIdx, 0];
                    int broIdx = table[childIdx, 2];
                    int checkingValue = table[checkingIdx, 1];

                    if (value == checkingValue)
                        return;

                    if (value < table[checkingIdx, 1] || table[childIdx, 2] == 0)
                        checkingIdx = childIdx;
                    else
                        checkingIdx = broIdx;

                    if (childIdx == 0 && broIdx == 0)
                    {
                        return;
                    }

                    Check();
                }
            }

            private int GetParentIdx(int myIdx)
            {
                int checkingIdx = topIdx;
                int myValue = table[myIdx, 1];
                if (myIdx == topIdx)
                    return 0;
                CheckParent();
                return checkingIdx;

                void CheckParent()
                {
                    int childIdx = table[checkingIdx, 0];
                    int broIdx = table[childIdx, 2];
                    if (broIdx == myIdx || childIdx == myIdx)
                        return;

                    if (table[checkingIdx, 1] < myValue && broIdx != 0)
                        checkingIdx = broIdx;
                    else
                        checkingIdx = childIdx;

                    CheckParent();
                }
            }

            private void InsertRoot(int myIdx, int upperIdx)
            {
                //PrintMyTree(this, 0, 10);
                while (myIdx != upperIdx)
                {
                    int parentIdx;
                    if (table[upperIdx, 0] == myIdx || table[upperIdx, 2] == myIdx)
                        parentIdx = upperIdx;
                    else
                        parentIdx = GetParentIdx(myIdx);
                    if (table[parentIdx, 1] > table[myIdx, 1])
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
                
                int[] Q = {table[myIdx, 0], table[myIdx, 1], table[myIdx, 2]};
                int[] P = {table[parentIdx, 0], table[parentIdx, 1], table[parentIdx, 2]};

                int A_idx = P[0];
                int B_idx = Q[0];
                int C_idx = table[B_idx, 2];

                // проверяем A
                if (table[A_idx, 2] == 0)
                {
                    A_idx = 0;
                }

                // если B справа, то меняем индексы B и C
                if (table[B_idx, 1] > Q[1])
                {
                    C_idx = C_idx + B_idx;
                    B_idx = C_idx - B_idx;
                    C_idx = C_idx - B_idx;
                }
                else
                {
                    table[B_idx, 2] = 0;
                }

                Q[0] = myIdx;
                Q[2] = P[2];
                P[2] = C_idx;
                
                if (A_idx == 0)
                {
                    P[0] = B_idx;
                }
                else
                {
                    table[A_idx, 2] = B_idx;
                }

                for (int i = 0; i < 3; i++)
                {
                    table[myIdx, i] = P[i];
                    table[parentIdx, i] = Q[i];
                }
            }

            private void RotateRight(int myIdx, int parentIdx)
            {
                //Console.WriteLine("RotateRight");

                int[] P = {table[myIdx, 0], table[myIdx, 1], table[myIdx, 2]};
                int[] Q = {table[parentIdx, 0], table[parentIdx, 1], table[parentIdx, 2]};

                int A_idx = P[0];
                int B_idx = table[A_idx, 2];
                int C_idx = table[myIdx, 2];
                // если А справа, то меняем индексы А и В
                if (table[A_idx, 1] > P[1])
                {
                    A_idx = A_idx + B_idx;
                    B_idx = A_idx - B_idx;
                    A_idx = A_idx - B_idx;
                }

                // указываем на Q (изменится потом)
                if (A_idx == 0)
                    P[0] = myIdx;
                else
                    table[A_idx, 2] = myIdx;

                // указываем на B и C
                if (B_idx == 0)
                    Q[0] = C_idx;
                else
                {
                    Q[0] = B_idx;
                    table[B_idx, 2] = C_idx;
                }

                P[2] = Q[2];
                Q[2] = 0;
                for (int i = 0; i < 3; i++)
                {
                    table[myIdx, i] = Q[i];
                    table[parentIdx, i] = P[i];
                }
            }
        }


        /// <summary>
        /// прямой обход бинарного дерева
        /// </summary>
        /// <param name="tree"></param>
        public static void PreOrder(MyTree tree)
        {
            // Проверяем, не является ли текущий узел пустым или null.
            // Показываем поле данных корня (или текущего узла).
            // Обходим левое поддерево рекурсивно, вызвав функцию прямого обхода.
            // Обходим правое поддерево рекурсивно, вызвав функцию прямого обхода.

            List<int> output = new List<int>();
            Search(tree.topIdx);
            Console.WriteLine("Прямой обход");
            Console.WriteLine(string.Join(", ", output));

            void Search(int index)
            {
                if (index == 0)
                    return;
                output.Add(tree.table[index, 1]);
                Search(tree.table[index, 0]);
                Search(tree.table[index, 2]);
            }
        }

        /// <summary>
        /// симметричный (центрированный) обход бинарного дерева
        /// </summary>
        /// <param name="tree"></param>
        public static void InOrder(MyTree tree)
        {
            // Проверяем, не является ли текущий узел пустым или null.
            // Обходим левое поддерево рекурсивно, вызвав функцию центрированного обхода.
            // Показываем поле данных корня (или текущего узла).
            // Обходим правое поддерево рекурсивно, вызвав функцию центрированного обхода.

            List<int> output = new List<int>();
            Search(tree.topIdx);
            Console.WriteLine("Симметричный обход");
            Console.WriteLine(string.Join(", ", output));

            void Search(int index)
            {
                if (index == 0)
                    return;
                int leftIdx = tree.table[index, 0];
                int rightIdx = tree.table[leftIdx, 2];if (tree.table[leftIdx, 1] > tree.table[index, 1])
                {
                    leftIdx=leftIdx^rightIdx;
                    rightIdx=leftIdx^rightIdx;
                    leftIdx=leftIdx^rightIdx;
                }
                Search(leftIdx);
                output.Add(tree.table[index, 1]);
                Search(rightIdx);
            }
        }

        public class BNode
        {
            public int item;
            public BNode right;
            public BNode left;

            public BNode(int item)
            {
                this.item = item;
            }
        }
        public static void PrintMyTree(MyTree myTree, int topMargin = 0, int leftMargin = 2)
        {
            BNode firstNode = new BNode(0);
            // Console.WriteLine($"Top = {myTree.topIdx}");
            // for (int i = 0; i <= myTree.treeSize; i++)
            // {
            //     Console.WriteLine($"{i,2}) {myTree.table[i, 0],2} {myTree.table[i, 1],2} {myTree.table[i, 2],2}");
            // }

            Add(firstNode, myTree.topIdx);

            TreePrinter.Print(firstNode, topMargin, leftMargin);

            void Add(BNode node, int index)
            {
                if (myTree.table[index, 1] != 0)
                {
                    node.item = myTree.table[index, 1];
                    // если есть сын
                    if (myTree.table[index, 0] != 0)
                    {
                        int childIdx = myTree.table[index, 0];
                        // если сын меньше ключа
                        if (myTree.table[childIdx, 1] < node.item)
                        {
                            node.left = new BNode(0);
                            Add(node.left, childIdx);
                            int broIdx = myTree.table[childIdx, 2];
                            // если у сына есть брат
                            if (broIdx != 0)
                            {
                                node.right = new BNode(0);
                                Add(node.right, broIdx);
                            }
                        }
                        // если сын больше ключа (значит сын у нас один)
                        else
                        {
                            node.right = new BNode(0);
                            Add(node.right, childIdx);
                        }
                    }
                }
                // else
                // {
                //     firstNode = null;
                // }

            }

        }
    }
}