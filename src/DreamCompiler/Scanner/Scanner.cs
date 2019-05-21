using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Diagnostics;

namespace DreamCompiler.Scanner
{
    public class Scanner
    {
        private int position;
        byte[] fileData;
        char[] whiteSpace = new char[] { '\r', '\n', '\t', ' ' };

        public Scanner(string path)
        {
            using (FileStream fileStream = File.Open(path, FileMode.Open))
            {
                double fileLength = fileStream.Length;

                this.fileData = new byte[(int)fileLength];
                fileStream.Read(this.fileData, 0, (int)fileLength);
            }
        }


        public IToken GetNextToken()
        {
            while (!IsEOF())
            {
                var t = ReadToken();
                Trace.WriteLine(t);
            }
            return null;
        }



        internal IToken ReadToken()
        {
            char t = (char)fileData[position];
            StringBuilder s = ExtractMethod(t);

            if (Char.IsLetter(t))
            {
                Identifier identifier = new Identifier() { value = s.ToString() };
                Trace.WriteLine(t);
                return identifier;
            }
            else if (Char.IsDigit(t) || Char.IsNumber(t))
            {
                NumberDigit numberDigit = null;
                int value;
                if (int.TryParse(s.ToString(), out value))
                {
                    numberDigit = new NumberDigit() { value = value };
                }

                return numberDigit;
            }
            else if (Char.IsPunctuation(t))
            {
                Trace.WriteLine(t);
                return null;
            }
            else if (Char.IsSymbol(t))
            {
                Trace.WriteLine(t);
                return null;
            }
            else if (char.IsWhiteSpace(t))
            {
                EatWhiteSpace();
            }

            return null;

        }

        private bool IsEOF()
        {
            if (position >= fileData.Length)
            {
                return true;
            }

            return false;
        }

        private StringBuilder ExtractMethod(char t)
        {
            StringBuilder s = new StringBuilder();
            s.Append(t);
            position++;
            while (!IsWhiteSpace())
            {
                s.Append((char)fileData[position++]);
            }

            return s;
        }

        internal void EatWhiteSpace()
        {
            while (IsWhiteSpace())
            {
                position++;
            }
        }

        internal bool IsWhiteSpace()
        {
            if (Char.IsWhiteSpace((char)fileData[position]))
            {
                return true;
            }

            return false;
        }

    }

    public interface IToken
    {

    }

    public class StringToken : IToken
    {

    }

    public class Identifier : IToken
    {
        public string value;
    }

    public class NumberDigit : IToken
    {
        public int value;
    }
}
