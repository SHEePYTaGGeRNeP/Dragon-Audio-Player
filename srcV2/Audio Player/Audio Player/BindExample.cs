using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audio_Player
{
    class BindExample
    {
        public List<BindClass> Bind = new List<BindClass>();
    }

    public class BindClass : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                this.OnPropertyChanged("Name");
            }
        }

        private string name2;
        public string Name2
        {
            get { return name2; }
            set
            {
                name2 = value;
                this.OnPropertyChanged("Name2");
            }
        }

        private DateTime modifiedDate;

        public DateTime ModifiedDate
        {
            get { return modifiedDate; }
            set { modifiedDate = value; }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
