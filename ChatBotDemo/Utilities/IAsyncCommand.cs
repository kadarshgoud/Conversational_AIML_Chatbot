using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatBotDemo.Utilities
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
