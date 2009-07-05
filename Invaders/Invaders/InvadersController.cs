using System;
using System.Drawing;
using System.Windows.Forms;

using Timer=System.Threading.Timer;

namespace Invaders
{
    public enum Command
    {
        None,
        Left,
        Right,
        Fire
    }

    internal class InvadersController
    {
        private readonly Form _form;

        private readonly float _width;
        private readonly float _height;

        private readonly StarSky _stars;

        private Timer _invaderTimer;

        private Size _invaderSize = new Size( 17, 10 );

        private Invader[,] _invaders;
        private int _invaderStepps;
        private float _scale = 3;
        private int _invaderTimeout = 1000 / 30;

        private int _waveNo;

        private Rectangle _invadersPos = new Rectangle( 0, 0, 11, 5 );
        private Point _movement = new Point( 3, 5 );

        private int _frameCount;
        private int _framesPerStep = 8;
        private Point _paddlePos;
        private readonly Invader _paddle = Invader.Paddle;
        private int _paddleSpeed = 5;

        private Command _command = Command.None;
        private Point _projectrilePos;
        private readonly Invader _projectile = Invader.Projectile;
        private int _projectileSpeed = 6;
        private int _score;

        public InvadersController( Form form, float width, float height )
        {
            _form = form;
            _width = width;
            _height = height;

            int numStars = (int) ( _width * height / 800f );
            _stars = new StarSky( _width, _height, numStars );

            Init();


            _paddlePos = new Point( (int) ( _width / 2 / _scale ),
                                    (int) ( _height / _scale - 1.5 * _paddle.Height ) );

            _invaderTimer = new Timer( OnInvaderFrame, null, _invaderTimeout, 0 );
        }

        private void Init()
        {
            ++_waveNo;
            _invadersPos.X = 0;
            _invadersPos.Y = _waveNo * _invaderSize.Height;
            _invadersPos.Width = 11;
            _framesPerStep = 8;

            _invaders = new Invader[_invadersPos.Height,_invadersPos.Width];
            for ( int i = 0; i < _invadersPos.Height; ++i )
            {
                for ( int j = 0; j < _invadersPos.Width; ++j )
                {
#if DEBUG
                    if (
                        j == 1 || j == 2 || j == 3 || j == 4
                        || j == _invadersPos.Width - 2 || j == _invadersPos.Width - 3 || j == _invadersPos.Width - 4 ||
                        j == _invadersPos.Width - 5
                        || ( ( j == 0 || j == _invadersPos.Width - 1 ) && i < 4 ) )
                    {
                        _invaders[ i, j ] = null;
                        continue;
                    }
#endif

                    switch ( i % 5 )
                    {
                        case 1:
                        case 2:
                            _invaders[ i, j ] = Invader.LevelTwo;
                            break;
                        case 3:
                        case 4:
                            _invaders[ i, j ] = Invader.LevelOne;
                            break;
                        default:
                            _invaders[ i, j ] = Invader.LevelThree;
                            break;
                    }
                }
            }
        }

        private int MaxX
        {
            get
            {
                return (int) ( _width / _scale - _invadersPos.Width * _invaderSize.Width );
            }
        }

        /// <summary>
        /// Happens every frame.
        /// </summary>
        /// <param name="state"></param>
        private void OnInvaderFrame( object state )
        {
            if ( ( ++_frameCount % _framesPerStep ) == 0 )
            {
                ++_invaderStepps;
                if ( _invadersPos.X + _movement.X < MaxX && _invadersPos.X + _movement.X > 0 )
                {
                    _invadersPos.X += _movement.X;
                }
                else
                {
                    _movement.X = -_movement.X;
                    _invadersPos.Y += _movement.Y;
                    if ( _framesPerStep > 5 || ( _movement.Y < 0 && _framesPerStep > 1 ) )
                    {
                        --_framesPerStep;
                    }
                }
            }

            if ( _command != Command.None )
            {
                PerformCommand();
            }

            if ( !_projectrilePos.IsEmpty )
            {
                _projectrilePos.Y -= _projectileSpeed;

                int column;
                bool invaderHit = DetectHit( out column );

                if ( invaderHit && ( column == 0 || column == _invadersPos.Width - 1 ) )
                {
                    TidyInvaderArray( column );
                    if ( _invadersPos.Width == 0 )
                    {
                        Init();
                    }
                }

                if ( invaderHit || _projectrilePos.Y < 0 )
                {
                    _projectrilePos = new Point();
                }
            }

            _stars.BlinkStars();
            _form.Invalidate();

            _invaderTimer = new Timer( OnInvaderFrame, null, _invaderTimeout, 0 );
        }

        private void PerformCommand()
        {
            switch ( _command )
            {
                case Command.Fire:
                    if ( _projectrilePos.IsEmpty )
                    {
                        ReleaseProjectile();
                    }
                    break;
                case Command.Left:
                    if ( _paddlePos.X - _paddleSpeed >= 0 )
                    {
                        _paddlePos.X -= _paddleSpeed;
                    }
                    break;
                case Command.Right:
                    if ( _paddlePos.X + _paddleSpeed <= _width / _scale - _paddle.Width )
                    {
                        _paddlePos.X += _paddleSpeed;
                    }
                    break;
            }
            _command = Command.None;
        }

