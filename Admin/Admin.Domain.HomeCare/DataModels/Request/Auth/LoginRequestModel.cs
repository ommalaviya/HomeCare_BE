namespace Admin.Domain.HomeCare.DataModels.Request.Auth
  {
      public class LoginRequestModel
      {
          public required string Email { get; set; }
   
          public required string Password { get; set; }
      }
  }