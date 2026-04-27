namespace Admin.Domain.HomeCare.DataModels.Request.Auth
  {
      public class ResetPasswordRequestModel
      {
          public required string Token { get; set; }
   
          public required string NewPassword { get; set; }
   
          public required string ConfirmPassword { get; set; }
      }
  }