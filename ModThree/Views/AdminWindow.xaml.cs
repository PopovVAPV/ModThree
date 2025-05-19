using ModThree.Model;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace ModThree.Views
{
    public partial class AdminWindow : Window
    {
        private string connect = "Server=(localdb)\\MSSQLLocalDB; Database=CompanyAuthDB; Integrated Security=True;";

        public AdminWindow()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            using (SqlConnection conn = new SqlConnection(connect))
            {
                conn.Open();
                string query = "SELECT Id, Username, Role, IsBlocked FROM Users";
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                usersDataGrid.ItemsSource = dt.DefaultView;
            }
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            string login = loginTextBox.Text;
            string password = passwordBox.Password;
            string role = roleComboBox.Text;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connect))
            {
                conn.Open();

                // Проверка существования пользователя
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username=@Login";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@Login", login);
                int exists = (int)checkCmd.ExecuteScalar();

                if (exists > 0)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!");
                    return;
                }

                // Добавление пользователя
                string insertQuery = "INSERT INTO Users (Username, Password, Role, FisrtIn) VALUES (@Login, @Password, @Role, 1)";
                SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@Login", login);
                insertCmd.Parameters.AddWithValue("@Password", password);
                insertCmd.Parameters.AddWithValue("@Role", role);
                insertCmd.ExecuteNonQuery();

                MessageBox.Show("Пользователь успешно добавлен!");
                LoadUsers();
            }
        }
    }
}