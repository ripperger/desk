using Desk.Models;
using System.Runtime.Versioning;
using System.Security.Claims;

namespace Desk.Contracts.Services
{
    /// <summary>
    /// Interface for user related services
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Returns the user with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<User> GetUserByIdAsync(int id);
        
        /// <summary>
        /// Returns the user with the given sid
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        Task<User> GetUserBySidAsync(string sid);
        
        /// <summary>
        /// Returns the user with the given username asynchronously
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<User> GetUserByUserNameAsync(string userName);
        
        /// <summary>
        /// Returns the user with the given username (not async)
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        User GetUserByUserName(string userName);

        Task Login(User user);
        
        /// <summary>
        /// Returns the logged in user's username
        /// </summary>
        /// <param name="httpContextUser"></param>
        /// <returns></returns>
        string LoggedInUserName(ClaimsPrincipal httpContextUser);
        
        /// <summary>
        /// Returns true if the given user is a developer user.
        /// Uses the logged in user's identity
        /// </summary>
        /// <param name="httpContextUser"></param>
        /// <returns></returns>
        bool IsDevUser(ClaimsPrincipal httpContextUser);

        /// <summary>
        /// Returns true if the given username is a developer user's username
        /// </summary>
        /// <param name="userName">Unformatted username</param>
        /// <returns></returns>
        bool IsDevUser(string? userName);

        /// <summary>
        /// Updates the given user with the new user
        /// </summary>
        /// <param name="existingUser"></param>
        /// <param name="newUser"></param>
        /// <returns></returns>
        Task<User?> UpdateUserAsync(User existingUser, User newUser);

        /// <summary>
        /// Registering a new site User from AD Users
        /// </summary>
        /// <param name="userName">Username of the user that's trying to log in</param>
        /// <returns>The newly registered User</returns>
        User RegisterUserFromAD(string userName);

        /// <summary>
        /// Registers and returns a temp user with the given username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        User RegisterTempUser(string userName);
    }
}
