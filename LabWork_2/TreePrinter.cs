using System;
using System.Collections.Generic;
using static LabWork_2.Program;

namespace LabWork_2
{

    public class BNode
    {
        public int Item;
        public BNode Right;
        public BNode Left;

        public BNode(int item)
        {
            this.Item = item;
        }
    }

    internal class TreePrinter
    {
        public static void PrintMyTree(MyTree myTree, int topMargin = 0, int leftMargin = 2)
        {
            BNode firstNode = new BNode(0);
            // Console.WriteLine($"Top = {myTree.topIdx}");
            // for (int i = 0; i <= myTree.treeSize; i++)
            // {
            //     Console.WriteLine($"{i,2}) {myTree.table[i, 0],2} {myTree.table[i, 1],2} {myTree.table[i, 2],2}");
            // }

            Add(firstNode, myTree.TopIdx);

            TreePrinter.Print(firstNode, topMargin, leftMargin);

            void Add(BNode node, int index)
            {
                if (myTree.Table[index, 1] != 0)
                {
                    node.Item = myTree.Table[index, 1];
                    // если есть сын
                    if (myTree.Table[index, 0] != 0)
                    {
                        int childIdx = myTree.Table[index, 0];
                        // если сын меньше ключа
                        if (myTree.Table[childIdx, 1] < node.Item)
                        {
                            node.Left = new BNode(0);
                            Add(node.Left, childIdx);
                            int broIdx = myTree.Table[childIdx, 2];
                            // если у сына есть брат
                            if (broIdx != 0)
                            {
                                node.Right = new BNode(0);
                                Add(node.Right, broIdx);
                            }
                        }
                        // если сын больше ключа (значит сын у нас один)
                        else
                        {
                            node.Right = new BNode(0);
                            Add(node.Right, childIdx);
                        }
                    }
                }
                // else
                // {
                //     firstNode = null;
                // }

            }

        }

        class NodeInfo
        {
            public BNode Node;
            public string Text;
            public int StartPos;

            public int Size
            {
                get { return Text.Length; }
            }

            public int EndPos
            {
                get { return StartPos + Size; }
                set { StartPos = value - Size; }
            }

            public NodeInfo Parent, Left, Right;
        }

        public static void Print(BNode root, int topMargin = 2, int leftMargin = 2)
        {
            if (root == null) return;
            int rootTop = Console.CursorTop + topMargin;
            var last = new List<NodeInfo>();
            var next = root;
            for (int level = 0; next != null; level++)
            {
                var item = new NodeInfo {Node = next, Text = next.Item.ToString(" 0 ")};
                if (level < last.Count)
                {
                    item.StartPos = last[level].EndPos + 1;
                    last[level] = item;
                }
                else
                {
                    item.StartPos = leftMargin;
                    last.Add(item);
                }

                if (level > 0)
                {
                    item.Parent = last[level - 1];
                    if (next == item.Parent.Node.Left)
                    {
                        item.Parent.Left = item;
                        item.EndPos = Math.Max(item.EndPos, item.Parent.StartPos);
                    }
                    else
                    {
                        item.Parent.Right = item;
                        item.StartPos = Math.Max(item.StartPos, item.Parent.EndPos);
                    }
                }

                next = next.Left ?? next.Right;
                for (; next == null; item = item.Parent)
                {
                    Print(item, rootTop + 2 * level);
                    if (--level < 0) break;
                    if (item == item.Parent.Left)
                    {
                        item.Parent.StartPos = item.EndPos;
                        next = item.Parent.Node.Right;
                    }
                    else
                    {
                        if (item.Parent.Left == null)
                            item.Parent.EndPos = item.StartPos;
                        else
                            item.Parent.StartPos += (item.StartPos - item.Parent.EndPos) / 2;
                    }
                }
            }

            Console.SetCursorPosition(0, rootTop + 2 * last.Count - 1);
        }

        private static void Print(NodeInfo item, int top)
        {
            SwapColors();
            Print(item.Text, top, item.StartPos);
            SwapColors();
            if (item.Left != null)
                PrintLink(top + 1, "┌", "┘", item.Left.StartPos + item.Left.Size / 2, item.StartPos);
            if (item.Right != null)
                PrintLink(top + 1, "└", "┐", item.EndPos - 1, item.Right.StartPos + item.Right.Size / 2);
        }

        private static void PrintLink(int top, string start, string end, int startPos, int endPos)
        {
            Print(start, top, startPos);
            Print("─", top, startPos + 1, endPos);
            Print(end, top, endPos);
        }

        private static void Print(string s, int top, int left, int right = -1)
        {
            Console.SetCursorPosition(left, top);
            if (right < 0) right = left + s.Length;
            while (Console.CursorLeft < right) Console.Write(s);
        }

        private static void SwapColors()
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = Console.BackgroundColor;
            Console.BackgroundColor = color;
        }
    }
}