        /// <summary>
        /// Find out if projectali hit the invaders. If so, return true and
        /// the index of column in which hit happened.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool DetectHit( out int column )
        {
            column = -1;
            bool hit = false;

            int j = ( ( _projectrilePos.X - _invadersPos.X ) / _invaderSize.Width );
            int i = ( ( _projectrilePos.Y - _invadersPos.Y ) / _invaderSize.Height );

            if ( i >= 0 && i < _invadersPos.Height && j >= 0 && j < _invadersPos.Width )
            {
                int dx = _projectrilePos.X - _invadersPos.X - j * _invaderSize.Width;
                int dy = _projectrilePos.Y - _invadersPos.Y - i * _invaderSize.Height;

                if ( _invaders[ i, j ] != null && _projectile.Overlaps( dx, dy, _invaders[ i, j ] ) )
                {
                    hit = true;
                    column = j;
                    if ( _invaders[ i, j ] == Invader.LevelOne )
                    {
                        _score += 10;
                    }
                    else if ( _invaders[ i, j ] == Invader.LevelTwo )
                    {
                        _score += 20;
                    }
                    else if ( _invaders[ i, j ] == Invader.LevelThree )
                    {
                        _score += 40;
                    }
                    _invaders[ i, j ] = null;
                }
            }

            return hit;
        }

        /// <summary>
        /// Remove empty columns on eather side of the field.
        /// </summary>
        /// <param name="removedCol"></param>
        private void TidyInvaderArray( int removedCol )
        {
            int numRemoved = 0;
            bool columnEmpty = true;
            int col = removedCol;

            while ( columnEmpty && col >= 0 && col < _invadersPos.Width )
            {
                for ( int k = 0; k < _invadersPos.Height; ++k )
                {
                    if ( _invaders[ k, col ] != null )
                    {
                        columnEmpty = false;
                        break;
                    }
                }

                if ( columnEmpty )
                {
                    ++numRemoved;
                }

                col = col + ( removedCol == 0 ? 1 : -1 );
            }

            if ( numRemoved > 0 )
            {
                _invadersPos.Width -= numRemoved;
                Invader[,] Xinvaders = new Invader[_invadersPos.Height,_invadersPos.Width];
                for ( int i = 0; i < _invadersPos.Height; ++i )
                {
                    for ( int j = 0; j < _invadersPos.Width; ++j )
                    {
                        Xinvaders[ i, j ] = _invaders[ i, j + ( removedCol == 0 ? numRemoved : 0 ) ];
                    }
                }
                _invaders = Xinvaders;

                if ( removedCol == 0 )
                {
                    _invadersPos.X += numRemoved * _invaderSize.Width;
                }
            }
        }

        private void ReleaseProjectile()
        {
            _projectrilePos = new Point( _paddlePos.X + _paddle.Width / 2,
                                         ( _paddlePos.Y - _projectile.Height ) );
        }

        /// <summary>
        /// Render background and stars to the canvas.
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public InvadersController RenderBackground( Graphics g )
        {
            g.FillRectangle( Brushes.Black, 0, 0, _width, _height );

            _stars.Render( g );

            return this;
        }

        /// <summary>
        /// Render invaders to the canvas.
        /// </summary>
        /// <param name="graphics"></param>
        /// <returns></returns>
        public InvadersController RenderInvaders( Graphics graphics )
        {
            for ( int i = 0; i < _invadersPos.Height; ++i )
            {
                for ( int j = 0; j < _invadersPos.Width; ++j )
                {
                    Invader invader = _invaders[ i, j ];
                    if ( invader != null )
                    {
                        float x = ( _invadersPos.X + _invaderSize.Width * j ) * _scale;
                        float y = ( _invadersPos.Y + _invaderSize.Height * i ) * _scale;

                        invader.Render( graphics, x, y, _scale, _invaderStepps );
                    }
#if DEBUG
                    else
                    {
                        float x = ( _invadersPos.X + _invaderSize.Width * j ) * _scale;
                        float y = ( _invadersPos.Y + _invaderSize.Height * i ) * _scale;

                        float width = ( _invaderSize.Width - 1 ) * _scale;
                        float height = ( _invaderSize.Height - 1 ) * _scale;

                        graphics.DrawRectangle( Pens.Gray, x, y, width, height );
                        graphics.DrawLine( Pens.Gray, x, y, x + width, y + height );
                        graphics.DrawLine( Pens.Gray, x, y + height, x + width, y );
                    }
#endif
                }
            }

            return this;
        }

        public InvadersController RenderPaddle( Graphics graphics )
        {
            _paddle.Render( graphics, _paddlePos.X * _scale, _paddlePos.Y * _scale, _scale, 0 );
            if ( !_projectrilePos.IsEmpty )
            {
                _projectile.Render( graphics, _projectrilePos.X * _scale, _projectrilePos.Y * _scale, _scale, 0 );
            }

            return this;
        }

        public void AddKey( Command comm )
        {
            if ( _command == Command.None )
            {
                _command = comm;
            }
        }

        public InvadersController RenderScore( Graphics graphics )
        {
            string wave = "Wave " + _waveNo;

            string score = String.Format( "{0:0000}", _score );
            SizeF size = graphics.MeasureString( score, _form.Font );

            graphics.DrawString( wave, _form.Font, Brushes.GreenYellow, (float) ( size.Height * .2 ),
                                 (float) ( size.Height * .2 ) );
            graphics.DrawString( score, _form.Font, Brushes.GreenYellow, (float) ( _width - size.Width * 1.2 ),
                                 (float) ( size.Height * .2 ) );

            return this;
        }
    }
}