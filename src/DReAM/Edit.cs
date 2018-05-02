using System;
using System.Drawing;
using System.Windows.Forms;
using ScintillaNET;

namespace DReAM
{
    public partial class Edit : Form
    {
        private bool drag = false;
        private int mousex = 0;
        private int mousey = 0;

        public Edit()
        {
            InitializeComponent();
        }

        Scintilla TextArea;

        private void Edit_Load(object sender, EventArgs e)
        {
            // CREATE CONTROL
            TextArea = new ScintillaNET.Scintilla();
            textPanel.Controls.Add(TextArea);

            // BASIC CONFIG
            TextArea.Dock = DockStyle.Fill;
            TextArea.TextChanged += (this.OnTextChanged);

            // INITIAL VIEW CONFIG
            TextArea.WrapMode = WrapMode.None;
            TextArea.IndentationGuides = IndentView.LookBoth;

            // STYLING
            InitColors();

        }

        private void OnTextChanged(object sender, EventArgs e)
        {

        }

        private void InitColors()
        {

            TextArea.SetSelectionBackColor(true, IntToColor(0x114D9C));

        }

        public static Color IntToColor(int rgb)
        {
            return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        public void InvokeIfNeeded(Action action)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(action);
            }
            else
            {
                action.Invoke();
            }
        }


        private void Edit_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag)
            {
                this.Top = Cursor.Position.Y - mousey;
                this.Left = Cursor.Position.X - mousex;
            }
        }

        private void Edit_MouseDown(object sender, MouseEventArgs e)
        {
            drag = true;
            mousex = Cursor.Position.X - this.Left;
            mousey = Cursor.Position.Y - this.Top;
        }

        private void Edit_MouseUp(object sender, MouseEventArgs e)
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
