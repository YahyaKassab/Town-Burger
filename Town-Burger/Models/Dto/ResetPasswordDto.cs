﻿namespace Town_Burger.Models.Dto
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }
    }
}
