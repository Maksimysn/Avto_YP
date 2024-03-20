using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Avto;

public partial class Main : Window
{
    public Main()
    {
        InitializeComponent();
    }

    private void OpenKlientForm(object? sender, RoutedEventArgs e)
    {
        
    }

    private void OpenMasinaForm(object? sender, RoutedEventArgs e)
    {
        
    }

    private void OpenRemontForm(object? sender, RoutedEventArgs e)
    {
        
    }

    private void OpenYslygaForm(object? sender, RoutedEventArgs e)
    {
        Yslyga YslygaForm = new Yslyga();
        YslygaForm.Show();
    }

    private void OpenZakazForm(object? sender, RoutedEventArgs e)
    {
        Zakaz ZakazForm = new Zakaz();
        ZakazForm.Show();
    }

    private void OpenRabForm(object? sender, RoutedEventArgs e)
    {
        
    }
}