using System.IO;
using System.Windows;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop;
using MicroStarter.Config;

using IWshRuntimeLibrary;
namespace MicroStarter;

public class FileDropHandler(
        TabControl mainTabControl)
        : IDropTarget
    {
        private readonly DefaultDropHandler _defaultDropHandler = new();

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = DragDropEffects.Move;
        }

        private static readonly Guid ClsidWshShell = new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8");

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is DataObject dataObject && dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                var dropFiles = dataObject.GetData(DataFormats.FileDrop) as string[];
                if (dropFiles != null && dropFiles.Length > 0)
                {
                    // 对拖放的文件进行处理
                    foreach (var filePath in dropFiles)
                    {
                        var tabItemData = new TabItemViewModel();
                        if (Path.GetExtension(filePath) == ".lnk")
                        {
                            dynamic objWshShell = Activator.CreateInstance(Type.GetTypeFromCLSID(ClsidWshShell));
                            var objShortcut = objWshShell?.CreateShortcut(filePath) as IWshShortcut;
                            tabItemData.ItemPath = objShortcut?.TargetPath;
                            string fileName = Path.GetFileNameWithoutExtension(objShortcut?.FullName);
                            tabItemData.ItemName = fileName;
                        }
                        else
                        {
                            var fileName = Path.GetFileNameWithoutExtension(filePath);
                            tabItemData.ItemName = fileName;
                            tabItemData.ItemPath = filePath;
                        }

                        if (ConfigManager.GetInstance().AddTabItemData(mainTabControl.SelectedIndex, tabItemData))
                        {
                            //添加到列表里
                            MainWindow.SetupTargetPathData(tabItemData);
                            MainWindow.SetupTargetIconSource(tabItemData);
                        }
                    }

                    ConfigManager.GetInstance().SaveConfig();
                }
            }
            else
            {
                _defaultDropHandler.Drop(dropInfo);
            }
        }
    }
