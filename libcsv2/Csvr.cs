using System;
using System.Collections.Generic;

namespace kenjiuno.libcsv2
{
    public class Csvr
    {
        public String[][] Rows = null;

        class Reader
        {
            int x, cx;
            String s;
            char Sep, Quote;

            public Reader(String s, char sep, char quote)
            {
                this.s = s;
                this.x = 0;
                this.cx = s.Length;
                this.Sep = sep;
                this.Quote = quote;
            }

            public bool EOF { get { return x >= cx; } }

            public ReadRes ReadStr(out String res)
            {
                String temp = "";
                if (EOF)
                {
                    res = null;
                    return ReadRes.EOF;
                }
                if (s[x] == Quote)
                {
                    x++;
                    while (x < cx)
                    {
                        if (s[x] == Quote)
                        {
                            if (x + 1 < cx && s[x + 1] == Quote)
                            {
                                x += 2;
                                temp += Quote;
                            }
                            else
                            {
                                x++;
                                break;
                            }
                        }
                        else
                        {
                            temp += s[x];
                            x++;
                        }
                    }
                }
                else if (s[x] == Sep)
                {
                    x++;
                    res = "";
                    return EOF ? ReadRes.Sep | ReadRes.EOF : ReadRes.Sep;
                }
                else if (CHTr.IsBR(s[x]))
                {
                    ReadBR();
                    res = null;
                    return EOF ? ReadRes.EOL | ReadRes.EOF : ReadRes.EOL;
                }

                while (true)
                {
                    if (EOF)
                    {
                        res = temp;
                        return ReadRes.DataRead | ReadRes.EOF;
                    }
                    else if (CHTr.IsBR(s[x]))
                    {
                        ReadBR();
                        res = temp;
                        return EOF
                            ? ReadRes.DataRead | ReadRes.EOL | ReadRes.EOF
                            : ReadRes.DataRead | ReadRes.EOL
                            ;
                    }
                    else if (s[x] == Sep)
                    {
                        x++;
                        res = temp;
                        return EOF
                            ? ReadRes.DataRead | ReadRes.Sep | ReadRes.EOF
                            : ReadRes.DataRead | ReadRes.Sep
                            ;
                    }
                    else
                    {
                        temp += s[x];
                        x++;
                    }
                }
            }

            private bool ReadBR()
            {
                if (x < cx)
                {
                    if (s[x] == '\r')
                    {
                        x++;
                        if (x < cx && s[x] == '\n')
                        {
                            x++;
                        }
                        return true;
                    }
                    else if (s[x] == '\n')
                    {
                        x++;
                        return true;
                    }
                }
                return false;
            }

            class CHTr
            {
                public static bool IsBR(char c)
                {
                    switch (c)
                    {
                        case '\r':
                        case '\n':
                            return true;
                    }
                    return false;
                }
            }
        }

        [Flags]
        enum ReadRes
        {
            None = 0,
            DataRead = 1,
            Sep = 2,
            EOF = 4,
            EOL = 8,

            SepAndEOF = Sep | EOF,
        }

        public Csvr ReadStr(String text, char sep, char quote)
        {
            Reader reader = new Reader(text, sep, quote);
            List<String> columns = new List<String>();
            List<String[]> rows = new List<String[]>();
            while (true)
            {
                String column;
                ReadRes res = reader.ReadStr(out column);
                if (0 != (res & ReadRes.DataRead))
                {
                    columns.Add(column);

                    if (ReadRes.SepAndEOF == (res & ReadRes.SepAndEOF))
                    {
                        columns.Add("");
                    }
                }
                else
                {
                    if (0 != (res & ReadRes.Sep))
                    {
                        columns.Add("");
                    }

                    if (0 != (res & ReadRes.EOF) || 0 != (res & ReadRes.EOL))
                    {
                        if (columns.Count != 0)
                        {
                            columns.Add("");
                        }
                    }
                }

                if (0 != (res & ReadRes.EOL))
                {
                    rows.Add(columns.ToArray());
                    columns.Clear();
                }
                else if (0 != (res & ReadRes.EOF))
                {
                    if (columns.Count != 0)
                    {
                        rows.Add(columns.ToArray());
                        columns.Clear();
                    }
                    break;
                }
            }
            Rows = rows.ToArray();
            return this;
        }
    }
}
