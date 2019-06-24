using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace DocsReport
{
    class Program
    {
        static void Main(string[] args)
        {
            //var docs = new[] { "abab", "ccab", "aa" };
            //var docs = new[] { "abababab", "cccabcab", "aaaa" };
            var docs = new[] { "aaaa" };
            var count = 1;

            var input = new KeyValuePair<string, byte[]>[count];
            var pattern = Encoding.ASCII.GetBytes("a");
            for (int i = 0; i < count; i++)
            {
                input[i] = new KeyValuePair<string, byte[]>("file" + i, Encoding.ASCII.GetBytes(docs[i]));
            }
            var index = new DocumentIndex(input);
            var actual = index.ReportOccurrences(pattern);
            Console.WriteLine();
            //var documents = new List<KeyValuePair<string, byte[]>>();
            //foreach (var file in Directory.GetFiles(@"..\..\docs\", "*"))
            //{
            //    string docKey = Path.GetFileName(file);
            //    byte[] docContent = File.ReadAllBytes(file);
            //    documents.Add(new KeyValuePair<string, byte[]>(docKey, docContent));
            //}

            //var docIndex = new DocumentIndex(documents);
            //var got = docIndex.ReportOccurrences(Encoding.ASCII.GetBytes("TEST"));
            //var expected = new Dictionary<string, int[]> {
            //        { "readme.txt", new[] { 0, 830, 1713 } },
            //        { "task-Match2d.txt", new[] { 679 } },
            //        { "task-Zfunc.txt", new[] { 2813 } }
            //    };
            //Console.WriteLine(AreSame(got, expected) ? "Test passed" : "Test not passed");

            //got = docIndex.ReportOccurrences(Encoding.UTF8.GetBytes("то есть"));
            //expected = new Dictionary<string, int[]> {
            //        { "bierce.txt", new[] { 1305, 2916 } },
            //        { "task-LZW.txt", new[] { 472, 1207 } },
            //        { "task-Match2d.txt", new[] { 409 } }
            //    };
            //Console.WriteLine(AreSame(got, expected) ? "Test passed" : "Test not passed");
        }

        static bool AreSame(List<DocumentOccurrences> got, Dictionary<string, int[]> expected)
        {
            var comparisons = expected.OrderBy(kv => kv.Key)
                .Zip(got.OrderBy(occs => occs.DocumentKey),
                    (kv, occs) => kv.Key == occs.DocumentKey &&
                        Enumerable.SequenceEqual(
                            kv.Value.OrderBy(x => x),
                            occs.OccurrencePositions.OrderBy(x => x)));
            return got.Count == expected.Count && comparisons.All(x => x);
        }
    }
}
