namespace MicroStarter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            TabPage mainPage = new TabPage();
            mainPage.Text = "常用工具";

            ListView tabListView = new ListView();
            tabListView.Dock = DockStyle.Fill;
            mainPage.Controls.Add(tabListView);

            mainTabControl.TabPages.Add(mainPage);

        }
    }
}