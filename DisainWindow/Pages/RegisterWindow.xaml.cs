using DisainWindow.Models;
using System;
using System.Linq;
using System.Windows;

namespace DisainWindow
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string lastName = txtLastName.Text.Trim();
            string firstName = txtFirstName.Text.Trim();
            string login = txtRegLogin.Text.Trim();
            string password = txtRegPassword.Password;

            if (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName) ||
                string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            try
            {
                // ВАЖНО: Проверь название контекста! (Model1Entities или demonEntities)
                using (var db = new Entities2())
                {
                    if (db.users.Any(u => u.login == login))
                    {
                        MessageBox.Show("Такой логин уже занят!");
                        return;
                    }

                    // Создаем клиента
                    var newClient = new clients
                    {
                        last_name = lastName,
                        first_name = firstName,
                        middle_name = "-",
                        passport = Guid.NewGuid().ToString().Substring(0, 10), // Временный уникальный паспорт
                        birth_date = DateTime.Now,
                        phone = "-",
                        email = "-"
                    };
                    db.clients.Add(newClient);
                    db.SaveChanges();

                    // Создаем пользователя
                    var newUser = new users
                    {
                        login = login,
                        password_hash = password,
                        role = "client",
                        client_id = newClient.id,
                        is_active = true
                    };
                    db.users.Add(newUser);
                    db.SaveChanges();

                    MessageBox.Show("Регистрация успешна! Теперь войдите.");
                    this.Close(); // Закрываем окно после успеха
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}