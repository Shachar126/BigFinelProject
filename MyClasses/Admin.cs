using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClasses
{
    public class Admin : Person
    {

        public DateTime date_started { get; set; }

        public Admin(DateTime date_started, string username,int user_id, string password, DateTime created_at, string first_name, string last_name, string email) : base(user_id, username, password, created_at, first_name, last_name, email)
        {

            date_started = date_started;
        }

    }
}