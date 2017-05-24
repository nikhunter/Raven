using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
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
using MySql.Data.MySqlClient;

namespace Raven.Windows {
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window {
        private const string ConnectionString = "Server=raven-gps.com;Database=raven;Uid=root;Pwd=Raven123";
        public bool LoginSuccess;

        public LoginWindow() {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e) {
            // If login is corrrect changes LoginSuccess to true and closes LoginWindow
            if (AuthenticateLogin(UsernameText.Text, PasswordText.Password)) {
                LoginSuccess = true;
                Close();
            }
            // If username or password is incorrect show error
            else {
                MessageBox.Show("Username or password is incorrect");
            }
        }

        public bool AuthenticateLogin(string username, string password) {
            // Stores hash value of password in hash
            var hash = GetHash(password);
            
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            DataTable dt = new DataTable();
            connection.Open();

            try
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM logins";

                using (MySqlDataReader dr = command.ExecuteReader())
                {
                    dt.Load(dr);

                    // Returns true using LINQ expression
                    if (dt.Rows.Cast<DataRow>().Where(variable => variable.Field<string>("username") == username).Any(variable => variable.Field<string>("password") == hash))
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (MySqlException exception)
            {
                MessageBox.Show(exception.ToString());
                return false;
            }
        }

        // Hashes Password and returns hash value
        //public static string GetHash(string inputString) {
        //    // Generates SHA1
        //    using (var sha1 = new SHA1Managed()) {
        //        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        //        var sb = new StringBuilder(hash.Length * 2);

        //        foreach (var b in hash) {
        //            sb.Append(b.ToString("X2"));
        //        }

        //        // Returns hash value
        //        return sb.ToString();
        //    }
        //}

        public static string GetHash(string inputString)
        {
            SHA512 sha512 = SHA512Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha512.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }

        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }

        private void PasswordText_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            PasswordText.Password = null;
        }

        private void PasswordText_LostGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            PasswordText.Password = "Password";
        }
    }
}