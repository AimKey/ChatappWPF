using ChatApp.MVVM.Core;
using ChatApp.MVVM.Model;
using ChatApp.Net;
using System.Collections.ObjectModel;
using System.Windows;

namespace ChatApp.MVVM.ViewModel
{
    public class MainViewModel
    {
        public ObservableCollection<UserModel> Users { get; set; } // List of users bind to the list view below connect button in the MainWindow.xaml
        public ObservableCollection<string> Messages { get; set; } // List of messages bind to the list view on the right in the MainWindow.xaml
        public RelayCommand ConnectToServerCommand { get; set; } // Bind to the connect button in the MainWindow.xaml
        public RelayCommand SendMessageCommand { get; set; } // Bind to the send button in the MainWindow.xaml
        private Server _server;
        public String Username { get; set; }
        public string Message { get; set; } // The message that current user will send to the server

        public MainViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();
            _server = new Server();

            _server.ConnectedEvent += UserConnected; // Delegate an event to the UserConnected function
            _server.MessageReceivedEvent += AddMessage;
            _server.UserDisconnectedEvent += RemoveUser;

            // In WPF there are 2 way of binding a button to a function, and this is one of it
            // The first param is the action that will be executed
            // The second param is the condition that will be checked before the action is executed
            // TLDR a connect function and a function to determine if the previous function should execute or not
            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username), o => !string.IsNullOrEmpty(Username));
            // Same as above but for sending message, bind to the send button
            SendMessageCommand = new RelayCommand(o => _server.sendMessageToServer(Message), o => !string.IsNullOrEmpty(Message));
        }

        /// <summary>
        /// Remove user from the list when disconnected
        /// </summary>
        private void RemoveUser()
        {
            var uid = _server.packetReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(Users.First(u => u.Id == Guid.Parse(uid))));
        }

        /// <summary>
        /// Add message to the list when received
        /// </summary>
        private void AddMessage()
        {
            var msg = _server.packetReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }

        /// <summary>
        /// Add user to the list when connected
        /// </summary>
        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = _server.packetReader.ReadMessage(),
                Id = Guid.Parse(_server.packetReader.ReadMessage()),
            };

            // Basically if the user is not in the list then add it
            if (!Users.Any(u => u.Id == user.Id))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }

        }
    }
}
