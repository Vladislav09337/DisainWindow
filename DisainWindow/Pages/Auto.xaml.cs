using DisainWindow.Models;
using System;
using System.Collections.Generic;
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

namespace DisainWindow.Pages
{
    /// <summary>
    /// Логика взаимодействия для Auto.xaml
    /// </summary>
    public partial class Auto : Page
    {
        public Auto()
        {
            InitializeComponent();
        }

        private void btnEnterGuests_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Client(null));
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            string login = txtblLogin.Text.Trim();
            string password = pswbPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            try
            {
                using (var db = new Entities2()) 
                {
                    // Ищем пользователя
                    var user = db.users.FirstOrDefault(u => u.login == login && u.password_hash == password);

                    if (user != null)
                    {
                        MessageBox.Show($"Добро пожаловать, {user.login}!");

                        // ВОТ ЗДЕСЬ ПРОВЕРЯЕМ РОЛЬ И РАСПРЕДЕЛЯЕМ, КУДА ИДТИ
                        if (user.role == "client")
                        {
                            NavigationService.Navigate(new Client(null));
                        }
                        else if (user.role == "accountant")
                        {
                            // Переходим на страницу бухгалтера (её нужно создать)
                            NavigationService.Navigate(new Accountant());
                        }
                        else if (user.role == "inspector")
                        {
                            // Переходим на страницу инспектора (если есть)
                            // NavigationService.Navigate(new Inspector()); 
                            MessageBox.Show("Страница инспектора еще в разработке");
                        }
                        else if (user.role == "admin")
                        {
                            // Для админа (например, тоже на страницу бухгалтера или отдельную)
                            NavigationService.Navigate(new Accountant());
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void btnGoToRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow regWin = new RegisterWindow();
            regWin.ShowDialog();
        }
    }
}
