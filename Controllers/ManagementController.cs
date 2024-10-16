using EC.Data;
using EC.DTO_Model;
using EC.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace EC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly JWTService _jwtService;
        private readonly ILogger<ManagementController> _logger;

        public ManagementController(DataContext context, IConfiguration configuration, JWTService jwtService, ILogger<ManagementController> logger)
        {
            _context = context;
            _configuration = configuration;
            _jwtService = jwtService;
            _logger = logger;
        }

        #region User
        [HttpGet("Token")]
        public async Task<ActionResult<UserDataDTOResponseModel>> GetToken([FromHeader(Name = "Authorization")] string authorization, string userID)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var user = await _context.UserData.FirstOrDefaultAsync(user => user.UserID == userID);

                if ((user == null) || (user.RefreshToken != token)) return BadRequest();

                var accessToken = _jwtService.GenerateJwtToken(user.UserID, DateTime.UtcNow.AddHours(Constants.TOKEN_VALID_HOURS));
                return Ok(new UserDataDTOResponseModel(user, accessToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("GetToken request:\r\nuserID: {0}\r\n\r\n\r\n", userID));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("Logout")]
        public async Task<ActionResult> GetLogout([FromHeader(Name = "Authorization")] string authorization, string userID)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var user = await _context.UserData.FirstOrDefaultAsync(user => user.UserID == userID);

                if (user == null) return BadRequest();

                user.RefreshToken = string.Empty;
                _context.UserData.Update(user);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("GetLogout request:\r\nuserID: {0}\r\n\r\n\r\n", userID));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("User")]
        public async Task<ActionResult<UserDataDTOResponseModel>> GetUser([FromHeader(Name = "Authorization")] string authorization, string keyword = "")
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var users = await _context.UserData.Where(user => string.IsNullOrEmpty(keyword) || user.UserName.Contains(keyword) || user.Email.Contains(keyword) || user.Phone.Contains(keyword)).ToArrayAsync();

                return Ok(users.Select(user => new UserDataDTOResponseModel(user)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("GetUser request keyword:\r\nkeyword: {0}\r\n\r\n\r\n", keyword));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("User")]
        public async Task<ActionResult<UserDataDTOResponseModel>> PostUser([FromHeader(Name = "Authorization")] string authorization, [FromBody] UserDataDTORequestModel userData)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var user = await _context.UserData.FirstOrDefaultAsync(user => (user.Email == userData.Email) || (user.Phone == userData.Phone));

                var error = string.Empty;
                if ((user != null) && (user.Email == userData.Email)) error = "Invalid email";
                else if ((user != null) && (user.Phone == userData.Phone)) error = "Invalid phone";
                else if (new Regex(Constants.REGEX_EMAIL).IsMatch(userData.Email) == false) error = "Invalid email format";
                else if (new Regex(Constants.REGEX_PHONE).IsMatch(userData.Phone) == false) error = "Invalid phone format";
                else if (string.IsNullOrEmpty(userData.Pass) || (userData.Pass.Length < Constants.MIN_PASS_LENGTH)) error = string.Format("Invalid min pass length is {0}", Constants.MIN_PASS_LENGTH);

                if (string.IsNullOrEmpty(error) == false) return BadRequest(new UserDataDTOResponseModel(error));

                user = new UserDataModel(userData);
                await _context.UserData.AddAsync(user);
                await _context.SaveChangesAsync();
                return Ok(new UserDataDTOResponseModel(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("PostUser request:\r\n{0}\r\n\r\n\r\n", userData));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDataDTOResponseModel>> PostLogin(LoginDataDTORequestModel userData)
        {
            try
            {
                var user = await _context.UserData.FirstOrDefaultAsync(user => user.Email == userData.Email);

                var error = string.Empty;
                if (user == null) error = "Invalid email";
                else if (userData.Pass.Length < Constants.MIN_PASS_LENGTH) error = string.Format("Invalid min pass length is {0}", Constants.MIN_PASS_LENGTH);
                else if (user.Pass != UserDataModel.HashPassword(userData.Pass, user.Salt)) error = "Invalid password";
                else if (user.IsLocked) error = "Invalid user is locked";

                if (string.IsNullOrEmpty(error) == false) return BadRequest(new UserDataDTOResponseModel(error));

                var accessToken = _jwtService.GenerateJwtToken(user.UserID, DateTime.UtcNow.AddHours(Constants.TOKEN_VALID_HOURS));
                var refreshToken = _jwtService.GenerateJwtToken(user.UserID, DateTime.UtcNow.AddDays(Constants.TOKEN_VALID_DAYS));
                user.RefreshToken = refreshToken;
                _context.UserData.Update(user);
                await _context.SaveChangesAsync();
                return Ok(new UserDataDTOResponseModel(user, accessToken, refreshToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("PostLogin request:\r\n{0}\r\n\r\n\r\n", userData));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("ChangePass")]
        public async Task<ActionResult> PostChangePass([FromHeader(Name = "Authorization")] string authorization, [FromBody] UserDataDTORequestModel userData)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var user = await _context.UserData.FirstOrDefaultAsync(user => user.UserID == userData.UserID);

                var error = string.Empty;
                if ((user == null) || string.IsNullOrEmpty(userData.OldPass) || string.IsNullOrEmpty(userData.Pass)) error = "Invalid data";
                else if (userData.Pass.Length < Constants.MIN_PASS_LENGTH) error = string.Format("Invalid min pass length is {0}", Constants.MIN_PASS_LENGTH);
                else if (userData.OldPass == userData.Pass) error = "Invalid old pass equal to new pass";
                else if (user.Pass != UserDataModel.HashPassword(userData.OldPass, user.Salt)) error = "Invalid old pass";

                if (string.IsNullOrEmpty(error) == false) return BadRequest(new UserDataDTOResponseModel(error));

                user.Salt = UserDataModel.GenerateSalt();
                user.Pass = UserDataModel.HashPassword(userData.Pass ?? string.Empty, user.Salt);
                user.RefreshToken = string.Empty;
                _context.UserData.Update(user);
                await _context.SaveChangesAsync();
                return Ok(new UserDataDTOResponseModel(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("PostChangePass request:\r\n{0}\r\n\r\n\r\n", userData));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("User")]
        public async Task<ActionResult> PutUser([FromHeader(Name = "Authorization")] string authorization, [FromBody] UserDataDTORequestModel userData)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var users = await _context.UserData.Where(user => (user.UserID == userData.UserID) || (user.Email == userData.Email) || (user.Phone == userData.Phone)).ToArrayAsync();

                var error = string.Empty;
                if (users.Any() == false) error = "Invalid user";
                else if (new Regex(Constants.REGEX_EMAIL).IsMatch(userData.Email) == false) error = "Invalid email format";
                else if (new Regex(Constants.REGEX_PHONE).IsMatch(userData.Phone) == false) error = "Invalid phone format";
                else if (users.Any(user => (user.UserID != userData.UserID) && (user.Email == userData.Email))) error = "Invalid email";
                else if (users.Any(user => (user.UserID != userData.UserID) && (user.Phone == userData.Phone))) error = "Invalid phone";

                if (string.IsNullOrEmpty(error) == false) return BadRequest(new UserDataDTOResponseModel(error));

                var user = users.First(user => user.UserID == userData.UserID);
                user.UserName = userData.UserName;
                user.Email = userData.Email;
                user.Phone = userData.Phone;
                user.IsLocked = userData.IsLocked ?? false;
                if (user.IsLocked) user.RefreshToken = string.Empty;
                _context.UserData.Update(user);
                await _context.SaveChangesAsync();
                return Ok(new UserDataDTOResponseModel(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("PutUser request:\r\n{0}\r\n\r\n\r\n", userData));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("User")]
        public async Task<ActionResult> DeleteUser([FromHeader(Name = "Authorization")] string authorization, string userID)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var user = await _context.UserData.FirstOrDefaultAsync(user => user.UserID == userID);

                if (user == null) return BadRequest();

                _context.UserData.Remove(user);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("DeleteUser request:\r\nuserID: {0}\r\n\r\n\r\n", userID));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion

        #region List
        [HttpGet("List")]
        public async Task<ActionResult<ListDataModel[]>> GetList()
        {
            try
            {
                return Ok(await _context.ListData.ToArrayAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetList\r\n\r\n\r\n");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("List")]
        public async Task<ActionResult<ListDataModel>> PostList([FromHeader(Name = "Authorization")] string authorization, [FromBody] ListDataDTORequestModel listData)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var list = new ListDataModel(listData);
                await _context.ListData.AddAsync(list);
                await _context.SaveChangesAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("PostList request:\r\n{0}\r\n\r\n\r\n", listData));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("List")]
        public async Task<ActionResult> PutList([FromHeader(Name = "Authorization")] string authorization, [FromBody] ListDataModel listData)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var list = await _context.ListData.FirstOrDefaultAsync(list => list.ListID == listData.ListID);

                if (list == null) return BadRequest();

                var newItemLength = listData.Items.Split(Constants.ITEM_SEPARATOR).Length;
                if (list.Items.Split(Constants.ITEM_SEPARATOR).Length > newItemLength)
                {
                    var items = await _context.ItemData.Where(item => (item.ListID == list.ListID) && (item.ListItem >= newItemLength)).ToArrayAsync();
                    foreach (var item in items)
                    {
                        item.ListItem = 0;
                    }
                    _context.ItemData.UpdateRange(items);
                }
                list.ListName = listData.ListName;
                list.Items = listData.Items;
                _context.ListData.Update(list);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("PutList request:\r\n{0}\r\n\r\n\r\n", listData));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("List")]
        public async Task<ActionResult> DeleteList([FromHeader(Name = "Authorization")] string authorization, string listID)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var list = await _context.ListData.FirstOrDefaultAsync(list => (list.ListID == listID));

                if (list == null) return BadRequest();

                _context.ListData.Remove(list);
                foreach (var item in await _context.ItemData.Where(item => item.ListID == list.ListID).ToArrayAsync())
                {
                    item.ListID = null;
                    item.ListItem = null;
                }
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("DeleteList request:\r\nlistID: {0}\r\n\r\n\r\n", listID));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion

        #region Category
        [HttpGet("Category")]
        public async Task<ActionResult<CategoryDataModel[]>> GetCategory()
        {
            try
            {
                return Ok(await _context.CategoryData.ToArrayAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCategory\r\n\r\n\r\n");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("Category")]
        public async Task<ActionResult<CategoryDataModel>> PostCategory([FromHeader(Name = "Authorization")] string authorization, [FromBody] CategoryDataDTORequestModel categoryData)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var category = new CategoryDataModel(categoryData);
                await _context.CategoryData.AddAsync(category);
                await _context.SaveChangesAsync();
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("PostCategory request:\r\n{0}\r\n\r\n\r\n", categoryData));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPatch("Category")]
        public async Task<ActionResult> PatchCategory([FromHeader(Name = "Authorization")] string authorization, [FromBody] CategoryDataModel categoryData)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var category = await _context.CategoryData
                    .Where(category => category.CategoryID == categoryData.CategoryID)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(category => category.CategoryName, categoryData.CategoryName));

                return (category > 0)
                    ? Ok()
                    : BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("PatchCategory request:\r\n{0}\r\n\r\n\r\n", categoryData));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("Category")]
        public async Task<ActionResult> DeleteCategory([FromHeader(Name = "Authorization")] string authorization, string categoryID)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var category = await _context.CategoryData.FirstOrDefaultAsync(category => (category.CategoryID == categoryID));

                if (category == null) return BadRequest();

                _context.ItemData.RemoveRange(await _context.ItemData.Where(item => item.CategoryID == category.CategoryID).ToArrayAsync());
                await _context.SaveChangesAsync();

                _context.CategoryData.Remove(category);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("DeleteCategory request:\n\ncategoryID: {0}\r\n\r\n\r\n", categoryID));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion

        #region Item
        [HttpGet("Item")]
        public async Task<ActionResult<ItemDataDTOResponseModel[]>> GetItem(string categoryID = "", string itemName = "", DateTime? endTime = null, string listItem = "", string customerName = "")
        {
            try
            {
                var items = await _context.ItemData.Where(item => (string.IsNullOrEmpty(categoryID) || (item.CategoryID == categoryID))
                        && (string.IsNullOrEmpty(itemName) || item.ItemName.Contains(itemName))
                        && ((endTime.HasValue == false) || (item.EndTime <= endTime.Value)))
                    .ToArrayAsync();

                
                if (string.IsNullOrEmpty(listItem) == false)
                {
                    var lists = await _context.ListData.Where(list => list.Items.Contains(listItem)).ToArrayAsync();
                    var listsSelect = lists.Select(list => new { list.ListID, Index = list.Items.Split(Constants.ITEM_SEPARATOR).ToList().IndexOf(listItem) });
                    items = items.Where(item => listsSelect.Any(list => (list.ListID == item.ListID) && item.ListItem.HasValue && (list.Index == item.ListItem.Value))).ToArray();
                }

                if (string.IsNullOrEmpty(customerName) == false)
                {
                    var customerIDs = items.Select(item => item.CustomerID).Distinct();
                    var customers = await _context.CustomerData.Where(customer => customerIDs.Contains(customer.CustomerID)).ToArrayAsync();
                    var newItems = new List<ItemDataModel>();
                    foreach (var item in items)
                    {
                        var customer = customers.FirstOrDefault(customer => customer.CustomerID == item.CustomerID);
                        if ((customer != null) && customer.CustomerName.Contains(customerName))
                        {
                            newItems.Add(item);
                        }
                    }
                    items = newItems.ToArray();
                }

                return Ok(items.Select(item => new ItemDataDTOResponseModel(item)).ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("GetItem request:\r\ncategoryID: {0}\r\nitemName: {1}\r\nendTime: {2}\r\nlistItem: {3}\r\ncustomerName: {4}\r\n\r\n\r\n", categoryID, itemName, endTime, listItem, customerName));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("Item")]
        public async Task<ActionResult<ItemDataDTOResponseModel>> PostItem([FromHeader(Name = "Authorization")] string authorization, [FromBody] ItemDataDTORequestModel itemData, string itemID = "")
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                if (await _context.CategoryData.FirstOrDefaultAsync(category => category.CategoryID == itemData.CategoryID) == null)
                    return BadRequest();

                var items = await _context.ItemData.Where(item => item.ItemID == itemID).ToArrayAsync();
                var newItem = new ItemDataModel(itemData, items.Any() ? items.Max(item => item.ItemNo) + 1 : 0, itemID);
                await _context.ItemData.AddAsync(newItem);
                await _context.SaveChangesAsync();
                return Ok(new ItemDataDTOResponseModel(newItem));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("PostItem request:\r\n{0}\r\nitemID: {1}\r\n\r\n\r\n", itemData, itemID));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("Item")]
        public async Task<ActionResult<ItemDataDTOResponseModel>> PutItem([FromHeader(Name = "Authorization")] string authorization, [FromBody] ItemDataDTORequestModel itemData, string itemID, int? itemNo = null)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var items = await _context.ItemData.Where(item => (item.ItemID == itemID)).ToArrayAsync();

                if ((items.Any() == false)
                    || ((itemNo.HasValue) && (items.FirstOrDefault(item => item.ItemNo == itemNo.Value) == null))
                    || ((string.IsNullOrEmpty(itemData.ListID) == false)
                        && (await _context.ListData.FirstOrDefaultAsync(list => list.ListID == itemData.ListID) == null)))
                    return BadRequest();

                
                var imageUrl = itemData.ImageUrl == null ? null : Convert.FromBase64String(itemData.ImageUrl);
                var digitalFile = itemData.DigitalFile == null ? null : Convert.FromBase64String(itemData.DigitalFile);
                if (itemNo.HasValue)
                {
                    var item = items.First(item => item.ItemNo == itemNo.Value);
                    item.ListID = itemData.ListID;
                    item.ListItem = itemData.ListItem;
                    item.DigitalFlg = itemData.DigitalFlg;
                    item.DigitalFile = digitalFile;
                    item.Location = itemData.Location;
                    item.Quantity = itemData.Quantity;
                    item.Memo = itemData.Memo;
                    item.LastChange = itemData.LastChange;
                    _context.ItemData.Update(item);
                }
                else
                {
                    foreach (var itemHeader in items)
                    {
                        itemHeader.ItemName = itemData.ItemName;
                        itemHeader.ImageUrl = imageUrl;
                    }
                    _context.ItemData.UpdateRange(items);
                }
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("PutItem request:\r\n{0}\r\nitemID: {1}\r\nitemNo: {2}\r\n\r\n\r\n", itemData, itemID, itemNo));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("Notification")]
        public async Task<ActionResult> PutNotification([FromHeader(Name = "Authorization")] string authorization, [FromBody] NotificationDataDTORequestModel itemData)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var item = await _context.ItemData.FirstOrDefaultAsync(item => (item.ItemID == itemData.ItemID) && (item.ItemNo == itemData.ItemNo));
                var customer = await _context.CustomerData.FirstOrDefaultAsync(customer => customer.CustomerID == itemData.CustomerID);

                if ((item == null)
                    || (itemData.StartTime.HasValue == false)
                    || (itemData.EndTime.HasValue == false)
                    || (customer == null)
                    || (itemData.NotificationTime.HasValue == false)
                    || string.IsNullOrEmpty(itemData.NotificationSubject)
                    || string.IsNullOrEmpty(itemData.NotificationBody)) return BadRequest();

                item.StartTime = itemData.StartTime;
                item.EndTime = itemData.EndTime;
                item.CustomerID = itemData.CustomerID;
                item.NotificationTime = itemData.NotificationTime;
                item.NotificationSubject = itemData.NotificationSubject;
                item.NotificationBody = itemData.NotificationBody;
                item.SendFlg = false;
                customer.Items = customer.Items.Any()
                    ? string.Format("{0}{1}{2}{3}{4}", customer.Items, Constants.ITEM_SEPARATOR, item.ItemID, Constants.ITEM_NO_SEPARATOR, item.ItemNo)
                    : string.Format("{0}{1}{2}", item.ItemID, Constants.ITEM_NO_SEPARATOR, item.ItemNo);
                _context.CustomerData.Update(customer);
                _context.ItemData.Update(item);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("PutNotification request:\r\n{0}\r\n\r\n\r\n", itemData));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("Item")]
        public async Task<ActionResult> DeleteItem([FromHeader(Name = "Authorization")] string authorization, string itemID, int? itemNo = null)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var items = await _context.ItemData.Where(item => (item.ItemID == itemID) && ((itemNo.HasValue == false) || (item.ItemNo == itemNo))).ToListAsync();

                if (items.Any() == false) return BadRequest();

                var itemsNotification = items.Select(item => string.Format("{0}{1}{2}", item.ItemID, Constants.ITEM_NO_SEPARATOR, item.ItemNo));
                var customers = await _context.CustomerData.ToArrayAsync();
                foreach (var customer in customers)
                {
                    var item = itemsNotification.FirstOrDefault(item => customer.Items.Contains(item));
                    if (item != null)
                    {
                        customer.Items = customer.Items
                            .Replace(string.Format("{0}{1}", Constants.ITEM_SEPARATOR, item), string.Empty)
                            .Replace(item, string.Empty);
                    }
                }
                _context.CustomerData.UpdateRange(customers);
                _context.ItemData.RemoveRange(items);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("DeleteItem request:\r\nitemID: {0}\r\nitemNo: {1}\r\n\r\n\r\n", itemID, itemNo));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("Notification")]
        public async Task<ActionResult> DeleteNotification([FromHeader(Name = "Authorization")] string authorization, string itemID, int itemNo)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var item = await _context.ItemData.FirstOrDefaultAsync(item => (item.ItemID == itemID) && (item.ItemNo == itemNo));

                if (item == null) return BadRequest();

                var customer = await _context.CustomerData.FirstOrDefaultAsync(customer => customer.CustomerID == item.CustomerID);
                if (customer != null)
                {
                    customer.Items = customer.Items
                        .Replace(string.Format("{0}{1}{2}{3}", Constants.ITEM_SEPARATOR, item.ItemID, Constants.ITEM_NO_SEPARATOR, item.ItemNo), string.Empty)
                        .Replace(string.Format("{0}{1}{2}", item.ItemID, Constants.ITEM_NO_SEPARATOR, item.ItemNo), string.Empty);
                    _context.CustomerData.Update(customer);
                }
                item.StartTime = null;
                item.EndTime = null;
                item.CustomerID = null;
                item.NotificationTime = null;
                item.NotificationSubject = string.Empty;
                item.NotificationBody = string.Empty;
                item.SendFlg = true;
                _context.ItemData.Update(item);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("DeleteNotification request:\r\nitemID: {0}\r\nitemNo: {1}\r\n\r\n\r\n", itemID, itemNo));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion

        #region Customer
        [HttpGet("Customer")]
        public async Task<ActionResult<CustomerDataModel[]>> GetCustomer([FromHeader(Name = "Authorization")] string authorization, string keyword = "")
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var customers = await _context.CustomerData.Where(customer => string.IsNullOrEmpty(keyword) || customer.CustomerName.Contains(keyword) || customer.Email.Contains(keyword) || customer.Phone.Contains(keyword)).ToArrayAsync();

                return Ok(customers.Select(customer => new CustomerDataDTOResponseModel(customer)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("GetCustomer request:\r\nkeyword: {0}\r\n\r\n\r\n", keyword));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("Customer")]
        public async Task<ActionResult<CustomerDataModel>> PostCustomer([FromHeader(Name = "Authorization")] string authorization, [FromBody] CustomerDataDTORequestModel customerData)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var customers = await _context.CustomerData.Where(customer => (customer.Email == customerData.Email) || (customer.Phone == customerData.Phone)).ToArrayAsync();

                var error = string.Empty;
                if (customers.Any(customer => customer.Email == customerData.Email)) error = "Invalid email";
                else if (customers.Any(customer => customer.Phone == customerData.Phone)) error = "Invalid phone";
                else if (new Regex(Constants.REGEX_EMAIL).IsMatch(customerData.Email) == false) error = "Invalid email format";
                else if (new Regex(Constants.REGEX_PHONE).IsMatch(customerData.Phone) == false) error = "Invalid phone format";

                if (string.IsNullOrEmpty(error) == false) return BadRequest(new CustomerDataDTOResponseModel(error));

                var customer = new CustomerDataModel(customerData);
                await _context.CustomerData.AddAsync(customer);
                await _context.SaveChangesAsync();
                return Ok(new CustomerDataDTOResponseModel(customer));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("PostCustomer request:\r\n{0}\r\n\r\n\r\n", customerData));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("Customer")]
        public async Task<ActionResult> PutCustomer([FromHeader(Name = "Authorization")] string authorization, [FromBody] CustomerDataDTORequestModel customerData)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var customers = await _context.CustomerData.Where(customer => (customer.CustomerID == customerData.CustomerID) || (customer.Email == customerData.Email) || (customer.Phone == customerData.Phone)).ToArrayAsync();

                var error = string.Empty;
                if (customers.Any() == false) error = "Invalid customer";
                else if (customers.Any(customer => (customer.CustomerID != customerData.CustomerID) && (customer.Email == customerData.Email))) error = "Invalid email";
                else if (customers.Any(customer => (customer.CustomerID != customerData.CustomerID) && (customer.Phone == customerData.Phone))) error = "Invalid phone";
                else if (new Regex(Constants.REGEX_EMAIL).IsMatch(customerData.Email) == false) error = "Invalid email format";
                else if (new Regex(Constants.REGEX_PHONE).IsMatch(customerData.Phone) == false) error = "Invalid phone format";

                if (string.IsNullOrEmpty(error) == false) return BadRequest(new CustomerDataDTOResponseModel(error));

                var customer = customers.First(customer => customer.CustomerID == customerData.CustomerID);
                customer.CustomerName = customerData.CustomerName;
                customer.Email = customerData.Email;
                customer.Phone = customerData.Phone;
                customer.Address = customerData.Address;
                if (customer.Items.Any())
                {
                    var items = await _context.ItemData.Where(item => customer.Items.Contains(item.ItemID + Constants.ITEM_NO_SEPARATOR + item.ItemNo)).ToArrayAsync();
                    customer.Items = string.Join(Constants.ITEM_SEPARATOR, items.Select(item => string.Format("{0}{1}{2}", item.ItemID, Constants.ITEM_NO_SEPARATOR, item.ItemNo)));
                }
                customer.MembershipValidUntil = customerData.MembershipValidUntil.HasValue ? customerData.MembershipValidUntil.Value.AddDays(Constants.MEMBERSHIP_VALID_DAYS) : customer.MembershipValidUntil;
                customer.LateFee = customerData.LateFee;
                _context.CustomerData.Update(customer);
                await _context.SaveChangesAsync();
                return Ok(new CustomerDataDTOResponseModel(customer));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("PutCustomer request:\r\n{0}\r\n\r\n\r\n", customerData));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("Customer")]
        public async Task<ActionResult> DeleteCustomer([FromHeader(Name = "Authorization")] string authorization, string customerID)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var customer = await _context.CustomerData.FirstOrDefaultAsync(customer => customer.CustomerID == customerID);

                if (customer == null) return BadRequest();

                var items = await _context.ItemData.Where(item => item.CustomerID == customer.CustomerID).ToArrayAsync();
                foreach (var item in items)
                {
                    item.StartTime = null;
                    item.EndTime = null;
                    item.CustomerID = null;
                    item.NotificationTime = null;
                    item.NotificationSubject = string.Empty;
                    item.NotificationBody = string.Empty;
                    item.SendFlg = false;
                }
                _context.ItemData.UpdateRange(items);
                _context.CustomerData.Remove(customer);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("DeleteCustomer request:\r\ncustomerID: {0}\r\n\r\n\r\n", customerID));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion

        #region Mail
        [HttpGet("Mail")]
        public async Task<ActionResult<MailDataModel[]>> GetMail([FromHeader(Name = "Authorization")] string authorization, string title = "")
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                return Ok(await _context.MailData.Where(mail => string.IsNullOrEmpty(title) || mail.MailTitle.Contains(title)).ToArrayAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("GetMail request:\r\ntitle: {0}\r\n\r\n\r\n", title));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("Mail")]
        public async Task<ActionResult<MailDataModel>> PostMail([FromHeader(Name = "Authorization")] string authorization, [FromBody] MailDataDTORequestModel mailData)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var mail = new MailDataModel(mailData);
                await _context.MailData.AddAsync(mail);
                await _context.SaveChangesAsync();
                return Ok(mail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("PostMail request:\r\n{0}\r\n\r\n\r\n", mailData));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("Mail")]
        public async Task<ActionResult> PutMail([FromHeader(Name = "Authorization")] string authorization, [FromBody] MailDataModel mailData)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var mail = await _context.MailData.FirstOrDefaultAsync(mail => mail.MailID == mailData.MailID);

                if (mail == null) return BadRequest();

                mail.MailTitle = mailData.MailTitle;
                mail.MailBody = mailData.MailBody;
                _context.MailData.Update(mail);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("PutMail request:\r\n{0}\r\n\r\n\r\n", mailData));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("Mail")]
        public async Task<ActionResult> DeleteMail([FromHeader(Name = "Authorization")] string authorization, string mailID)
        {
            try
            {
                var token = authorization?.Split(" ").Last();

                if (string.IsNullOrEmpty(token) || _jwtService.ValidateJwtToken(token) == false) return Unauthorized();

                var mail = await _context.MailData.FirstOrDefaultAsync(mail => mail.MailID == mailID);

                if (mail == null) return BadRequest();

                _context.MailData.Remove(mail);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("DeleteMail request:\r\nmailID: {0}\r\n\r\n\r\n", mailID));
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion
    }
}
