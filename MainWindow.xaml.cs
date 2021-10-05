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
using System.IO;

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

        #region "Event"

        /// <summary>
        /// Attemps to calculate and display the appropirate data to the form
        /// </summary>
        private void CalculateClick(object sender, RoutedEventArgs e)
        {
            // Clean up the old errors
            ClearErrorMessages();

            bool isFilled = true;

            // Check if value was entered for messages
            if (textBoxMessagesSent.Text == "")
            {
                labelMessageError.Content = "You must enter the number of messages sent.";
                // Highlight message textbox
                textBoxMessagesSent.BorderBrush = Brushes.Red;
                // Focus on text box
                textBoxMessagesSent.Focus();
                // Mark as not filled
                isFilled = false;
            }
            // Check if value was entered for messages
            if (textBoxWorkerName.Text == "")
            {
                labelNameError.Content = "You must enter the name of the worker.";
                // Highlight message textbox
                textBoxWorkerName.BorderBrush = Brushes.Red;
                // Focus on text box
                textBoxWorkerName.Focus();
                // Mark as not filled
                isFilled = false;
            }

            if (isFilled)
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
                        
                        // Record current time and remove colons
                        string date = DateTime.Now.ToString();
                        date = date.Replace(':', '-');

                        // String with file path
                        string filepath = @"Payroll\";
                        
                        // Check if the directory exists
                        if (!Directory.Exists(filepath))
                        {
                            // Create it if not
                            Directory.CreateDirectory(filepath);
                        }

                        // Create file access stream
                        FileStream payroll = new FileStream(filepath + date + ".txt", FileMode.Create, FileAccess.Write);
                        // Create stream writer
                        StreamWriter writer = new StreamWriter(payroll);
                        // Write payroll info into text file
                        writer.Write(date + " Worker " + worker.Name + " has been entered with " + worker.Messages +
                                        " messages and pay of " + worker.Pay.ToString("c"));

                        // Close the streams
                        writer.Close();
                        payroll.Close();
                    }
                }
                catch (ArgumentOutOfRangeException exception)
                {
                    // Dispaly error message based on paramater name of exception
                    if (exception.ParamName == "Out of Range")
                    {
                        labelMessageError.Content = "Messages sent must be between 1 and 15000.";
                    }
                    else
                    {
                        labelMessageError.Content = "Messages sent must be entered as a number.";
                    }               
                    // Highlight message textbox
                    textBoxMessagesSent.BorderBrush = Brushes.Red;
                    // Focus on text box
                    textBoxMessagesSent.Focus();
                }
                catch (ArgumentException exception)
                {
                    // Dispaly error message based on paramater name of exception
                    if (exception.ParamName == "Too Short")
                    {
                        labelNameError.Content = "Name must be atleast 2 characters long.";
                    }
                    else
                    {
                        labelNameError.Content = "Name must have at least 2 letters in it.";
                    }
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

        #endregion

        #region "Function"

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

        #endregion
    }
}
