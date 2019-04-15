using GalaSoft.MvvmLight;
using AppWpf.Model;
using System.Collections.ObjectModel;
using App.Wpf.Model;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using Microsoft.Win32;
using System;
using System.IO;
using System.Net.Mail;

namespace AppWpf.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class UserViewModel : ViewModelBase
    {
        #region Fields

        private readonly IDataService _dataService;

        #endregion

        #region Backing Fields

        ObservableCollection<User> _users;
        User _tempUser;
        Visibility _isUserPopupOpen;

        #endregion

        #region Properties

        public ObservableCollection<User> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                RaisePropertyChanged();
            }
        }

        public User TempUser
        {
            get { return _tempUser; }
            set
            {
                _tempUser = value;
                RaisePropertyChanged(nameof(TempUser));
                SaveUserCommand.RaiseCanExecuteChanged();
            }
        }

        public Visibility IsUserPopupOpen
        {
            get { return _isUserPopupOpen; }
            set
            {
                _isUserPopupOpen = value;
                RaisePropertyChanged();
                if (SaveUserCommand != null)
                    SaveUserCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand CreateUserCommand { get; set; }
        public RelayCommand CloseUserPopupCommand { get; set; }
        public RelayCommand LoadImageFileCommand { get; set; }
        public RelayCommand SaveUserCommand { get; set; }
        public RelayCommand<int> EditUserCommand { get; set; }
        public RelayCommand<int> DeleteUserCommand { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the UserViewModel class.
        /// </summary>
        public UserViewModel(IDataService dataService)
        {
            IsUserPopupOpen = Visibility.Hidden;

            _dataService = dataService;

            CreateUserCommand = new RelayCommand(CreateUserHandler);
            CloseUserPopupCommand = new RelayCommand(CloseUserPopupHandler);
            LoadImageFileCommand = new RelayCommand(LoadImageFileHandler);
            SaveUserCommand = new RelayCommand(SaveUserHandler, CanSaveUser);
            EditUserCommand = new RelayCommand<int>(EditUserHandler);
            DeleteUserCommand = new RelayCommand<int>(DeleteUserHandler);

            RefreshUsersList();
        }

        #endregion

        #region Command Handlers

        public void CreateUserHandler()
        {
            IsUserPopupOpen = Visibility.Visible;
            TempUser = new User();
            TempUser.PropertyChanged += TempUser_PropertyChanged;
        }

        public void CloseUserPopupHandler() {
            IsUserPopupOpen = Visibility.Hidden;
            TempUser.PropertyChanged -= TempUser_PropertyChanged;
        }

        async void LoadImageFileHandler()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Select a picture";
                openFileDialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                                          "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                                          "Portable Network Graphic (*.png)|*.png";

                if (openFileDialog.ShowDialog() == true)
                {
                    using (FileStream stream = File.Open(openFileDialog.FileName, FileMode.Open))
                    {
                        var byteArrayImage = new byte[stream.Length];
                        await stream.ReadAsync(byteArrayImage, 0, (int)stream.Length);
                        TempUser.Avatar = byteArrayImage;
                    }
                }
            }
            catch (Exception e)
            {
                //Log the error
                var error = e.Message;
            }
        }

        async public void SaveUserHandler()
        {
            try
            {
                var user = await _dataService.GetUser(TempUser.Id);

                await _dataService.UpdateUser(TempUser);
            }
            catch (Exception e)
            {
                //Log the error
                var error = e.Message;

                if (error == "Not Found")
                {
                    try
                    {
                        await _dataService.CreateUser(TempUser);
                    }
                    catch (Exception CreateUserException)
                    {
                        //Log CreateUser error
                        var errorCreateUser = CreateUserException.Message;
                    }
                }
                else {
                    //Log UpdateUser error
                    var errorUpdateUser = e.Message;
                }
            }
            finally
            {
                RefreshUsersList();

                IsUserPopupOpen = Visibility.Hidden;
                TempUser.PropertyChanged -= TempUser_PropertyChanged;
            }
        }

        public async void EditUserHandler(int userId)
        {
            try
            {
                TempUser = await _dataService.GetUser(userId);
                TempUser.PropertyChanged += TempUser_PropertyChanged;
                IsUserPopupOpen = Visibility.Visible;
            }
            catch (Exception e)
            {
                //Log the error
                var error = e.Message;
            }
        }

        public async void DeleteUserHandler(int userId)
        {
            try
            {
                await _dataService.DeleteUser(userId);
                RefreshUsersList();
            }
            catch (Exception e)
            {
                //Log the error
                var error = e.Message;
            }
        }

        #endregion

        #region Event Handlers

        void TempUser_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (IsUserPopupOpen == Visibility.Visible)
                SaveUserCommand.RaiseCanExecuteChanged();
        }

        #endregion

        #region Command Predicates

        public bool CanSaveUser()
        {
            if (TempUser == null)
                return false;

            if (string.IsNullOrWhiteSpace(TempUser.Name) ||
                string.IsNullOrWhiteSpace(TempUser.Email) ||
                !IsValidEmail(TempUser.Email) ||
                TempUser.Avatar == null ||
                TempUser.Avatar.Length == 0)
                return false;

            return true;
        }

        #endregion

        #region Methods

        private async void RefreshUsersList() {
            try
            {
                var users = await _dataService.GetUsers();
                Users = new ObservableCollection<User>(users);
            }
            catch (Exception e)
            {
                //Log the error
                var error = e.Message;
            }
        }

        private bool IsValidEmail(string emailAddress)
        {
            try
            {
                MailAddress mailAddress = new MailAddress(emailAddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        #endregion
    }
}