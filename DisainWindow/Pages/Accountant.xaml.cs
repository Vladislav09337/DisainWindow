using DisainWindow.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DisainWindow.Pages
{
    public partial class Accountant : Page
    {
        // ID текущего бухгалтера (для записи в payments)
        // В реальной программе мы бы передавали его из окна авторизации.
        // Но для лабораторной: id 3 - это kozlova (бухгалтер)
        private int _currentUserId = 3;

        public Accountant()
        {
            InitializeComponent();
            LoadInvoices();
        }

        // Загрузка счетов
        private void LoadInvoices()
        {
            try
            {
                using (var db = new Entities2()) // Проверь имя контекста!
                {
                    // Загружаем счета и подтягиваем статусы
                    var invoicesList = db.invoices.ToList();
                    dgInvoices.ItemsSource = invoicesList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        // Обработчик кнопки "Оплатить"
        private void btnPay_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверяем, выбрал ли бухгалтер строку
            var selectedInvoice = dgInvoices.SelectedItem as invoices;
            if (selectedInvoice == null)
            {
                MessageBox.Show("Сначала выберите счет из таблицы!");
                return;
            }

            // 2. Проверяем, не оплачен ли он уже (status_id = 2 это 'paid')
            if (selectedInvoice.status_id == 2)
            {
                MessageBox.Show("Этот счет уже оплачен!");
                return;
            }

            // 3. Спрашиваем подтверждение
            var result = MessageBox.Show($"Оплатить счет {selectedInvoice.number} на сумму {selectedInvoice.amount} руб.?",
                                         "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new Entities2())
                    {
                        // Находим этот счет в базе заново (чтобы обновить)
                        var invoiceToPay = db.invoices.FirstOrDefault(i => i.id == selectedInvoice.id);

                        if (invoiceToPay != null)
                        {
                            // 1. Меняем статус счета на "Оплачен" (id=2)
                            invoiceToPay.status_id = 2;
                            invoiceToPay.date_paid = DateTime.Now; // Ставим дату оплаты

                            // 2. Создаем запись о платеже
                            var newPayment = new payments
                            {
                                invoice_id = invoiceToPay.id,
                                user_id = _currentUserId, // Кто оплатил (бухгалтер)
                                date_pay = DateTime.Now,
                                amount = invoiceToPay.amount,
                                purpose = "Оплата по счету " + invoiceToPay.number
                            };

                            db.payments.Add(newPayment);
                            db.SaveChanges(); // Сохраняем всё в БД

                            MessageBox.Show("Счет успешно оплачен!");

                            // Обновляем таблицу на экране
                            LoadInvoices();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при оплате: " + ex.Message);
                }
            }
        }
    }
}