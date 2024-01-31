using ProaktifArizaTahmini.BLL.Models.RequestModel;
using ProaktifArizaTahmini.CORE.Entities;
using System.DirectoryServices;

namespace ProaktifArizaTahmini.UI.Helper
{
    public class ActiveDirectory
    {
        #region Active Directory

        public User Authenticate(LoginModel model, out User domainUser)
        {
            try
            {
                var entry = new DirectoryEntry("LDAP://" + model.Domain, model.Username, model.Password);
                foreach (var group in model.Groups)
                {
                    DirectorySearcher dSearch = new DirectorySearcher(entry);
                    dSearch.Sort = new System.DirectoryServices.SortOption("cn", (System.DirectoryServices.SortDirection)SortDirection.Ascending);
                    dSearch.Filter = "(&(objectCategory=user)(objectClass=user)(samAccountName=" + model.Username + ")(memberOf=" + group + "))";
                    var sr = dSearch.FindOne();
                    if (sr != null)
                    {
                        DirectoryEntry de = sr.GetDirectoryEntry();
                        domainUser = new User()
                        {
                            Username = model.Username,
                            TcIdentificationNumber = de.Properties["employeenumber"].Value?.ToString(),
                            Email = de.Properties["mail"].Value?.ToString(),
                            Name = de.Properties["givenname"].Value?.ToString(),
                            Surname = de.Properties["sn"].Value?.ToString(),
                            Phone = de.Properties["telephonenumber"].Value?.ToString(),
                            Mobile = de.Properties["mobile"].Value?.ToString(),
                            Title = de.Properties["title"].Value?.ToString(),
                            Departure = de.Properties["department"].Value?.ToString(),
                            Manager = de.Properties["manager"].Value?.ToString(),
                            Adress = de.Properties["streetaddress"].Value?.ToString(),
                            Company = de.Properties["company"].Value?.ToString(),
                        };
                        return domainUser;
                    }
                }

            }
            catch (DirectoryServicesCOMException dx)
            {
                //return null;
            }
            domainUser = null;
            return domainUser;
        }

        public User IsActiveDirectory(LoginModel model)
        {
            var domains = new string[2];
            User user = null;
            foreach (var item in model.DomainGroups)
            {
                user = Authenticate(
                    new LoginModel
                    {
                        Domain = item.Key,
                        Username = model.Username,
                        Password = model.Password,
                        Groups = item.Values
                    }, out user);
                if (user != null)
                {
                    return user;
                }

            }
            return user;
        }

        public bool IsAuthenticate(LoginModel model)
        {
            var auhenticate = false;
            foreach (var item in model.DomainGroups)
            {
                try
                {
                    var entry = new DirectoryEntry("LDAP://" + item.Key, model.Username, model.Password);
                    var nativeObject = entry.NativeObject;
                    auhenticate = true;
                }
                catch (DirectoryServicesCOMException dException)
                {
                    return false;
                }
                if (auhenticate)
                {
                    break;
                }
            }
            return auhenticate;
        }
        #endregion
    }
}
