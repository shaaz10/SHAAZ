using System.Text;
using System.Security.Cryptography;

namespace hash
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes("Admin123"));
            Console.WriteLine(Convert.ToBase64String(hash));
        }
    }
}
