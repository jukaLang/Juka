using System;
using System.Windows.Forms;

namespace DReAMLegacy
{
    public partial class Compile : Form
    {
        private bool drag = false;
        private int mousex = 0;
        private int mousey = 0;

        public Compile()
        {
            InitializeComponent();
        }

        private void Compile_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag)
            {
                this.Top = Cursor.Position.Y - mousey;
                this.Left = Cursor.Position.X - mousex;
            }
        }

        private void Compile_MouseDown(object sender, MouseEventArgs e)
        {
            drag = true;
            mousex = Cursor.Position.X - this.Left;
            mousey = Cursor.Position.Y - this.Top;
        }

        private void Compile_MouseUp(object sender, MouseEventArgs e)
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

        OpenFileDialog ifd = new OpenFileDialog();
        SaveFileDialog sfd = new SaveFileDialog();

        private void input_btn_Click(object sender, EventArgs e)
        {
            if(ifd.ShowDialog() == DialogResult.OK)
            {
                input_txt.Text = ifd.FileName;
            }
        }

        private void output_btn_Click(object sender, EventArgs e)
        {
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                output_txt.Text = sfd.FileName;
            }
        }

        private void compile_btn_Click(object sender, EventArgs e)
        {

        }
    }
}
