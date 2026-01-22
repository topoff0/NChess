using Microsoft.AspNetCore.Mvc;


namespace Account.API.Controllers
{
    [ApiController]
    [Route("api/Account")]
    public class AccountController : ControllerBase
    {
    //     private readonly IAccountService _userService;
    //     private readonly ILogger<AccountController> _logger;
    //     private readonly IEmailService _emailService;
    //     private readonly IEncryptionService _encryption;
    //     private readonly IJwtService _jwtService;
    //
    //     private readonly UserDbContext _db;
    //
    //     public AccountController(IAccountService userService, ILogger<AccountController> logger,
    //                              IEmailService emailService, UserDbContext db, IEncryptionService encryption, IJwtService jwtService)
    //     {
    //         _userService = userService;
    //         _logger = logger;
    //         _emailService = emailService;
    //         _db = db;
    //         _encryption = encryption;
    //         _jwtService = jwtService;
    //     }
    //
    //     [HttpPost("IsUserExistsAndEmailConfirmed")]
    //     public async Task<IActionResult> IsUserExistsAndEmailConfirmed([FromBody] string email)
    //     {
    //         try
    //         {
    //             IsUserExistsAndEmailConfirmedResponse response =
    //             await _userService.IsUserExistsAndEmailConfirmed(email);
    //
    //             return Ok(response);
    //         }
    //         catch (Exception ex)
    //         {
    //             _logger.LogCritical($"Exception in AccountController HttpPost(IsUserExists). Exception: {ex.Message}");
    //             return BadRequest("Something went wrong, while we were checking your email");
    //         }
    //     }
    //
    //     [HttpPost("AddNewUser")]
    //     public async Task<IActionResult> AddNewUser([FromBody] string email)
    //     {
    //         try
    //         {
    //             // Generate code
    //             string code = _userService.GenerateVerificationCode();
    //             string encryptedCode = _encryption.Encrypt(code);
    //
    //             // Add new player to database
    //             Player player = new()
    //             {
    //                 Email = email,
    //                 VerificationCode = encryptedCode
    //             };
    //
    //             // Save new player with encrypted code in DB
    //             _db.Players.Add(player);
    //
    //             await _db.SaveChangesAsync();
    //
    //             // Send verification code to user's email
    //             SendEmailRequest request = new(email, "Confirm your email", code);
    //             await _emailService.SendEmailAsync(request);
    //
    //             return Ok(new RegisterUserResponse(true, "Code was successfully sent", true));
    //         }
    //         catch (ArgumentException ArgEx) // Exception with encrypting code
    //         {
    //             _logger.LogCritical($"Exception in AccountController HttpPost(AddNewUser). {ArgEx.Message}");
    //         }
    //         catch (DbUpdateException dbEx)
    //         {
    //             _logger.LogCritical($"DB error: {dbEx.InnerException?.Message ?? dbEx.Message}");
    //         }
    //         catch (Exception ex)
    //         {
    //             _logger.LogCritical($"Exception in AccountController HttpPost(AddNewUser). Exception: {ex.Message}");
    //         }
    //         return BadRequest(new BaseResponse(false, "Something went wrong, while we were trying to send you email."));
    //     }
    //
    //     [HttpPost("SendVerificationCode")]
    //     public async Task<IActionResult> SendVerificationCode([FromBody] string email)
    //     {
    //         try
    //         {
    //             // Generate code
    //             string code = _userService.GenerateVerificationCode();
    //             string encryptedCode = _encryption.Encrypt(code);
    //
    //             // Update player's verification code in DB
    //             Player player = await _db.Players.FirstAsync(p => p.Email == email);
    //             player.VerificationCode = encryptedCode;
    //             _db.Players.Update(player);
    //             await _db.SaveChangesAsync();
    //
    //             // Send verification code to user's email
    //             SendEmailRequest request = new(email, "Confirm your email", code);
    //             await _emailService.SendEmailAsync(request);
    //
    //             return Ok(new RegisterUserResponse(true, "Code was successfully sent", true));
    //         }
    //         catch (ArgumentException ArgEx) // Exception with encrypting code
    //         {
    //             _logger.LogCritical($"ArgumentException in AccountController HttpPost(AddNewUser). {ArgEx.Message}");
    //         }
    //         catch (InvalidOperationException operationException) // Player with this email not found
    //         {
    //             _logger.LogCritical($"InvalidOperationException in AccountController HttpPost(AddNewUser). {operationException.Message}");
    //         }
    //         catch (Exception ex) // Other exceptions
    //         {
    //             _logger.LogCritical($"Exception in AccountController HttpPost(SendVerificationCode). Exception: {ex.Message}");
    //         }
    //         return BadRequest(new BaseResponse(false, "Something went wrong, while we were trying to send you email."));
    //     }
    //
    //     [HttpPut("VerifyCode")]
    //     public async Task<IActionResult> VerifyCode(VerifyCodeRequest request)
    //     {
    //         try
    //         {
    //             Player? player = _db.Players.FirstOrDefault(p => p.Email == request.Email);
    //             if (player == null)
    //             {
    //                 throw new ArgumentException("Player with this email was not found.");
    //             }
    //             // Get verification code from DB
    //             string decryptedCode = _encryption.Decrypt(player.VerificationCode);
    //
    //             // If user's input code is wrong
    //             if (decryptedCode != request.Code)
    //                 return Ok(new VerifyResponse(false, "The code is wrong.", false, null));
    //
    //             player.IsEmailConfirmed = true;
    //             _db.Players.Update(player);
    //             await _db.SaveChangesAsync();
    //
    //             // Generate JWT token
    //             string jwtToken = _jwtService.GenerateAccessToken(
    //                 new GenerateAccessTokenRequest(player.Id, "", player.Email));
    //
    //             // Generate refresh token
    //             string refreshToken = _jwtService.GenerateRefreshToken();
    //             await _jwtService.SaveRefreshTokenAsync(player.Id.ToString(), refreshToken);
    //
    //             SetRefreshTokenCookie(refreshToken);
    //
    //             return Ok(new VerifyResponse(true, "Email successfully confirmed", true, jwtToken));
    //         }
    //         catch (ArgumentException ex)
    //         {
    //             _logger.LogCritical($"Exception in AccountController HttpPost(VerifyCode). Exception: {ex.Message}");
    //             return BadRequest(new BaseResponse(false, "Something went wrong, while we were trying confirm email."));
    //         }
    //         catch (Exception ex)
    //         {
    //             _logger.LogCritical($"Exception in AccountController HttpPost(VerifyCode). Exception: {ex.Message}");
    //             return BadRequest(new BaseResponse(false, "Something went wrong, while we were trying confirm email."));
    //         }
    //     }
    //     [HttpPost("LoginByPassword")]
    //     public async Task<IActionResult> LoginByPassword([FromBody] LoginByPasswordRequest request)
    //     {
    //         try
    //         {
    //             LoginResponse response = await _userService.LoginByPassword(request);
    //
    //             string refreshToken = _jwtService.GenerateRefreshToken();
    //             await _jwtService.SaveRefreshTokenAsync(response.UserId.ToString(), refreshToken);
    //
    //             SetRefreshTokenCookie(refreshToken);
    //             return Ok(new
    //             {
    //                 response.IsSuccess,
    //                 response.ResponseMessage,
    //                 response.JwtToken,
    //             });
    //         }
    //         catch (Exception ex)
    //         {
    //             _logger.LogCritical($"Exception, while trying to login: {ex.Message}");
    //             var badResponse = new BaseResponse(false, ex.Message);
    //             return BadRequest(badResponse);
    //         }
    //     }
    //
    //     [HttpPost("CreateAccount")]
    //     [Authorize]
    //     public async Task<IActionResult> CreateAccount([FromForm] CreateAccountRequest request)
    //     {
    //         try
    //         {
    //             await _userService.CreateAccount(request, User);
    //         }
    //         catch (ArgumentNullException ex)
    //         {
    //             _logger.LogCritical($"ArgumentException in AccountController(CreateAccount): {ex.Message}");
    //             return BadRequest(new BaseResponse(false, ex.Message));
    //         }
    //         catch (KeyNotFoundException ex)
    //         {
    //             _logger.LogCritical($"ArgumentException in AccountController(CreateAccount): {ex.Message}");
    //             return BadRequest(new BaseResponse(false, ex.Message));
    //         }
    //         catch (Exception ex)
    //         {
    //             _logger.LogCritical($"Exception in AccountController(CreateAccount): {ex.Message}");
    //             return BadRequest(new BaseResponse(false, "Something went wrong, while we were trying to create your account"));
    //         }
    //
    //         return Ok(new BaseResponse(true, "Account was successfully created"));
    //     }
    //     [HttpPost("RefreshToken")]
    //     public async Task<IActionResult> RefreshToken()
    //     {
    //         try
    //         {
    //             // Get Refresh Token from cookies
    //             var refreshToken = Request.Cookies["refreshToken"];
    //             if (string.IsNullOrEmpty(refreshToken))
    //                 return Unauthorized(new { Message = "Refresh token is missing" });
    //
    //             // Check refresh token on valid
    //             var storedToken = await _jwtService.GetRefreshToken(refreshToken);
    //             if (storedToken == null || !storedToken.IsActive)
    //                 return Unauthorized(new { Message = "Invalid refresh token" });
    //
    //             // Get user
    //             var player = await _db.Players.FindAsync(storedToken.UserId);
    //             if (player == null || player.Username == null)
    //                 return Unauthorized(new { Message = "User not found" });
    //
    //             // Generate new access token
    //             var newAccessToken = _jwtService.GenerateAccessToken(
    //                 new GenerateAccessTokenRequest(player.Id, player.Username, player.Email));
    //
    //             // Generate new refresh token
    //             var newRefreshToken = _jwtService.GenerateRefreshToken();
    //
    //             // Удаляем старый и сохраняем новый Refresh Token
    //             await _jwtService.RevokeRefreshToken(refreshToken);
    //             await _jwtService.SaveRefreshTokenAsync(player.Id.ToString(), newRefreshToken);
    //
    //             // Set new refresh token in cookies
    //             SetRefreshTokenCookie(newRefreshToken);
    //
    //             return Ok(new
    //             {
    //                 AccessToken = newAccessToken
    //             });
    //         }
    //         catch (Exception ex)
    //         {
    //             _logger.LogError($"Refresh token failed: {ex.Message}");
    //             return StatusCode(500, new { Message = "Internal server error" });
    //         }
    //     }
    //
    //     [HttpGet("GetUserData")]
    //     [Authorize]
    //     public IActionResult GetData()
    //     {
    //         try
    //         {
    //             var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //
    //             var username = User.FindFirst(ClaimTypes.Name)?.Value;
    //
    //             var email = User.FindFirst(ClaimTypes.Email)?.Value;
    //
    //             return Ok(new { UserId = userId, Username = username, Email = email });
    //         }
    //         catch (ArgumentNullException ex)
    //         {
    //             _logger.LogCritical($"ArgumentNullException in AccountController(GetUserData): {ex.Message}");
    //         }
    //         catch (Exception ex)
    //         {
    //             _logger.LogCritical($"Exception in AccountController(GetUserData): {ex.Message}");
    //         }
    //
    //         return BadRequest(new BaseResponse(false, "Something went wrong, while we were trying to get your data"));
    //     }
    //
    //     [HttpGet("ValidateToken")]
    //     [Authorize]
    //     public IActionResult ValidateToken()
    //     {
    //         return Ok(new { isValid = true });
    //     }
    //
    //     private void SetRefreshTokenCookie(string refreshToken)
    //     {
    //         var cookieOptions = new CookieOptions
    //         {
    //             HttpOnly = true,
    //             Secure = true,
    //             SameSite = SameSiteMode.Strict,
    //             Expires = DateTime.UtcNow.AddDays(14)
    //         };
    //
    //         Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    //     }
    }
}
