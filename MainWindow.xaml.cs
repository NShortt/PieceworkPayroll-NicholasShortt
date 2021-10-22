/*
 *  File:   MainWindow.xaml.cs
 *  Author: Nicholas Shortt
 *  Last    Modified: October 19, 2021
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
using System.Windows.Threading;

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
            
            // Create Timer for live clock
            DispatcherTimer updateClock = new DispatcherTimer();
            // Run the ClockTick event handler every tick
            updateClock.Tick += ClockTick;
            // Set tick intervals for 1 second
            updateClock.Interval = TimeSpan.FromSeconds(1);
            // Start timer
            updateClock.Start();

            // Set current time and date on label to now
            labelDate.Content = DateTime.Now; 
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
                PieceworkWorker worker = new PieceworkWorker(textBoxWorkerName.Text.Trim().Split(" "), textBoxMessagesSent.Text, textBoxWorkerID.Text);

                // Displays data
                textBoxPay.Text = worker.Pay.ToString("c");
                UpdateStatus("Worker " + worker.Name + " entered with " + worker.Messages + " messages and pay of " + worker.Pay.ToString("c"));

                // Disable input and focus on clear
                textBoxWorkerName.IsReadOnly = true;
                textBoxMessagesSent.IsReadOnly = true;
                textBoxWorkerID.IsReadOnly = true;
                buttonCalculate.IsEnabled = false;
                buttonClear.Focus();               
            }
            catch (ArgumentException exception)
            {
                // Dispaly error message based on paramater name of exception
                if (exception.ParamName == PieceworkWorker.MessagesParameter)
                {
                    labelMessageError.Content = exception.Message;
                    HighlightTextbox(textBoxMessagesSent);
                    UpdateStatus("Invalid entry for message");
                }
                else if (exception.ParamName == PieceworkWorker.NameParameter)
                {
                    labelNameError.Content = exception.Message;
                    HighlightTextbox(textBoxWorkerName);
                    UpdateStatus("Invalid entry for name");
                }
                else if (exception.ParamName == PieceworkWorker.IDParameter)
                {
                    labelIDError.Content = exception.Message;
                    HighlightTextbox(textBoxWorkerID);
                    UpdateStatus("Invalid entry for ID");
                }

            }     
            catch (Exception error)
            {
                MessageBox.Show("An unexpected error has occured! Please contact developer Nicholas Shortt or Kyle Champman for assistance." +
                    "\nMessage: " + error.Message +
                    "\n Source: " + error.Source +
                    "\n\nStack Trace: " + error.StackTrace);
                UpdateStatus("Unknown Error in " + error.Source);
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
            textBoxWorkerID.Clear();
            textBoxPay.Clear();
           
            // Enable input and focus on first entry
            textBoxWorkerName.IsReadOnly = false;
            textBoxMessagesSent.IsReadOnly = false;
            textBoxWorkerID.IsReadOnly = false;
            buttonCalculate.IsEnabled = true;
            textBoxWorkerID.Focus();

            UpdateStatus("Cleared all fields");
        }

        /// <summary>
        /// Me close form.
        /// </summary>
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to exit the application?", "Confirm Exit", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        /// <summary>
        /// When the TabControl's selection is changed, respond based on which tab it was changed to.
        /// </summary>
        private void TabChange(object sender, SelectionChangedEventArgs e)
        {
            // If payroll entry tab is selected, Focus on the clear button
            if (tabControlInterface.SelectedItem == tabPayrollEntry)
            {
                buttonClear.Focus();
                UpdateStatus("Viewing payroll entry interface");
            }
            // Else if summary tab is selected, Display the summary values
            else if (tabControlInterface.SelectedItem == tabSummary)
            {
                DisplaySummaryValues();
                UpdateStatus("Viewing summary data interface");
            }
            // Else if employee list tab is selected, Populate it from the database
            else if (tabControlInterface.SelectedItem == tabEmployeeList)
            {
                dataGridEmployees.ItemsSource = PieceworkWorker.AllWorkers.DefaultView;
                UpdateStatus("Viewing employee list interface");
            }
            else if (tabControlInterface.SelectedItem == tabEmployeeEntries)
            {
                comboBoxEmployee.ItemsSource = PieceworkWorker.AllWorkers.DefaultView;
                comboBoxEmployee.DisplayMemberPath = "Name";
                comboBoxEmployee.SelectedValuePath = "ID";
                dataGridEntries.ItemsSource = null;
                UpdateStatus("Viewing entries list interface");
            }
        }

        /// <summary>
        /// When the selected item in comboBoxEmployee changes
        /// </summary>
        private void SelectedEmployeeChanged(object sender, SelectionChangedEventArgs e)
        {
            // If the selected item is not null
            if (comboBoxEmployee.SelectedItem != null)
            {
                // Change the data grid source to selected employee
                dataGridEntries.ItemsSource = PieceworkWorker.GetWorkerEntries((int)comboBoxEmployee.SelectedValue).DefaultView;
            }

            // Set handled to true to prevent SelectionChangeEventArgs from triggering TabChange event
            e.Handled = true;
        }

        /// <summary>
        /// When the comboBoxEmployee closes update the status bar
        /// </summary>
        private void SelectedEmployeeChanged(object sender, EventArgs e)
        {
            // If the Selected item is not null
            if (comboBoxEmployee.SelectedItem != null)
            {
                // Update status
                UpdateStatus("Viewing " + comboBoxEmployee.Text + "'s entries");
            }
        }

        /// <summary>
        /// Everytime the timer tickers update the clock to the current time
        /// </summary>
        private void ClockTick(object sender, EventArgs e)
        {
            labelDate.Content = DateTime.Now;
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
            textBoxWorkerID.BorderBrush = background;

            // Set entry box background back to default
            textBoxWorkerName.Background = background;
            textBoxMessagesSent.Background = background;
            textBoxWorkerID.Background = background;

            // Remove error messages
            labelNameError.Content = String.Empty;
            labelMessageError.Content = String.Empty;
            labelIDError.Content = String.Empty;
        }

        /// <summary>
        /// Highlight and select given textbox
        /// </summary>
        /// <param name="textbox">Textbox you wish to higlight</param>
        private void HighlightTextbox(TextBox textbox)
        {
            textbox.BorderBrush = Brushes.Red;
            textbox.Background = Brushes.LightPink;

            textbox.SelectAll();
            textbox.Focus();
        }

        /// <summary>
        /// Sets the values displayed on the summary form
        /// </summary>
        private void DisplaySummaryValues()
        {
            textBoxTotalWorkers.Text = PieceworkWorker.TotalWorkers.ToString();
            textBoxTotalMessages.Text = PieceworkWorker.TotalMessages.ToString();
            textBoxTotalPay.Text = PieceworkWorker.TotalPay.ToString("c");
            textBoxAveragePay.Text = PieceworkWorker.AveragePay.ToString("c");
        }

        /// <summary>
        /// Changes the status message for the status bar
        /// </summary>
        /// <param name="status">Status message you wish to show</param>
        private void UpdateStatus(string status)
        {
            labelStatus.Content = status;
        }



        #endregion


    }
}
