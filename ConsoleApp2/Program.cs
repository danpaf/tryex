using System;
using Bogus;
using System.Data.SqlClient;
using ConsoleApp2.Models;

class Program
{
    static void Main()
    {
        var connectionString = "server=.\\SQLEXPRESS;database=SharpWork;integrated security=true";
        using (var connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("bad connection " + e);
            }

            var faker = new Faker<User>()
                .RuleFor(p => p.FirstName, f => f.Name.FirstName())
                .RuleFor(p => p.LastName, f => f.Name.LastName())
                .RuleFor(p => p.BirthDate, f => f.Date.Between(new DateTime(1950, 1, 1), new DateTime(2008, 12, 31)));

            var people = faker.Generate(10); 

            foreach (var person in people)
            {
                if (DateTime.Now.Year - person.BirthDate.Year >= 14)
                {
                    var insertQuery = "INSERT INTO People (FirstName, LastName, BirthDate) VALUES (@FirstName, @LastName, @BirthDate)";
                    using (var command = new SqlCommand(insertQuery, connection))
                    {
                        try
                        {
                            command.Parameters.AddWithValue("@FirstName", person.FirstName);
                            command.Parameters.AddWithValue("@LastName", person.LastName);
                            command.Parameters.AddWithValue("@BirthDate", person.BirthDate);

                            command.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e + " UNLUCK");
                        }
                        
                    }  
                    
                }
                else
                {
                    Console.WriteLine($"Пользователь {person.FirstName} {person.LastName} младше 14 лет и не будет зарегистрирован.");
                }
            }
        }
    }
}