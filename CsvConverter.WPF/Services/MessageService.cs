using System.Windows;

namespace CsvConverter.WPF.Services
{
    /// <summary>
    /// メッセージサービス
    /// </summary>
    public class MessageService : IMessageService
    {
        /// <summary>
        /// メッセージボックス表示
        /// </summary>
        /// <param name="message">メッセージ</param>
        public MessageBoxResult Question(string message)
        {
            return MessageBox.Show(
                message,
                "問い合わせ",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question);
        }

        /// <summary>
        /// 問い合わせメッセージボックス表示
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <returns></returns>
        public void ShowDialog(string message)
        {
            MessageBox.Show(message);
        }
    }
}
