using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Counter
{
    public class Bindable : INotifyPropertyChanged
    {
        int count = 0;
        string fibText = "N = 0, Fib = 0";

        public int Count
        {
            get => count;
            set
            {
                count = value;
                this.NotyfyPropertyChanged();
            }
        }

        public string FibText
        {
            get => this.fibText;
            set
            {
                this.fibText = value;
                this.NotyfyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotyfyPropertyChanged([CallerMemberName] string name = "")
        {
            this.PropertyChanged?.Invoke(this, new(name));
        }
    }

    /// <summary>
    /// バインド可能なプロパティ
    /// </summary>
    /// <typeparam name="T">プロパティの型</typeparam>
    public class State<T> : INotifyPropertyChanged
    {
        private T value;

        public T Value
        {
            get => this.value;
            set
            {
                this.value = value;
                this.NotyfyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public State(T initialValue)
        {
            this.value = initialValue;
        }

        protected void NotyfyPropertyChanged([CallerMemberName] string name = "")
        {
            this.PropertyChanged?.Invoke(this, new(name));
        }
    }

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private Bindable Bindable { get; } = new();
        private State<int> Counter = new(1);
        private State<string> Text = new("");
        private int fibN = 0;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void HandleCounter1(object sender, RoutedEventArgs e)
        {
            this.Bindable.Count++;
        }

        private void HandleCounter2(object sender, RoutedEventArgs e)
        {
            this.Counter.Value++;
        }

        private void HandleFib(object sender, RoutedEventArgs e)
        {
            this.fibN++;
            this.Bindable.FibText = $"N = {this.fibN}, Fib = {CalcFib(this.fibN)}";
        }

        /// <summary>
        /// フィボナッチ数を生成します
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private static long CalcFib(int n)
        {
            return n switch
            {
                0 => 0,
                1 => 1,
                _ => CalcFib(n - 1) + CalcFib(n - 2)
            };
        }
    }
}
