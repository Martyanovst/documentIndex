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

		public DocumentIndex(IReadOnlyList<KeyValuePair<string, byte[]>> documents)
		{
			var intCast = documents.Select(d => d.Value.Select(x => (int)x)).ToList();
			var inputAlphabet = Enumerable.Range(0, 256 + intCast.Count);
			input = intCast.SelectMany((d, i) => d.Concat(new[] { 256 + i })).ToList();
			tree = new SuffixTree<int>(input, inputAlphabet);
		}
		public List<DocumentOccurrences> ReportOccurrences(byte[] pattern)
		{
			var results = new List<DocumentOccurrences>();
			return results;
		}
	}
}
