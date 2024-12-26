using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClasses
{
    public class Person
    {


        public int user_id { get; set; }

        public string username { get; set; }

        public string password { get; set; }


        public DateTime created_at { get; set; }

        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }

        public Person(int user_id, string username, string password, DateTime created_at, string first_name, string last_name, string email)
        {
            user_id = user_id;
            username = username;
            password = password;
            created_at = created_at;
            first_name = first_name;
            last_name = last_name;
            email = email;
        }


    }

}
