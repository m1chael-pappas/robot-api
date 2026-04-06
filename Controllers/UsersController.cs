using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using robot_api.Models;
using robot_api.Persistence;

namespace robot_api.Controllers;

/// <summary>
/// Controller for managing users.
/// </summary>
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    /// <summary>
    /// Retrieves all users.
    /// </summary>
    /// <returns>A list of all users.</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet, Authorize(Policy = "UserOnly")]
    public IEnumerable<UserModel> GetAllUsers()
    {
        return UserDataAccess.GetUsers();
    }

    /// <summary>
    /// Retrieves all users with the Admin role.
    /// </summary>
    /// <returns>A list of admin users.</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("admin"), Authorize(Policy = "AdminOnly")]
    public IEnumerable<UserModel> GetAdminUsers()
    {
        return UserDataAccess.GetAdminUsers();
    }

    /// <summary>
    /// Retrieves a specific user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The user with the specified ID.</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}", Name = "GetUser"), Authorize(Policy = "UserOnly")]
    public IActionResult GetUserById(int id)
    {
        var user = UserDataAccess.GetUserById(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Registers a new user. The password will be hashed before storage.
    /// </summary>
    /// <param name="newUser">The new user to register.</param>
    /// <returns>The newly created user.</returns>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost, Authorize(Policy = "AdminOnly")]
    public IActionResult AddUser(UserModel newUser)
    {
        if (newUser == null)
            return BadRequest();

        var hasher = new PasswordHasher<UserModel>();
        newUser.PasswordHash = hasher.HashPassword(newUser, newUser.PasswordHash);
        newUser.CreatedDate = DateTime.Now;
        newUser.ModifiedDate = DateTime.Now;

        var user = UserDataAccess.InsertUser(newUser);

        return CreatedAtRoute("GetUser", new { id = user.Id }, user);
    }

    /// <summary>
    /// Updates an existing user. Email and password changes are ignored.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="updatedUser">The updated user data.</param>
    /// <returns>No content.</returns>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateUser(int id, UserModel updatedUser)
    {
        var user = UserDataAccess.GetUserById(id);
        if (user == null)
            return NotFound();

        UserDataAccess.UpdateUser(id, updatedUser);

        return NoContent();
    }

    /// <summary>
    /// Deletes a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>No content.</returns>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult DeleteUser(int id)
    {
        var deleted = UserDataAccess.DeleteUser(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Updates a user's email and password.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="loginModel">The new email and password.</param>
    /// <returns>No content.</returns>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPatch("{id}"), Authorize(Policy = "UserOnly")]
    public IActionResult UpdateUserCredentials(int id, LoginModel loginModel)
    {
        var user = UserDataAccess.GetUserById(id);
        if (user == null)
            return NotFound();

        var hasher = new PasswordHasher<UserModel>();
        var passwordHash = hasher.HashPassword(user, loginModel.Password);

        UserDataAccess.UpdateUserCredentials(id, loginModel.Email, passwordHash);

        return NoContent();
    }
}
