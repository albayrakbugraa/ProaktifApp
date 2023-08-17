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
                DirectorySearcher dSearch = new DirectorySearcher(entry);
                dSearch.Filter = "(&(objectCategory=user)(objectClass=user))";
                dSearch.Sort = new System.DirectoryServices.SortOption("cn", (System.DirectoryServices.SortDirection)SortDirection.Ascending);
                dSearch.Filter = string.Format("(&(objectClass=user)(samAccountName=" + model.Username + "))");
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
            domains[0] = "bedas.local";
            domains[1] = "clkenerji.local";
            foreach (var item in domains)
            {

                user = Authenticate(
                    new LoginModel
                    {
                        ////LDAP://CORP.CompanyName.COM/DC=CORP,DC=CompanyName,DC=COM
                        Domain = item,
                        Username = model.Username,
                        Password = model.Password,
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
            var domains = new string[2];
            var auhenticate = false;
            domains[0] = "bedas.local";
            domains[1] = "clkenerji.local";
            foreach (var item in domains)
            {
                try
                {
                    var entry = new DirectoryEntry("LDAP://" + item, model.Username, model.Password);
                    var nativeObject = entry.NativeObject;
                    auhenticate = true;
                }
                catch (DirectoryServicesCOMException dException)
                {
                    Console.WriteLine(dException);
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
