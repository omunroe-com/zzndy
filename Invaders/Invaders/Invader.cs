using System.Drawing;

namespace Invaders
{
    internal class Invader : PixelMovie
    {
        private readonly int _width;
        private readonly int _height;

        private static Invader _levelOne;
        private static Invader _levelTwo;
        private static Invader _levelThree;
        private static Invader _paddle;
        private static Invader _projectile;

        protected Invader( int width, int height )
        {
            _width = width;
            _height = height;
        }

        public int Width
        {
            get
            {
                return _width;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
        }

        public static Invader LevelOne
        {
            get
            {
                if ( _levelOne == null )
                {
                    _levelOne = MakeLevelOne();
                }
                return _levelOne;
            }
        }

        public static Invader LevelTwo
        {
            get
            {
                if ( _levelTwo == null )
                {
                    _levelTwo = MakeLevelTwo();
                }
                return _levelTwo;
            }
        }


        public static Invader LevelThree
        {
            get
            {
                if ( _levelThree == null )
                {
                    _levelThree = MakeLevelThree();
                }
                return _levelThree;
            }
        }

        public static Invader Paddle
        {
            get
            {
                if ( _paddle == null )
                {
                    bool[,] body
                        = new[,]
                              {
                                  { false, false, false, false, false, false, false, true },
                                  { false, false, false, false, false, false, true, true },
                                  { false, false, false, false, false, false, true, true },
                                  { false, true, true, true, true, true, true, true },
                                  { true, true, true, true, true, true, true, true },
                                  { true, true, true, true, true, true, true, true },
                                  { true, true, true, true, true, true, true, true },
                                  { true, true, true, true, true, true, true, true },
                              };

                    _paddle = MakeInvader( Color.Green, true, body );
                }

                return _paddle;
            }
        }

        public static Invader Projectile
        {
            get
            {
                if ( _projectile == null )
                {
                    bool[,] body
                        = new[,]
                              {
                                  { true },
                                  { true },
                                  { true },
                                  { true },
                                  { true }
                              };


                    _projectile = MakeInvader( Color.Silver, true, body );
                }
                return _projectile;
            }
        }


        private static Invader MakeLevelOne()
        {
            Color color = Color.FromArgb( 255, 0xe6, 0x08, 0xa7 );

            // half of invader profile
            bool[,] body
                = new[,]
                      {
                          { false, false, false, false, true, true },
                          { false, true, true, true, true, true },
                          { true, true, true, true, true, true },
                          { true, true, true, false, false, true },
                          { true, true, true, true, true, true },
                          { false, false, false, true, true, false },
                          { false, false, true, true, false, true },
                          { true, true, false, false, false, false }
                      };

            bool[,] body1
                = new[,]
                      {
                          { false, false, false, false, true, true },
                          { false, true, true, true, true, true },
                          { true, true, true, true, true, true },
                          { true, true, true, false, false, true },
                          { true, true, true, true, true, true },
                          { false, false, true, true, true, false },
                          { false, true, true, false, false, true },
                          { false, false, true, true, false, false }
                      };

            return MakeInvader( color, false, body, body1 );
        }

        private static Invader MakeLevelTwo()
        {
            //06b8ff
            Color color = Color.FromArgb( 255, 0x06, 0xb8, 0xff );

            bool[,] btrue =
                new[,]
                    {
                        { false, false, true, false, false, false },
                        { true, false, false, true, false, false },
                        { true, false, true, true, true, true },
                        { true, true, true, false, true, true },
                        { true, true, true, true, true, true },
                        { false, true, true, true, true, true },
                        { false, false, true, false, false, false },
                        { false, true, false, false, false, false }
                    };

            bool[,] b2 =
                new[,]
                    {
                        { false, false, true, false, false, false },
                        { false, false, false, true, false, false },
                        { false, false, true, true, true, true },
                        { false, true, true, false, true, true },
                        { true, true, true, true, true, true },
                        { true, true, true, true, true, true },
                        { true, false, true, false, false, false },
                        { false, false, false, true, true, false }
                    };

            return MakeInvader( color, false, btrue, b2 );
        }

        private static Invader MakeLevelThree()
        {
            Color color = Color.FromArgb( 255, 0xa6, 0xff, 0x06 );

            bool[,] btrue =
                new[,]
                    {
                        { false, false, false, false, false, true },
                        { false, false, false, false, true, true },
                        { false, false, false, true, true, true },
                        { false, false, true, true, false, true },
                        { false, false, true, true, true, true },
                        { false, false, false, false, true, false },
                        { false, false, false, true, false, true },
                        { false, false, true, false, true, false }
                    };

            bool[,] b2 =
                new[,]
                    {
                        { false, false, false, false, false, false },
                        { false, false, false, false, true, true },
                        { false, false, false, true, true, true },
                        { false, false, true, true, false, true },
                        { false, false, true, true, true, true },
                        { false, false, false, true, false, true },
                        { false, false, true, false, false, false },
                        { false, false, false, true, false, false }
                    };

            return MakeInvader( color, false, btrue, b2 );
        }

        protected static Invader MakeInvader( Color color, bool widthIsOdd, params bool[][,] bodies )
        {
            int height = bodies[ 0 ].GetLength( 0 );
            int width = bodies[ 0 ].GetLength( 1 );
            Invader invader = new Invader( width * 2 - ( widthIsOdd ? 1 : 0 ), height );

            foreach ( bool[,] b in bodies )
            {
                Color[,] body = new Color[invader.Height,invader.Width];

                for ( int i = 0; i < invader.Height; ++i )
                {
                    for ( int j = 0; j < invader.Width; ++j )
                    {
                        bool cellSet = b[ i, j < width ? j : 2 * width - j - 1 - ( widthIsOdd ? 1 : 0 ) ];
                        body[ i, j ] = cellSet ? color : Color.Transparent;
                    }
                }

                invader.AddFrame( body );
            }

            return invader;
        }

        public bool Overlaps( int dx, int dy, Invader invader )
        {
            bool yes = false;

            for ( int i = 0; i < _height; ++i )
            {
                for ( int j = 0; j < _width; ++j )
                {
                    int ti = i + dy;
                    int tj = j + dx;
                    if ( ti > 0 && tj > 0 && ti < invader._height && tj < invader._width &&
                         _frames[ 0 ][ i, j ] != Color.Transparent && invader._frames[ 0 ][ ti, tj ] != Color.Transparent )
                    {
                        yes = true;
                        break;
                    }
                }
            }

            return yes;
        }
    }
}