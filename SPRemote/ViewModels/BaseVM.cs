using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SPRemote.ViewModels
{
    public class BaseVM:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


        public BaseVM()
        {
        }
    }
}
