using System.Collections.Generic;
using System.Linq;
using Node = DocsReport.SuffixTree<int>.Node;

namespace DocsReport
{
    class DocumentOccurrences
    {
        public DocumentOccurrences(string documentKey)
        {
            DocumentKey = documentKey;
            OccurrencePositions = new List<int>();
        }

        public string DocumentKey { get; set; }
        public List<int> OccurrencePositions { get; set; }
    }
    class DocumentIndex
    {
        private SuffixTree<int> tree;
        private List<int> input;
        private string[] filesNames;
        private int[] filesStartPositions;

        public DocumentIndex(IReadOnlyList<KeyValuePair<string, byte[]>> documents)
        {
            var intCast = documents.Select(d => d.Value.Select(x => (int)x)).ToList();
            var inputAlphabet = Enumerable.Range(0, 256 + intCast.Count);
            input = new List<int>();
            filesStartPositions = new int[documents.Count];
            filesNames = new string[documents.Count];
            for (var docIndex = 0; docIndex < documents.Count; docIndex++)
            {
                filesStartPositions[docIndex] = input.Count();
                input.AddRange(intCast[docIndex]);
                input.Add(256 + docIndex);
                filesNames[docIndex] = documents[docIndex].Key;
            }
            tree = new SuffixTree<int>(input, inputAlphabet,
                symbol => symbol - 255,
                symbol => symbol > 255);
        }
        public List<DocumentOccurrences> ReportOccurrences(byte[] pattern)
        {
            var results = new Dictionary<string, DocumentOccurrences>();
            var intPattern = pattern.Select(b => (int)b).ToList();
            var node = tree.Root;
            var pathLength = 0;
            for (; pathLength < pattern.Length; pathLength += node.Len)
            {
                if (node.Next.TryGetValue(intPattern[pathLength], out node)) { }
                else return default;
            }

            SearchLeaf(node, pathLength, results);
            return results.Values.ToList();
        }

        private void SearchLeaf(Node node, int pathLen, Dictionary<string, DocumentOccurrences> results)
        {
            if (node.Mark)
            {
                var docNumber = node.StrNumber - 1;
                var docName = filesNames[docNumber];
                var idx = node.Pos - pathLen - filesStartPositions[docNumber];
                if (!results.ContainsKey(docName))
                    results.Add(docName, new DocumentOccurrences(docName));
                results[docName].OccurrencePositions.Add(idx);
            }
            foreach (var nextNode in node.Next.Values)
                SearchLeaf(nextNode, pathLen + nextNode.Len, results);
        }
    }
}
