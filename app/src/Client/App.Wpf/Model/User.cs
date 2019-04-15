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

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged(nameof(Name));
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

        /*
        public User()
        {
            _id = 0;
            _name = string.Empty;
            _avatar = new byte[0];
            _email = string.Empty;
        }
        */
    }
}
