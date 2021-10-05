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

            try
            {
                // Create worker
                PieceworkWorker worker = new PieceworkWorker(textBoxWorkerName.Text, textBoxMessagesSent.Text);

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
            catch (ArgumentException exception)
            {
                // Dispaly error message based on paramater name of exception
                if (exception.ParamName == PieceworkWorker.MessagesParameter)
                {
                    labelMessageError.Content = exception.Message;
                    HighlightTextbox(textBoxMessagesSent);
                }
                else if (exception.ParamName == PieceworkWorker.NameParameter)
                {
                    labelNameError.Content = exception.Message;
                    HighlightTextbox(textBoxWorkerName);
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

        /// <summary>
        /// Launches the summary form modal.
        /// </summary>
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
            // Create brushes based on textbox defualts
            Brush border = (Brush)new BrushConverter().ConvertFromString("#FFABADB3");
            Brush background = (Brush)new BrushConverter().ConvertFromString("#FFFFFFFF");

            // Set entry box boarder back to default
            textBoxWorkerName.BorderBrush = border;
            textBoxMessagesSent.BorderBrush = border;

            // Set entry box background back to default
            textBoxWorkerName.Background = background;
            textBoxMessagesSent.Background = background;

            // Remove error messages
            labelNameError.Content = String.Empty;
            labelMessageError.Content = String.Empty;
        }

        /// <summary>
        /// Highlight and select given textbox
        /// </summary>
        private void HighlightTextbox(TextBox textbox)
        {
            textbox.BorderBrush = Brushes.Red;
            textbox.Background = Brushes.LightPink;

            textbox.SelectAll();
            textbox.Focus();
        }

        #endregion
    }
}
