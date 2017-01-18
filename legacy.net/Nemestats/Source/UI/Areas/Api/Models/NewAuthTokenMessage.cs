﻿
using System;

namespace UI.Areas.Api.Models
{
    public class NewAuthTokenMessage
    {
        public string AuthenticationToken { get; set; }
        public DateTime? AuthenticationTokenExpirationDateTime { get; set; }
    }
}
