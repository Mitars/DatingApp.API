using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;

namespace DatingApp.DataAccess
{
    /// <summary>
    /// The dating repository inferface.
    /// </summary>
    public interface IMessageRepository
    {   
        Task<Result<Message, Error>> Get(int id);
            
        Task<Result<PagedList<Message>, Error>> GetMessagesForUser(MessageParams messageParams);
        
        Task<Result<IEnumerable<Message>, Error>> GetThread(int senderId, int recipientId);

        Task<Result<Message, Error>> Add(Message entity);
            
        Task<Result<Message, Error>> Update(Message message);
            
        Task<Result<None, Error>> Delete(Message message);
    }
}