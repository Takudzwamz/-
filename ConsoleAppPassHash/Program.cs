using Bogus;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace ConsoleAppPassHash
{
    class Program
    {
        static void Main(string[] args)
        {
            int? v = null;
            MarkEnum? m;
            m = (MarkEnum?)v;
            if (Enum.IsDefined(typeof(MarkEnum), v)) 
            {

            }
            else
            {
                Console.Write("Not in range");
            }
            //SeedData.Seed();
            //Console.Write(Guid.NewGuid().ToString());
            Console.ReadLine();
            return;

            Console.Write("Enter a password: ");
            string password = Console.ReadLine();

            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            Console.WriteLine($"Hashed: {hashed}");
            Console.ReadLine();
        }
    }
    public enum MarkEnum
    {
        Неудовлетворительно = 2,
        Удовлетворительно = 3,
        Хорошо = 4,
        Отлично = 5
    }
    public static class SeedData
    {
        public static void Seed()
        {

            var Кафедры = GetItems<Cathedra>(@"E:\Visual Studio 2017\Projects\Факультет\Описание\Seed\Кафедры.txt");
            //var Предметы = GetItems<Subject>(@"E:\Visual Studio 2017\Projects\Факультет\Описание\Seed\Предметы.txt");

            var Employees = GetItems<Employee>(@"E:\Visual Studio 2017\Projects\Факультет\Описание\Seed\Преподаватели.txt");

            var rand = new Random();
            foreach (var e in Employees)
            {
                e.CathedraId = rand.Next(1, Кафедры.Count() + 1);//The exclusive upper bound
                Console.WriteLine($"Id={e.Id} CathedraId={e.CathedraId} Name={e.Name}");
            }



        }

        //@"E:\Visual Studio 2017\Projects\Факультет\Описание\Seed\Кафедры.txt"
        public static List<T> GetItems<T>(string fileName) where T : IEntityBase, new()
        {
            var items = new List<T>();
            // for Encoding.GetEncoding(1251) to work properly in net.core install nuget System.Text.Encoding.CodePages and RegisterProvider
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            StreamReader file = new StreamReader(fileName, Encoding.GetEncoding(1251));//Cyrillic (Windows)
            string line;
            int counter = 0;
            while ((line = file.ReadLine()) != null)
            {
                items.Add(new T() { Id = counter++, Name = line });
            }
            file.Close();
            return items;
        }


    }
    public interface IEntityBase
    {
        int Id { get; set; }
        string Name { get; set; }
    }

    public class Employee : IEntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int CathedraId { get; set; }
        public virtual Cathedra Cathedra { get; set; }
    }
    public class Cathedra : IEntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Subject : IEntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}
