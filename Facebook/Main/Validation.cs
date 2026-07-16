using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public static class Validation
    {
        public static bool Password(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            if (value.Length < 8) return false;
            if (value.Contains(" "))
                return false;
            if (!value.Any(char.IsDigit) || !value.Any(char.IsUpper) || !value.Any(char.IsLower))
                return false;
            return true;
        }

        public static bool Email(string email)
        {
            email = email.Trim();
            if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
                return false;
            int index = email.IndexOf("@");
            if (index == -1 || index == 0)
                return false;
            if (email.Count(a => a == '@') > 1 || !char.IsLetterOrDigit(email[0]))
                return false;
            if (email.LastIndexOf(".") <= index + 1 || email.LastIndexOf(".") == email.Length - 1)
                return false;
            if (email.Contains("..") || email.Contains(" "))
                return false;
            return true;

        }

        public static bool Post(string post)
        {
            if (string.IsNullOrEmpty(post) || string.IsNullOrWhiteSpace(post))
                return false;
            if (post.Split(" ").Length < 5 || post.Split(" ").Length > 100)
                return false;
            return true;
        }
        
        public static bool Date(string date)
        {
            if (string.IsNullOrEmpty(date) || string.IsNullOrWhiteSpace(date))
                return false;
            if (!DateTime.TryParse(date, out DateTime result))
                return false;
            if (result > DateTime.Now)
                return false;
            return true;
        }
        public static bool Exit(string code)
        {
            if(code.ToLower() == "exit")
                return true;
            return false;
        }

        public static bool CheckIDinList(List<int> list,  string id)
        {
            if(!int.TryParse(id, out int result))
            {
                return false;
            }
            if (list.Contains(result))
            {
                return true;
            }
            else return false;
        }
    }
}
