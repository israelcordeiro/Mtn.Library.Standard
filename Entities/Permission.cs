using System;

namespace Mtn.Library.Entities
{
    /// <summary>
    /// Permission entity
    /// </summary>
    public class Permission
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
        /// Indicate if can read data
        /// </summary>
        public Boolean CanRead { get; set; }
        /// <summary>
        /// Indicate if can create data
        /// </summary>
        public Boolean CanCreate{ get; set; }
        /// <summary>
        /// Indicate if can edit/update data
        /// </summary>
        public Boolean CanEdit{ get; set; }
        /// <summary>
        /// Indicate if can delete data
        /// </summary>
        public Boolean CanDelete{ get; set; }
        /// <summary>
        /// Indicate if can customize data
        /// </summary>
        public Boolean CanCustomize{ get; set; }
        /// <summary>
        /// Indicate if this permission is active permission
        /// </summary>
        public Boolean Active{ get; set; }
    }
}
