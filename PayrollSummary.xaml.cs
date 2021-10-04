using System;
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
using System.Windows.Shapes;

using PayrollDemo;

namespace PieceworkPayroll_NicholasShortt
{
    /// <summary>
    /// Interaction logic for PayrollSummary.xaml
    /// </summary>
    public partial class PayrollSummary : Window
    {
        public PayrollSummary()
        {
            InitializeComponent();
            textBoxTotalWorkers.Text = PieceworkWorker.TotalWorkers.ToString();
            textBoxTotalMessages.Text = PieceworkWorker.TotalMessages.ToString();
            textBoxTotalPay.Text = PieceworkWorker.TotalPay.ToString("c");
            textBoxAveragePay.Text = PieceworkWorker.AveragePay.ToString("c");
            buttonExit.Focus();
        }

        /// <summary>
        /// Close the window
        /// </summary>
        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
