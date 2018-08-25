using System;
using System.Threading;

namespace DesperateDevs.CLI.Utils
{
    public class Spinner
    {
        readonly int _interval;
        readonly string[] _frames;

        int _index;

        public Spinner(SpinnerStyle style) : this(style.interval, style.frames) { }

        public Spinner(int interval, params string[] frames)
        {
            _interval = interval;
            _frames = frames;
            _index = 0;
        }

        public string Next()
        {
            if (_index >= _frames.Length)
                _index = 0;
            return _frames[_index++];
        }

        public void Write(int left, int top)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(Next());
        }

        public void WaitForFrame() => Thread.Sleep(_interval);

        public void WriteWhile(int left, int top, Func<bool> condition)
        {
            while (condition())
            {
                Write(left, top);
                WaitForFrame();
            }
        }
    }
}
