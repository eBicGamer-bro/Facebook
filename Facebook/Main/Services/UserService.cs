using Data;
using Data.DTOs;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Services
{
    public class UserService : Service
    { 
        
        public void AddUser(string name, string email, string password, string bio, string dob)
        {
            using var _db = new FacebookDbContext();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(bio) || string.IsNullOrWhiteSpace(dob))
            {
                throw new ArgumentException("Name, email, password, bio, and date of birth cannot be empty.");
            }
            if (!Validation.Email(email))
            {
                throw new ArgumentException("Email is Invalid");
            }
            if (!Validation.Password(password))
            {
                throw new ArgumentException("Password is Invalid");
            }
            if (!Validation.Date(dob))
            {
                throw new ArgumentException("Date of Birth is Invalid");
            }
            if (CheckUsername(name))
            {
                throw new ArgumentException("Username already exists.");
            }
            if (CheckEmail(email))
            {
                throw new ArgumentException("Email already exists.");
            }
            var user = new User
            {
                Username = name,
                Email = email,
                Password = password
            };
            var userProfile = new UserProfile
            {
                Bio = bio,
                DateofBirth = DateTime.Parse(dob),
                User = user
            };
            _db.Users.Add(user);
            _db.UserProfiles.Add(userProfile);
            _db.SaveChanges();
        }

        public int Login(string email, string password)
        {
            if (!CheckEmail(email))
            {
                throw new ArgumentException("Invalid Email or Password");
            }
            using var _db = new FacebookDbContext();
            var acc = _db.Users.First(u => u.Email.ToLower() == email.ToLower());
            if (acc.Password != password)
            {
                throw new ArgumentException("Invalid Email or Password");
            }
            return acc.UserID;
        }
        public FriendProfileDto GetFriendProfile(int userId)
        {
            using var _db = new FacebookDbContext();
            if (!CheckID(userId))
            {
                throw new ArgumentException("User does not exist.");
            }

            var friendProfile = _db.Users.Include(u => u.Posts)
                .First(u => u.UserID == userId);
                    var profile = new FriendProfileDto
                    {
                        FriendId = friendProfile.UserID,
                        Name = friendProfile.Username,
                        Email = friendProfile.Email,
                        Posts = friendProfile.Posts.Select(p => p.Message).ToList()
                    };
                
            return profile;
        }

        public List<UserDto> GetUsers()
        {
            using var _db = new FacebookDbContext();
            return _db.Users.Select(u => new UserDto
            {
                UserID = u.UserID,
                UserName = u.Username
            }).ToList();
        }
        public string GetUsername(int userId)
        {
            using var _db = new FacebookDbContext();
            if (!CheckID(userId))
            {
                throw new ArgumentException("User does not exist.");
            }
            return _db.Users.First(u => u.UserID == userId).Username;
        }
        public bool CheckEmail(string email)
        {
            using var _db = new FacebookDbContext();

            if (_db.Users.Any(u => u.Email.ToLower() == email.ToLower()))
            {
                return true;
            }
            return false;
        }

        public bool CheckUsername(string username)
        {
            using var _db = new FacebookDbContext();

            if (_db.Users.Any(u => u.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        public override bool CheckID(int id)
        {
            using var _db = new FacebookDbContext();
            if (_db.Users.Any(u => u.UserID == id))
            {
                return true;
            }
            return false;
        }




    }
}
