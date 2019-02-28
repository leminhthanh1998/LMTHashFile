using System;
using System.Collections.Generic;
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
using MahApps.Metro.Controls;

namespace LMTHashFiles
{
    /// <summary>
    /// Interaction logic for Property.xaml
    /// </summary>
    public partial class Property : MetroWindow
    {
        public Property(string filename, string md5, string sha1, string sha256, string path, string filesize, string fileType, DateTime dateCreate, DateTime dateModify)
        {
            InitializeComponent();
            TextBoxName.Text = filename;
            TextBoxMD5.Text = md5;
            TextBoxSHA1.Text = sha1;
            TextBoxSHA256.Text = sha256;
            TextBoxPath.Text = path;
            TextBoxSize.Text = filesize;
            TextBoxFileType.Text = fileType;
            TextBoxDateCreate.Text =dateCreate.ToLongTimeString()+" "+ dateCreate.ToLongDateString();
            TextBoxDateModify.Text =dateModify.ToLongTimeString()+" "+ dateModify.ToLongDateString();
        }
    }
}
