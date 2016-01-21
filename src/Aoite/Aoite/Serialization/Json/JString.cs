using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Serialization.Json
{
    internal class JString
    {
        private string _s;
        private int _index;

        internal JString(string s)
        {
            _s = s;
        }

        internal char? GetNextNonEmptyChar()
        {
            while(_s.Length > _index)
            {
                char c = _s[_index++];
                if(!char.IsWhiteSpace(c))
                {
                    return c;
                }
            }

            return null;
        }

        internal char? MoveNext()
        {
            if(_s.Length > _index)
            {
                return _s[_index++];
            }

            return null;
        }

        internal string MoveNext(int count)
        {
            if(_s.Length >= _index + count)
            {
                string result = _s.Substring(_index, count);
                _index += count;

                return result;
            }

            return null;
        }

        internal void MovePrev()
        {
            if(_index > 0)
            {
                _index--;
            }
        }

        internal void MovePrev(int count)
        {
            while(_index > 0 && count > 0)
            {
                _index--;
                count--;
            }
        }

        public override string ToString()
        {
            if(_s.Length > _index)
            {
                return _s.Substring(_index);
            }

            return string.Empty;
        }

        internal string GetDebugString(string message)
        {
            return message + " (" + _index + "): " + _s;
        }

        internal int IndexOf(string substr)
        {
            if(_s.Length > _index)
            {
                return _s.IndexOf(substr, _index, StringComparison.CurrentCulture) - _index;
            }

            return -1;
        }

        internal string Substring(int length)
        {
            if(_s.Length > _index + length)
            {
                return _s.Substring(_index, length);
            }

            return ToString();
        }
    }
}
