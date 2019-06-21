using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

		public DocumentIndex(IReadOnlyList<KeyValuePair<string, byte[]>> documents)
		{
			var intCast = documents.Select(d => d.Value.Select(x => (int)x)).ToList();
			var inputAlphabet = Enumerable.Range(0, 256 + intCast.Count);
			input = intCast.SelectMany((d, i) => d.Concat(new[] { 256 + i })).ToList();
			tree = new SuffixTree<int>(input, inputAlphabet);
            filesNames = documents.Select(d => d.Key).ToArray();
        }
		public List<DocumentOccurrences> ReportOccurrences(byte[] pattern)
		{
			var results = new List<DocumentOccurrences>();
            var intPattern = pattern.Select(b => (int)b);
            var node = tree.Root;
            foreach (var symbol in intPattern)
            {
                if (node.Next.TryGetValue(symbol, out var nextNode))
                {
                    node = nextNode;
                }
            }
			return results;
		}
	}
}
