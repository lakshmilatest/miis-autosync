﻿using System;
using System.Collections.Generic;
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
using Lithnet.Miiserver.Autosync.UI.ViewModels;
using MahApps.Metro.Controls;

namespace Lithnet.Miiserver.Autosync.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            MainWindowViewModel m = new MainWindowViewModel();

            m.ConfigFile = new  ConfigFileViewModel(AutoSync.ConfigFile.Load("D:\\temp\\config.xml"));

            this.DataContext = m;
        }
    }
}