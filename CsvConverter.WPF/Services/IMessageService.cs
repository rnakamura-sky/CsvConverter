using System.Windows;

namespace CsvConverter.WPF.Services
{
    /// <summary>
    /// メッセージサービス
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// メッセージボックス表示
        /// </summary>
        /// <param name="message">メッセージ</param>
        void ShowDialog(string message);

        /// <summary>
        /// 問い合わせメッセージボックス表示
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <returns></returns>
        MessageBoxResult Question(string message);
    }
}
