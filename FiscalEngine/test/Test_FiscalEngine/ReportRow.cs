using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using IEnumerableExtras;

namespace Test_FiscalEngine
{
    public class ReportRow : IEnumerable<decimal>
    {
        private readonly string _title;
        private readonly IEnumerable<decimal> _data;

        public ReportRow( string title, IEnumerable<decimal> data )
        {
            _title = title;
            _data = data;
        }

        public string Title
        {
            get
            {
                return _title;
            }
        }

        public IEnumerator<decimal> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format( "{0} {1}", Title,
                                  _data.Reduce( new StringBuilder(), ( a, b ) => a.AppendFormat( ", {0:0.##}", b ) ) );
        }
    }
}