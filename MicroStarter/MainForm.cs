using WinStarter;

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
            var mainConfigData = ConfigManager.GetInstance().LoadConfig();
            foreach (TabData tabData in mainConfigData.TabRootData)
            {
                TabPage mainPage = new TabPage();
                mainPage.Text = tabData.TabName;

                ListView tabListView = new ListView();
                tabListView.Dock = DockStyle.Fill;
                mainPage.Controls.Add(tabListView);

                mainTabControl.TabPages.Add(mainPage);
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
    }
}