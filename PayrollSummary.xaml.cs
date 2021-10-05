/*
 *  File:   PayrollSummary.xaml.cs
 *  Author: Nicholas Shortt
 *  Last    Modified: October 04, 2021
 *  
 *  Description: A form used to display the summary data for
 *      current pay period.
 *      
 */
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
            // Get and Dispaly summary values
            UpdateValues();
            // Set focus on the clear button
            buttonExit.Focus();
        }

        #region "Event"

        /// <summary>
        /// Close the window
        /// </summary>
        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        /// <summary>
        /// Reset the current running totals for the payroll
        /// </summary>
        private void ResetClick(object sender, RoutedEventArgs e)
        {
            PieceworkWorker.TotalsRest();
            // Display default values
            UpdateValues();
        }

        #region "Function"
        private void UpdateValues()
        {
            textBoxTotalWorkers.Text = PieceworkWorker.TotalWorkers.ToString();
            textBoxTotalMessages.Text = PieceworkWorker.TotalMessages.ToString();
            textBoxTotalPay.Text = PieceworkWorker.TotalPay.ToString("c");
            textBoxAveragePay.Text = PieceworkWorker.AveragePay.ToString("c");
        }
        #endregion
    }
}
