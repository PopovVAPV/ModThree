using ModThree.Model;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace ModThree.Views
{
    public partial class ChangePasswordWindow : Window
    {
        private string connect = "Server=(localdb)\\MSSQLLocalDB; Database=CompanyAuthDB; Integrated Security=True;";
        private string username;

        public ChangePasswordWindow(string username)
        {
            InitializeComponent();
            this.username = username;
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string currentPassword = currentPasswordBox.Text;
            string newPassword = newPasswordBox.Password;
            string confirmPassword = confirmPasswordBox.Password;

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Новый пароль и подтверждение не совпадают!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connect))
            {
                conn.Open();

                // Проверка текущего пароля
                string checkQuery = "SELECT Password FROM Users WHERE Username=@Username";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@Username", username);
                string dbPassword = checkCmd.ExecuteScalar()?.ToString();

                if (dbPassword != currentPassword)
                {
                    MessageBox.Show("Неверный текущий пароль!");
                    return;
                }

                // Обновление пароля
                string updateQuery = "UPDATE Users SET Password=@Password, FisrtIn=0 WHERE Username=@Username";
                SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@Password", newPassword);
                updateCmd.Parameters.AddWithValue("@Username", username);
                updateCmd.ExecuteNonQuery();

                MessageBox.Show("Пароль успешно изменен!");

                // Открываем соответствующее окно в зависимости от роли
                string roleQuery = "SELECT Role FROM Users WHERE Username=@Username";
                SqlCommand roleCmd = new SqlCommand(roleQuery, conn);
                roleCmd.Parameters.AddWithValue("@Username", username);
                string role = roleCmd.ExecuteScalar()?.ToString();

                if (role == "admin")
                {
                    new AdminWindow().Show();
                }
                else
                {
                    new UserWindow().Show();
                }

                this.Close();
            }
        }
    }
}