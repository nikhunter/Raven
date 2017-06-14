using System;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using MySql.Data.MySqlClient;

namespace Raven.Windows {
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow {
        private const string ConnectionString = "Server=raven-gps.com;Database=raven;Uid=root;Pwd=Raven123";
        public bool LoginSuccess;

        [DllImport("user32.dll")]
        public static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        private const int GwlStyle = -16;

        private const uint WsSysmenu = 0x80000;

        protected override void OnSourceInitialized(EventArgs e) {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GwlStyle,
                GetWindowLong(hwnd, GwlStyle) & (0xFFFFFFFF ^ WsSysmenu));

            base.OnSourceInitialized(e);
        }

        public LoginWindow() {
            InitializeComponent(); // TODO Figure out how to get the close button back
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

        public bool AuthenticateLogin(string username, string password) {
            // Stores hash value of password in hash
            var hash = GetHash(password);

            var connection = new MySqlConnection(ConnectionString);
            var dt = new DataTable();
            connection.Open();

            try {
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM logins";

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

        public static string GetHash(string inputString) {
            var sha512 = SHA512.Create();
            var bytes = Encoding.UTF8.GetBytes(inputString);
            var hash = sha512.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }

        private static string GetStringFromHash(byte[] hash) {
            var result = new StringBuilder();
            foreach (var t in hash) {
                result.Append(t.ToString("X2"));
            }
            return result.ToString();
        }

        private void UsernameText_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            if (string.IsNullOrEmpty(UsernameText.Text)) {
                UsernameText.Text = null;
            }
        }

        private void PasswordText_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            if (PasswordText.Password == "PPPPPPPP") {
                PasswordText.Password = null;
            }
        }

        private void PasswordText_LostGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            if (PasswordText.Password == "") {
                PasswordText.Password = "PPPPPPPP";
            }
        }

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