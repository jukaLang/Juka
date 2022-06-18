using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juka
{
    public static class XConsole
    {
        public static string CancelableReadLine(out bool isCancelled)
        {

            var cancelKey = ConsoleKey.F10;
            var builder = new StringBuilder();
            var cki = Console.ReadKey(true);
            int index = 0;
            (int left, int top) startPosition;

            while (cki.Key != ConsoleKey.Enter && cki.Key != cancelKey)
            {
                if (cki.Key == ConsoleKey.LeftArrow)
                {
                    if (index < 1)
                    {
                        cki = Console.ReadKey(true);
                        continue;
                    }

                    LeftArrow(ref index, cki);
                }
                else if (cki.Key == ConsoleKey.RightArrow)
                {
                    if (index >= builder.Length)
                    {
                        cki = Console.ReadKey(true);
                        continue;
                    }

                    RightArrow(ref index, cki, builder);
                }
                else if (cki.Key == ConsoleKey.Backspace)
                {
                    if (index < 1)
                    {
                        cki = Console.ReadKey(true);
                        continue;
                    }

                    BackSpace(ref index, cki, builder);
                }
                else if (cki.Key == ConsoleKey.Delete)
                {
                    if (index >= builder.Length)
                    {
                        cki = Console.ReadKey(true);
                        continue;
                    }

                    Delete(ref index, cki, builder);
                }
                else if (cki.Key == ConsoleKey.Tab)
                {
                    cki = Console.ReadKey(true);
                    continue;
                }
                else
                {
                    if (cki.KeyChar == '\0')
                    {
                        cki = Console.ReadKey(true);
                        continue;
                    }

                    Default(ref index, cki, builder);
                }

                cki = Console.ReadKey(true);
            }

            if (cki.Key == cancelKey)
            {
                /*startPosition = GetStartPosition(index);
                ErasePrint(builder, startPosition);

                isCancelled = true;
                return string.Empty;*/

                isCancelled = true;
                return builder.ToString();
            }

            isCancelled = false;

            startPosition = GetStartPosition(index);
            var endPosition = GetEndPosition(startPosition.left, builder.Length);
            var left = 0;
            var top = startPosition.top + endPosition.top + 1;

            Console.SetCursorPosition(left, top);

            var value = builder.ToString();
            return value;
        }

        private static void LeftArrow(ref int index, ConsoleKeyInfo cki)
        {
            var previousIndex = index;
            index--;

            if (cki.Modifiers == ConsoleModifiers.Control)
            {
                index = 0;

                var startPosition = GetStartPosition(previousIndex);
                Console.SetCursorPosition(startPosition.left, startPosition.top);

                return;
            }

            if (Console.CursorLeft > 0)
                Console.CursorLeft--;
            else
            {
                Console.CursorTop--;
                Console.CursorLeft = Console.BufferWidth - 1;
            }
        }

        private static void RightArrow(ref int index, ConsoleKeyInfo cki, StringBuilder builder)
        {
            var previousIndex = index;
            index++;

            if (cki.Modifiers == ConsoleModifiers.Control)
            {
                index = builder.Length;

                var startPosition = GetStartPosition(previousIndex);
                var endPosition = GetEndPosition(startPosition.left, builder.Length);
                var top = startPosition.top + endPosition.top;
                var left = endPosition.left;

                Console.SetCursorPosition(left, top);

                return;
            }

            if (Console.CursorLeft < Console.BufferWidth - 1)
                Console.CursorLeft++;
            else
            {
                Console.CursorTop++;
                Console.CursorLeft = 0;
            }
        }

        private static void BackSpace(ref int index, ConsoleKeyInfo cki, StringBuilder builder)
        {
            var previousIndex = index;
            index--;

            var startPosition = GetStartPosition(previousIndex);
            Console.CursorVisible = false;
            ErasePrint(builder, startPosition);

            builder.Remove(index, 1);
            Console.Write(builder.ToString());

            GoBackToCurrentPosition(index, startPosition);
            Console.CursorVisible = true;
        }

        private static void Delete(ref int index, ConsoleKeyInfo cki, StringBuilder builder)
        {
            var startPosition = GetStartPosition(index);
            Console.CursorVisible = false;
            ErasePrint(builder, startPosition);

            if (cki.Modifiers == ConsoleModifiers.Control)
            {
                builder.Remove(index, builder.Length - index);
                Console.Write(builder.ToString());

                GoBackToCurrentPosition(index, startPosition);
                Console.CursorVisible = true;
                return;
            }

            builder.Remove(index, 1);
            Console.Write(builder.ToString());

            GoBackToCurrentPosition(index, startPosition);
            Console.CursorVisible = true;
        }

        private static void Default(ref int index, ConsoleKeyInfo cki, StringBuilder builder)
        {
            var previousIndex = index;
            index++;

            builder.Insert(previousIndex, cki.KeyChar);

            
            var startPosition = GetStartPosition(previousIndex);
            Console.CursorVisible = false;
            Console.SetCursorPosition(startPosition.left, startPosition.top);
            Console.Write(builder.ToString());
            GoBackToCurrentPosition(index, startPosition);
            Console.CursorVisible = true;
        }

        private static (int left, int top) GetStartPosition(int previousIndex)
        {
            int top;
            int left;

            if (previousIndex <= Console.CursorLeft)
            {
                top = Console.CursorTop;
                left = Console.CursorLeft - previousIndex;
            }
            else
            {
                var decrementValue = previousIndex - Console.CursorLeft;
                var rowsFromStart = decrementValue / Console.BufferWidth;
                top = Console.CursorTop - rowsFromStart;
                left = decrementValue - rowsFromStart * Console.BufferWidth;

                if (left != 0)
                {
                    top--;
                    left = Console.BufferWidth - left;
                }
            }

            return (left, top);
        }

        private static void GoBackToCurrentPosition(int index, (int left, int top) startPosition)
        {
            var rowsToGo = (index + startPosition.left) / Console.BufferWidth;
            var rowIndex = index - rowsToGo * Console.BufferWidth;

            var left = startPosition.left + rowIndex;
            var top = startPosition.top + rowsToGo;

            Console.SetCursorPosition(left, top);
        }

        private static (int left, int top) GetEndPosition(int startColumn, int builderLength)
        {
            var cursorTop = (builderLength + startColumn) / Console.BufferWidth;
            var cursorLeft = startColumn + (builderLength - cursorTop * Console.BufferWidth);

            return (cursorLeft, cursorTop);
        }

        private static void ErasePrint(StringBuilder builder, (int left, int top) startPosition)
        {
            Console.SetCursorPosition(startPosition.left, startPosition.top);
            Console.Write(new string(Enumerable.Range(0, builder.Length).Select(o => ' ').ToArray()));

            Console.SetCursorPosition(startPosition.left, startPosition.top);
        }
    }
}
