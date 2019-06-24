using System;
using System.Collections.Generic;

namespace DocsReport
{
	class SuffixTree<TChar> where TChar : IComparable<TChar>
	{
        public class Node
        {
            public SortedDictionary<TChar, Node> Next { get; set; }
            public SortedDictionary<TChar, Node> Plink { get; set; }
            public Node Par { get; set; }
            public int Pos { get; set; }
            public int Len { get; set; }
            public int StrNumber {get; set;}
            public bool Mark { get; set; }
			public Node()
			{
				Next = new SortedDictionary<TChar, Node>();
				Plink = new SortedDictionary<TChar, Node>();
			}
		}

		public Node Root { get; private set; }
		private Node fake;
        private readonly Func<TChar, int> getStrNumber;
        private readonly Func<TChar, bool> IsStrNumber;


        public SuffixTree(IReadOnlyList<TChar> input, IEnumerable<TChar> inputAlphabet, Func<TChar, int> getStrNumber, Func<TChar, bool> isStrNumber)
		{
            this.getStrNumber = getStrNumber;
            this.IsStrNumber = isStrNumber;
            fake = new Node { Mark = false };
			Root = new Node { Par = fake, Pos = 0, Len = 1, Mark = true };
			foreach (var c in inputAlphabet)
				fake.Next[c] = fake.Plink[c] = Root;
			var last = Root;
			for (int k = input.Count - 1; k >= 0; k--)
				last = Extend(input[k], input, last);
        }

		private void Attach(Node child, Node parent, int edgeLen, TChar c)
		{
			parent.Next[c] = child;
			child.Len = edgeLen;
			child.Par = parent;
            
        }
		private Node Extend(TChar c, IReadOnlyList<TChar> input, Node last)
		{
			int i = input.Count;
			Node parse, split, splitChild;
			Node newNode = new Node { Mark = false };
            for (parse = last; !parse.Plink.TryGetValue(c, out split); parse = parse.Par)
                i -= parse.Len;
            
            if (split.Next.TryGetValue(input[i], out splitChild))
			{
				int splitEdgeStart = splitChild.Pos - splitChild.Len;
				newNode.Pos = splitEdgeStart;
                
                while (EqualityComparer<TChar>.Default.Equals(input[newNode.Pos], input[i]))
				{
					parse = parse.Next[input[i]];
					newNode.Pos += parse.Len;
					i += parse.Len;
                }
                parse.Plink[c] = newNode;
                Attach(newNode, split, newNode.Pos - splitEdgeStart, input[splitEdgeStart]);
				Attach(splitChild, newNode, splitChild.Pos - newNode.Pos, input[newNode.Pos]);
                split = newNode;
				newNode = new Node();
			}
			last.Plink[c] = newNode;
			Attach(newNode, split, input.Count - i, input[i]);
            newNode.StrNumber = IsStrNumber(c) ? getStrNumber(c) : last.StrNumber;
            newNode.Pos = input.Count;
			newNode.Mark = true;
            
			return newNode;
		}
	}
}
