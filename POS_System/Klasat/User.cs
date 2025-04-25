using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_System.Klasat
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Admin" ose "Puntor"

        public User() { }

        public User(int userId, string username, string password, string role)
        {
            UserID = userId;
            Username = username;
            Password = password;
            Role = role;
        }
    }
}