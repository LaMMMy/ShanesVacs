using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace shanesvacuums.Models
{
    public class MailModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Message { get; set; }
        public bool EmailSent { get; set; }
    }
}