using System;
using System.Globalization;

namespace Mtn.Library.Entities
{
    /// <summary>
    /// Ticket class
    /// </summary>
    public class Ticket
    {
        /// <summary>
        /// Contructor
        /// </summary>
        public Ticket()
        {
            GuidToken = Guid.NewGuid();
            LoginToken = GuidToken.ToString();
        }

        private bool _isLogged;
        /// <summary>
        /// Return if is logged
        /// </summary>
        public Boolean IsLogged
        {
            get { return _isLogged; }
        }

        /// <summary>
        /// Simple login
        /// </summary>
        public void Login(User user = null)
        {
            _isLogged = true;
            if (user != null)
                this.UserCredential = user;
        }

        /// <summary>
        /// Logout
        /// </summary>
        public void Logout()
        {
            _isLogged = false;
            this.UserCredential = null;
        }
        /// <summary>
        /// Unique session identifier
        /// </summary>
        public String LoginToken { get; set; }

        /// <summary>
        /// Unique session identifier
        /// </summary>
        public Guid GuidToken { get; set; }

        /// <summary>
        /// User credentials
        /// </summary>
        public User UserCredential { get; set; }
        
        /// <summary>
        /// CultureInfo
        /// </summary>
        public CultureInfo Culture { get; set; }
        
    }
}
