using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace LabWork_2
{
    internal class Program
    {
        //Левый сын, правый брат (таблица, массив)
        public static void Main(string[] args)
        {
            /*int[] array = Enumerable.Range(1, 10).ToArray();
            int maxElement = array.Max() + 1;
            int idx = array.Length / 2;
            int[,] cellspace = new int[array.Length, 3]; // leftmost_child_idx, key, right_sibling_idx
            int startIdx = 0;*/

            Node node = new Node(5);
            node = insertRand(node, 1);
            node = insertRand(node, 2);
            node = insertRand(node, 3);
            node = insertRand(node, 4);
            node = insertRand(node, 6);
            node = insertRand(node, 7);
            node = insertRand(node, 8);
            node = insertRand(node, 9);
            node = insertroot(node, 2);
            Console.WriteLine();

            MyTree myTree = new MyTree(10);
            PrintMyTree(myTree);
            myTree.Add(5);
            myTree.Add(2);
            myTree.Add(3);
            myTree.Add(1);
            myTree.Add(4);
            myTree.Add(7);
            myTree.Add(6);
            myTree.Add(10);
            myTree.Add(9);
            myTree.Add(8);
            PrintMyTree(myTree,5);
        }

        public static void PrintMyTree(MyTree myTree,  int topMargin = 2, int leftMargin = 2)
        {
            BNode firstNode = new BNode(0);
            Add(firstNode, myTree.topIdx);
            TreePrinter.Print(firstNode);

            void Add(BNode node, int index)
            {
                if (myTree.table[index, 1] != 0)
                {
                    node.item = (myTree.table[index, 1]);
                    if (myTree.table[index, 0] != 0)
                    {
                        node.left = new BNode(0);
                        Add(node.left, myTree.table[index, 0]);
                    }
                    if(myTree.table[index, 2] != 0)
                    {
                        node.right = new BNode(0);
                        Add(node.right, myTree.table[index, 2]);
                    }
                }
                else
                {
                    firstNode = null;
                }
                    
            }
            
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

            public void Add(int key)
            {
                AddOnIdx(topIdx, key);
                addIdx++;
            }

            private void AddOnIdx(int index, int key)
            {
                if (addIdx == 1)
                {
                    table[addIdx, 1] = key;
                    topIdx = 1;
                    return;
                }

                if (table[index, 1] < key)
                {
                    if (table[index, 2] == 0)
                    {
                        table[index, 2] = addIdx;
                        table[addIdx, 1] = key;
                    }
                    else
                        AddOnIdx(table[index, 2], key);
                }

                if (table[index, 1] > key)
                {
                    if (table[index, 0] == 0)
                    {
                        table[index, 0] = addIdx;
                        table[addIdx, 1] = key;
                    }
                    else
                        AddOnIdx(table[index, 0], key);
                }

            }

            // void AddRandomly(int index, int key)
            // {
            //     if (table[index, 1] == 0)
            //     {
            //         table[index, 1] = key;
            //         topIdx = addIdx;
            //     }
            //
            //     ;
            //     if (new Random().Next() % addIdx == 0)
            //         return insertroot(p, k);
            //     if (p.key > k)
            //         p.left = insertRand(p.left, k);
            //     else
            //         p.right = insertRand(p.right, k);
            //     fixsize(p);
            //     return p;
            // }
            //
            // void InsertRoot(int index, int key)
            // {
            //     if (p == null) return new Node(k);
            //     if (k < p.key)
            //     {
            //         p.left = insertroot(p.left, k);
            //         return rotateright(p);
            //     }
            //     else
            //     {
            //         p.right = insertroot(p.right, k);
            //         return rotateleft(p);
            //     }
            // }
            //
            // void RotateRight(int p) // правый поворот вокруг узла p
            // {
            //     int q = table[p, 0];
            //     if (q == 0) return;
            //     table[p, 0] = table[q, 2];
            //     table[q, 2] = p;
            //     
            //     if (topIdx == 0)
            //         topIdx = q;
            // }
            //
            // void RotateLeft(int q) // левый поворот вокруг узла q
            // {
            //     int p = table[q, 2];
            //     if (p == 0) return;
            //     table[q, 2] = table[p, 0];
            //     table[p, 0] = q;
            //     
            //     if (topIdx == 0)
            //         topIdx = p;
            // }
        }

        #region functions

        public static Node find(Node p, int k) // поиск ключа k в дереве p
        {
            if (p == null) return null; // в пустом дереве можно не искать
            if (k == p.key)
                return p;
            if (k < p.key)
                return find(p.left, k);
            else
                return find(p.right, k);
        }

        public static Node insert(Node p, int k) // классическая вставка нового узла с ключом k в дерево p
        {
            if (p == null) return new Node(k);
            if (p.key > k)
                p.left = insert(p.left, k);
            else
                p.right = insert(p.right, k);
            fixsize(p);
            return p;
        }

        public static Node insertRand(Node p, int k) // рандомизированная вставка нового узла с ключом k в дерево p
        {
            if (p == null) return new Node(k);
            if (new Random().Next() % (p.size + 1) == 0)
                return insertroot(p, k);
            if (p.key > k)
                p.left = insertRand(p.left, k);
            else
                p.right = insertRand(p.right, k);
            fixsize(p);
            return p;
        }

        public static int getsize(Node p) // обертка для поля size, работает с пустыми деревьями (t=NULL)
        {
            if (p == null) return 0;
            return p.size;
        }

        public static void fixsize(Node p) // установление корректного размера дерева
        {
            p.size = getsize(p.left) + getsize(p.right) + 1;
        }

        public static Node insertroot(Node p, int k) // вставка нового узла с ключом k в корень дерева p 
        {
            if (p == null) return new Node(k);
            if (k < p.key)
            {
                p.left = insertroot(p.left, k);
                return rotateright(p);
            }
            else
            {
                p.right = insertroot(p.right, k);
                return rotateleft(p);
            }
        }

        public static Node rotateright(Node p) // правый поворот вокруг узла p
        {
            Node q = p.left;
            if (q == null) return p;
            p.left = q.right;
            q.right = p;
            q.size = p.size;
            fixsize(p);
            return q;
        }

        public static Node rotateleft(Node q) // левый поворот вокруг узла q
        {
            Node p = q.right;
            if (p == null) return q;
            q.right = p.left;
            p.left = q;
            p.size = q.size;
            fixsize(q);
            return p;
        }

        #endregion

        public class Node
        {
            public int key;
            public int size;
            public Node left;
            public Node right;

            public Node(int k)
            {
                key = k;
                size = 1;
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

        public class BTree
        {
            private BNode _root;
            private int _count;
            private IComparer<int> _comparer = Comparer<int>.Default;

            public BNode Root
            {
                get { return _root; }
            }

            public BTree()
            {
                _root = null;
                _count = 0;
            }


            public bool Add(int Item)
            {
                if (_root == null)
                {
                    _root = new BNode(Item);
                    _count++;
                    return true;
                }
                else
                {
                    return Add_Sub(_root, Item);
                }
            }

            private bool Add_Sub(BNode Node, int Item)
            {
                if (_comparer.Compare(Node.item, Item) < 0)
                {
                    if (Node.right == null)
                    {
                        Node.right = new BNode(Item);
                        _count++;
                        return true;
                    }
                    else
                    {
                        return Add_Sub(Node.right, Item);
                    }
                }
                else if (_comparer.Compare(Node.item, Item) > 0)
                {
                    if (Node.left == null)
                    {
                        Node.left = new BNode(Item);
                        _count++;
                        return true;
                    }
                    else
                    {
                        return Add_Sub(Node.left, Item);
                    }
                }
                else
                {
                    return false;
                }
            }

            public void Print()
            {
                Print(_root, 4);
            }

            public void Print(BNode p, int padding)
            {
                if (p != null)
                {
                    if (p.right != null)
                    {
                        Print(p.right, padding + 4);
                    }

                    if (padding > 0)
                    {
                        Console.Write(" ".PadLeft(padding));
                    }

                    if (p.right != null)
                    {
                        Console.Write("/\n");
                        Console.Write(" ".PadLeft(padding));
                    }

                    Console.Write(p.item.ToString() + "\n ");
                    if (p.left != null)
                    {
                        Console.Write(" ".PadLeft(padding) + "\\\n");
                        Print(p.left, padding + 4);
                    }
                }
            }
        }

    }
}