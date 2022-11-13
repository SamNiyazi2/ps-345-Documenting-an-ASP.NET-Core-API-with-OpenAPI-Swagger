
using System.Collections.Concurrent;
using static Library.API.Controllers.IndexPageVariablesController;

// 11/13/2022 12:44 am - SSN 

namespace Library.API.Authentication
{
    public class AuthenticationUtil
    {

        // 11/12/2022 01:37 pm - SSN - Adding for dynamically assigned username/password. Replace Pluralsight/Pluralsight
        // 11/13/2022 12:07 am - SSN - Accommodate multiple users
        // Pluralsight/Pluralsight
        public static ConcurrentDictionary<string, Random_Data_API_Record> apiCredList = new ConcurrentDictionary<string, Random_Data_API_Record>();


        public static string createDicKey(string userName, string password)
        {
            return $"{userName?.Trim()}{password?.Trim()}";
        }


        public class Random_Data_API_Record
        {

            private string _first_Name;

            public string First_Name
            {
                get { return _first_Name; }
                set { _first_Name = value?.Trim(); }
            }

            private string _password;

            public string Password
            {
                get { return _password; }
                set { _password = value?.Trim(); }
            }

        }

    }
}

