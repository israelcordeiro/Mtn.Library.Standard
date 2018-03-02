using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
#if ! UseSystemExtensions
using Mtn.Library.Extensions;
#endif
namespace Mtn.Library.Service
{
    /// <summary>
    /// 
    /// </summary>
    public static class ActiveDirectory
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <param name="rethrow"></param>
        /// <param name="contextType"> </param>
        /// <returns></returns>
        public static Boolean Logon(String userName, String password, String domain, bool rethrow = false, ContextType contextType = ContextType.Domain)
        {
            var groups = new List<string>();
            return Logon(userName, password, domain, out groups, rethrow, contextType, false);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <param name="rethrow"></param>
        /// <param name="contextType"></param>
        /// <param name="getGroups"></param>
        /// <param name="groups"></param>
        /// <returns></returns>
        public static Boolean Logon(String userName, String password, String domain, out List<String> groups, Boolean rethrow = false, ContextType contextType = ContextType.Domain, Boolean getGroups = false)
        {
            groups = null;
            bool logged = false ;
            try
            {
                using (var pc = new PrincipalContext(contextType, domain))
                {
                    logged = pc.ValidateCredentials(userName, password);

                    if (getGroups)
                    {
                        using (var usrPrincipal = UserPrincipal.FindByIdentity(pc, IdentityType.Name, userName))
                        {
                            try
                            {
                                groups = usrPrincipal.GetAuthorizationGroups().Where(x => x.Name.IsNullOrWhiteSpaceMtn() == false).Select(x => x.Name).ToList();
                            }
                            catch (Exception ex)
                            {
                                if (rethrow)
                                    throw new Exception("Rethrow exception, see innerException for detail", ex);
                            }

                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                if (rethrow)
                    throw new Exception("Rethrow exception, see innerException for detail", ex);

            }
            return logged;
        }

        /// <summary>
        /// List groups form domain
        /// </summary>
        /// <returns>List of groups</returns>
        /// <exception cref="Exception"></exception>
        public static IList<String> GroupList(String userName, String password, String domain, bool rethrow = false, ContextType contextType = ContextType.Domain)
        {
            var groups = new List<String>();
            try
            {
                using (var pc = new PrincipalContext(contextType, domain, userName, password))
                {
                    using (var gr = new GroupPrincipal(pc, "*"))
                    {
                        using (var ps = new PrincipalSearcher(gr))
                        {
                            using (var list = ps.FindAll())
                            {
                                groups =
                                    list.Where(x => x.Name.IsNullOrWhiteSpaceMtn() == false).Select(x => x.Name).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (rethrow)
                    throw new Exception("Rethrow exception, see innerException for detail", ex);

            }
            return groups;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <param name="rethrow"></param>
        /// <param name="contextType"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IList<String> GroupsByUser(String userName, String password, String domain, bool rethrow = false, ContextType contextType = ContextType.Domain)
        {
            var groups = new List<String>();
            try
            {

                using (var pc = (password.IsNullOrWhiteSpaceMtn() ?
                    new PrincipalContext(contextType, domain) :
                    new PrincipalContext(contextType, domain, userName, password)))
                {
                    using (var usrPrincipal = UserPrincipal.FindByIdentity(pc, userName))
                    {
                        try
                        {
                            if (usrPrincipal != null)
                            {
                                var groupsList =
                                    usrPrincipal.GetAuthorizationGroups();
                                groups = groupsList.Where(
                                        x => x.Name.IsNullOrWhiteSpaceMtn() == false).Select(x => x.Name).ToList();
                            }
                        }
                        catch (Exception ex)
                        {
                            if (rethrow)
                                throw new Exception("Rethrow exception, see innerException for detail", ex);

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                if (rethrow)
                    throw new Exception("Rethrow exception, see innerException for detail", ex);

            }
            return groups;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <param name="groupName"></param>
        /// <param name="rethrow"></param>
        /// <param name="contextType"></param>
        /// <returns></returns>
        public static IList<String> UsersByGroup(String userName, String password, String domain, String groupName, bool rethrow = false, ContextType contextType = ContextType.Domain)
        {
            var users = new List<String>();
            try
            {
                using (var pc = new PrincipalContext(contextType, domain, userName, password))
                {
                    using (var gr = GroupPrincipal.FindByIdentity(pc, groupName))
                    {
                        if (gr != null)
                            users = gr.Members.Where(x => x.Name.IsNullOrWhiteSpaceMtn() == false).Select(x => x.Name).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                if (rethrow)
                    throw new Exception("Rethrow exception, see innerException for detail", ex);

            }
            return users;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <param name="rethrow"></param>
        /// <param name="contextType"></param>
        /// <returns></returns>
        public static IList<String> UserList(String userName, String password, String domain, bool rethrow = false, ContextType contextType = ContextType.Domain)
        {
            var users = new List<String>();
            try
            {
                using (var pc = new PrincipalContext(contextType, domain, userName, password))
                {
                    using (var usr = new UserPrincipal(pc))
                    {
                            var ps = new PrincipalSearcher(usr);
                            var results = ps.FindAll();
                            users = results.Where(x => x.Name.IsNullOrWhiteSpaceMtn() == false).Select(x => x.Name).ToList();
                        
                    }
                }
            }
            catch (Exception ex)
            {
                if (rethrow)
                    throw new Exception("Rethrow exception, see innerException for detail", ex);

            }
            return users;
        }
    }
}
