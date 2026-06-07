using Microsoft.EntityFrameworkCore;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using System.Xml.Linq;
using Desk.Data;
using Desk.Models;
using Desk.Constants;
using Microsoft.AspNetCore.Authentication;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.DirectoryServices;
using Desk.Contracts.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Desk.Services
{

    public class UserService : IUserService
    {
        private readonly DeskContext _context;

        public UserService(DeskContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get user from db by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The User object with the given id</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<User> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user ?? throw new ArgumentNullException("Felhasználó nem található");
        }
        
        /// <summary>
        /// Get user from db by sid
        /// </summary>
        /// <param name="sid"></param>
        /// <returns>The User object with the given sid</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<User> GetUserBySidAsync(string sid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Sid == sid);
            return user ?? throw new ArgumentNullException("Felhasználó nem található");
        }

        /// <summary>
        /// Get user from db by username async
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>The User object with the given username</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.UserName != null && u.UserName.ToLower() == userName.ToLower());
            return user ?? throw new ArgumentNullException("Felhasználó nem található"); ;
        }

        /// <summary>
        /// Get user from db by username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>The User object with the given username</returns>
        public User GetUserByUserName(string userName)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.UserName != null && u.UserName.ToLower() == userName.ToLower());

            // If there is no such user, then pick it from AD users
            return user ?? RegisterUserFromAD(userName);
        }

        /// <summary>
        /// Get the logged in user's username
        /// </summary>
        /// <param name="httpContextUser"></param>
        /// <returns>The username of the developer user, the logged in user's username without the domain, or throws an exception</returns>
        /// <exception cref="ArgumentNullException">"Felhasználó nem azonosítható"</exception>
        public string LoggedInUserName(ClaimsPrincipal httpContextUser)
        {
            if (IsDevUser(httpContextUser))
                return Dictionaries.Role["DEV_USER"];

            return httpContextUser.Identity?.Name?.Split('\\')[1]
                ?? throw new ArgumentNullException("Felhasználó nem azonosítható");
        }

        public async Task Login(User user)
        {
            if (user != null)
            {
                user.LastLogin = DateTime.Now;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Checks if the user is a developer user
        /// </summary>
        /// <param name="httpContextUser">The logged in user's object</param>
        /// <returns>True if the user is a developer user</returns>
        /// <exception cref="ArgumentNullException">"Felhasználó nem azonosítható"</exception>
        public bool IsDevUser(ClaimsPrincipal httpContextUser)
        {
            if (httpContextUser.Identity == null) throw new ArgumentNullException("Felhasználó nem azonosítható");
            return httpContextUser.Identity?.Name?.ToLower() == Dictionaries.Role["DEV_USER"].ToLower();
        }

        /// <summary>
        /// Checks if the user is a developer user.
        /// </summary>
        /// <param name="userName">Pre-formatted username (e.g. LoggedInUserName)</param>
        /// <returns>True if the user is a developer user</returns>
        public bool IsDevUser(string? userName)
        {
            if (string.IsNullOrEmpty(userName)) return false;

            return userName.ToLower() == Dictionaries.Role["DEV_USER"].ToLower();
        }

        /// <summary>
        /// Updates a user's properties.
        /// The LastUpdated field will be the current time.
        /// </summary>
        /// <param name="existingUser"></param>
        /// <param name="newUser">A User object with the updated properties</param>
        /// <returns>The updated User object</returns>
        /// <exception cref="ArgumentNullException">"Felhasználó nem azonosítható"</exception>
        public async Task<User?> UpdateUserAsync(User existingUser, User newUser)
        {
            if (existingUser == null) throw new ArgumentNullException("Felhasználó nem azonosítható");

            existingUser.UserName = newUser.UserName;
            existingUser.FullName = newUser.FullName;
            existingUser.Rank = newUser.Rank;
            existingUser.Email = newUser.Email;
            existingUser.Phone = newUser.Phone;
            existingUser.Department = newUser.Department;
            existingUser.IsVip = newUser.IsVip;
            //existingUser.LastUpdated = DateTime.Now;

            _context.Update(existingUser);
            await _context.SaveChangesAsync();
            return existingUser;
        }

        /// <summary>
        /// Registering a new site User from AD Users
        /// </summary>
        /// <param name="userName">Username of the user that's trying to log in</param>
        /// <returns>The newly registered User</returns>
        public User RegisterUserFromAD(string userName)
        {
            if (!_context.Users.Any(u => u.UserName == userName))
            {
                var aDUser = _context.ADUsers.FirstOrDefault(a => a.UserName == userName);

                if (aDUser == null) throw new Exception("AD user nem található.");

                User newUser = new User
                {
                    Sid = aDUser.Sid,
                    UserName = aDUser.UserName,
                    FullName = aDUser.FullName,
                    Rank = aDUser.Rank,
                    Email = aDUser.Email,
                    Phone = aDUser.Phone,
                    Department = aDUser.Department,
                    IsVip = aDUser.IsVip
                };
                _context.Users.Add(newUser);
                _context.SaveChanges();
                return newUser;
            }
            else { return _context.Users.First(u => u.UserName == userName); }
        }

        /// <summary>
        /// Registers a temporary user if the user with the given username doesn't exist.
        /// The LastUpdated field will be the current time.
        /// The FullName will get a "(nem azonosított)" suffix.
        /// The Email will get a "@unidentified.user" suffix.
        /// </summary>
        /// <param name="userName">Pre-formatted username (e.g. LoggedInUserName)</param>
        /// <returns>The new User object or the existing one</returns>
        public User RegisterTempUser(string userName)
        {
            if (!_context.Users.Any(u => u.UserName == userName))
            {
                var newUser = new User()
                {
                    UserName = userName,
                    FullName = userName + " (nem azonosított)",
                    Email = userName + "@unidentified.user",
                    //LastUpdated = DateTime.Now
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();
                return newUser;
            }
            else { return _context.Users.First(u => u.UserName == userName); }
        }

    }

}
