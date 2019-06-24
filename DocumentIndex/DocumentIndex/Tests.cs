using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocsReport
{
    [TestFixture]
    class Tests
    {
        [Test]
        public void SimpleTest()
        {
            var docs = new[] { "abababab", "cccabcab", "aaaa"};
            var input = new KeyValuePair<string, byte[]>[3];
            var pattern = Encoding.ASCII.GetBytes("ab");
            for (int i = 0; i < 3; i++)
            {
                input[i] = new KeyValuePair<string, byte[]>("file" + i, Encoding.ASCII.GetBytes(docs[i]));
            }
            var index = new DocumentIndex(input);
            var actual = index.ReportOccurrences(pattern);
            Assert.AreEqual(actual[0].OccurrencePositions.Count, 5);
        }
    }
}
