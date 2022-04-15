using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;

namespace QRcodeDemo
{
    public static class LoggingExtension
    {
        private static TextBox Console { get => (App.Current.MainWindow as MainWindow).fe_display2; }
        private static Dispatcher _Disaptcher { get => (App.Current.MainWindow as MainWindow).Dispatcher; }
        private static readonly string newLine = "\n";

        public static void WriteLine(object message)
        {
            _Disaptcher.Invoke(() =>
            Console.Text += message.ToString() + newLine);
        }

        public static void WriteLine()
        {
            _Disaptcher.Invoke(() =>
            Console.Text += newLine);
        }

        public static void Write(object message)
        {
            _Disaptcher.Invoke(()=>
            Console.Text += message.ToString());
        }

        public static void Clear()
        {
            _Disaptcher.Invoke(() =>
            Console.Text = "");
        }
    }
}
