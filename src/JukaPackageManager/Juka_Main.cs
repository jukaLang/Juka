using System.Net;

namespace JukaPackageManager
{
    public partial class Juka_Main : Form
    {
        public Juka_Main()
        {
            InitializeComponent();
        }

        private async Task check_juka_btn_ClickAsync(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            string url = "http://jukalang.com/latest_version";
            HttpResponseMessage response = await client.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();

        }

        private void blockchain_btn_Click(object sender, EventArgs e)
        {
            BlockchainUI blockchain = new BlockchainUI();
            blockchain.Show();
        }
    }
}