using Npgsql;
using robot_api.Models;

namespace robot_api.Persistence;

public static class UserDataAccess
{
    private const string CONNECTION_STRING =
        "Host=localhost;Username=postgres;Password=;Database=sit331";

    private static UserModel ReadUser(NpgsqlDataReader dr)
    {
        return new UserModel
        {
            Id = (int)dr["id"],
            Email = (string)dr["email"],
            FirstName = (string)dr["firstname"],
            LastName = (string)dr["lastname"],
            PasswordHash = (string)dr["passwordhash"],
            Description = dr["description"] as string,
            Role = dr["role"] as string,
            CreatedDate = (DateTime)dr["createddate"],
            ModifiedDate = (DateTime)dr["modifieddate"],
        };
    }

    public static List<UserModel> GetUsers()
    {
        var users = new List<UserModel>();
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM public.user", conn);
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            users.Add(ReadUser(dr));
        }
        return users;
    }

    public static List<UserModel> GetAdminUsers()
    {
        var users = new List<UserModel>();
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM public.user WHERE role = 'Admin'", conn);
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            users.Add(ReadUser(dr));
        }
        return users;
    }

    public static UserModel? GetUserById(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM public.user WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        using var dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            return ReadUser(dr);
        }
        return null;
    }

    public static UserModel? GetUserByEmail(string email)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM public.user WHERE email = @email", conn);
        cmd.Parameters.AddWithValue("email", email);
        using var dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            return ReadUser(dr);
        }
        return null;
    }

    public static UserModel InsertUser(UserModel user)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand(
            @"INSERT INTO public.user (email, firstname, lastname, passwordhash, description, role, createddate, modifieddate)
              VALUES (@email, @firstname, @lastname, @passwordhash, @description, @role, @createddate, @modifieddate)
              RETURNING id",
            conn
        );
        cmd.Parameters.AddWithValue("email", user.Email);
        cmd.Parameters.AddWithValue("firstname", user.FirstName);
        cmd.Parameters.AddWithValue("lastname", user.LastName);
        cmd.Parameters.AddWithValue("passwordhash", user.PasswordHash);
        cmd.Parameters.AddWithValue("description", (object?)user.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("role", (object?)user.Role ?? DBNull.Value);
        cmd.Parameters.AddWithValue("createddate", DateTime.Now);
        cmd.Parameters.AddWithValue("modifieddate", DateTime.Now);
        var id = (int)cmd.ExecuteScalar()!;
        user.Id = id;
        return user;
    }

    public static void UpdateUser(int id, UserModel user)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand(
            @"UPDATE public.user SET firstname = @firstname, lastname = @lastname,
              description = @description, role = @role, modifieddate = @modifieddate
              WHERE id = @id",
            conn
        );
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("firstname", user.FirstName);
        cmd.Parameters.AddWithValue("lastname", user.LastName);
        cmd.Parameters.AddWithValue("description", (object?)user.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("role", (object?)user.Role ?? DBNull.Value);
        cmd.Parameters.AddWithValue("modifieddate", DateTime.Now);
        cmd.ExecuteNonQuery();
    }

    public static bool DeleteUser(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("DELETE FROM public.user WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        return cmd.ExecuteNonQuery() > 0;
    }

    public static void UpdateUserCredentials(int id, string email, string passwordHash)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand(
            @"UPDATE public.user SET email = @email, passwordhash = @passwordhash,
              modifieddate = @modifieddate WHERE id = @id",
            conn
        );
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("email", email);
        cmd.Parameters.AddWithValue("passwordhash", passwordHash);
        cmd.Parameters.AddWithValue("modifieddate", DateTime.Now);
        cmd.ExecuteNonQuery();
    }
}
