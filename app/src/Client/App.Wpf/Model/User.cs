using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Wpf.Model
{
    public class User : ObservableObject
    {
        private int _id;
        private string _name;
        private byte[] _avatar;
        private string _email;
        private int _age;

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        public byte[] Avatar
        {
            get { return _avatar; }
            set
            {
                _avatar = value;
                RaisePropertyChanged(nameof(Avatar));
            }
        }

        public string Email {
            get { return _email; }
            set
            {
                _email = value;
                RaisePropertyChanged(nameof(Email));
            }
        }

        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;
                RaisePropertyChanged(nameof(Age));
            }
        }
    }
}
