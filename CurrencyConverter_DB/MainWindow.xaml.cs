using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
using System.Text.RegularExpressions;

namespace CurrencyConverter_DB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        SqlConnection connection;
        SqlCommand command;
        SqlDataAdapter adapter;

        private int currencyId = 0;
        private double fromAmount = 0;
        private double toAmount = 0;
        public MainWindow()
        {
            InitializeComponent();
            CreateDBConnection();
            ClearControls();
            BindCurrency();
            GetData();
        }

        public void CreateDBConnection()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string databasePath = System.IO.Path.Combine(baseDirectory, "Database\\CurrencyConverter.mdf");
            string connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={databasePath};Integrated Security=True";
            connection = new SqlConnection(connectionString);
        }

        public void GetData()
        {
            DataTable dt = new DataTable();
            command = new SqlCommand("SELECT * FROM Currency_Master", connection);
            command.CommandType = CommandType.Text;

            adapter = new SqlDataAdapter(command);

            adapter.Fill(dt);

            if (dt != null && dt.Rows.Count > 0)
            {
                dgvCurrency.ItemsSource = dt.DefaultView;
            }
            else
            {
                dgvCurrency.ItemsSource = null;
            }
        }

        private void ClearControls()
        {
            try
            {
                txtCurrency.Text = string.Empty;

                if (cmbFromCurrency.Items.Count > 0)
                {
                    cmbFromCurrency.SelectedIndex = 0;
                }

                if (cmbToCurrency.Items.Count > 0)
                {
                    cmbToCurrency.SelectedIndex = 0;
                }

                lblCurrency.Content = string.Empty;

                txtCurrency.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //public void mycon()
        //{
        //    string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        //    connection = new SqlConnection(connectionString);
        //    connection.Open();
        //}

        private void BindCurrency()
        {
            DataTable dt = new DataTable();
            //mycon();
            command = new SqlCommand("SELECT Id, CurrencyName FROM Currency_Master", connection);
            command.CommandType = CommandType.Text;

            adapter = new SqlDataAdapter(command);

            adapter.Fill(dt);
            //connection.Close();

            DataRow dataRow = dt.NewRow();
            dataRow["Id"] = 0;
            dataRow["CurrencyName"] = "--SELECT--";

            dt.Rows.InsertAt(dataRow, 0);

            if (dt != null && dt.Rows.Count > 0)
            {
                cmbFromCurrency.ItemsSource = dt.DefaultView;
                cmbToCurrency.ItemsSource = dt.DefaultView;
            }

            cmbFromCurrency.DisplayMemberPath = "CurrencyName";
            cmbFromCurrency.SelectedValuePath = "Id";
            cmbFromCurrency.SelectedIndex = 0;

            cmbToCurrency.DisplayMemberPath = "CurrencyName";
            cmbToCurrency.SelectedValuePath = "Id";
            cmbToCurrency.SelectedIndex = 0;
        }

        private void ClearMaster()
        {
            try
            {
                txtAmount.Text = string.Empty;
                txtCurrencyName.Text = string.Empty;
                btnSave.Content = "Save";
                GetData();
                currencyId = 0;
                BindCurrency();
                txtAmount.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^(0-9)\x2E]");

            e.Handled = regex.IsMatch(e.Text);
        }

        private void cmbFromCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbFromCurrency.SelectedValue != null && int.Parse(cmbFromCurrency.SelectedValue.ToString()) != 0 && cmbFromCurrency.SelectedIndex != 0)
                {
                    int CurrencyFromId = int.Parse(cmbFromCurrency.SelectedValue.ToString());
                    //mycon();
                    DataTable dt = new DataTable();
                    command = new SqlCommand("SELECT Amount FROM Currency_Master WHERE Id = @CurrencyFromId", connection);
                    command.Parameters.AddWithValue("@CurrencyFromId", CurrencyFromId);
                    command.CommandType = CommandType.Text;

                    adapter = new SqlDataAdapter(command);

                    adapter.Fill(dt);
                    //connection.Close();

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        fromAmount = double.Parse(dt.Rows[0]["Amount"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmbFromCurrency_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.SystemKey == Key.Enter)
            {
                cmbFromCurrency_SelectionChanged(sender, null);
            }
        }

        private void cmbToCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbToCurrency.SelectedValue != null && int.Parse(cmbToCurrency.SelectedValue.ToString()) != 0 && cmbToCurrency.SelectedIndex != 0)
                {
                    int CurrencyToId = int.Parse(cmbToCurrency.SelectedValue.ToString());

                    DataTable dt = new DataTable();
                    //mycon();
                    command = new SqlCommand("SELECT Amount FROM Currency_Master WHERE Id = @CurrencyToId", connection);
                    command.Parameters.AddWithValue("@CurrencyToId", CurrencyToId);
                    command.CommandType = CommandType.Text;

                    adapter = new SqlDataAdapter(command);

                    adapter.Fill(dt);
                    //connection.Close();

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        toAmount = double.Parse(dt.Rows[0]["Amount"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmbToCurrency_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.SystemKey == Key.Enter)
            {
                cmbToCurrency_SelectionChanged(sender, null);
            }
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            double ConvertedValue;

            if (string.IsNullOrWhiteSpace(txtCurrency.Text))
            {
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCurrency.Focus();
            }
            else if (cmbFromCurrency.SelectedValue is null || cmbFromCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbFromCurrency.Focus();
            }
            else if (cmbToCurrency.SelectedValue is null || cmbToCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbToCurrency.Focus();
            }
            else
            {
                if (cmbFromCurrency.SelectedValue.ToString().Equals(cmbToCurrency.SelectedValue.ToString()))
                {
                    ConvertedValue = double.Parse(txtCurrency.Text);
                }
                else
                {
                    ConvertedValue = toAmount * double.Parse(txtCurrency.Text) / fromAmount;
                }

                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtAmount.Text))
                {
                    MessageBox.Show("Please enter amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtAmount.Focus();
                }
                else if (string.IsNullOrWhiteSpace(txtCurrencyName.Text))
                {
                    MessageBox.Show("Please enter currency name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrencyName.Focus();
                }
                else
                {
                    if (currencyId > 0)
                    {
                        if (MessageBox.Show("Are you sure you want to Update ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            DataTable dt = new DataTable();
                            //mycon();
                            connection.Open();
                            command = new SqlCommand("UPDATE Currency_Master SET Amount = @Amount, CurrencyName = @CurrencyName WHERE Id = @Id", connection);
                            command.CommandType = CommandType.Text;
                            command.Parameters.AddWithValue("@Id", currencyId);
                            command.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            command.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            command.ExecuteNonQuery();
                            connection.Close();

                            MessageBox.Show("Data Updated successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else if (MessageBox.Show("Are you sure you want to save?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        //mycon();
                        connection.Open();
                        command = new SqlCommand("INSERT INTO Currency_Master(Amount, CurrencyName) VALUES(@Amount, @CurrencyName)", connection);
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@Amount", txtAmount.Text);
                        command.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                        command.ExecuteNonQuery();
                        connection.Close();

                        MessageBox.Show("Data saved successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    ClearMaster();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearMaster();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                //DataGrid dataGrid = (DataGrid)sender;

                DataRowView row_selected = dgvCurrency.CurrentItem as DataRowView;

                if (!(row_selected is null))
                {
                    if (dgvCurrency.Items.Count > 0)
                    {
                        if (dgvCurrency.SelectedCells.Count > 0)
                        {
                            currencyId = Int32.Parse(row_selected["Id"].ToString());

                            //if DisplayIndex == zero, it is an Edit cell
                            if (dgvCurrency.SelectedCells[0].Column.DisplayIndex == 0)
                            {
                                txtAmount.Text = row_selected["Amount"].ToString();
                                txtCurrencyName.Text = row_selected["CurrencyName"].ToString();
                                btnSave.Content = "Update";
                            }
                            //if DisplayIndex == 1, it is a Delete cell                    
                            else if (dgvCurrency.SelectedCells[0].Column.DisplayIndex == 1)
                            {
                                if (MessageBox.Show("Are you sure you want to delete?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    connection.Open();
                                    command = new SqlCommand("DELETE FROM Currency_Master WHERE Id = @Id", connection);
                                    command.CommandType = CommandType.Text;
                                    command.Parameters.AddWithValue("@Id", currencyId);
                                    command.ExecuteNonQuery();
                                    connection.Close();

                                    MessageBox.Show("Data deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                    ClearMaster();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtCurrency_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtCurrencyName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
