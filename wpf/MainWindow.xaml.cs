using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CaesarCipherWinFormsApplication.wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CaesarCipherLogic cipherLogic = new();   

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Result.Clear();
            Message.Clear();
            Key.Clear();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var key = Key.Text;
            var message = Message.Text;
            var change = (Change.SelectedItem as ListBoxItem).Content.ToString();

            message = cipherLogic.FilterSymbols(message);
            if (message == "")
            {
                MessageBox.Show("Сообщение должно содержать только буквы алфавита, выбранного вами языка");
                return;
            }

            if (change == "Зашифровать")
                HandleEncryptionAction(key, message);
            else if (change == "Расшифровать")
                HandleDecryptionAction(key, message);
        }

        private void HandleEncryptionAction(string key, string message)
        {
            if (int.TryParse(key, out var keyInt))
            {
                message = cipherLogic.EncryptMessage(keyInt, message);
                message = cipherLogic.SplitStr(message, 5);
                Result.Text = message;
            }
            else
            {
                MessageBox.Show("Введите ключ-число");
            }
        }

        private void HandleDecryptionAction(string key, string message)
        {
            if (int.TryParse(key, out var keyInt))
            {
                message = cipherLogic.EncryptMessage(-keyInt, message);
                message = cipherLogic.SplitStr(message, 5);
                Result.Text = message;
            }
            else
            {
                foreach (var messageChar in message)
                {
                    if (!CaesarCipherLogic.RuAlphabetChars.Contains(messageChar))
                    {
                        MessageBox.Show("Расшифровка без ключа доступна только для русского текста");
                        return;
                    }
                }

                var (decryptedMessage, decryptedKey) = cipherLogic.DecryptedMessageWithoutKey(message);        
                Result.Text = "Ключ: " + decryptedKey + "\n" + cipherLogic.SplitStr(decryptedMessage, 5);
            }
        }
        
        private void Message_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = ShouldStopEvent(sender, e.Text);
        }

        // вставка через ctrl v
        private void Key_OnPasting(object sender, DataObjectPastingEventArgs e) 
        {
            if (e.DataObject.GetDataPresent(typeof(string)))  
            {
                var text = (string)(e.DataObject.GetData(typeof(string)));

                if (ShouldStopEvent(sender, text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private static bool ShouldStopEvent(object sender, string text)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, text); 
            var parsed = int.TryParse(fullText, out _); 
            return !(fullText == "-" || parsed);
        }
    }
}