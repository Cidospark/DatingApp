using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            _config = config;
            _repo = repo;

        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserForRegisterDto model)
        {
            model.Username = model.Username.ToLower();

            if (await _repo.UserExists(model.Username))
            {
                return BadRequest("Username already exists");
            }

            var userToCreate = _mapper.Map<User>(model);

            var createdUser = await _repo.Register(userToCreate, model.Password);

            var userToReturn = _mapper.Map<UserForDetailedDto>(createdUser);

            return CreatedAtRoute("GetUser", new {controller = "Users", id = createdUser.Id}, userToReturn);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDto model)
        {
            // comfirm login credentials and return user obj
            var userFromRepo = await _repo.Login(model.Username.ToLower(), model.Password);

            // ensure the user obj is not null
            if (userFromRepo == null)
            {
                return Unauthorized();
            }

            // start building up our token
            // the token will contain two claims - the user Id and username
            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            // to ensure that the token is valid when it comes back, the server needs to sign it
            // so... we generate a security key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            // Use the key as part of server-signature credential by encrypting the key with a hashing algorithm to produce the credential 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // describe the token in a descriptor obj
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            // use a token handler to create the token obj
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var user = _mapper.Map<UserForListDto>(userFromRepo);

            // and write it into a reponse obj
            return Ok(new
            {
                token = tokenHandler.WriteToken(token), user
            });
        }


    }
}