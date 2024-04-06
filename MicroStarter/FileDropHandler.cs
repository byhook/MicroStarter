using System.IO;
using System.Windows;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop;
using MicroStarter.Config;

namespace MicroStarter;

public class FileDropHandler(
        TabControl mainTabControl,
        TabPageViewModel tabPageViewModel)
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
                            var objShortcut = objWshShell?.CreateShortcut(filePath);
                            tabItemData.ItemPath = objShortcut?.TargetPath;
                            string fileName = Path.GetFileName(objShortcut?.TargetPath);
                            tabItemData.ItemName = fileName;
                        }
                        else
                        {
                            var fileName = Path.GetFileName(filePath);
                            tabItemData.ItemName = fileName;
                            tabItemData.ItemPath = filePath;
                        }

                        if (ConfigManager.GetInstance().AddTabItemData(mainTabControl.SelectedIndex, tabItemData))
                        {
                            //添加到列表里
                            MainWindow.SetupTargetIconWithData(tabItemData);
                            MainWindow.SetupTargetIconSource(tabItemData);
                            tabPageViewModel.TabItemDataList.Add(tabItemData);
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
