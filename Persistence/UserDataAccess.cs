using Npgsql;
using robot_api.Models;

namespace robot_api.Persistence;

public static class UserDataAccess
{
    private static string CONNECTION_STRING => DbConfig.ConnectionString;

    private static UserModel ReadUser(NpgsqlDataReader dr)
    {
        return new UserModel
        {
            Id = (int)dr["id"],
            Email = (string)dr["email"],
            FirstName = (string)dr["first_name"],
            LastName = (string)dr["last_name"],
            PasswordHash = (string)dr["password_hash"],
            Description = dr["description"] as string,
            Role = dr["role"] as string,
            CreatedDate = (DateTime)dr["created_date"],
            ModifiedDate = (DateTime)dr["modified_date"],
        };
    }

    public static List<UserModel> GetUsers()
    {
        var users = new List<UserModel>();
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM public.\"user\"", conn);
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
        using var cmd = new NpgsqlCommand(
            "SELECT * FROM public.\"user\" WHERE role = 'Admin'",
            conn
        );
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
        using var cmd = new NpgsqlCommand("SELECT * FROM public.\"user\" WHERE id = @id", conn);
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
        using var cmd = new NpgsqlCommand(
            "SELECT * FROM public.\"user\" WHERE email = @email",
            conn
        );
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
            @"INSERT INTO public.""user"" (email, first_name, last_name, password_hash, description, role, created_date, modified_date)
              VALUES (@email, @first_name, @last_name, @password_hash, @description, @role, @created_date, @modified_date)
              RETURNING id",
            conn
        );
        cmd.Parameters.AddWithValue("email", user.Email);
        cmd.Parameters.AddWithValue("first_name", user.FirstName);
        cmd.Parameters.AddWithValue("last_name", user.LastName);
        cmd.Parameters.AddWithValue("password_hash", user.PasswordHash);
        cmd.Parameters.AddWithValue("description", (object?)user.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("role", (object?)user.Role ?? DBNull.Value);
        cmd.Parameters.AddWithValue("created_date", DateTime.Now);
        cmd.Parameters.AddWithValue("modified_date", DateTime.Now);
        var id = (int)cmd.ExecuteScalar()!;
        user.Id = id;
        return user;
    }

    public static void UpdateUser(int id, UserModel user)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand(
            @"UPDATE public.""user"" SET first_name = @first_name, last_name = @last_name,
              description = @description, role = @role, modified_date = @modified_date
              WHERE id = @id",
            conn
        );
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("first_name", user.FirstName);
        cmd.Parameters.AddWithValue("last_name", user.LastName);
        cmd.Parameters.AddWithValue("description", (object?)user.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("role", (object?)user.Role ?? DBNull.Value);
        cmd.Parameters.AddWithValue("modified_date", DateTime.Now);
        cmd.ExecuteNonQuery();
    }

    public static bool DeleteUser(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("DELETE FROM public.\"user\" WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        return cmd.ExecuteNonQuery() > 0;
    }

    public static void UpdateUserCredentials(int id, string email, string passwordHash)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand(
            @"UPDATE public.""user"" SET email = @email, password_hash = @password_hash,
              modified_date = @modified_date WHERE id = @id",
            conn
        );
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("email", email);
        cmd.Parameters.AddWithValue("password_hash", passwordHash);
        cmd.Parameters.AddWithValue("modified_date", DateTime.Now);
        cmd.ExecuteNonQuery();
    }
}
