using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace chatt.Models
{
    public class mychat
    {
        public class ChatHub : Hub
        {
            /// <summary>
            /// Ключ - Group.Id; Значение - GroupName SignalR;
            /// </summary>
            private readonly ConcurrentDictionary<int, string> chats = new();
            private readonly ConcurrentDictionary<int, string> users = new();

            private readonly IUserIdProvider _userIdProvider;
            static List<MessageViewModel> messages = new();


            private readonly ILogger _logger;
            private readonly ApplicationContext _context;
            public ChatHub(ApplicationContext db, ILogger<ChatHub> logger)
            {
                _context = db;
                _logger = logger;

                foreach (var group in _context.Groups)
                {
                    chats.TryAdd(group.Id, Guid.NewGuid().ToString());
                }
            }

            public async Task<Groups> CreateChat(string chatName)
            {
                var user = await getUserInfo();
                if (user == null) return null!;
                Groups group = new Groups()
                {
                    Name = chatName,
                    UserId = user.id
                };
                _context.Groups.Add(group);
                _context.SaveChanges();
                chats.TryAdd(group.Id, Guid.NewGuid().ToString());

                await AddUserToChat(user.id, group.Id);

                return group;
            }

            public async Task AddUserToChat(int userId, int groupId)
            {
                var user = getUserInfo();
                if (user == null) return;
                //Принадлежит ли текущий пользователь группе
                if (_context.GroupUsers.Where(x => x.GroupId == groupId && x.UserId == user.Id).Any())
                {
                    //Если добавляемый пользователь не находится в группе
                    if (!_context.GroupUsers.Where(x => x.GroupId == groupId && x.UserId == userId).Any())
                    {
                        GroupUsers groupUser = new GroupUsers()
                        {
                            GroupId = groupId,
                            UserId = userId
                        };
                        _context.GroupUsers.Add(groupUser);
                        _context.SaveChanges();
                        await Groups.AddToGroupAsync(users[userId], chats[groupId]);
                    }
                }
            }

            public async Task<List<Groups>> GetChats()
            {
                var user = await getUserInfo();
                if (user != null)
                {
                    var x = from g in _context.Groups
                            join gu in _context.GroupUsers on g.Id equals gu.GroupId
                            where gu.UserId == user.id
                            select g;


                    return x.ToList();
                }
                return null!;

            }

            public async Task Send(string message, int groupId)
            {
                var user = await getUserInfo();
                if (user != null)
                {
                    if (_context.GroupUsers.Where(x => x.GroupId == groupId && x.UserId == user.id).Any())
                    {
                        Messages messageObj = new Messages()
                        {
                            UserId = user.id,
                            DateStamp = DateTime.UtcNow,
                            Text = message,
                            GroupId = groupId
                        };
                        _context.Messages.Add(messageObj);
                        _context.SaveChanges();
                        MessageViewModel M = new MessageViewModel()
                        {
                            userId = messageObj.UserId,
                            text = messageObj.Text,
                            dateStamp = messageObj.DateStamp,
                            userName = user.name,
                            id = messageObj.Id,
                        };
                        if (chats.ContainsKey(groupId))
                            await Clients.Group(chats[groupId]).SendAsync("Receive", M);

                    }
                }
            }
            public void SetUserName(string userName)
            {
                string IP = Context.GetHttpContext()?.Connection.RemoteIpAddress?.ToString();
                Users user = _context.Users.Where(x => x.IP == IP).FirstOrDefault();
                user.Name = userName;
                _context.SaveChanges();
            }
            public async Task<List<MessageViewModel>> getHistory()
            {

                return _context.Messages.Include(x => x.User).OrderBy(x => x.DateStamp).Select(x => new MessageViewModel()
                {
                    dateStamp = x.DateStamp.ToLocalTime(),
                    id = x.Id,
                    text = x.Text,
                    userId = x.UserId,
                    userName = x.User.Name
                }).ToList();

            }
            public class MessageViewModel
            {
                public int id { get; set; }
                public int userId { get; set; }
                public DateTime dateStamp { get; set; }
                public string text { get; set; }

                public string userName { get; set; }
            }
            public override Task OnConnectedAsync()
            {
                //получаем IP
                string IP = Context.GetHttpContext()?.Connection.RemoteIpAddress?.ToString();
                //Проверяем наличие пользователя в базе данных
                if (!_context.Users.Where(x => x.IP == IP).Any())
                {
                    _context.Users.Add(new Users() { IP = IP, Name = "12345" });
                    _context.SaveChanges();
                }

                CacheUser(getUserInfo().Result.id);
                return base.OnConnectedAsync();
            }
            public async Task removeMessage(int ID)
            {
                string IP = Context.GetHttpContext()?.Connection.RemoteIpAddress?.ToString();
                var user = _context.Users.Where(x => x.IP == IP).FirstOrDefault();
                var message = _context.Messages.Where(x => x.Id == ID).FirstOrDefault();
                if (user != null && message != null && message.UserId == user.Id)
                {
                    _context.Messages.Remove(message);
                    _context.SaveChanges();
                    await this.Clients.All.SendAsync("MessageDeleted", ID);
                }


            }
            /// <summary>
            /// получение текущего объекта пользователя 
            /// </summary>
            /// <returns></returns>
            public async Task<UserInfo> getUserInfo()
            {
                return new UserInfo(getUser(getCurrentIP()));
            }

            /// <summary>
            /// получение текущего ip пользователя\
            /// </summary>
            /// <returns></returns>
            private string getCurrentIP()
            {
                return Context.GetHttpContext()?.Connection.RemoteIpAddress?.ToString();
            }

            /// <summary>
            /// получение юзера по ip
            /// </summary>
            /// <param name="IP"></param>
            /// <returns></returns>
            private Users? getUser(string IP)
            {
                return _context.Users.Where(x => x.IP == IP).FirstOrDefault();
            }

            /// <summary>
            /// Записываем пользователя во внутренние словари.
            /// </summary>
            /// <param name="userId"></param>
            void CacheUser(int userId)
            {
                if (!users.ContainsKey(userId))
                    users.TryAdd(userId, Context.ConnectionId);
                else users[userId] = Context.ConnectionId;
            }
        }

    }
}

