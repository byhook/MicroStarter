using System.IO;
using System.Windows.Forms;
using WinStarter;
using static System.Windows.Forms.LinkLabel;

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

                ImageList imageList = new ImageList();
                imageList.ImageSize = new Size(48,48);
                imageList.ColorDepth = ColorDepth.Depth32Bit;
            
                tabListView.View = View.LargeIcon;
                tabListView.LargeImageList = imageList;

                if(!(tabData.TabItemDatas is null))
                {
                    tabListView.BeginUpdate();
                    foreach (TabItemData tabItemData in tabData.TabItemDatas)
                    {
                        Icon icon = IconManager.GetInstance().getTargetIcon(tabItemData.ItemPath);
                        imageList.Images.Add(icon);
                        ListViewItem item = new ListViewItem(tabItemData.ItemName);
                        item.ImageIndex = tabListView.Items.Count;
                        tabListView.Items.Add(item);
                    }
                    tabListView.EndUpdate();
                }

                mainTabControl.TabPages.Add(mainPage);
            }
        }

        public static readonly Guid CLSID_WSH_SHELL = new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8");
        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] dropFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            // 对拖放的文件进行处理
            foreach (string filePath in dropFiles)
            {
                var tabItemData = new TabItemData();
                if (Path.GetExtension(filePath) == ".lnk")
                {
                    dynamic objWshShell = Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_WSH_SHELL));
                    var objShortcut = objWshShell.CreateShortcut(filePath);
                    tabItemData.ItemPath = objShortcut.TargetPath;
                    string fileName = Path.GetFileName(objShortcut.TargetPath);
                    tabItemData.ItemName = fileName;
                } else
                {
                    string fileName = Path.GetFileName(filePath);
                    tabItemData.ItemName = fileName;
                    tabItemData.ItemPath = filePath;
                }
                ConfigManager.GetInstance().AddTabItemData(mainTabControl.SelectedIndex, tabItemData);
            }
            ConfigManager.GetInstance().SaveConfig();
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
    }
}