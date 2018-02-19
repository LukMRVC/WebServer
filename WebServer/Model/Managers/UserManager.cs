﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using WebServer.Http.REST;

namespace WebServer.Model.Managers
{
    class UserManager
    {

        public static int? VerifyUser(string json)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            using (var ctx = new MenuDbContext())
            {
                var user = ctx.User.Where(u => u.Email.Equals( Token.Decrypt(dict["email"]) )).First();
                if ( user.ValidatePassword( Token.Decrypt(dict["password"]) ) )
                {
                    return user.Id;
                }
                return null;
                
            }
        }

        public static User AddUser(string json)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            User user = new User(Token.Decrypt(dict["email"]), Token.Decrypt(dict["password"]));
            try
            {
                using (var ctx = new MenuDbContext())
                {
                    ctx.User.Add(user);
                    ctx.SaveChanges();
                }
            }
            catch (Exception)
            {
                return null;
            }
            return user;


        }


    }
}
