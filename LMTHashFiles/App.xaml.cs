using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LMTHashFiles
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string[] args;
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
           if(e.Args.Length>0)
               args = e.Args;
        }
    }
}
