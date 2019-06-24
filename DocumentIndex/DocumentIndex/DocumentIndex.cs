using System;
using System.Collections.Generic;
using System.Linq;
using Node = DocsReport.SuffixTree<int>.Node;

namespace DocsReport
{
    class DocumentOccurrences
	{
		public string DocumentKey { get; set; }
		public List<int> OccurrencePositions { get; set; }
	}
	class DocumentIndex
	{
		private SuffixTree<int> tree;
		private List<int> input;
        private string[] filesNames;
        private int[] filesLengths;

		public DocumentIndex(IReadOnlyList<KeyValuePair<string, byte[]>> documents)
		{
			var intCast = documents.Select(d => d.Value.Select(x => (int)x)).ToList();
			var inputAlphabet = Enumerable.Range(0, 256 + intCast.Count);
			input = intCast.SelectMany((d, i) => d.Concat(new[] { 256 + i })).ToList();
			tree = new SuffixTree<int>(input, inputAlphabet,
                symbol => symbol - 255,
                symbol => symbol > 255);
            filesNames = documents.Select(d => d.Key).ToArray();
            filesLengths = documents.Select(d => d.Value.Length).ToArray();
        }
		public List<DocumentOccurrences> ReportOccurrences(byte[] pattern)
		{
			var results = new List<DocumentOccurrences>();
            foreach (var doc in filesNames)
            {
                results.Add(new DocumentOccurrences() { OccurrencePositions = new List<int>() });
            }
            var intPattern = pattern.Select(b => (int)b);
            var node = tree.Root;
            foreach (var symbol in intPattern)
                if (node.Next.TryGetValue(symbol, out node)) { }
                else return results;


            var queue = new Stack<Node>();
            queue.Push(node);
            SearchLeaf(node, 0, pattern.Length, results);
			return results;
		}

        private void SearchLeaf(Node node, int pathLen, int patternLength, List<DocumentOccurrences> results)
        {
            if (node.Mark)
            {
                var docNumber = node.StrNumber - 1;
                if (docNumber < 0) return;
                var idx = filesLengths[docNumber] - patternLength - node.StrNumberPos - pathLen + 1;
                results[docNumber].OccurrencePositions.Add(idx);
                results[docNumber].DocumentKey = filesNames[docNumber];
            }
            foreach (var nextNode in node.Next.Values)
                SearchLeaf(nextNode, pathLen + (nextNode.StrNumberPos > 0 ?  nextNode.StrNumberPos: nextNode.Len), patternLength, results);
        }
	}
}
