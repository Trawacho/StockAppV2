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

namespace StockApp.Prints.Receipts
{
    /// <summary>
    /// Interaction logic for ReceiptTemplate.xaml
    /// </summary>
    public partial class ReceiptTemplate : UserControl
    {
        public ReceiptTemplate()
        {
            InitializeComponent();
        }
        public ReceiptTemplate(ReceiptTemplateViewModel viewModel):this()
        {
                DataContext = viewModel;
        }
    }
}
