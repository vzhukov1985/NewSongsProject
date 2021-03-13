using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NewSongsProject.Services
{
    public class Wrapper<T> : INotifyPropertyChanged
    {
        private T value;
        public T Value
        {
            get { return value; }
            set
            {
                {
                    this.value = value;
                    OnPropertyChanged();
                }
            }
        }

        public static implicit operator Wrapper<T>(T value)
        {
            return new Wrapper<T> { value = value };
        }
        public static implicit operator T(Wrapper<T> wrapper)
        {
            return wrapper.value;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
