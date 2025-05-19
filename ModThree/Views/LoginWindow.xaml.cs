using ModThree.Model;
using System.Data.SqlClient;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ModThree.Views
{
    public partial class LoginWindow : Window
    {
        private string connect = "Server=(localdb)\\MSSQLLocalDB; Database=CompanyAuthDB; Integrated Security=True;";
        private int attemptCount = 0;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = loginTextBox.Text;
            string password = passwordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connect))
            {
                conn.Open();
                string query = "SELECT * FROM Users WHERE Username=@Login AND Password=@Password AND IsBlocked=0";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Password", password);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    string role = reader["Role"].ToString();
                    bool firstIn = Convert.ToBoolean(reader["FisrtIn"]);

                    // Обновляем дату последнего входа
                    reader.Close();
                    string updateQuery = "UPDATE Users SET LastLoginDate=GETDATE() WHERE Username=@Login";
                    SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@Login", login);
                    updateCmd.ExecuteNonQuery();

                    if (firstIn)
                    {
                        // Первый вход - меняем пароль
                        ChangePasswordWindow changePassWindow = new ChangePasswordWindow(login);
                        changePassWindow.Show();
                    }
                    else if (role == "admin")
                    {
                        AdminWindow adminWindow = new AdminWindow();
                        adminWindow.Show();
                    }
                    else
                    {
                        UserWindow userWindow = new UserWindow();
                        userWindow.Show();
                    }

                    this.Close();
                }
                else
                {
                    attemptCount++;
                    if (attemptCount >= 3)
                    {
                        // Блокировка пользователя
                        reader.Close();
                        string blockQuery = "UPDATE Users SET IsBlocked=1 WHERE Username=@Login";
                        SqlCommand blockCmd = new SqlCommand(blockQuery, conn);
                        blockCmd.Parameters.AddWithValue("@Login", login);
                        blockCmd.ExecuteNonQuery();
                        MessageBox.Show("Вы заблокированы. Обратитесь к администратору.");
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль!");
                    }
                }
            }
        }
    }
}