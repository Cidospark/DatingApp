using System.Collections.Generic;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly DataContext _context;
        public Seed(DataContext context)
        {
            _context = context;

        }

        // read user seed from the json file
        public void SeedUser(){
            var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
           // foreach user record
            var users = JsonConvert.DeserializeObject<List<User>>(userData);
            foreach(var user in users){
                // generate passwordhash and passwordsalt
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash("password", out passwordHash, out passwordSalt);
                // assign passwordhash and passwordsalt to the user record collected
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Username = user.Username.ToLower();

                // add user to context object
                _context.Add(user);
            }
            _context.SaveChanges();
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // generate a hash
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key; // get the key to the hash value
                // compute a new hash mixing the hash value with password
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}