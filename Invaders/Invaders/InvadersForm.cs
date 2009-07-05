using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Invaders
{
    public partial class InvadersForm : Form
    {
        private readonly InvadersController _invaders;

        public InvadersForm()
        {
            InitializeComponent();
            SetStyle( ControlStyles.AllPaintingInWmPaint, true );
            SetStyle( ControlStyles.OptimizedDoubleBuffer, true );

            _invaders = new InvadersController( this, ClientRectangle.Width, ClientRectangle.Height );
        }

        protected override void OnPaint( PaintEventArgs e )
        {
            Graphics g = e.Graphics;
            //g.SmoothingMode = SmoothingMode.AntiAlias;
            //g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            _invaders.RenderBackground( g )
                .RenderInvaders( g )
                .RenderPaddle( g )
                .RenderScore( g );
        }

        private void InvadersForm_KeyDown( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.Escape )
            {
                Close();
            }


            Command comm;
            switch ( e.KeyCode )
            {
                case Keys.Left:
                    comm = Command.Left;
                    break;
                case Keys.Right:
                    comm = Command.Right;
                    break;
                case Keys.Space:
                case Keys.Control:
                    comm = Command.Fire;
                    break;
                default:
                    comm = Command.None;
                    break;
            }

            _invaders.AddKey( comm );
        }
    }
}