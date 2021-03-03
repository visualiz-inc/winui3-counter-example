using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Counter
{
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
        // 最大行数
        const int MaxLineCount = 1000;

        State<string> Command { get; } = new("ipconfig");
        State<string> Args { get; } = new("-all");
        State<ObservableCollection<string>> Logs { get; } = new(new() { "ログ" });

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async void ExecuteCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                using var process = new Process();

                process.StartInfo.FileName = this.Command.Value;
                process.StartInfo.Arguments = this.Args.Value;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;

                process.OutputDataReceived += async (_, data) =>
                {
                    if (data.Data is not null and string text)
                    {
                        // UIスレッドで実行
                        this.DispatcherQueue.TryEnqueue(() =>
                        {
                            this.Logs.Value.Add(text);
                            // 最大行数を超えたら先頭を削除
                            if (this.Logs.Value.Count > MaxLineCount)
                                this.Logs.Value.RemoveAt(0);

                        });
                    }
                };

                // 開始
                process.Start();

                // 標準出力の読み込みを開始
                process.BeginOutputReadLine();

                // 終了するまで待機
                await process.WaitForExitAsync();

                // 終了コード
                var code = process.ExitCode;

                // スタックオーバーフローで終了したかどうか
                var isStackOverflow = process.ExitCode == unchecked((int)0xC00000FD);

                // UIスレッドで実行
                this.DispatcherQueue.TryEnqueue(async () =>
                {
                    await new ContentDialog()
                    {
                        Title = $"終了コード : {code}",
                        XamlRoot = this.Content.XamlRoot,
                        CloseButtonText = "Ok"
                    }.ShowAsync();
                });
            }
            catch (Exception ex)
            {
                // UIスレッドで実行
                this.DispatcherQueue.TryEnqueue(async () =>
                {
                    await new ContentDialog()
                    {
                        Title = ex.Message,
                        XamlRoot = this.Content.XamlRoot,
                        CloseButtonText = "Ok"
                    }.ShowAsync();
                });
            }
        }
    }
}
