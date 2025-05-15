using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Optepafi.ViewModels.Main;

namespace Optepafi.Views.Main.Windows;

public partial class SimpleMessageWindow : ReactiveWindow<SimpleMessageWindowViewModel>
{
    public SimpleMessageWindow()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}