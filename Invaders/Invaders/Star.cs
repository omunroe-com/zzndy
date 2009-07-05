using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Invaders
{
    /// <summary>
    /// Represents a single star on a sky.
    /// </summary>
    class Star
    {
        private readonly float _x;
        private readonly float _y;
        private readonly Color _color;
        private readonly float _size;
        private readonly Brush _brush;

        /// <summary>
        /// Initialize an instance of class <see cref="Star"/> with float coordinates
        /// and color.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public Star(float x, float y, Color color):this(x,y,color,1)
        {
        }

        /// <summary>
        /// Initialize an instance of class <see cref="Star"/> with float coordinates,
        /// color and size.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        public Star(float x, float y, Color color, float size)
        {
            _x = x;
            _y = y;
            _color = color;
            _size = size;
            _brush = new SolidBrush( _color );
        }

        /// <summary>
        /// Draw this star on a canvas.
        /// </summary>
        /// <param name="g"></param>
        public void Render(Graphics g )
        {
            g.FillEllipse( _brush, _x, _y, _size, _size );
        }
    }
}
