using AuthMe.Application.DTOs.Request;
using AuthMe.Application.DTOs.Response;
using AuthMe.Application.Interfaces.Services;
using AuthMe.Identity.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthMe.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtOptions _jwtOptions;
        private readonly RedisConnector _redisConnector;

        public IdentityService(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IOptions<JwtOptions> jwtOptions,
            RedisConnector redisConnector)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
            _redisConnector = redisConnector;
        }

        public async Task<UserSignUpResponse> SignUp(UserSignUpRequest request)
        {
            var identityUser = new IdentityUser
            {
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, request.Password);
            if (result.Succeeded)
                await _userManager.SetLockoutEnabledAsync(identityUser, false);

            var response = new UserSignUpResponse(result.Succeeded);
            if (!result.Succeeded && result.Errors.Count() > 0)
                response.AddErrors(result.Errors.Select(x => x.Description));

            return response;
        }

        public async Task<UserSignInResponse> SignIn(UserSignInRequest request)
        {
            var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, true);
            if (result.Succeeded)
                return await GenerateCredencials(request.Email);

            var response = new UserSignInResponse();
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    response.AddError("Essa conta está bloqueada");
                else if (result.IsNotAllowed)
                    response.AddError("Essa conta não tem permissão para fazer login");
                else if (result.RequiresTwoFactor)
                    response.AddError("É necessário confirmar o login no seu segundo fator de autenticação");
                else
                    response.AddError("Usuário ou senha estão incorretos");
            }

            return response;
        }

        public async Task<UserSignInResponse> SignInWithoutPassword(string userId)
        {
            var response = new UserSignInResponse();
            var user = await _userManager.FindByIdAsync(userId);

            if (await _userManager.IsLockedOutAsync(user))
                response.AddError("Essa conta está bloqueada");
            else if (!await _userManager.IsEmailConfirmedAsync(user))
                response.AddError("Essa conta precisa confirmar seu e-mail antes de realizar o login");

            if (response.Success)
                return await GenerateCredencials(user.Email);

            return response;
        }

        private async Task<string> GenerateToken(IEnumerable<Claim> claims, DateTime dateExpiration)
        {
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: dateExpiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            var redisKey = $"token:{jwt.Id}";
            var redisValue = token;
            var redisExpiry = dateExpiration - DateTime.Now;

            await _redisConnector.GetDatabase().StringSetAsync(redisKey, redisValue, redisExpiry);

            return token;
        }

        private async Task<IList<Claim>> GetClaims(IdentityUser user, bool addClaimsUser)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString())
            };

            if (addClaimsUser)
            {
                var userClaims = await _userManager.GetClaimsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);

                claims.AddRange(userClaims);

                foreach (var role in roles)
                    claims.Add(new Claim("role", role));
            }

            return claims;
        }

        private async Task<UserSignInResponse> GenerateCredencials(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var accessTokenClaims = await GetClaims(user, addClaimsUser: true);
            var refreshTokenClaims = await GetClaims(user, addClaimsUser: false);

            var dateExpiractionAccessToken = DateTime.Now.AddSeconds(_jwtOptions.AccessTokenExpiration);
            var dateExpiractionRefreshToken = DateTime.Now.AddSeconds(_jwtOptions.RefreshTokenExpiration);

            var accessToken = await GenerateToken(accessTokenClaims, dateExpiractionAccessToken);
            var refreshToken = await GenerateToken(refreshTokenClaims, dateExpiractionRefreshToken);

            return new UserSignInResponse
            (
                success: true,
                accessToken: accessToken,
                refreshToken: refreshToken
            );
        }

    }
}
