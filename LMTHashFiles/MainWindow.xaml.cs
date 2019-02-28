using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IWshRuntimeLibrary;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using File = System.IO.File;


namespace LMTHashFiles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            args = App.args;
            ListViewData.AllowDrop = true;
            #region SendtoExe

            if (args != null)
            {
                foreach (var path in args)
                {
                    var att = File.GetAttributes(path);
                    if(att.HasFlag(FileAttributes.Directory))
                        dsPathFolder.Add(path);
                    else dsPathFile.Add(path);
                }
                workerHashFile.RunWorkerAsync();
            }

            #endregion

            #region VarWorker


            workerHashFile.DoWork += WorkerHashFile_DoWork;
            workerHashFile.RunWorkerCompleted += WorkerHashFile_RunWorkerCompleted;


            #endregion

            #region CheckShortcut

            if (File.Exists(shortcutPath)) CheckSendto.IsChecked = true;
            else CheckSendto.IsChecked = false;

            #endregion

            #region Command

            CommandOpenFile.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            CommandOpenFolder.InputGestures.Add(new KeyGesture(Key.F1));
            CommandOpenProcess.InputGestures.Add(new KeyGesture(Key.F2));
            CommandDelete.InputGestures.Add(new KeyGesture(Key.Delete));
            CommandSave.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));

            #endregion
        }



        #region  Variable
        BackgroundWorker workerHashFile = new BackgroundWorker();
        List<FileHash> dataList= new List<FileHash>();
        private string[] args;
        private List<string> dsPathFile= new List<string>();
        private List<string> dsPathFolder = new List<string>();
        private string folderPath;
        string MD5;
        string SHA1;
        string SHA256;
        long _size;
        string _ex;
        private string _fileName;
        private bool checkMD5;
        private bool checkSHA1;
        private bool checksha256;
        DateTime _createTime;
        DateTime _modifyTime;
        string _path;
        string exePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\" + "LMT Hash Files.exe";
        string shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.SendTo) + "//LMT Hash Files.lnk";
        public static RoutedCommand CommandOpenFile = new RoutedCommand();
        public static RoutedCommand CommandOpenFolder = new RoutedCommand();
        public static RoutedCommand CommandOpenProcess= new RoutedCommand();
        public static RoutedCommand CommandDelete= new RoutedCommand();
        public static RoutedCommand CommandSave = new RoutedCommand();
        #endregion

        #region MyWorker


        private void WorkerHashFile_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ListViewData.ItemsSource = dataList;
            ListViewData.Items.Refresh();
            TextBlockStatus.Text = ListViewData.Items.Count + " tệp";
            dsPathFolder.Clear();
            dsPathFile.Clear();
            folderPath =MD5=SHA1=SHA256= null;
            Mouse.OverrideCursor = null;
            Menu.IsEnabled = ToolBar.IsEnabled=ListViewData.IsEnabled=TextBoxSearch.IsEnabled = true;
        }

        private void WorkerHashFile_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.Invoke(() =>
                {
                    Menu.IsEnabled = ToolBar.IsEnabled=ListViewData.IsEnabled=TextBoxSearch.IsEnabled = false;
                    checkMD5 = CheckMD5.IsChecked;
                    checkSHA1 = CheckSHA1.IsChecked;
                    checksha256 = CheckSHA256.IsChecked;
                    Mouse.OverrideCursor = Cursors.Wait;
                    TextBlockStatus.Text = "Đang nạp tệp tin";

                }
            );
            if (dsPathFolder.Count > 0)
            {
                foreach (string path in dsPathFolder)
                {
                    FileInFolder(path);
                }
            }
            foreach (string file in dsPathFile)
            {
                if(checkMD5)
                    MD5=HashFile.MD5(file);
                if(checkSHA1)
                    SHA1=HashFile.SHA1(file);
                if(checksha256)
                    SHA256=HashFile.SHA256(file);
                PropertyFile(file, out _size, out _ex, out _createTime, out _modifyTime, out _path);
                dataList.Add(new FileHash() {FileName = _fileName, MD5 = MD5, SHA1 = SHA1, SHA256 = SHA256, FileType = _ex, Path = _path, Size = _size, TimeCreate = _createTime, TimeModify = _modifyTime});
                
            }
        }

        #endregion

        #region Toolbar
        
        private void ButtonAddFile_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFile();
            workerHashFile.RunWorkerAsync();
        }

        private void ButtonAddFolder_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFolder();
            workerHashFile.RunWorkerAsync();
        }

        private void ButtonAddProcess_OnClick(object sender, RoutedEventArgs e)
        {
            ProcessFile p= new ProcessFile();
            p.Owner = this;
            p.ShowDialog();
            if (p.Path != null)
            {
                dsPathFile.Add(p.Path);
                workerHashFile.RunWorkerAsync();
            }
        }

        private void ButtonVirusTotal_OnClick(object sender, RoutedEventArgs e)
        {
            VirusTotal();
        }

        private void ButtonProperty_OnClick(object sender, RoutedEventArgs e)
        {
            Properties();
        }

        private void ButtonSaveItem_OnClick(object sender, RoutedEventArgs e)
        {
            SaveItems();
        }

        private void ButtonCleanList_OnClick(object sender, RoutedEventArgs e)
        {
            dataList.Clear();
            ListViewData.ItemsSource = null;
            ListViewData.Items.Clear();
            TextBlockStatus.Text = "0 tệp";
        }

        private void ButtonDeleteFile_OnClick(object sender, RoutedEventArgs e)
        {
            DeleteFile();
        }
        private void ButtonOpenFolder_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListViewData.SelectedIndex != -1)
            {
                var path =(FileHash) ListViewData.SelectedItems[0];
                string args = string.Format("/e, /select, \"{0}\"", path.Path);

                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "explorer";
                info.Arguments = args;
                System.Diagnostics.Process.Start(info);
            }
            else this.ShowMessageAsync("Thông báo", "Bạn chưa chọn mục để xem!");

        }

        #endregion

        #region Menu

        private void MenuItemAddFile_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFile();
            workerHashFile.RunWorkerAsync();
        }

        private void MenuItemAddFolder_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFolder();
            workerHashFile.RunWorkerAsync();
        }

        private void MenuItemAddProcess_OnClick(object sender, RoutedEventArgs e)
        {
            ProcessFile p = new ProcessFile();
            p.Owner = this;
            p.ShowDialog();
            if (p.Path != null)
            {
                dsPathFile.Add(p.Path);
                workerHashFile.RunWorkerAsync();
            }
        }

        private void MenuItemCleanList_OnClick(object sender, RoutedEventArgs e)
        {
            dataList.Clear();
            ListViewData.ItemsSource = null;
            ListViewData.Items.Clear();
            TextBlockStatus.Text = "0 tệp";
        }

        private void MenuItemDeleteFile_OnClick(object sender, RoutedEventArgs e)
        {
            DeleteFile();
        }

        private void MenuItemVirusTotal_OnClick(object sender, RoutedEventArgs e)
        {
            VirusTotal();
        }

        private void MenuItemExit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItemCopyMD5_OnClick(object sender, RoutedEventArgs e)
        {
            CopyMD5();
        }

        private void MenuItemCopySHA1_OnClick(object sender, RoutedEventArgs e)
        {
            CopySHA1();
        }

        private void MenuItemCopySHA256_OnClick(object sender, RoutedEventArgs e)
        {
            CopySHA256();
        }
        private void CheckSendto_OnClick(object sender, RoutedEventArgs e)
        {
            if (File.Exists(shortcutPath))
            {
                File.Delete(shortcutPath);
            }
            else CreateShortcut();
        }

        /// <summary>
        /// Restart as admin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemRunAsAd_OnClick(object sender, RoutedEventArgs e)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = exePath;
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.Verb = "runas";
            proc.Start();
            Close();
        }

        private void MenuItemAbout_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://xn--lminhthnh-w1a7h.vn/");
        }
        #endregion

        #region Method

        /// <summary>
        /// Chon file can lay ma hash
        /// </summary>
        private void OpenFile()
        {
            OpenFileDialog dlgFileDialog = new OpenFileDialog();
            dlgFileDialog.Filter = "All file(*.*)|*.*";
            dlgFileDialog.Multiselect = true;
            bool? result = dlgFileDialog.ShowDialog();
            if (result == true)
            {
                dsPathFile = dlgFileDialog.FileNames.ToList();
            }
        }

        /// <summary>
        /// Chon thu muc de lay ma hash cua cac file
        /// </summary>
        private void OpenFolder()
        {
            FolderSelectDialog dlgFolderSelectDialog = new FolderSelectDialog();
            dlgFolderSelectDialog.Title = "Xin chọn thư mục";
            if (dlgFolderSelectDialog.ShowDialog())
            {
                folderPath = dlgFolderSelectDialog.Folder;
                dsPathFolder.Add(folderPath);
            }

        }

        /// <summary>
        /// Lay thuoc tinh cua file
        /// </summary>
        /// <param name="path"></param>
        private void PropertyFile(string path, out long _size, out string _ex, out DateTime _createTime, out DateTime _modifyTime, out string _path)
        {
            FileInfo info= new FileInfo(path);
            _fileName = info.Name;
            _size = info.Length;
            _ex = info.Extension;
            _createTime = info.CreationTime;
            _modifyTime = info.LastWriteTime;
            _path = info.FullName;
        }

        /// <summary>
        /// Lay danh sach cac file co tron thu muc
        /// </summary>
        /// <param name="path"></param>
        private void FileInFolder(string path)
        {
            try
            {
                string[] files = Directory.GetFiles(path);
                string[] folders = Directory.GetDirectories(path);
                for (int i = 0; i < files.Length; i++)
                {
                    dsPathFile.Add(files[i]);
                }
                for (int i = 0; i < folders.Length; i++)
                {
                    FileInFolder(folders[i]);
                }
            }
            catch (Exception e)
            {
                
            }
        }
        /// <summary>
        /// Tim kiem trong danh sach
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxSearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextBoxSearch.Text != "")
            {
                var result = from c in dataList
                    where c.MD5.Contains(TextBoxSearch.Text) || c.SHA256.Contains(TextBoxSearch.Text) ||
                          c.SHA1.Contains(TextBoxSearch.Text) || c.FileName.Contains(TextBoxSearch.Text)
                    select c;
                if (result != null)
                    ListViewData.ItemsSource = result.ToList();
            }
            else ListViewData.ItemsSource = dataList;
            TextBlockStatus.Text = "Đã tìm thấy " + ListViewData.Items.Count + " tệp";

        }

        /// <summary>
        /// Hien cua so property
        /// </summary>
        private void Properties()
        {
            if (ListViewData.SelectedIndex != -1)
            {
                var properties = (FileHash)ListViewData.SelectedItems[0];
                Property thuoctinh = new Property(properties.FileName, properties.MD5, properties.SHA1, properties.SHA256, properties.Path, properties.FileSize, properties.FileType, properties.TimeCreate, properties.TimeModify);
                thuoctinh.Owner = this;
                thuoctinh.Show();
            }
            else this.ShowMessageAsync("Thông báo", "Bạn chưa chọn mục để xem!");
        }

        private void SaveItems()
        {
            if (ListViewData.SelectedIndex != -1)
            {
                SaveFileDialog dlgSaveFileDialog = new SaveFileDialog();
                dlgSaveFileDialog.FileName = "LMT Hash File";
                dlgSaveFileDialog.DefaultExt = ".txt";
                dlgSaveFileDialog.Filter = "Text documents (.txt)|*.txt";
                bool? result = dlgSaveFileDialog.ShowDialog();
                if (result == true)
                {
                    var textFileHash = (FileHash)ListViewData.SelectedItems[0];
                    string[] text =
                    {
                        "Name: " + textFileHash.FileName, "MD5: " + textFileHash.MD5, "SHA1: " +
                                                                                      textFileHash.SHA1,
                        "SHA256: " + textFileHash.SHA256, "Path: " +
                                                          textFileHash.Path,
                        "Extension: " + textFileHash.FileType, "Modified Time: " + textFileHash.TimeModify
                    };
                    File.WriteAllLines(dlgSaveFileDialog.FileName, text);
                }
            }
            else this.ShowMessageAsync("Thông báo", "Bạn chưa chọn mục để lưu!");
        }

        /// <summary>
        /// Double click cac muc trong listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewData_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Properties();
        }

        private void CopyMD5()
        {
            if (ListViewData.SelectedIndex != -1)
            {
                var md5 = (FileHash) ListViewData.SelectedItems[0];
                string textMD5 = md5.MD5;
                Clipboard.SetDataObject(textMD5);
            }
            else this.ShowMessageAsync("Thông báo", "Bạn chưa chọn mục để sao chép!");
        }
        private void CopySHA1()
        {
            if (ListViewData.SelectedIndex != -1)
            {
                var sha1 = (FileHash)ListViewData.SelectedItems[0];
                string textSHA1 = sha1.SHA1;
                Clipboard.SetDataObject(textSHA1);
            }
            else this.ShowMessageAsync("Thông báo", "Bạn chưa chọn mục để sao chép!");

        }
        private void CopySHA256()
        {
            if (ListViewData.SelectedIndex != -1)
            {
                var sha256 = (FileHash)ListViewData.SelectedItems[0];
                string textSHA256 = sha256.SHA256;
                Clipboard.SetDataObject(textSHA256);
            }
            else this.ShowMessageAsync("Thông báo", "Bạn chưa chọn mục để sao chép!");

        }

        private async void DeleteFile()
        {
            if (ListViewData.SelectedIndex != -1)
            {
                var path = (FileHash)ListViewData.SelectedItems[0];
                var result = await this.ShowMessageAsync("Thông báo", "Bạn có chắc là muốn xóa tệp này không?",
                    MessageDialogStyle.AffirmativeAndNegative);
                bool delete = result == MessageDialogResult.Affirmative;
                if (delete)
                {
                    try
                    {
                        File.Delete(path.Path);
                        TextBoxSearch.Text = "";
                        dataList.RemoveAt(ListViewData.SelectedIndex);
                        ListViewData.Items.Refresh();
                    }
                    catch (Exception)
                    {
                        await this.ShowMessageAsync("Thông báo", "Đã có lỗi trong quá trình xóa tệp!");
                    }
                }
            }
            else await this.ShowMessageAsync("Thông báo", "Bạn chưa chọn mục để xóa!");
        }

        /// <summary>
        /// Mo trinh duyet vao virustotal.com
        /// </summary>
        private void VirusTotal()
        {
            if (ListViewData.SelectedIndex != -1)
            {
                var virusTotal = (FileHash)ListViewData.SelectedItems[0];
                string link = "https://www.virustotal.com/en/file/" + virusTotal.SHA256 + "/analysis/";
                System.Diagnostics.Process.Start(link);
            }
            else this.ShowMessageAsync("Thông báo", "Bạn chưa chọn mục để xem!");
        }

        /// <summary>
        /// tao shortcut cho phan mem
        /// </summary>
        private void CreateShortcut()
        {
            
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

            shortcut.Description = "MD5 & SHA Checksum";   
            shortcut.IconLocation = System.AppDomain.CurrentDomain.BaseDirectory + "\\" + "Icon.ico";           
            shortcut.TargetPath = exePath;                 
            shortcut.Save();
        }
        #endregion

        #region ContextMenu listview
        private void MenuItemListviewCopyMD5_OnClick(object sender, RoutedEventArgs e)
        {
            CopyMD5();
        }

        private void MenuItemListViewCopySHA1_OnClick(object sender, RoutedEventArgs e)
        {
            CopySHA1();
        }

        private void MenuItemListviewCopySHA256_OnClick(object sender, RoutedEventArgs e)
        {
            CopySHA256();
        }
        private void MenuItemListviewDelete_OnClick(object sender, RoutedEventArgs e)
        {
            DeleteFile();
        }
        #endregion

        /// <summary>
        /// Keo tha file vao de quet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewData_OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                ListViewData.IsEnabled = false;
                Mouse.OverrideCursor = Cursors.Wait;
                string[] dsPath = (string[]) e.Data.GetData(DataFormats.FileDrop, true);
                foreach (var path in dsPath)
                {
                    var att = File.GetAttributes(path);
                    if (att.HasFlag(FileAttributes.Directory))
                        dsPathFolder.Add(path);
                    else dsPathFile.Add(path);
                }
                workerHashFile.RunWorkerAsync();
            }
        }

        #region Shortcut

        private void CommandOpenFileExcute(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFile();
            workerHashFile.RunWorkerAsync();
        }

        private void CommandOpenFolderExcute(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFolder();
            workerHashFile.RunWorkerAsync();
        }

        private void CommandDeleteFile(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteFile();
        }

        private void CommandSaveExcute(object sender, ExecutedRoutedEventArgs e)
        {
            SaveItems();
        }
        private void CommandOpenProcessExcute(object sender, ExecutedRoutedEventArgs e)
        {
            ProcessFile p = new ProcessFile();
            p.Owner = this;
            p.ShowDialog();
            if (p.Path != null)
            {
                dsPathFile.Add(p.Path);
                workerHashFile.RunWorkerAsync();
            }
        }

        #endregion


        private void MenuItemHelp_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://lêminhthành.vn/lmt-hash-file/");
        }
    }

    public class FileHash
    {
        public string FileName { get; set; }
        public string MD5 { get; set; }
        public string SHA1 { get; set; }
        public string SHA256 { get; set; }
        public string Path { get; set; }
        public string FileType { get; set; }
        public DateTime TimeCreate { get; set; }
        public DateTime TimeModify { get; set; }
        public long Size { get; set; }
        public string FileSize
        {
            get => GetFileSize(Size);
        }
        private string GetFileSize(long size)
        {
            string result;
            double sum = (double)(size);
            if (sum >= 1048576.0 && sum < 1073741824.0)
            {
                sum /= 1047576.0;
                result = string.Format("{0:0.00}", sum) + " MB";
            }
            else if (sum >= 1073741824.0)
            {
                sum /= 1073741824.0;
                result = string.Format("{0:0.00}", sum) + " GB";
            }
            else
            {
                sum /= 1024.0;
                result = string.Format("{0:0.00}", sum) + " KB";
            }

            return result;
        }
    }

    
}
