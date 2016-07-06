using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace HtmlAgilityPack.Tests
{ 
	public class HtmlNode
	{
        [Fact]
        public void EnsureAttributeOriginalCaseIsPreserved()
		{
            
            var html = "<html><body><div AttributeIsThis=\"val\"></div></body></html>";
			var doc = new HtmlDocument
				          {
					          OptionOutputOriginalCase = true
				          };
			doc.LoadHtml(html);
			var div = doc.DocumentNode.Descendants("div").FirstOrDefault();
			var writer = new StringWriter();
			div.WriteAttributes(writer, false);
			var result = writer.GetStringBuilder().ToString();
			Assert.Equal(" AttributeIsThis=\"val\"", result);
		}
	}
}
