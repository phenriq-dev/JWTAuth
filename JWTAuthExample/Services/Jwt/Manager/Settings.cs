﻿using JWTAuthExample.Models;
using JWTAuthExample.Services.Jwt.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace JWTAuthExample.Services.Jwt.Manager
{
    public static class Settings
    {
        public static string Secret = "fedaf7d8863b48e197b9287d492b708e";
    }
}
