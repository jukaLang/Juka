using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DreamCompiler;
using System.IO;
using System.Text;

namespace Dream.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Editor : ContentPage
    {
        public Editor()
        {
            InitializeComponent();


            MemoryStream input_memory = new MemoryStream(Encoding.UTF8.GetBytes(""));
            var compiler = new Compiler();
            /*compiler.Go(input_memory);*/
        }
    }
}