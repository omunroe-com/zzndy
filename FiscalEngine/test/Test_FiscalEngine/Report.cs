using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IEnumerableExtras;

namespace Test_FiscalEngine
{
    /// <summary>
    /// Prepresents fiscal engine report.
    /// </summary>
    public class Report : IEnumerable<BaseSection>, IEquatable<Report>
    {
        private readonly string _title;
        private readonly List<BaseSection> _sections = new List<BaseSection>();
        private List<string> _inequalities = new List<string>();

        /// <summary>
        /// Initializes an instance of <see cref="Report"/> with a title.
        /// </summary>
        /// <param name="title"></param>
        public Report( string title )
        {
            _title = title;
        }

        /// <summary>
        /// Gets report title.
        /// </summary>
        public string Title
        {
            get
            {
                return _title;
            }
        }

        /// <summary>
        /// Add section to this report.
        /// </summary>
        /// <param name="section"></param>
        public void Add( BaseSection section )
        {
            _sections.Add( section );
        }

        public IEnumerator<BaseSection> GetEnumerator()
        {
            return _sections.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Comapre two reports on equality. In order to be equal reports
        /// must have same set of sections, with same set of rows with same 
        /// numbers.
        /// </summary>
        /// <param name="a">Left report</param>
        /// <param name="b">Right report</param>
        /// <returns>True if reports are considered equal</returns>
        public static bool operator ==( Report a, Report b )
        {
            bool equal = true;

            // Setup list of inequalities (for unit testing)
            a._inequalities = b._inequalities = new List<string>();

            Func<BaseSection, BaseSection, bool> sectionEquivalent = ( s1, s2 ) => s1.Title == s2.Title;
            Func<ReportRow, ReportRow, bool> rowsEquivalent = ( r1, r2 ) => r1.Title == r2.Title;

            // Compare section sets
            IEnumerable<BaseSection> missingB;
            IEnumerable<BaseSection> missingA;
            IEnumerable<Pair<BaseSection>> commonSections = a.Compare( b, out missingB, sectionEquivalent );
            b.Compare( a, out missingA, sectionEquivalent );


            // Report sections present only in one fo the reports
            foreach ( BaseSection section in missingB )
            {
                equal = false;
                Unequal(a,b, "Section {0} missing from report {1}", section.Title, b.Title );
            }

            foreach ( BaseSection section in missingA )
            {
                equal = false;
                Unequal( a,b, "Section {0} missing from report {1}", section.Title, a.Title );
            }


            // Compare common sections the same way as reports
            foreach ( Pair<BaseSection> pairOfSections in commonSections )
            {
                IEnumerable<ReportRow> missingBr;
                IEnumerable<ReportRow> missingAr;

                IEnumerable<Pair<ReportRow>> commonRows = pairOfSections.A.Compare( pairOfSections.B, out missingBr,
                                                                                    rowsEquivalent );
                pairOfSections.B.Compare( pairOfSections.A, out missingAr, rowsEquivalent );


                // Report missing rows
                foreach ( ReportRow row in missingBr )
                {
                    equal = false;
                    Unequal( a, b, "Row {0} missing from section {1} in report {2}", row.Title, pairOfSections.B.Title,
                             b.Title );
                }

                foreach ( ReportRow row in missingAr )
                {
                    equal = false;
                    Unequal( a, b, "Row {0} missing from section {1} in report {2}", row.Title, pairOfSections.A.Title,
                             a.Title );
                }


                // Compare reports
                foreach ( Pair<ReportRow> pairOfRows in commonRows )
                {
                    IEnumerable<decimal> unequal;
                    pairOfRows.A.Compare( pairOfRows.B, out unequal, ( d1, d2 ) => d1 == d2 );

                    if ( unequal.Count() > 0 )
                    {
                        equal = false;
                        Unequal(a, b, "Rows {0} in section {5} differ:{6}\t{1}\t{2}{6}\t{3}\t{4}", pairOfRows.A.Title,
                                 a._title, pairOfRows.A.ToString(), b._title, pairOfRows.B.ToString(),
                                 pairOfSections.A.Title, Environment.NewLine );
                    }
                }
            }

            return equal;
        }

        /// <summary>
        /// Check if two reports are not equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=( Report a, Report b )
        {
            return !( a == b );
        }

        /// <summary>
        /// Add a log entry on inequality of two given instances of <see cref="Report"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="message"></param>
        private static void Unequal(Report a, Report b, string message )
        {
            a._inequalities.Add( message );

            if ( a._inequalities != b._inequalities )
            {
                b._inequalities.Add(message);
            }
        }

        /// <summary>
        /// Add a log entry on inequality of thwo given instances of <see cref="Report"/>
        /// creating it using formatting similar to <see cref="string.Format(string,object[])"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        private static void Unequal( Report a, Report b, string format, params object[] args )
        {
            Unequal( a, b, String.Format( format, args ) );
        }

        /// <summary>
        /// Get list of inequalities produced by last comparision.
        /// </summary>
        /// <returns></returns>
        public string GetLastInequalities()
        {
            StringBuilder buffer = new StringBuilder();
            return _inequalities.Reduce( buffer, ( a, b ) => a.AppendLine( b ) ).ToString();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.
        ///                 </param>
        public bool Equals( Report other )
        {
            return this == other;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
        ///                 </param><exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.
        ///                 </exception><filterpriority>2</filterpriority>
        public override bool Equals( object obj )
        {
            return obj.GetType() == typeof( Report ) && Equals( (Report) obj );
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return _sections.GetHashCode();
        }
    }
}