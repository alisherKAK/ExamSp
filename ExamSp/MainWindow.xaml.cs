using ExamSp.DataAccess;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ExamSp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int FIRST_ELEMENT = 0;
        private int SECOND_ELEMENT = 1;

        private int[] integerArray;
        private int addNumber = 0;
        private object lockObject = new object();

        public MainWindow()
        {
            InitializeComponent();
            DatabaseInitialize();
        }

        private async void FillArrayButtonClick(object sender, RoutedEventArgs e)
        {
            var array = integerArray;
            int operationCount = int.Parse(integerNumberTextBox.Text);
            integerArray = new int[operationCount];           

            await Task.Run(() =>
            {
                for (int i = 0; i < operationCount; i++)
                {
                    Task.Run(() =>
                    {
                        lock (lockObject)
                        {
                            integerArray[addNumber] = addNumber;
                            ++addNumber;
                        }
                    });
                }
            });

            addNumber = 0;

            MessageBox.Show("Массив был заполнен");
        }

        private void IntegerNumberTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private async void CreateAndSaveButtonClick(object sender, RoutedEventArgs e)
        {
            if(CheckUserProperty(loginTextBox.Text, passwordTextBox.Password, emailTextBox.Text, phoneTextBox.Text))
            {
                using(var context = new UsersContext())
                {
                    context.Users.Add(new Models.User()
                    {
                        Address = addressTextBox.Text,
                        Email = emailTextBox.Text,
                        Login = loginTextBox.Text,
                        Password = passwordTextBox.Password,
                        Phone = phoneTextBox.Text
                    });
                    await context.SaveChangesAsync();
                }

                MessageBox.Show("Успешная загрузка");
            }
            else
            {
                MessageBox.Show("Некоторые поля заполнены неверно");
            }
        }

        private bool CheckUserProperty(string login, string password, string email, string phone)
        {
            if (login.All(letter => (letter >= 'a' && letter <= 'z') || (letter >= 'A' && letter <= 'Z') || (letter >= '0' && letter <= '9')))
            {
                if(password.All(letter => (letter >= 'a' && letter <= 'z') || (letter >= 'A' && letter <= 'Z') || (letter >= '0' && letter <= '9')))
                {
                    if(phone[FIRST_ELEMENT] == '+' && phone.Remove(FIRST_ELEMENT).All(number => number >= '0' && number <= '9'))
                    {
                        if(email.Split('@').Length == 2)
                        {
                            string emailsFirstPart = email.Split('@')[FIRST_ELEMENT];
                            string emailsSecondPart = email.Split('@')[SECOND_ELEMENT];

                            if(emailsFirstPart.All(letter => (letter >= 'a' && letter <= 'z') || (letter >= 'A' && letter <= 'Z') || (letter >= '0' && letter <= '9')) && emailsSecondPart.Split('.').Length == 2)
                            {
                                if(emailsSecondPart.Split('.')[FIRST_ELEMENT].All(letter => (letter >= 'a' && letter <= 'z') || (letter >= 'A' && letter <= 'Z') || (letter >= '0' && letter <= '9')) && emailsSecondPart.Split('.')[SECOND_ELEMENT].All(letter => (letter >= 'a' && letter <= 'z') || (letter >= 'A' && letter <= 'Z') || (letter >= '0' && letter <= '9')))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private async void DatabaseInitialize()
        {
            using (var context = new UsersContext())
            {
                context.Users.ToList();
                await context.SaveChangesAsync();
            }
        }
    }
}
