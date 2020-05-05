using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Username == username);

            if(user == null){
                return null;
            }

            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)){
                return null;
            }

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            // retieve the hash value of the key
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){
                // compute the retrieved hash value with the password given
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                // check if the new gotten has is thesame with the originally gotten has saved in the database
                // by looping through each character and matching their eqaulity
                for (int i = 0; i < computedHash.Length; i++){
                    if(computedHash[i] != passwordHash[i]) return false;
                }
            }

            return true;
            
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
 
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // generate a hash
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
                passwordSalt = hmac.Key; // get the key to the hash value
                // compute a new hash mixing the hash value with password
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x => x.Username == username)){
                return true;
            }

            return false;
        }
    }
}