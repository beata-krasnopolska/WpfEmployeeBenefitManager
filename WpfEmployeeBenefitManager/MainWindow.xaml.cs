using System;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace WpfEmployeeBenefitManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;
        public MainWindow()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["WpfEmployeeBenefitManager.Properties.Settings.CompanyBenefitDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);

            ShowEmployees();
            ShowAllBenefits();
        }

        private void ShowEmployees()
        {
            try
            {
                string query = "select * from Employee";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable employeeTable = new DataTable();

                    sqlDataAdapter.Fill(employeeTable);

                    listEmployees.DisplayMemberPath = "NameAndSurname";
                    listEmployees.SelectedValuePath = "Id";
                    listEmployees.ItemsSource = employeeTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }            
        }

        private void ShowAssociatedBenefits()
        {
            try
            {
                string query = "select * from Benefit b inner join EmployeeBenefit " +
                    "eb on b.Id = eb.BenefitId where eb.EmployeeId = @EmployeeId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                // the SqlDataAdapter can be imagined like an Interface to make Tables usable by C#-Objects
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {

                    sqlCommand.Parameters.AddWithValue("@EmployeeId", listEmployees.SelectedValue);

                    DataTable benefitTable = new DataTable();

                    sqlDataAdapter.Fill(benefitTable);

                    //Which Information of the Table in DataTable should be shown in our ListBox?
                    listAssociatedBenefits.DisplayMemberPath = "BenefitName";
                    //Which Value should be delivered, when an Item from our ListBox is selected?
                    listAssociatedBenefits.SelectedValuePath = "Id";
                    //The Reference to the Data the ListBox should populate
                    listAssociatedBenefits.ItemsSource = benefitTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }

        private void ListEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAssociatedBenefits();
            ShowSelectedEmployeeInTextBox();
        }

        private void ShowAllBenefits()
        {
            try
            {
                string query = "select * from Benefit";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable benefitTable = new DataTable();

                    sqlDataAdapter.Fill(benefitTable);

                    listAllBenefits.DisplayMemberPath = "BenefitName";
                    listAllBenefits.SelectedValuePath = "Id";
                    listAllBenefits.ItemsSource = benefitTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void DeleteEmployee_Click (object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Employee where id = @Employee";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Employee", listEmployees.SelectedValue);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowEmployees();
            }
        }
        
        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Employee values (@NameAndSurname)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@NameAndSurname", myTextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowEmployees();
            }
        }

        private void AddBenefit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Benefit values (@BenefitName)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@BenefitName", myTextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAllBenefits();
            }
        }

        private void ShowSelectedEmployeeInTextBox()
        {
            try
            {
                string query = "select nameAndSurname from Employee where Id = @EmployeeId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                // the SqlDataAdapter can be imagined like an Interface to make Tables usable by C#-Objects
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    if (listEmployees.SelectedValue == null)
                        return;
                    sqlCommand.Parameters.AddWithValue("@EmployeeId", listEmployees.SelectedValue);

                    DataTable employeeDataTable = new DataTable();

                    sqlDataAdapter.Fill(employeeDataTable);

                    myTextBox.Text = employeeDataTable.Rows[0]["NameAndSurname"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ShowSelectedBenefitInTextBox()
        {
            try
            {
                string query = "select benefitName from Benefit where Id = @BenefitId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    if (listAllBenefits.SelectedValue == null)
                        return;
                    sqlCommand.Parameters.AddWithValue("@BenefitId", listAllBenefits.SelectedValue);

                    DataTable employeeDataTable = new DataTable();

                    sqlDataAdapter.Fill(employeeDataTable);

                    myTextBox.Text = employeeDataTable.Rows[0]["BenefitName"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void AddBenefitToEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into EmployeeBenefit values (@EmployeeId, @BenefitId)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@EmployeeId", listEmployees.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@BenefitId", listAllBenefits.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedBenefits();
            }
        }

        private void DeleteBenefit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Benefit where id = @BenefitId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@BenefitId", listAllBenefits.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAllBenefits();
            }
        }

        private void RemoveBenefit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Benefit where id = @BenefitId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@BenefitId", listAssociatedBenefits.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedBenefits();
            }
        }

        private void UpdateEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update Employee Set NameAndSurname = @NameAndSurname where Id = @EmployeeId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@EmployeeId", listEmployees.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@NameAndSurname", myTextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowEmployees();
            }
        }

        private void UpdateBenefit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update Benefit Set BenefitName = @BenefitName where Id = @BenefitId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@BenefitId", listAllBenefits.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@BenefitName", myTextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAllBenefits();
            }
        }
        private void ListAllBenefits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowSelectedBenefitInTextBox();
        }
    }
}
