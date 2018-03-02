using System;
using System.Collections.Generic;


namespace Mtn.Library.Entities
{
    /// <summary>
    /// User entity
    /// </summary>
    public class User
    {
        /// <summary>
        /// Id
        /// </summary>
        public Int32 Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// Login
        /// </summary>
        public String Login { get; set; }
        /// <summary>
        /// Permissions
        /// </summary>
        public IList<Permission> Permissions { get; set; }
    }
}
