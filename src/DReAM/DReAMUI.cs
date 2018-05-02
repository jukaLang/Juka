using System;
using System.Windows.Forms;

namespace DReAM
{
    public partial class DReAMUI : Form
    {
        private bool drag = false;
        private int mousex = 0;
        private int mousey = 0;

        public DReAMUI()
        {
            InitializeComponent();
        }

        private void DReAMUI_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag)
            {
                this.Top = Cursor.Position.Y - mousey;
                this.Left = Cursor.Position.X - mousex;
            }
        }

        private void DReAMUI_MouseDown(object sender, MouseEventArgs e)
        {
            drag = true;
            mousex = Cursor.Position.X - this.Left;
            mousey = Cursor.Position.Y - this.Top;
        }

        private void DReAMUI_MouseUp(object sender, MouseEventArgs e)
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

        private void compile_btn_Click(object sender, EventArgs e)
        {
            Form compile = new Compile();
            compile.Show();
        }

        private void edit_btn_Click(object sender, EventArgs e)
        {
            Form edit = new Edit();
            edit.Show();
        }

        private void communicate_btn_Click(object sender, EventArgs e)
        {
            Form communicate = new Communicate();
            communicate.Show();
        }

        private void settings_btn_Click(object sender, EventArgs e)
        {
            Form settings = new Settings();
            settings.Show();
        }
    }
}
