using System;
using System.Collections;
using System.Collections.Generic;

namespace Test_FiscalEngine
{
    public class BaseSection:IEnumerable<ReportRow>
    {
        private readonly string _title;
        private readonly string _curency;
        private readonly List<ReportRow> _rows = new List<ReportRow>();

        public BaseSection( string title, string curency )
        {
            _title = title;
            _curency = curency;
        }

        public string Title
        {
            get
            {
                return _title;
            }
        }

        public string Curency
        {
            get
            {
                return _curency;
            }
        }

        public void Add( ReportRow reportRow )
        {
            _rows.Add( reportRow );
        }

        public IEnumerator<ReportRow> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format( "{0} ({1} rows)", this.Title, _rows.Count );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}