using System;
using System.IO;

namespace kenjiuno.libcsv2
{
    /// <summary>
    /// CSV text writer
    /// </summary>
    public class Csvw
    {
        TextWriter writer;
        char delim;
        char quote;

        /// <summary>
        /// Create a CSV text writer
        /// </summary>
        /// <param name="writer">A text writer</param>
        /// <param name="sep">A separator character like:
        /// <list type="bullet">
        /// <item>`&apos;,&apos;`</item>
        /// <item>`&apos;\t&apos;`</item>
        /// </list>
        /// </param>
        /// <param name="quote">A quote character like:
        /// <list type="bullet">
        /// <item>`&apos;"&apos;`</item>
        /// </list>
        /// </param>
        public Csvw(TextWriter writer, char sep, char quote)
        {
            this.writer = writer;
            this.delim = sep;
            this.quote = quote;
        }

        int x = 0;

        /// <summary>
        /// Write a token.
        /// </summary>
        /// <remarks>
        /// Token is automatically escaped:
        /// <list type="bullet">
        /// <item>`"text"` → `""text""`</item>
        /// </list>
        /// Token is automatically quoted:
        /// <list type="bullet">
        /// <item>`one\ntwo` → `"one\ntwo"`</item>
        /// </list>
        /// </remarks>
        /// <param name="token">Any string text</param>
        /// <returns>Self instance</returns>
        public Csvw Write(String token)
        {
            token = token ?? "";
            if (x != 0)
            {
                writer.Write("" + delim);
            }
            if (token.IndexOfAny(new char[] { delim, quote, '\r', '\n' }) < 0)
            {
                writer.Write(token);
            }
            else
            {
                writer.Write(quote + token.Replace("" + quote, "" + quote + quote) + quote);
            }
            x++;
            return this;
        }

        /// <summary>
        /// Write a CRLF to begin new line.
        /// </summary>
        /// <example>
        /// Usage:<br />
        /// ```cs
        /// new Csvw(...)
        ///   .Write('header')
        ///   .NextRow()
        ///   .Write('cell');
        /// ```
        /// </example>
        /// <returns>Self instance</returns>
        public Csvw NextRow()
        {
            x = 0;
            writer.WriteLine();
            return this;
        }
    }
}
