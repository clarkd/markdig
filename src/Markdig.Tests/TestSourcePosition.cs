﻿// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Text;
using Markdig.Helpers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using NUnit.Framework;

namespace Markdig.Tests
{
    /// <summary>
    /// Test the precise source location of all Markdown elements, including extensions
    /// </summary>
    [TestFixture]
    public class TestSourcePosition
    {
        [Test]
        public void TestParagraph()
        {
            Check("0123456789", @"
paragraph    ( 0, 0)  0-9
literal      ( 0, 0)  0-9
");
        }

        [Test]
        public void TestParagraphAndNewLine()
        {
            Check("0123456789\n0123456789", @"
paragraph    ( 0, 0)  0-20
literal      ( 0, 0)  0-9
linebreak    ( 0,10) 10-10
literal      ( 1, 0) 11-20
");

            Check("0123456789\r\n0123456789", @"
paragraph    ( 0, 0)  0-21
literal      ( 0, 0)  0-9
linebreak    ( 0,10) 10-10
literal      ( 1, 0) 12-21
");
        }

        [Test]
        public void TestParagraphNewLineAndSpaces()
        {
            //     0123 45678
            Check("012\n  345", @"
paragraph    ( 0, 0)  0-8
literal      ( 0, 0)  0-2
linebreak    ( 0, 3)  3-3
literal      ( 1, 2)  6-8
");
        }

        [Test]
        public void TestParagraph2()
        {
            Check("0123456789\n\n0123456789", @"
paragraph    ( 0, 0)  0-9
literal      ( 0, 0)  0-9
paragraph    ( 2, 0) 12-21
literal      ( 2, 0) 12-21
");
        }

        [Test]
        public void TestEmphasis()
        {
            Check("012**3456789**", @"
paragraph    ( 0, 0)  0-13
literal      ( 0, 0)  0-2
emphasis     ( 0, 3)  3-13
literal      ( 0, 5)  5-11
");
        }

        [Test]
        public void TestEmphasis2()
        {
            //     01234567
            Check("01*2**3*", @"
paragraph    ( 0, 0)  0-7
literal      ( 0, 0)  0-1
emphasis     ( 0, 2)  2-4
literal      ( 0, 3)  3-3
emphasis     ( 0, 5)  5-7
literal      ( 0, 6)  6-6
");
        }

        [Test]
        public void TestEmphasis3()
        {
            //     0123456789
            Check("01**2***3*", @"
paragraph    ( 0, 0)  0-9
literal      ( 0, 0)  0-1
emphasis     ( 0, 2)  2-6
literal      ( 0, 4)  4-4
emphasis     ( 0, 7)  7-9
literal      ( 0, 8)  8-8
");
        }

        [Test]
        public void TestEmphasisFalse()
        {
            Check("0123456789**0123", @"
paragraph    ( 0, 0)  0-15
literal      ( 0, 0)  0-9
literal      ( 0,10) 10-11
literal      ( 0,12) 12-15
");
        }

        [Test]
        public void TestHeading()
        {
            //     012345
            Check("# 2345", @"
heading      ( 0, 0)  0-5
literal      ( 0, 2)  2-5
");
        }

        [Test]
        public void TestHeadingWithEmphasis()
        {
            //     0123456789
            Check("# 23**45**", @"
heading      ( 0, 0)  0-9
literal      ( 0, 2)  2-3
emphasis     ( 0, 4)  4-9
literal      ( 0, 6)  6-7
");
        }

        [Test]
        public void TestCodeSpan()
        {
            //     012345678
            Check("0123`456`", @"
paragraph    ( 0, 0)  0-8
literal      ( 0, 0)  0-3
code         ( 0, 4)  4-8
");
        }

        [Test]
        public void TestLink()
        {
            //     0123456789
            Check("012[45](#)", @"
paragraph    ( 0, 0)  0-9
literal      ( 0, 0)  0-2
link         ( 0, 3)  3-9
literal      ( 0, 4)  4-5
");
        }

        [Test]
        public void TestHtmlInline()
        {
            //     0123456789
            Check("01<b>4</b>", @"
paragraph    ( 0, 0)  0-9
literal      ( 0, 0)  0-1
html         ( 0, 2)  2-4
literal      ( 0, 5)  5-5
html         ( 0, 6)  6-9
");
        }

        [Test]
        public void TestAutolinkInline()
        {
            //     0123456789ABCD
            Check("01<http://yes>", @"
paragraph    ( 0, 0)  0-13
literal      ( 0, 0)  0-1
autolink     ( 0, 2)  2-13
");
        }

        [Test]
        public void TestFencedCodeBlock()
        {
            //     012 3456 78 9ABC
            Check("01\n```\n3\n```\n", @"
paragraph    ( 0, 0)  0-1
literal      ( 0, 0)  0-1
fencedcode   ( 1, 0)  3-11
");
        }

        [Test]
        public void TestHtmlBlock()
        {
            //     012345 67 89ABCDE F 0
            Check("<div>\n0\n</div>\n\n1", @"
html         ( 0, 0)  0-13
paragraph    ( 4, 0) 16-16
literal      ( 4, 0) 16-16
");
        }

        [Test]
        public void TestThematicBreak()
        {
            //     0123 4567
            Check("---\n---\n", @"
thematicbreak ( 0, 0)  0-2
thematicbreak ( 1, 0)  4-6
");
        }

        [Test]
        public void TestQuoteBlock()
        {
            //     0123456
            Check("> 2345\n", @"
quote        ( 0, 0)  0-5
paragraph    ( 0, 2)  2-5
literal      ( 0, 2)  2-5
");
        }

        [Test]
        public void TestQuoteBlockWithLines()
        {
            //     01234 56789A
            Check("> 01\n>  23\n", @"
quote        ( 0, 0)  0-9
paragraph    ( 0, 2)  2-9
literal      ( 0, 2)  2-3
linebreak    ( 0, 4)  4-4
literal      ( 1, 3)  8-9
");
        }

        [Test]
        public void TestQuoteBlockWithLazyContinuation()
        {
            //     01234 56
            Check("> 01\n23\n", @"
quote        ( 0, 0)  0-6
paragraph    ( 0, 2)  2-6
literal      ( 0, 2)  2-3
linebreak    ( 0, 4)  4-4
literal      ( 1, 0)  5-6
");
        }

        [Test]
        public void TestListBlock()
        {
            //     0123 4567
            Check("- 0\n- 1\n", @"
list         ( 0, 0)  0-6
listitem     ( 0, 0)  0-2
paragraph    ( 0, 2)  2-2
literal      ( 0, 2)  2-2
listitem     ( 1, 0)  4-6
paragraph    ( 1, 2)  6-6
literal      ( 1, 2)  6-6
");
        }

        [Test]
        public void TestEscapeInline()
        {
            //      0123
            Check(@"\-\)", @"
paragraph    ( 0, 0)  0-3
literal      ( 0, 0)  0-1
literal      ( 0, 2)  2-3
");
        }

        [Test]
        public void TestHtmlEntityInline()
        {
            //     01234567
            Check("0&nbsp;1", @"
paragraph    ( 0, 0)  0-7
literal      ( 0, 0)  0-0
htmlentity   ( 0, 1)  1-6
literal      ( 0, 7)  7-7
");
        }

        [Test]
        public void TestAbbreviations()
        {
            Check("*[HTML]: Hypertext Markup Language\r\n\r\nLater in a text we are using HTML and it becomes an abbr tag HTML", @"
paragraph    ( 2, 0) 38-102
container    ( 2, 0) 38-102
literal      ( 2, 0) 38-66
abbreviation ( 2,29) 67-70
literal      ( 2,33) 71-98
abbreviation ( 2,61) 99-102
", "abbreviations");
        }

        [Test]
        public void TestCitation()
        {
            //     0123 4 567 8
            Check("01 \"\"23\"\"", @"
paragraph    ( 0, 0)  0-8
literal      ( 0, 0)  0-2
emphasis     ( 0, 3)  3-8
literal      ( 0, 5)  5-6
", "citations");
        }

        [Test]
        public void TestCustomContainer()
        {
            //     01 2345 678 9ABC DEF
            Check("0\n:::\n23\n:::\n45\n", @"
paragraph    ( 0, 0)  0-0
literal      ( 0, 0)  0-0
customcontainer ( 1, 0)  2-11
paragraph    ( 2, 0)  6-7
literal      ( 2, 0)  6-7
paragraph    ( 4, 0) 13-14
literal      ( 4, 0) 13-14
", "customcontainers");
        }

        [Test]
        public void TestDefinitionList()
        {
            //     012 3456789A
            Check("a0\n:   1234", @"
definitionlist ( 0, 0)  0-10
definitionitem ( 1, 0)  3-10
definitionterm ( 0, 0)  0-1
literal      ( 0, 0)  0-1
paragraph    ( 1, 4)  7-10
literal      ( 1, 4)  7-10
", "definitionlists");
        }

        [Test]
        public void TestDefinitionList2()
        {
            //     012 3456789AB CDEF01234
            Check("a0\n:   1234\n:    5678", @"
definitionlist ( 0, 0)  0-20
definitionitem ( 1, 0)  3-10
definitionterm ( 0, 0)  0-1
literal      ( 0, 0)  0-1
paragraph    ( 1, 4)  7-10
literal      ( 1, 4)  7-10
definitionitem ( 2, 4) 12-20
paragraph    ( 2, 5) 17-20
literal      ( 2, 5) 17-20
", "definitionlists");
        }

        [Test]
        public void TestEmoji()
        {
            //     01 2345
            Check("0\n :)\n", @"
paragraph    ( 0, 0)  0-4
literal      ( 0, 0)  0-0
linebreak    ( 0, 1)  1-1
emoji        ( 1, 1)  3-4
", "emojis");
        }

        [Test]
        public void TestEmphasisExtra()
        {
            //     0123456
            Check("0 ~~1~~", @"
paragraph    ( 0, 0)  0-6
literal      ( 0, 0)  0-1
emphasis     ( 0, 2)  2-6
literal      ( 0, 4)  4-4
", "emphasisextras");
        }

        [Test]
        public void TestFigures()
        {
            //     01 2345 67 89AB
            Check("0\n^^^\n0\n^^^\n", @"
paragraph    ( 0, 0)  0-0
literal      ( 0, 0)  0-0
figure       ( 1, 0)  2-10
paragraph    ( 2, 0)  6-6
literal      ( 2, 0)  6-6
", "figures");
        }

        [Test]
        public void TestFiguresCaption1()
        {
            //     01 234567 89 ABCD
            Check("0\n^^^ab\n0\n^^^\n", @"
paragraph    ( 0, 0)  0-0
literal      ( 0, 0)  0-0
figure       ( 1, 0)  2-12
figurecaption ( 1, 3)  5-6
literal      ( 1, 3)  5-6
paragraph    ( 2, 0)  8-8
literal      ( 2, 0)  8-8
", "figures");
        }

        [Test]
        public void TestFiguresCaption2()
        {
            //     01 2345 67 89ABCD
            Check("0\n^^^\n0\n^^^ab\n", @"
paragraph    ( 0, 0)  0-0
literal      ( 0, 0)  0-0
figure       ( 1, 0)  2-12
paragraph    ( 2, 0)  6-6
literal      ( 2, 0)  6-6
figurecaption ( 3, 3) 11-12
literal      ( 3, 3) 11-12
", "figures");
        }

        [Test]
        public void TestFooters()
        {
            //     01 234567 89ABCD
            Check("0\n^^ 12\n^^ 34\n", @"
paragraph    ( 0, 0)  0-0
literal      ( 0, 0)  0-0
footer       ( 1, 0)  2-12
paragraph    ( 1, 3)  5-12
literal      ( 1, 3)  5-6
linebreak    ( 1, 5)  7-7
literal      ( 2, 3) 11-12
", "footers");
        }


        [Test]
        public void TestAttributes()
        {
            //     0123456789
            Check("0123{#456}", @"
paragraph    ( 0, 0)  0-9
attributes   ( 0, 4)  4-9
literal      ( 0, 0)  0-3
", "attributes");
        }

        [Test]
        public void TestAttributesForHeading()
        {
            //     0123456789ABC
            Check("# 01 {#456}", @"
heading      ( 0, 0)  0-4
attributes   ( 0, 5)  5-10
literal      ( 0, 2)  2-3
", "attributes");
        }

        [Test]
        public void TestMathematicsInline()
        {
            //     01 23456789AB
            Check("0\n012 $abcd$", @"
paragraph    ( 0, 0)  0-11
literal      ( 0, 0)  0-0
linebreak    ( 0, 1)  1-1
literal      ( 1, 0)  2-5
math         ( 1, 4)  6-11
attributes   ( 0, 0)  0-0
", "mathematics");
        }

        [Test]
        public void TestSmartyPants()
        {
            //        01234567
            //     01 23456789
            Check("0\n2 <<45>>", @"
paragraph    ( 0, 0)  0-9
literal      ( 0, 0)  0-0
linebreak    ( 0, 1)  1-1
literal      ( 1, 0)  2-3
smartypant   ( 1, 2)  4-5
literal      ( 1, 4)  6-7
smartypant   ( 1, 6)  8-9
", "smartypants");
        }

        [Test]
        public void TestSmartyPantsUnbalanced()
        {
            //        012345
            //     01 234567
            Check("0\n2 <<45", @"
paragraph    ( 0, 0)  0-7
literal      ( 0, 0)  0-0
linebreak    ( 0, 1)  1-1
literal      ( 1, 0)  2-3
literal      ( 1, 2)  4-5
literal      ( 1, 4)  6-7
", "smartypants");
        }

        [Test]
        public void TestPipeTable()
        {
            //     0123 4567 89AB
            Check("a|b\n-|-\n0|1\n", @"
table        ( 0, 0)  0-10
tablerow     ( 0, 0)  0-2
tablecell    ( 0, 0)  0-0
paragraph    ( 0, 0)  0-0
literal      ( 0, 0)  0-0
tablecell    ( 0, 2)  2-2
paragraph    ( 0, 2)  2-2
literal      ( 0, 2)  2-2
tablerow     ( 2, 0)  8-10
tablecell    ( 2, 0)  8-8
paragraph    ( 2, 0)  8-8
literal      ( 2, 0)  8-8
tablecell    ( 2, 2) 10-10
paragraph    ( 2, 2) 10-10
literal      ( 2, 2) 10-10
", "pipetables");
        }

        [Test]
        public void TestPipeTable2()
        {
            //     01 2 3456 789A BCD
            Check("0\n\na|b\n-|-\n0|1\n", @"
paragraph    ( 0, 0)  0-0
literal      ( 0, 0)  0-0
table        ( 2, 0)  3-13
tablerow     ( 2, 0)  3-5
tablecell    ( 2, 0)  3-3
paragraph    ( 2, 0)  3-3
literal      ( 2, 0)  3-3
tablecell    ( 2, 2)  5-5
paragraph    ( 2, 2)  5-5
literal      ( 2, 2)  5-5
tablerow     ( 4, 0) 11-13
tablecell    ( 4, 0) 11-11
paragraph    ( 4, 0) 11-11
literal      ( 4, 0) 11-11
tablecell    ( 4, 2) 13-13
paragraph    ( 4, 2) 13-13
literal      ( 4, 2) 13-13
", "pipetables");
        }

        [Test]
        public void TestIndentedCode()
        {
            //     01 2 345678 9ABCDE
            Check("0\n\n    0\n    1\n", @"
paragraph    ( 0, 0)  0-0
literal      ( 0, 0)  0-0
code         ( 2, 4)  7-13
");
        }

        [Test]
        public void TestIndentedCodeWithTabs()
        {
            //     01 2 3 45 6 78
            Check("0\n\n\t0\n\t1\n", @"
paragraph    ( 0, 0)  0-0
literal      ( 0, 0)  0-0
code         ( 2, 4)  4-7
");
        }

        [Test]
        public void TestIndentedCodeWithMixedTabs()
        {
            //     01 2 34 56 78 9
            Check("0\n\n \t0\n \t1\n", @"
paragraph    ( 0, 0)  0-0
literal      ( 0, 0)  0-0
code         ( 2, 4)  5-9
");
        }

        [Test]
        public void TestTabsInList()
        {
            //     012 34 567 89
            Check("- \t0\n- \t1\n", @"
list         ( 0, 0)  0-8
listitem     ( 0, 0)  0-3
paragraph    ( 0, 4)  3-3
literal      ( 0, 4)  3-3
listitem     ( 1, 0)  5-8
paragraph    ( 1, 4)  8-8
literal      ( 1, 4)  8-8
");
        }

        [Test]
        public void TestDocument()
        {
            //     L0       L0           L1L2         L3         L4         L5L6                    L7L8         
            //     0        10        20          30         40         50          60        70          80        90
            //     012345678901234567890 1 2345678901 2345678901 2345678901 2 345678901234567890123 4 5678901234567890123
            Check("# This is a document\n\n1) item 1\n2) item 2\n3) item 4\n\nWith an **emphasis**\n\n> and a blockquote\n", @"
heading      ( 0, 0)  0-19
literal      ( 0, 2)  2-19
list         ( 2, 0) 22-51
listitem     ( 2, 0) 22-30
paragraph    ( 2, 3) 25-30
literal      ( 2, 3) 25-30
listitem     ( 3, 0) 32-40
paragraph    ( 3, 3) 35-40
literal      ( 3, 3) 35-40
listitem     ( 4, 0) 42-51
paragraph    ( 4, 3) 45-50
literal      ( 4, 3) 45-50
paragraph    ( 6, 0) 53-72
literal      ( 6, 0) 53-60
emphasis     ( 6, 8) 61-72
literal      ( 6,10) 63-70
quote        ( 8, 0) 75-92
paragraph    ( 8, 2) 77-92
literal      ( 8, 2) 77-92
");
        }

        private static void Check(string text, string expectedResult, string extensions = null)
        {
            var pipelineBuilder = new MarkdownPipelineBuilder().UsePreciseSourceLocation();
            if (extensions != null)
            {
                pipelineBuilder.Configure(extensions);
            }
            var pipeline = pipelineBuilder.Build();

            var document = Markdown.Parse(text, pipeline);

            var build = new StringBuilder();
            foreach (var val in document.Descendants())
            {
                var name = GetTypeName(val.GetType());
                build.Append($"{name,-12} ({val.Line,2},{val.Column,2}) {val.SourceStartPosition,2}-{val.SourceEndPosition}\n");
                var attributes = val.TryGetAttributes();
                if (attributes != null)
                {
                    build.Append($"{"attributes",-12} ({attributes.Line,2},{attributes.Column,2}) {attributes.SourceStartPosition,2}-{attributes.SourceEndPosition}\n");
                }
            }
            var result = build.ToString().Trim();

            expectedResult = expectedResult.Trim();
            expectedResult = expectedResult.Replace("\r\n", "\n").Replace("\r", "\n");

            if (expectedResult != result)
            {
                Console.WriteLine("```````````````````Source");
                Console.WriteLine(TestParser.DisplaySpaceAndTabs(text));
                Console.WriteLine("```````````````````Result");
                Console.WriteLine(result);
                Console.WriteLine("```````````````````Expected");
                Console.WriteLine(expectedResult);
                Console.WriteLine("```````````````````");
                Console.WriteLine();
            }

            TextAssert.AreEqual(expectedResult, result);
        }

        private static string GetTypeName(Type type)
        {
            return type.Name.ToLowerInvariant()
                .Replace("block", string.Empty)
                .Replace("inline", string.Empty);
        }
    }
}