using System.IO;
using System.Linq;
using System.Net;
using System;
using Xunit;

namespace HtmlAgilityPack.Tests
{
	public class HtmlDocumentTests
	{
		private string _contentDirectory;	

        public HtmlDocumentTests()
        {
            _contentDirectory = Path.Combine(Directory.GetCurrentDirectory(), "files");
        }

		private HtmlDocument GetMshomeDocument()
		{
			var doc = new HtmlDocument();
            doc.Load(Path.Combine(_contentDirectory, "mshome.htm"));
            return doc;
		}

		[Fact]
		public void CreateAttribute()
		{
			var doc = new HtmlDocument();
			var a = doc.CreateAttribute("href");
			Assert.Equal("href", a.Name);
		}

		[Fact]
		public void CreateAttributeWithEncodedText()
		{
			var doc = new HtmlDocument();
			var a = doc.CreateAttribute("href", "http://something.com\"&<>");
			Assert.Equal("href", a.Name);
			Assert.Equal("http://something.com\"&<>", a.Value);
		}

		[Fact]
		public void CreateAttributeWithText()
		{
			var doc = new HtmlDocument();
			var a = doc.CreateAttribute("href", "http://something.com");
			Assert.Equal("href", a.Name);
			Assert.Equal("http://something.com", a.Value);
		}

		[Fact]
		public void CreateElement()
		{
			var doc = new HtmlDocument();
			var a = doc.CreateElement("a");
			Assert.Equal("a", a.Name);
			Assert.Equal(a.NodeType, HtmlNodeType.Element);
		}

		[Fact]
		public void CreateTextNodeWithText()
		{
			var doc = new HtmlDocument();
			var a = doc.CreateTextNode("something");
			Assert.Equal("something", a.InnerText);
			Assert.Equal(a.NodeType, HtmlNodeType.Text);
		}

		[Fact]
		public void HtmlEncode()
		{
			var result = HtmlDocument.HtmlEncode("http://something.com\"&<>");
			Assert.Equal("http://something.com&quot;&amp;&lt;&gt;", result);
		}

		[Fact]
		public void TestParse()
		{
			var doc = GetMshomeDocument();
			Assert.True(doc.DocumentNode.Descendants().Count() > 0);
		}

        [Fact]
        public void TestLimitDepthParse()
        {
            HtmlAgilityPack.HtmlDocument.MaxDepthLevel = 10;
            var doc = GetMshomeDocument();
            try
            {
                Assert.True(doc.DocumentNode.Descendants().Count() > 0);
            }
            catch (ArgumentException e)
            {
                Assert.True(e.Message == HtmlAgilityPack.HtmlNode.DepthLevelExceptionMessage);
            }
            HtmlAgilityPack.HtmlDocument.MaxDepthLevel = int.MaxValue;
        }

        [Fact]
		public void TestParseSaveParse()
		{
			var doc = GetMshomeDocument();
			var doc1desc =
				doc.DocumentNode.Descendants().Where(x => !string.IsNullOrWhiteSpace(x.InnerText)).ToList();
			doc.Save(_contentDirectory + "testsaveparse.html");

			var doc2 = new HtmlDocument();
			doc2.Load(_contentDirectory + "testsaveparse.html");
			var doc2desc =
				doc2.DocumentNode.Descendants().Where(x => !string.IsNullOrWhiteSpace(x.InnerText)).ToList();
			Assert.Equal(doc1desc.Count, doc2desc.Count);
		}

		[Fact]
		public void TestRemoveUpdatesPreviousSibling()
		{
			var doc = GetMshomeDocument();
			var docDesc = doc.DocumentNode.Descendants().ToList();
			var toRemove = docDesc[1200];
			var toRemovePrevSibling = toRemove.PreviousSibling;
			var toRemoveNextSibling = toRemove.NextSibling;
			toRemove.Remove();
			Assert.Same(toRemovePrevSibling, toRemoveNextSibling.PreviousSibling);
		}

		[Fact]
		public void TestReplaceUpdatesSiblings()
		{
			var doc = GetMshomeDocument();
			var docDesc = doc.DocumentNode.Descendants().ToList();
			var toReplace = docDesc[1200];
			var toReplacePrevSibling = toReplace.PreviousSibling;
			var toReplaceNextSibling = toReplace.NextSibling;
			var newNode = doc.CreateElement("tr");
			toReplace.ParentNode.ReplaceChild(newNode, toReplace);
			Assert.Same(toReplacePrevSibling, newNode.PreviousSibling);
			Assert.Same(toReplaceNextSibling, newNode.NextSibling);
		}

		[Fact]
		public void TestInsertUpdateSiblings()
		{
			var doc = GetMshomeDocument();
			var newNode = doc.CreateElement("td");
			var toReplace = doc.DocumentNode.ChildNodes[2];
			var toReplacePrevSibling = toReplace.PreviousSibling;
			var toReplaceNextSibling = toReplace.NextSibling;
			doc.DocumentNode.ChildNodes.Insert(2, newNode);
			Assert.Same(newNode.NextSibling, toReplace);
			Assert.Same(newNode.PreviousSibling, toReplacePrevSibling);
			Assert.Same(toReplaceNextSibling, toReplace.NextSibling);
		}
	}
}