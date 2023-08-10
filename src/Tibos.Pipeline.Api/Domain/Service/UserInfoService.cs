using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tibos.Pipeline.Api.Common;
using Tibos.Pipeline.Api.Domain.IService;
using Tibos.Pipeline.Api.Model.Config;
using Tibos.Pipeline.Api.Model.Entity;
using Tibos.Pipeline.Api.Model.Enum;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;

namespace Tibos.Pipeline.Api.Domain.Service
{
    public class UserInfoService: UserInfoExtensions,IUserInfoService
    {
        private readonly PipelineDBContext _context;
        private readonly IMapper _mapper;
        private readonly IGitlabService _gitlabService;
        private readonly IOptions<JwtOptions> _jwtOptions;

        public UserInfoService(PipelineDBContext context, IMapper mapper, IGitlabService gitlabService, IOptions<JwtOptions> jwtOptions)
        {
            _context = context;
            _mapper = mapper;
            _gitlabService= gitlabService;
            _jwtOptions = jwtOptions;
        }

        /// <summary>
        /// gitlab 登录
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<JsonResponse<LoginResponse>> GitlabLogin(string code)
        {
            var response = new JsonResponse<LoginResponse>();
            try
            {
                var auth = await _gitlabService.AuthToken(code);
                if (auth.code == "0")
                {
                    var access_token = auth.data.access_token;
                    //var user = await _gitlabService.GetUser(access_token);
                    var userInfo = await _gitlabService.GetUserInfo(access_token);
                    if (userInfo.code != "0")
                    {
                        response.code = "-1";
                        response.message = "gitlab oAuth 获取用户信息失败!";
                        return response;
                    }
                    var request = new GitlabLoginRequest()
                    {
                        Avatar = userInfo.data.Picture,
                        Groups = userInfo.data.Groups,
                        NickName = userInfo.data.NickName,
                        OpenId = userInfo.data.SubLegacy
                    };
                    var res = await GitlabLogin(request);
                    if (res.code != "0")
                    {
                        response.code = res.code;
                        response.message = res.message;
                        return response;
                    }
                    return await CreateToken(res.data);
                }
                else
                {
                    response.code = auth.code;
                    response.message = auth.message;
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = "系统错误";
                return response;
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse<LoginResponse>> Login(AccountLoginRequest request)
        {
            var response = new JsonResponse<LoginResponse>();
            try
            {
                var res = await AccountLogin(request);
                if (res.code == "0")
                {
                    return await CreateToken(res.data);
                }
                else
                {
                    response.code = res.code;
                    response.message = res.message;
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = "系统错误";
                return response;
            }
        }


        /// <summary>
        /// 创建登录类型
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> CreateUserLogin(CreateUserLoginRequest request)
        {
            var response = new JsonResponse();
            try
            {
                if(request == null || string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.Account)) 
                {
                    response.code = "-1";
                    response.message = "参数错误";
                    return response;
                }

                var user = await _context.UserInfo.FirstOrDefaultAsync(m => m.Id == request.UserId);
                if(user == null) 
                {
                    response.code = "-1";
                    response.message = "用户不存在";
                    return response;
                }

                var bl = await _context.UserLogin.AnyAsync(m => m.UserId == request.UserId && m.LoginType == (int)LoginType.Default);
                if (bl)
                {
                    response.code = "-1";
                    response.message = "已创建过账号,无法从新创建";
                    return response;
                }
                bl = await _context.UserLogin.AnyAsync(m => m.Account == request.Account);
                if (bl) 
                {
                    response.code = "-1";
                    response.message = "账号已存在";
                    return response;
                }

                var model = _mapper.Map<UserLoginEntity>(request);
                model.Pwd = EncryptHelper.Md5Encrypt(request.Pwd, "utf-8");
                model.LoginType = (int)LoginType.Default;
                model.CreateTime = DateTime.Now;
                await _context.UserLogin.AddAsync(model);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> UpdateUserInfo(UpdateUserInfoRequest request)
        {
            var response = new JsonResponse();
            try
            {


                var model = await _context.UserInfo.FirstOrDefaultAsync(m => m.Id == request.UserId);
                if (model == null)
                {
                    response.code = "-1";
                    response.message = "数据不存在!";
                    return response;
                }
                if (model.Roles.Split(',').Contains(RoleType.Admin.ToString())) 
                {
                    response.code = "-1";
                    response.message = "管理员信息无法修改!";
                    return response;
                }
                var userLogin = await _context.UserLogin.FirstOrDefaultAsync(m => m.UserId == request.UserId && m.LoginType ==(int)LoginType.Default);
                if (userLogin == null) 
                {
                    response.code = "-1";
                    response.message = "用户信息错误!";
                    return response;
                }
                var bl = await _context.UserLogin.AnyAsync(m => m.Account == request.Account && m.UserId != request.UserId);
                if (bl) 
                {
                    response.code = "-1";
                    response.message = "账号已存在!";
                    return response;
                }

                userLogin.Account = request.Account;
                userLogin.Pwd = EncryptHelper.Md5Encrypt(request.Pwd, "utf-8");
                model.Roles = string.Join(",", request.Roles);
                model.Phone = request.Phone;
                _context.UserLogin.Update(userLogin);
                _context.UserInfo.Update(model);

                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }


        /// <summary>
        /// 修改用户状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> UpdateUserStatus(UpdateUserStatusRequest request) 
        {
            var response = new JsonResponse();
            try
            {


                var model = await _context.UserInfo.FirstOrDefaultAsync(m => m.Id == request.UserId);
                if (model == null)
                {
                    response.code = "-1";
                    response.message = "数据不存在!";
                    return response;
                }
                if (model.Roles.Split(',').Contains(RoleType.Admin.ToString()))
                {
                    response.code = "-1";
                    response.message = "管理员状态无法修改!";
                    return response;
                }
                model.Status = request.Status;
                _context.UserInfo.Update(model);

                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }


        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResponse<GetUserLoginResponse>> GetUserLogin(string userId)
        {
            var response = new JsonResponse<GetUserLoginResponse>();
            try
            {
                var query = from user in _context.UserInfo
                            join userLogin in _context.UserLogin on user.Id equals userLogin.UserId
                            where userLogin.UserId == userId && userLogin.LoginType ==(int)LoginType.Default
                            select new GetUserLoginResponse() 
                            {
                                Account = userLogin.Account,
                                Phone = user.Phone,
                                Roles= user.Roles,
                                UserId= userLogin.UserId
                            };



                var model = await query.FirstOrDefaultAsync();
                if (model == null)
                {
                    response.code = "-1";
                    response.message = "数据不存在!";
                    return response;
                }
                response.data = model;
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse<List<UserInfoResponse>>> GetUserInfoList(GetUserInfoListRequest request)
        {
            var response = new JsonResponse<List<UserInfoResponse>>();
            try
            {
                var query = _context.UserInfo.AsQueryable();
                if (!string.IsNullOrEmpty(request.Phone))
                {
                    query = query.Where(m => m.Phone.Contains(request.Phone)); ;
                }
                if (!string.IsNullOrEmpty(request.NickName))
                {
                    query = query.Where(m => m.NickName.Contains(request.NickName));
                }
                if (request.Status.HasValue)
                {
                    query = query.Where(m => m.Status == request.Status);
                }
                var list = await query.Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize).ToListAsync();
                if (list.Count > 0)
                {
                    response.data = _mapper.Map<List<UserInfoResponse>>(list);
                    foreach (var item in response.data)
                    {
                        var list_userLogin = await _context.UserLogin.Where(m => m.UserId == item.Id).ToListAsync();
                        item.LoginTypes = list_userLogin.Select(m => m.LoginType).ToList();
                    }
                }
                response.total = await query.CountAsync();
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }


        /// <summary>
        /// 查询用户信息
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResponse<UserInfoResponse>> GetUserInfo()
        {
            var response = new JsonResponse<UserInfoResponse> ();
            try
            {
                var model = await _context.UserInfo.FirstOrDefaultAsync(m=>m.Id == UserId);
                if(model == null) 
                {
                    response.code = "-1";
                    response.message = "用户不存在!";
                    return response;
                }
                response.data = _mapper.Map<UserInfoResponse>(model);
                response.data.LoginTypes = await _context.UserLogin.Where(m => m.UserId == UserId).Select(m => m.LoginType).ToListAsync();
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// 获取所有角色(排除Admin)
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResponse<List<EnumberEntity>>> GetRoles() 
        {
            var response = new JsonResponse<List<EnumberEntity>>();
            response.data = EnumberHelper.EnumToList<RoleType>().Where(m=>m.EnumName != RoleType.Admin.ToString()).ToList();
            return response;
        }



        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> UpdateUserPwd(UpdateUserPwdDto request)
        {
            var response = new JsonResponse();
            try
            {


                var bl = await _context.UserInfo.AnyAsync(m => m.Id == request.UserId);
                if (!bl)
                {
                    response.code = "-1";
                    response.message = "数据不存在!";
                    return response;
                }
                var userLogin = await _context.UserLogin.FirstOrDefaultAsync(m => m.UserId == request.UserId && m.LoginType ==(int)LoginType.Default);
                if (userLogin == null) 
                {
                    response.code = "-1";
                    response.message = "账号不存在!";
                    return response;
                }
                var oldPwd = EncryptHelper.Md5Encrypt(request.OldPwd, "utf-8");
                if(userLogin.Pwd != oldPwd) 
                {
                    response.code = "-1";
                    response.message = "原密码错误!";
                    return response;
                }
                userLogin.Pwd = EncryptHelper.Md5Encrypt(request.Pwd, "utf-8");

                _context.UserLogin.Update(userLogin);

                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }







        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private async Task<JsonResponse<LoginResponse>> CreateToken(UserDto dto) 
        {
            var response = new JsonResponse<LoginResponse>();
            try
            {
                //生成JWT
                //秘钥，就是标头，这里用Hmacsha256算法，需要256bit的密钥
                var securityKey = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Secret)), SecurityAlgorithms.HmacSha256);
                //相当于有效载荷
                var claims = new Claim[] {
                    new Claim(JwtRegisteredClaimNames.Iss,_jwtOptions.Value.Iss),
                    new Claim(JwtRegisteredClaimNames.Aud,_jwtOptions.Value.Aud),
                    new Claim("UserId",dto.UserId),
                    new Claim("UserName",dto.UserName),
                    new Claim(ClaimTypes.Role, dto.Roles),

                    };
                SecurityToken securityToken = new JwtSecurityToken(
                    signingCredentials: securityKey,
                    expires: DateTime.Now.AddMinutes(60 * 2),//过期时间
                    claims: claims
                );
                //生成jwt令牌
                var jwtToken = new JwtSecurityTokenHandler().WriteToken(securityToken);
                response.data = new LoginResponse()
                {
                    Token = jwtToken,
                    ExpireAt = DateTime.Now.AddMinutes(60 * 2),
                    User = dto,
                    Permissions = new List<Permissions>()
                        {
                        new Permissions()
                        {
                            Id = "queryForm",
                            Operation = new List<string>(){ "add","edit"}
                        },
                        new Permissions()
                        {
                            Id = "analysis",
                            Operation = new List<string>(){ "add","edit"}
                        }
                    }
                };
                response.message = $"欢迎{dto.UserName}回来";
                return response;
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = "生成token失败";
                return response;
            }
        }


        /// <summary>
        /// gitlab 登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<JsonResponse<UserDto>> GitlabLogin(GitlabLoginRequest request) 
        {
            var response = new JsonResponse<UserDto>();
            using (var db = _context)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var model = await _context.UserLogin.AsQueryable().FirstOrDefaultAsync(m => m.LoginType == (int)LoginType.Gitlab && m.OpenId == request.OpenId);
                        if (model == null)
                        {
                            var user = new UserInfoEntity()
                            {
                                Id = Guid.NewGuid().ToString("N"),
                                AvatarUrl = request.Avatar,
                                CreateTime = DateTime.Now,
                                Group = string.Join(',', request.Groups??new List<string>()),
                                NickName = request.NickName,
                                Phone = "",
                                Roles = RoleType.Other.ToString(),
                                Status = 1
                            };

                            model = new UserLoginEntity()
                            {
                                UserId = user.Id,
                                CreateTime = DateTime.Now,
                                LoginType = (int)LoginType.Gitlab,
                                OpenId = request.OpenId,
                                LastLoginTime = DateTime.Now,

                            };
                            await _context.UserInfo.AddAsync(user);
                            await _context.UserLogin.AddAsync(model);

                            response.data = new UserDto()
                            {
                                UserId = user.Id,
                                UserName = user.NickName,
                                Avatar = user.AvatarUrl,
                                Phone = user.Phone,
                                Roles = user.Roles
                                //Position = new { CN = "产品分析师 | 蚂蚁金服-计算服务事业群-IOS平台部", HK = "產品分析師 | 螞蟻金服-計算服務事業群-IOS平台部", US = "Product analyst | Ant Financial - Computing services business group - IOS platform division" }
                            };
                        }
                        else
                        {
                            var user = await _context.UserInfo.AsQueryable().FirstOrDefaultAsync(m => m.Id == model.UserId);
                            if (user == null)
                            {
                                response.code = "-1";
                                response.message = "账号异常!";
                                return response;
                            }
                            if(user.Status == 0) 
                            {
                                response.code = "-1";
                                response.message = "账号已禁用!";
                                return response;
                            }
                            user.AvatarUrl = request.Avatar;
                            user.NickName = request.NickName;
                            user.Group = string.Join(',', request.Groups??new List<string>());
                            model.LastLoginTime = DateTime.Now;

                            _context.UserLogin.Update(model);
                            _context.UserInfo.Update(user);

                            response.data = new UserDto()
                            {
                                UserId = user.Id,
                                UserName = user.NickName,
                                Avatar = user.AvatarUrl,
                                Phone = user.Phone,
                                Roles = user.Roles
                                //Position = new { CN = "产品分析师 | 蚂蚁金服-计算服务事业群-IOS平台部", HK = "產品分析師 | 螞蟻金服-計算服務事業群-IOS平台部", US = "Product analyst | Ant Financial - Computing services business group - IOS platform division" }
                            };
                        }
                        await _context.SaveChangesAsync();
                        await db.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        response.code = "-1";
                        response.message = "系统错误!";
                        return response;
                    }
                    return response;
                }
            }
        }


        /// <summary>
        /// 账号登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<JsonResponse<UserDto>> AccountLogin(AccountLoginRequest request) 
        {
            var response = new JsonResponse<UserDto>();
            try
            {
                if(request == null || string.IsNullOrEmpty(request.Account) || string.IsNullOrEmpty(request.Pwd)) 
                {
                    response.code= "-1";
                    response.message = "参数错误!";
                    return response;
                }

                var pwd = EncryptHelper.Md5Encrypt(request.Pwd, "utf-8");
                var model = await _context.UserLogin.FirstOrDefaultAsync(m=>m.LoginType ==(int)LoginType.Default && m.Account == request.Account && m.Pwd == pwd);
                if(model == null) 
                {
                    response.code = "-1";
                    response.message = "账号或密码错误!";
                    return response;
                }
                var user = await _context.UserInfo.FirstOrDefaultAsync(m => m.Id == model.UserId);
                if(user == null) 
                {
                    response.code = "-1";
                    response.message = "账号异常!";
                    return response;
                }
                if (user.Status == 0)
                {
                    response.code = "-1";
                    response.message = "账号已禁用!";
                    return response;
                }
                model.LastLoginTime = DateTime.Now;
                _context.Update(model);
                await _context.SaveChangesAsync();
                response.data = new UserDto()
                {
                    UserId = user.Id,
                    UserName = user.NickName,
                    Avatar = user.AvatarUrl,
                    Phone = user.Phone,
                    Roles = user.Roles
                    //Position = new { CN = "产品分析师 | 蚂蚁金服-计算服务事业群-IOS平台部", HK = "產品分析師 | 螞蟻金服-計算服務事業群-IOS平台部", US = "Product analyst | Ant Financial - Computing services business group - IOS platform division" }
                };
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = "系统错误!";
                return response;
            }
            return response;
        }
    }
}
