using System;
using System.Collections.Generic;
using System.Text;

namespace Tibos.Pipeline.Api.Model.Response
{
    public class LoginResponse
    {
        public UserDto User { get; set; }

        public List<Permissions> Permissions { get; set; }

        public string Token { get; set; }

        public DateTime ExpireAt { get; set; }
    }

    public class UserDto 
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string Phone { get; set; }
        public object Position { get; set; }

        public string Roles { get; set; }
    }

    public class Permissions 
    {
        public string Id { get; set; }

        public List<string> Operation { get; set; }
    }
}
