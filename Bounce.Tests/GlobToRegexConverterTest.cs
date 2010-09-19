using Bounce.Framework;
using NUnit.Framework;

namespace Bounce.Tests {
    [TestFixture]
    public class GlobToRegexConverterTest {
        [Test]
        public void ShouldConvertSimpleGlobToRegex() {
            AssertGlobConversion("*.xml", @"^[^\\/]*\.xml$", new [] {@"something.xml", @"other.xml"}, new [] {@"c:\temp\something.xml", @"something.xm", @"somethingxml"});
            AssertGlobConversion("**.xml", @"^.*\.xml$", new[] {@"something.xml", @"temp\something.xml", @"temp\b\something.xml", @"c:\temp\something.xml"}, new [] {@"somethingxml"});
            AssertGlobConversion("high.???", @"^high\.[^\\/][^\\/][^\\/]$", new[] {@"high.xml", @"high.txt"}, new[] {@"low.xml", @"high.xm"});
        }

        private void AssertGlobConversion(string glob, string expectedRegex, string[] pathsThatMatch, string [] pathsThatDontMatch) {
            var regex = new GlobToRegexConverter().ConvertToRegex(glob);
            Assert.That(regex.ToString(), Is.EqualTo(expectedRegex));

            if (pathsThatMatch != null) {
                foreach (var path in pathsThatMatch) {
                    Assert.That(regex.IsMatch(path), string.Format("expected {0} to match {1}", path, regex));
                }
            }

            if (pathsThatDontMatch != null) {
                foreach (var path in pathsThatDontMatch) {
                    Assert.That(regex.IsMatch(path), Is.EqualTo(false), string.Format("expected {0} not to match {1}", path, regex));
                }
            }
        }
    }
}