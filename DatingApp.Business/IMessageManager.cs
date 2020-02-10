using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;

namespace DatingApp.Business
{
    public interface IMessageManager
    {
        Task<Result<Message, Error>> Get(int id);
        Task<Result<PagedList<Message>, Error>> Get(MessageParams messageParams);
        Task<Result<IEnumerable<Message>, Error>> GetThread(int senderId, int recipientId);
        Task<Result<Message, Error>> Add(int userId, Message message);
        Task<Result<None, Error>> Delete(int userId, int id);
        Task<Result<Message, Error>> MarkAsRead(int userId, int id);
    }
}