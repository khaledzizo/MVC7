﻿using System.ComponentModel.DataAnnotations;

namespace Route.PL.Models
{
	public class LoginVM
	{
		[Required]
		[EmailAddress(ErrorMessage = "Invalid Email")]
		public string Email { get; set; }
		[Required]
		[StringLength(6, MinimumLength = 6)]
		public string Password { get; set; }
		public bool RememberMe { get; set; }
	}
}
