using System;
using System.Collections.Generic;
using System.Text;

namespace OpenLib
{
    public class PageBuilder
    {
        public StringBuilder Content { get; set; }
        public int PageNumber { get; set; }
    }

    public class PageSplitter
    {
        public static List<PageBuilder> SplitTextIntoPages(string inputText, int maxLinesPerPage, string nextPageBlurb)
        {
            string[] lines = inputText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            List<PageBuilder> pages = [];
            int lineNumber = 0;
            int pageNumber = 1;

            while (lineNumber < lines.Length)
            {
                PageBuilder page = new() { Content = new StringBuilder(), PageNumber = pageNumber };

                // Add header for each page
                //page.Content.AppendLine($"=== Page {pageNumber} ===\r\n\r\n");

                // Skip leading blank lines
                while (lineNumber < lines.Length && string.IsNullOrWhiteSpace(lines[lineNumber]))
                {
                    lineNumber++;
                }

                for (int i = 0; i < maxLinesPerPage && lineNumber < lines.Length; i++)
                {
                    page.Content.AppendLine(lines[lineNumber]);
                    lineNumber++;
                }

                if (lineNumber < lines.Length - 2)
                {
                    page.Content.AppendLine($"{nextPageBlurb}"); //\r\n> Use command \"next\" to see the next page!\r\n
                }
                else
                    page.Content.AppendLine("\r\n");

                pages.Add(page);
                pageNumber++;
            }

            return pages;
        }
    }
}
