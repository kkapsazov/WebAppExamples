using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GraphQLExample.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace GraphQLExample
{
    public static class Utils
    {
        public static IQueryable<T> If<T>(this IQueryable<T> source, bool condition, Func<IQueryable<T>, IQueryable<T>> action)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (condition)
            {
                return action(source);
            }

            return source;
        }

        public static IEnumerable<T> If<T>(this IEnumerable<T> source, bool condition, Func<IEnumerable<T>, IEnumerable<T>> action)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (condition)
            {
                return action(source);
            }

            return source;
        }

        public static string GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

        public static string HashPassword(string password, string salt)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password,
                Convert.FromBase64String(salt),
                KeyDerivationPrf.HMACSHA1,
                10000,
                256 / 8));
            return hashed;
        }

        public static string GenerateRandomPassword(int passwordSize, bool useLowercase = true, bool useUppercase = true, bool useNumbers = true, bool useSpecial = true)
        {
            const string LOWER_CASE = "abcdefghijklmnopqursuvwxyz";
            const string UPPER_CAES = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string NUMBERS = "123456789";
            const string SPECIALS = @"!@$%^&*()#";

            string charSet = "";

            if (useLowercase)
            {
                charSet += LOWER_CASE;
            }

            if (useUppercase)
            {
                charSet += UPPER_CAES;
            }

            if (useNumbers)
            {
                charSet += NUMBERS;
            }

            if (useSpecial)
            {
                charSet += SPECIALS;
            }

            Random random = new();
            StringBuilder sb = new();
            for (int i = 0; i < passwordSize; i++)
            {
                sb.Append(charSet[random.Next(charSet.Length - 1)]);
            }

            return sb.ToString();
        }

        public static T GetAs<T>(this object dict)
        {
            string dictInJson = JsonConvert.SerializeObject(dict);
            return JsonConvert.DeserializeObject<T>(dictInJson);
        }

        public static string GenerateJwtToken(User user, string key, string issuer)
        {
            // generate token that is valid for 7 days
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, "Admin")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = issuer,
                Audience = issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
