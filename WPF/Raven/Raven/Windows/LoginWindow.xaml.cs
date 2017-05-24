﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
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

        [DllImport("user32.dll")]
        static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        private const int GwlStyle = -16;

        private const uint WsSysmenu = 0x80000;

        protected override void OnSourceInitialized(EventArgs e) {
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GwlStyle,
                GetWindowLong(hwnd, GwlStyle) & (0xFFFFFFFF ^ WsSysmenu));

            base.OnSourceInitialized(e);
        }

        public LoginWindow() {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e) {
            AttemptLogin();
        }

        private void AttemptLogin() {
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

            try {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM logins";

                using (MySqlDataReader dr = command.ExecuteReader()) {
                    dt.Load(dr);

                    // Returns true using LINQ expression
                    if (dt.Rows.Cast<DataRow>().Where(variable => variable.Field<string>("username") == username).Any(variable => variable.Field<string>("password") == hash)) {
                        return true;
                    }
                    return false;
                }
            }
            catch (MySqlException exception) {
                MessageBox.Show(exception.ToString());
                return false;
            }
        }

        public static string GetHash(string inputString) {
            SHA512 sha512 = SHA512.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha512.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }

        private static string GetStringFromHash(byte[] hash) {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++) {
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

        private void LoginWindow_OnKeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Enter:
                    if (Keyboard.IsKeyDown(Key.RightShift)) {
                        LoginSuccess = true;
                        Close();
                    }
                    AttemptLogin();
                    break;
            }
        }
    }
}