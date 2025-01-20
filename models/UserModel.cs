using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPropertySystemApp.models
{
    public class UserModel : INotifyPropertyChanged
    {
        private long _id;
        private string _login;
        private string _password;
        private string _firstname;
        private string _lastname;
        private TimeSpan? _sessionTimeout;
        private DateTime? _birthDate;
        private Passport? _pp;


        private UserRole? _userRole;


        public long id
        {
            get => _id; set => _id = value;
        }

        public string login
        {
            get { return _login; }
            set 
            { 
                _login = value; 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(login)));

            }
        }

        public string password
        {
            get { return _password; }
            set 
            { 
                _password = value; 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(password)));
            }
        }

        public string firstName
        {
            get
            {
                return _firstname; 
            }
            set
            {
                _firstname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(firstName)));
            }
        }

        public string lastName
        {
            get => _lastname;
            set 
            { 
                _lastname = value; 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lastName)));
            }
        }

        public DateTime? birthDate
        {
            get { return _birthDate; }
            set 
            { 
                _birthDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs($"{nameof(birthDate)}"));
            }
        }

        [JsonIgnore]
        public TimeSpan? SessionTimeout
        {
            get { return _sessionTimeout; }
            set { _sessionTimeout = value; }
        }

        
        public Passport passport
        {
            get => _pp;
            set => _pp = value;
        }

        public UserRole userRole
        {
            get { return _userRole; }
            set { _userRole = value; }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        
    }
}
