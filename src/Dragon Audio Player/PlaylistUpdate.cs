using System.Windows.Forms;

namespace Dragon_Audio_Player
{
    public partial class PlaylistUpdate : Form
    {
        public PlaylistUpdate(string pText)
        {
            InitializeComponent();
            rtbxMessage.Text = pText;
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

    }
}
