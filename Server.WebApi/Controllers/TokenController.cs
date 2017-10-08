﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Server.DataAccess.Model;
using Server.DataAccess.Repository;
using Server.WebApi.AppSetting;
using Server.WebApi.ExceptionHandler;
using Server.WebApi.ExceptionHandler.ValidateModel;
using Server.WebApi.ViewModel;

namespace Server.WebApi.Controllers
{
    [Route("api/token")]
    [ValidateModel]
    public class TokenController : Controller
    {
        private readonly JwtConfigs _settings;
        private readonly ITokenRepository _tokenRepository;
        private readonly IAccountRepository _accountRepository;

        public TokenController(IOptions<JwtConfigs> settings, ITokenRepository tokenRepository, IAccountRepository accountRepository)
        {
            _settings = settings.Value;
            _tokenRepository = tokenRepository;
            _accountRepository = accountRepository;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody]LoginParameters parameters)
        {
            var accountId = _accountRepository.GetAccountId(parameters.Login, parameters.Password);

            if (accountId == null) throw new CustomUserException("Invalid login or password");

            var refreshToken = Guid.NewGuid().ToString().Replace("-", "");

            if (_tokenRepository.AddToken(new Token
            {
                AccountId = accountId.Value,
                RefreshToken = refreshToken,
                IsStop = 0
            }))
            {
                return Json(GetJwt(accountId.ToString(), refreshToken));
            }

            throw new CustomUserException("Can not login");
        }

        [HttpPost("refreshToken")]
        public IActionResult Refresh([FromBody]RefreshTokenParameters parameters)
        {
            var token = _tokenRepository.GetToken(parameters.RefreshToken, parameters.AccountId);

            if (token == null) throw new CustomUserException("Invalid refresh token or accountId");

            if (token.IsStop == 1) throw new CustomUserException("Refresh token is expired");

            var refreshToken = Guid.NewGuid().ToString().Replace("-", "");

            var updateFlag = _tokenRepository.ExpireToken(token);

            var addFlag = _tokenRepository.AddToken(new Token
            {
                AccountId = new Guid(parameters.AccountId),
                RefreshToken = refreshToken,
                IsStop = 0
            });

            if (updateFlag && addFlag)
            {
                return Json(GetJwt(parameters.AccountId, refreshToken));
            }

            throw new CustomUserException("Can not update expired token");
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout([FromBody]LogoutParameters parameters)
        {
            if (_tokenRepository.ExpireToken(parameters.RefreshToken, parameters.AccountId))
            {
                return Ok();
            }
            throw new CustomUserException("Can't log out. Wrong data");
        }

        [HttpPost("registration")]
        public IActionResult Registration([FromBody]RegistrationParameters parameters)
        {
            var login = parameters.Login.Trim().ToLower();
            if (_accountRepository.IsAccountExist(login))
            {
                throw new CustomUserException("User with this login is already exist");
            }

            _accountRepository.AddAccount(parameters.Login, BCrypt.BCryptHelper.HashPassword(parameters.Password, BCrypt.BCryptHelper.GenerateSalt()));
            return Ok();
        }

        private ResponseJwtFormat GetJwt(string accountId, string refreshToken)
        {
            var now = DateTime.Now;
            var expiresIn = now.Add(TimeSpan.FromDays(1));

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, accountId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64)
            };

            var keyByteArray = Encoding.ASCII.GetBytes(_settings.Secret);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            var jwt = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: now,
                expires: expiresIn,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new ResponseJwtFormat
            {
                AccessToken = encodedJwt,
                ExpiresIn = expiresIn,
                RefreshToken = refreshToken
            };
        }
    }
}
