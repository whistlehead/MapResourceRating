using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MapResourceRating
{
    public class Model : INotifyPropertyChanged
    {
        private string _mapFolder;

        public string MapFolder
        {
            get { return _mapFolder; }
            set
            {
                if (value != _mapFolder)
                {
                    _mapFolder = value;
                    RaisePropertyChanged("MapFolder");
                }
            }
        }

        private ObservableCollection<string> _messages;

        public ObservableCollection<string> Messages
        {
            get
            {
                if (_messages == null)
                {
                    _messages = new ObservableCollection<string>();
                    RaisePropertyChanged("Messages");
                }
                return _messages;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
