using System;
using System.Globalization;
using System.IO;

namespace Test_FiscalEngine
{
    internal class CashflowReader
    {
        private CashflowReader()
        {
        }

        internal static Report Read( string title, string fileName )
        {
            Report report = new Report( title );

            TextReader r = new StreamReader( fileName );
            string line;
            int numRows = 0;
            int rowsRead = 0;
            decimal[][] numbers = null;
            ReportSection currentSection = null;
            string[] titles = null;
            string[] parts = new string[] { };

            // Parse cashflows
            while ( ( line = r.ReadLine() ) != null )
            {
                parts = line.Split( new[] { ',' } );
                if ( parts.Length > 5 && parts[ 5 ].Contains( "ECONOMIC SUMMARY" ) )
                {
                    break;
                }

                if ( currentSection == null )
                {
                    numRows = Int32.Parse( parts[ 3 ] );
                    currentSection
                        = new ReportSection( Int32.Parse( parts[ 0 ] ), Int32.Parse( parts[ 1 ] ),
                                             Int32.Parse( parts[ 2 ] ), Int32.Parse( parts[ 4 ] ), parts[ 5 ],
                                             Int32.Parse( parts[ 6 ] ), Int32.Parse( parts[ 7 ] ),
                                             Double.Parse( parts[ 8 ] ), parts[ 9 ] );
                    rowsRead = -1;
                }
                else
                {
                    if ( rowsRead == -1 ) // First row of a section contains titles
                    {
                        numbers = new decimal[parts.Length][];
                        for ( int i = 0; i < parts.Length; i++ )
                        {
                            numbers[ i ] = new decimal[numRows];
                        }
                        titles = parts;
                    }
                    else if ( numbers != null )
                    {
                        for ( int i = 0; i < parts.Length; i++ )
                        {
                            numbers[ i ][ rowsRead ] = Decimal.Parse( parts[ i ], NumberStyles.Float );
                        }
                    }

                    if ( ++rowsRead == numRows )
                    {
                        for ( int i = 0; i < titles.Length; i++ )
                        {
                            ReportRow row = new ReportRow( titles[ i ], numbers[ i ] );
                            currentSection.Add( row );
                        }

                        numbers = null;
                        titles = null;
                        report.Add( currentSection );
                        currentSection = null;
                    }
                }
            }

            BaseSection summary = new BaseSection( parts[ 5 ], parts[ 9 ] );
            line = r.ReadLine();
            parts = line.Split( new[] { ',' } );

            double[] discounts = new double[parts.Length];

            // Parse summaries
            while ( ( line = r.ReadLine() ) != null )
            {
                parts = line.Split( new[] { ',' } );
                decimal[] number = new decimal[parts.Length - 1];
                for ( int i = 0; i < parts.Length - 1; i++ )
                {
                    number[ i ] = Decimal.Parse( parts[ i + 1 ], NumberStyles.Float );
                }

                ReportRow row = new ReportRow( parts[ 0 ], number );
                summary.Add( row );
            }

            report.Add( summary );

            return report;
        }
    }
}