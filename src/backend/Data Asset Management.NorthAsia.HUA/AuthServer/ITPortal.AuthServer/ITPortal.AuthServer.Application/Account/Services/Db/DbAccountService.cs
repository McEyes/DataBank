using Elastic.Clients.Elasticsearch.MachineLearning;

using Furion;
using Furion.JsonSerialization;

using ITPortal.AuthServer.Application.Account.Dtos;
using ITPortal.AuthServer.Application.Account.Services.Dtos;
using ITPortal.Core.Encrypt;
using ITPortal.Core.Services;

using MapsterMapper;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Core.AccountService
{
    public class DbAccountService : AccountService
    {
        //private readonly IMapper _mapper; IMapper mapper,
        private readonly PasswordHasher<UserEntity> _passwordHasher;
        private readonly IUserService _userService;
        public DbAccountService(IUserService userService)
        {
            //_mapper = mapper;
            _passwordHasher = new PasswordHasher<UserEntity>();
            _userService = userService;
        }

        public override async Task<ITPortal.Core.Services.IResult> Login(LoginInput input)
        {
            var result = new Result<LoginDto>();
            var userInfo = await _userService.GetUserRoleInfo(input.Name);
            if (userInfo == null)
            {
                result.Success = false;
                result.Msg = "账号或者密码错误！";
                return result;
            }
            string pwdHash = _passwordHasher.HashPassword(userInfo, input.Password);
            var flag = _passwordHasher.VerifyHashedPassword(userInfo, userInfo.PasswordHash, input.Password);

            var flag2 = _passwordHasher.VerifyHashedPassword(userInfo, pwdHash, input.Password);

            byte[] decodedHashedPassword = Convert.FromBase64String(userInfo.PasswordHash);

            if (VerifyHashedPasswordV3(decodedHashedPassword, input.Password, out int embeddedIterCount, out KeyDerivationPrf prf))
            {
                var _iterCount = 100_000;
                // If this hasher was configured with a higher iteration count, change the entry now.
                if (embeddedIterCount < _iterCount)
                {
                    //return PasswordVerificationResult.SuccessRehashNeeded;
                }

                // V3 now requires SHA512. If the old PRF is SHA1 or SHA256, upgrade to SHA512 and rehash.
                if (prf == KeyDerivationPrf.HMACSHA1 || prf == KeyDerivationPrf.HMACSHA256)
                {
                    //return PasswordVerificationResult.SuccessRehashNeeded;
                }

                //return PasswordVerificationResult.Success;
            }
            //else
            //{
            //    return PasswordVerificationResult.Failed;
            //}
            //var data = PasswordHasherSHA256.VerifyHashedPassword(userInfo.PasswordHash, password);
            if (userInfo != null && flag != PasswordVerificationResult.Failed)
            {
                result.Data = userInfo.Adapt<LoginDto>();// _mapper.Map<UserEntity, LoginDto>(userInfo);
                result.Data.Token = _userService.GenerateJwtToken(userInfo);
                result.Data.UserToken = ITPortal.Core.DESEncryption.Encrypt(userInfo.Id.ToString());
            }
            else
            {
                result.Success = false;
                result.Msg = "账号或者密码错误！";
            }
            return result;
        }




        public override async Task<ITPortal.Core.Services.IResult> Reset(ResetInput input)
        {
            var result = new Result<LoginDto>();
            var userInfo = await _userService.GetUserRoleInfo(input.Name);
            if (userInfo != null)
            {
                result.Success = false;
                result.Msg = "账号或者密码错误！";
                return result;
            }
            string pwdHash = _passwordHasher.HashPassword(userInfo, input.Password);
            var flag = _passwordHasher.VerifyHashedPassword(userInfo, userInfo.PasswordHash, input.Password);
            if (userInfo != null && flag != PasswordVerificationResult.Failed)
            {
                userInfo.PasswordHash = _passwordHasher.HashPassword(userInfo, input.NewPassword);
                await _userService.Modify(userInfo);

                result.Data = userInfo.Adapt<LoginDto>(); // _mapper.Map<UserEntity, LoginDto>(userInfo);
                result.Data.Token = _userService.GenerateJwtToken(userInfo);
                result.Data.UserToken = ITPortal.Core.DESEncryption.Encrypt(userInfo.Id.ToString());
            }
            else
            {
                result.Success = false;
                result.Msg = "账号或者密码错误！";
            }
            return result;
        }

        public async Task<List<UserEntity>> GetAllUser()
        {
            return await _userService.GetAllUser();
        }

        public override async Task InitResetPwd()
        {
            await Task.CompletedTask;
        }


        public override async Task<ITPortal.Core.Services.IResult> InitReset(ResetInput input)
        {
            var result = new Result<LoginDto>();
            // 连接的是jabus数据库不容许修改，什么时候换成自己的数据库时才考虑
            return result;
            var userInfo = await _userService.GetUserRoleInfo(input.Name);
            if (userInfo != null)
            {
                result.Success = false;
                result.Msg = "账号或者密码错误！";
                return result;
            }
            if (userInfo != null)
            {
                userInfo.PasswordHash = _passwordHasher.HashPassword(userInfo, input.NewPassword);
                await _userService.Modify(userInfo);

                result.Data = userInfo.Adapt<LoginDto>();// _mapper.Map<UserEntity, LoginDto>(userInfo);
                result.Data.Token = _userService.GenerateJwtToken(userInfo);
                result.Data.UserToken = ITPortal.Core.DESEncryption.Encrypt(userInfo.Id.ToString());
            }
            else
            {
                result.Success = false;
                result.Msg = "账号或者密码错误！";
            }
            return result;
        }

        public override ITPortal.Core.Services.IResult Logout()
        {
            return new Result<bool>() { Data = true };
        }


        public static bool VerifyHashedPasswordV3(byte[] hashedPassword, string password, out int iterCount, out KeyDerivationPrf prf)
        {
            iterCount = default(int);
            prf = default(KeyDerivationPrf);

            try
            {
                // Read header information
                prf = (KeyDerivationPrf)ReadNetworkByteOrder(hashedPassword, 1);
                iterCount = (int)ReadNetworkByteOrder(hashedPassword, 5);
                int saltLength = (int)ReadNetworkByteOrder(hashedPassword, 9);

                // Read the salt: must be >= 128 bits
                if (saltLength < 128 / 8)
                {
                    return false;
                }
                byte[] salt = new byte[saltLength];
                Buffer.BlockCopy(hashedPassword, 13, salt, 0, salt.Length);

                // Read the subkey (the rest of the payload): must be >= 128 bits
                int subkeyLength = hashedPassword.Length - 13 - salt.Length;
                if (subkeyLength < 128 / 8)
                {
                    return false;
                }
                byte[] expectedSubkey = new byte[subkeyLength];
                Buffer.BlockCopy(hashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

                // Hash the incoming password and verify it
                byte[] actualSubkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, subkeyLength);
#if NETSTANDARD2_0 || NETFRAMEWORK
            return ByteArraysEqual(actualSubkey, expectedSubkey);
#elif NETCOREAPP
                return FixedTimeEquals(actualSubkey, expectedSubkey);
                //return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
#else
#error Update target frameworks
#endif
            }
            catch
            {
                // This should never occur except in the case of a malformed payload, where
                // we might go off the end of the array. Regardless, a malformed payload
                // implies verification failed.
                return false;
            }
        }
        /// <summary>
        /// Determine the equality of two byte sequences in an amount of time which depends on
        /// the length of the sequences, but not the values.
        /// </summary>
        /// <param name="left">The first buffer to compare.</param>
        /// <param name="right">The second buffer to compare.</param>
        /// <returns>
        ///   <c>true</c> if <paramref name="left"/> and <paramref name="right"/> have the same
        ///   values for <see cref="ReadOnlySpan{T}.Length"/> and the same contents, <c>false</c>
        ///   otherwise.
        /// </returns>
        /// <remarks>
        ///   This method compares two buffers' contents for equality in a manner which does not
        ///   leak timing information, making it ideal for use within cryptographic routines.
        ///   This method will short-circuit and return <c>false</c> only if <paramref name="left"/>
        ///   and <paramref name="right"/> have different lengths.
        ///
        ///   Fixed-time behavior is guaranteed in all other cases, including if <paramref name="left"/>
        ///   and <paramref name="right"/> reference the same address.
        /// </remarks>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool FixedTimeEquals(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
        {
            // NoOptimization because we want this method to be exactly as non-short-circuiting
            // as written.
            //
            // NoInlining because the NoOptimization would get lost if the method got inlined.

            if (left.Length != right.Length)
            {
                return false;
            }

            int length = left.Length;
            int accum = 0;

            for (int i = 0; i < length; i++)
            {
                accum |= left[i] - right[i];
            }

            return accum == 0;
        }

        public static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                | ((uint)(buffer[offset + 1]) << 16)
                | ((uint)(buffer[offset + 2]) << 8)
                | ((uint)(buffer[offset + 3]));
        }

        public static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }

    }
}
