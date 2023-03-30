using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.Users
    {
    public class UserUpdateRequest : UserAddRequest , IModelIdentifier
        {
        public int Id { get; set; }
        }
    }
