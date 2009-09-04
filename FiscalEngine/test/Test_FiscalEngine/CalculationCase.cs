using System;
using System.IO;

namespace Test_FiscalEngine
{
    internal enum ECalculationCaseType
    {
        Vb6,
        DotNet
    }

    internal class CalculationCase
    {
        private const string DefaultFileName = "~gnt1f.tmp";

        private readonly string _location;
        private readonly ECalculationCaseType _type;
        private readonly string _startFile;
        private readonly string _name;

        public CalculationCase( string path, ECalculationCaseType type ) : this( path, type, DefaultFileName )
        {
        }

        public CalculationCase( string path, ECalculationCaseType type, string filename )
        {
            _location = path;
            _type = type;
            _startFile = Path.Combine( path, filename );
            _name = Path.GetFileName( path );
        }

        public FileStream Open( string filename )
        {
            var fs = File.Open( Path.Combine( Location, filename ), FileMode.Open, FileAccess.ReadWrite );
            return fs;
        }

        public string Location
        {
            get
            {
                return _location;
            }
        }

        public string StartFile
        {
            get
            {
                return _startFile;
            }
        }

        public ECalculationCaseType Type
        {
            get
            {
                return _type;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public override string ToString()
        {
            return String.Format( "{0} {1}", _type, Name );
        }
    }
}