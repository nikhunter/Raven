using System;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using MySql.Data.MySqlClient;

namespace Raven.Windows {
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow {
        private const string ConnectionString = "Server=raven-gps.com;Database=raven;Uid=root;Pwd=Raven123";
        public bool LoginSuccess;

        public LoginWindow() {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e) {
            AttemptLogin();
        }

        private void AttemptLogin() {
            // If login is corrrect changes LoginSuccess to true and closes LoginWindow
            if (AuthenticateLogin(UsernameText.Text.ToLower(), PasswordText.Password)) {
                LoginSuccess = true;
                MainWindow.Username = UsernameText.Text.ToLower();
                Close();
            }
            // If username or password is incorrect show error
            else {
                MessageBox.Show("Username or password is incorrect");
            }
        }

        // Returns true or false depending on a SQL check for a login that matches username and password
        public bool AuthenticateLogin(string username, string password) {
            // Stores hash value of password in hash
            var hash = GetHash(password);

            var connection = new MySqlConnection(ConnectionString);
            var dt = new DataTable();
            connection.Open();

            try {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM logins";

                using (var dr = command.ExecuteReader()) {
                    dt.Load(dr);

                    // Returns true using LINQ expression
                    if (dt.Rows.Cast<DataRow>().Where(variable => variable.Field<string>("username") == username).Any(variable => variable.Field<string>("password") == hash)) {
                        return true;
                    }
                }
            }
            catch (MySqlException exception) {
                MessageBox.Show(exception.ToString());
                return false;
            }
            return false;
        }
        
        // Returns plain password converted to SHA512 encrypted as string
        public static string GetHash(string inputString) {
            var sha512 = SHA512.Create();
            var bytes = Encoding.UTF8.GetBytes(inputString);
            var hash = sha512.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }

        // Returns a hash string built from a byte array
        private static string GetStringFromHash(byte[] hash) {
            var result = new StringBuilder();
            foreach (var t in hash) {
                result.Append(t.ToString("X2"));
            }
            return result.ToString();
        }

        // Empties UsernameText if there are no characters when losing focus
        private void UsernameText_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            if (string.IsNullOrEmpty(UsernameText.Text)) {
                UsernameText.Text = null;
            }
        }
        
        // Empties PasswordText if it contains dummy characters when getting focus
        private void PasswordText_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            if (PasswordText.Password == "PPPPPPPP") {
                PasswordText.Password = null;
            }
        }

        // Fills PasswordText with dummy characters when losing focus
        private void PasswordText_LostGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            if (PasswordText.Password == "") {
                PasswordText.Password = "PPPPPPPP";
            }
        }

        // Key event that registers Enter or RShift + Enter
        private void LoginWindow_OnKeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Enter:
                    if (Keyboard.IsKeyDown(Key.RightShift)) {
                        LoginSuccess = true;
                        MainWindow.Username = "DebugUser";
                        Close();
                    }
                    else {
                        AttemptLogin();
                    }
                    break;
            }
        }
    }
}