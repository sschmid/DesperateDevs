using System;
using System.Threading;

namespace DesperateDevs.Cli.Utils
{
    public class Spinner
    {
        readonly int _interval;
        readonly string[] _frames;

        int _index;
        string _appendix;

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

        public void Append(string text) => _appendix = text;

        public void Write(int left, int top)
        {
            Console.SetCursorPosition(left, top);
            Console.WriteLine(Next());
            if (_appendix != null)
                Console.WriteLine(_appendix);
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
