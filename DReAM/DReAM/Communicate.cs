using System;
using System.Windows.Forms;

namespace DReAM
{
    public partial class Communicate : Form
    {
        private bool drag = false;
        private int mousex = 0;
        private int mousey = 0;

        public Communicate()
        {
            InitializeComponent();
        }

        private void Communicate_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag)
            {
                this.Top = Cursor.Position.Y - mousey;
                this.Left = Cursor.Position.X - mousex;
            }
        }

        private void Communicate_MouseDown(object sender, MouseEventArgs e)
        {
            drag = true;
            mousex = Cursor.Position.X - this.Left;
            mousey = Cursor.Position.Y - this.Top;
        }

        private void Communicate_MouseUp(object sender, MouseEventArgs e)
        {
            drag = false;
        }

        private void minimize_btn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void close_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
