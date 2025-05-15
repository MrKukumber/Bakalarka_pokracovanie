using ReactiveUI;

namespace Optepafi.ViewModels.Main;

public class SimpleMessageWindowViewModel : ViewModelBase
{
    public SimpleMessageWindowViewModel(string message)
    {
        Message = message;
    }

    private string _message;
    public string Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message, value);
    }
    
}