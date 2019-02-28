using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace LMTHashFiles
{
    /// <summary>
    /// Interaction logic for Process.xaml
    /// </summary>
    public partial class ProcessFile : MetroWindow
    {
        public ProcessFile()
        {
            InitializeComponent();
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.DoWork+=WorkerOnDoWork;
            Mouse.OverrideCursor = Cursors.Wait;
            worker.RunWorkerAsync();
        }

        private void WorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            foreach (var p in Process.GetProcesses())
            {
                try
                {

                    dsTienTrinh.Add(new TienTrinh() { ID = p.Id, Name = p.ProcessName, Path = p.MainModule.FileName });
                }
                catch (Exception e)
                {

                }

            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Mouse.OverrideCursor = null;
            ListViewProcess.ItemsSource = dsTienTrinh;
        }
        #region Var
        List<TienTrinh> dsTienTrinh= new List<TienTrinh>();
        private string path;
        BackgroundWorker worker= new BackgroundWorker();
#endregion
        public string Path { get => path; set => path = value; }

        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListViewProcess.SelectedIndex != -1)
            {
                var p = (TienTrinh) ListViewProcess.SelectedItems[0];
                Path = p.Path;
                Close();
            }
            else this.ShowMessageAsync("Thông báo", "Bạn chưa chọn mục để kiểm tra!");
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ListViewProcess_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListViewProcess.SelectedIndex != -1)
            {
                var p = (TienTrinh)ListViewProcess.SelectedItems[0];
                Path = p.Path;
                Close();
            }
        }

        private void TextBoxSearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextBoxSearch.Text != "")
            {
                var list = from c in dsTienTrinh
                    where c.Path.Contains(TextBoxSearch.Text) || c.ID.ToString().Contains(TextBoxSearch.Text) ||
                          c.Name.Contains(TextBoxSearch.Text)
                    select c;
                if (list != null)
                    ListViewProcess.ItemsSource = list.ToList();
            }
            else ListViewProcess.ItemsSource = dsTienTrinh;
        }
    }

    public class TienTrinh
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
