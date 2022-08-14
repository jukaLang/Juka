using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JukaCompiler.Exceptions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace JukaCompiler.Interpreter
{
    internal class ArrayImplementation
    {
        internal string arrayName;
        internal object[] internalArray;
        internal int internalSize = 0;

        internal ArrayImplementation(string arrayName, int size)
        {
            this.arrayName = arrayName;
            this.internalSize = size;
            this.internalArray = new object[size];
            for (int i = 0; i < size; i++)
            {
                internalArray[i] = new Object();
            }
        }

        internal object GetAt(int i)
        {
            if (i < 0 || i > this.internalSize)
            {
                throw new JRuntimeException("index out of bounds");
            }

            return internalArray[i-1];
        }
    }
}
