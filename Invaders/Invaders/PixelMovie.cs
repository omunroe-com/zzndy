using System.Collections.Generic;
using System.Drawing;

namespace Invaders
{
    internal class PixelMovie
    {
        protected readonly List<Color[,]> _frames;

        public PixelMovie()
        {
            _frames = new List<Color[,]>();
        }

        public void AddFrame( Color[,] colors )
        {
            _frames.Add( colors );
        }

        public void Render( Graphics g, float x, float y, float scale, int count )
        {
            Color[,] frame = _frames[ count % _frames.Count ];
            int height = frame.GetLength( 0 );
            int width = frame.GetLength( 1 );

            var state = g.Save();
            g.TranslateTransform( x, y );
            g.ScaleTransform( scale, scale);

            for ( int i = 0; i < height; ++i )
            {
                for ( int j = 0; j < width; ++j )
                {
                    g.FillRectangle(new SolidBrush( frame[i,j] ), j, i, 1, 1 );
                }
            }

            g.Restore( state  );
        }
    }
}