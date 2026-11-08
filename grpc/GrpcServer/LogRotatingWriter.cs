using System;
using System.IO;
using System.Text;

namespace Byac.Server
{

    public class RotatingTextWriter : TextWriter
    {
        private readonly string _path;
        private readonly long _maxBytes;
        private readonly object _lk = new object();
        private StreamWriter _w;
        private int _tick;

        public RotatingTextWriter(string path, long maxBytes = 32L << 20)
        {
            _path = path;
            _maxBytes = maxBytes;
            TryRotateIfFull();
            _w = Open();
        }

        public override Encoding Encoding => Encoding.UTF8;

        private StreamWriter Open() => new StreamWriter(_path, true) { AutoFlush = true };

        private void TryRotateIfFull()
        {
            try
            {
                if (!File.Exists(_path) || new FileInfo(_path).Length < _maxBytes) return;
                var bak = _path + ".1";
                if (File.Exists(bak)) File.Delete(bak);
                File.Move(_path, bak);
            }
            catch { }
        }

        private void CheckRotate()
        {
            if (++_tick % 4096 != 0) return;
            try
            {
                if (!File.Exists(_path) || new FileInfo(_path).Length < _maxBytes) return;
            }
            catch { return; }
            try { _w.Dispose(); } catch { }
            TryRotateIfFull();
            _w = Open();
        }

        public override void Write(char value) { lock (_lk) { CheckRotate(); _w.Write(value); } }
        public override void Write(string value) { lock (_lk) { CheckRotate(); _w.Write(value); } }
        public override void Write(char[] buffer, int index, int count)
        {
            lock (_lk) { CheckRotate(); _w.Write(buffer, index, count); }
        }
        public override void WriteLine(string value) { lock (_lk) { CheckRotate(); _w.WriteLine(value); } }
        public override void WriteLine() { lock (_lk) { CheckRotate(); _w.WriteLine(); } }
        public override void Flush() { lock (_lk) { _w.Flush(); } }

        protected override void Dispose(bool disposing)
        {
            lock (_lk) { try { _w.Dispose(); } catch { } }
            base.Dispose(disposing);
        }
    }
}
