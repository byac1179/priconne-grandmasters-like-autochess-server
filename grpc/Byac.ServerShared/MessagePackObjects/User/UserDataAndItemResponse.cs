using MessagePack;

namespace Byac.ServerShared.MessagePackObjects.User
{
    [MessagePackObject]
    public class UserDataAndItemResponse
    {
        [Key(0)]
        public UserResponse UserData { get; set; }

        [Key(1)]
        public UserItemResponse Items { get; set; }
    }
}
