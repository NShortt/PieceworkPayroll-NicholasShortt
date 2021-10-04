/*
 *  File:   MainWindow.xaml.cs
 *  Author: Nicholas Shortt
 *  Last    Modified: October 04, 2021
 *  
 *  Description: A form used as data entry and pay calulcations for
 *      a piecework payroll.  Users will enter a worker name and
 *      number of messages sent this week in.  The form will then 
 *      caluclates their pay based on rates deside by number of
 *      messages sent.  Then calculated info is dispalyed and running 
 *      totals are updated.      
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using PayrollDemo;

namespace PieceworkPayroll_NicholasShortt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Attemps to calculate and display the appropirate data to the form
        /// </summary>
        private void CalculateClick(object sender, RoutedEventArgs e)
        {
            // Clean up the old errors
            ClearErrorMessages();

            // Check if value was entered for messages
            if (textBoxMessagesSent.Text == "")
            {
                labelMessageError.Content = "You must enter the number of messages sent.";
                // Highlight message textbox
                textBoxMessagesSent.BorderBrush = Brushes.Red;
                // Focus on text box
                textBoxMessagesSent.Focus();
            }
            // Check if value was entered for messages
            else if (textBoxWorkerName.Text == "")
            {
                labelMessageError.Content = "You must enter the name of the worker.";
                // Highlight message textbox
                textBoxWorkerName.BorderBrush = Brushes.Red;
                // Focus on text box
                textBoxWorkerName.Focus();
            }
            else
            {
                try
                {
                    // Create worker
                    PieceworkWorker worker = new PieceworkWorker(textBoxWorkerName.Text, textBoxMessagesSent.Text);

                    // Check if the worker has pay
                    if (worker.Pay == 0)
                    {
                        // Set focus to first entry if not
                        textBoxWorkerName.Focus();
                        textBoxWorkerName.SelectAll();
                    }
                    else
                    {
                        // Displays data
                        textBoxPay.Text = worker.Pay.ToString("c");

                        // Disable input and focus on clear
                        textBoxWorkerName.IsReadOnly = true;
                        textBoxMessagesSent.IsReadOnly = true;
                        buttonCalculate.IsEnabled = false;
                        buttonClear.Focus();
                    }
                }
                catch (ArgumentOutOfRangeException exception)
                {

                    // Dispaly error in message error label
                    labelMessageError.Content = exception.Message;
                    // Highlight message textbox
                    textBoxMessagesSent.BorderBrush = Brushes.Red;
                    // Focus on text box
                    textBoxMessagesSent.Focus();
                }
                catch (ArgumentException exception)
                {
                    // Dispaly error in name error label
                    labelNameError.Content = exception.Message;
                    // Highlight name textbox
                    textBoxWorkerName.BorderBrush = Brushes.Red;
                    // Focus on text box
                    textBoxWorkerName.Focus();
                }
            }
        }

        /// <summary>
        /// Set entry area to it's default state, and refocus on first field
        /// </summary>
        private void ClearClick(object sender, RoutedEventArgs e)
        {
            // Clear error messages
            ClearErrorMessages();

            // Clear data fields
            textBoxWorkerName.Clear();
            textBoxMessagesSent.Clear();
            textBoxPay.Clear();
           
            // Enable input and focus on first entry
            textBoxWorkerName.IsReadOnly = false;
            textBoxMessagesSent.IsReadOnly = false;
            buttonCalculate.IsEnabled = true;
            textBoxWorkerName.Focus();
        }

        /// <summary>
        /// Me close form.
        /// </summary>
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SummaryClick(object sender, RoutedEventArgs e)
        {
            PayrollSummary summary = new PayrollSummary();
            summary.ShowDialog();
        }

        /// <summary>
        /// Removes the error messages and their highlights
        /// </summary>
        private void ClearErrorMessages()
        {
            // Set entry box boarder back to default
            textBoxWorkerName.BorderBrush = textBoxPay.BorderBrush;
            textBoxMessagesSent.BorderBrush = textBoxPay.BorderBrush;

            // Remove error messages
            labelNameError.Content = "";
            labelMessageError.Content = "";
        }
    }
}
