using System.Collections.Generic;

namespace Test_FiscalEngine
{
    public class ReportSection:BaseSection
    {
        private readonly int _order;
        private readonly int _startYear;
        private readonly int _p1;
        private readonly int _p2;
        private readonly int _p3;
        private readonly int _p4;
        private readonly double _p5;
        

        public ReportSection( int order, int startYear, int p1, int p2, string title, int p3, int p4, double p5,
                              string currency ):base(title, currency)
        {
            _order = order;
            _startYear = startYear;
            _p1 = p1;
            _p2 = p2;
            _p3 = p3;
            _p4 = p4;
            _p5 = p5;
        }

     
    }
}