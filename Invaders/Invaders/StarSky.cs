using System;
using System.Drawing;

namespace Invaders
{
    /// <summary>
    /// Represents a field of stars.
    /// </summary>
    internal class StarSky
    {
        private readonly float _width;
        private readonly float _height;
        private readonly int _numStars;
        private Star[] _stars;
        private readonly Random _rand = new Random( (int) DateTime.Now.Ticks );

        /// <summary>
        /// Initialize an instance of <see cref="StarSky"/> with width, height 
        /// and a number of stars.
        /// </summary>
        /// <param name="width">Width of star sky.</param>
        /// <param name="height">Height of star sky.</param>
        /// <param name="num">Number of stars.</param>
        public StarSky( float width, float height, int num )
        {
            _width = width;
            _height = height;
            _numStars = num;
        }

        /// <summary>
        /// Blink some stars.
        /// </summary>
        public void BlinkStars()
        {
            BlinkStar();
            if ( _rand.Next( 100 ) < 50 )
            {
                BlinkStar();
            }
            if ( _rand.Next( 100 ) < 20 )
            {
                BlinkStar();
            }
            if ( _rand.Next( 100 ) < 5 )
            {
                BlinkStar();
            }
        }

        /// <summary>
        /// Render current star sky on given canvas.
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public StarSky Render( Graphics g )
        {
            foreach ( Star star in Stars )
            {
                star.Render( g );
            }

            return this;
        }

        /// <summary>
        /// Replace single star with another one.
        /// </summary>
        private void BlinkStar()
        {
            int i = _rand.Next( _numStars );
            Stars[ i ] = GetStar();
        }


        /// <summary>
        /// Gets array of stars currently present on a sky.
        /// </summary>
        protected Star[] Stars
        {
            get
            {
                if ( _stars == null )
                {
                    // Lazy initialize the stars
                    _stars = new Star[_numStars];

                    for ( int i = 0; i < _numStars; ++i )
                    {
                        Star s = GetStar();
                        _stars[ i ] = s;
                    }
                }

                return _stars;
            }
        }
        
        /// <summary>
        /// Create new star.
        /// </summary>
        /// <returns></returns>
        private Star GetStar()
        {
            int x = _rand.Next((int)_width);
            int y = _rand.Next((int)_height);
            Color c = _rand.Next(100) < 70 ? Color.LightPink : Color.LightCyan;

            return new Star(x, y, c, 1 + _rand.Next(200) / 200f);
        }
    }
}