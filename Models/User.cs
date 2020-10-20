using System;

namespace Task4.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string State { get; set; }
        public DateTime RegistrationTime { get; set; }
        public DateTime LoginTime { get; set; }
        public string Password { get; set; }
    }
}
