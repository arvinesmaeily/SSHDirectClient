using System.ComponentModel;

namespace SSHDirectClient
{
    public class ConnectionState : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _UserConnectionFlag = false;

        public bool UserConnectionFlag
        {
            get { return _UserConnectionFlag; }

            set
            {
                if (_UserConnectionFlag != value)
                {
                    _UserConnectionFlag = value;
                    OnPropertyChanged(nameof(_UserConnectionFlag));
                }
            }
        }


        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
