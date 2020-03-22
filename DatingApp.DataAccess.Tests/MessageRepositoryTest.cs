using System;
using System.Collections.Generic;
using System.Linq;
using DatingApp.Models;
using FluentAssertions;
using Xunit;

namespace DatingApp.DataAccess.Tests
{
    public class MessageRepositoryTest : IDisposable
    {
        private readonly DatabaseFixture fixture;
        private readonly IMessageRepository messageRepository;

        public MessageRepositoryTest()
        {
            this.fixture = new DatabaseFixture();
            this.messageRepository = fixture.GetService<IMessageRepository>();

            new List<Message>
            {
                new Message
                {
                    SenderId = 2,
                    RecipientId = 3,
                    Content = "Hi",
                    IsRead = false,
                    MessageSent = DateTime.UtcNow,
                    SenderDeleted = false,
                    RecipientDeleted = false
                },
                new Message
                {
                    SenderId = 3,
                    RecipientId = 2,
                    Content = "Hi",
                    IsRead = false,
                    MessageSent = DateTime.UtcNow,
                    SenderDeleted = false,
                    RecipientDeleted = false
                },
                new Message
                {
                    SenderId = 2,
                    RecipientId = 3,
                    Content = "Nice to meet you :)",
                    IsRead = false,
                    MessageSent = DateTime.UtcNow,
                    SenderDeleted = false,
                    RecipientDeleted = false
                }
            }.ForEach(message => this.messageRepository.Add(message).GetAwaiter().GetResult());
        }

        public void Dispose() =>
            this.fixture.Dispose();

        [Fact]
        private async void Get_MessageDoesNotExist_Null()
        {
            var user = await messageRepository.Get(12);
            user.Value.Should().BeNull();
        }

        [Fact]
        private async void Get_UnreadMessagesFromSpecifiedUser_Message()
        {
            var messageParams = new MessageParams
            {
                UserId = 2
            };

            var user = await messageRepository.Get(messageParams);
            user.Value.Count().Should().Be(1);
        }

        [Fact]
        private async void Get_UnreadMessagesFromSpecifieduser_Messages()
        {
            var messageParams = new MessageParams
            {
                UserId = 3
            };

            var user = await messageRepository.Get(messageParams);
            user.Value.Count().Should().Be(2);
        }

        [Fact]
        private async void Get_InboxMessages_Messages()
        {
            var messageParams = new MessageParams
            {
                MessageContainer = "Inbox",
                UserId = 2,
            };

            var user = await messageRepository.Get(messageParams);
            user.Value.Count().Should().Be(1);
        }

        [Fact]
        private async void Get_OutboxMessages_Messages()
        {
            var messageParams = new MessageParams
            {
                MessageContainer = "Outbox",
                UserId = 2,
            };

            var user = await messageRepository.Get(messageParams);
            user.Value.Count().Should().Be(2);
        }

        [Fact]
        private async void Get_MessageThreadExist_Messages()
        {
            var user = await messageRepository.GetThread(2, 3);
            user.Value.Count().Should().Be(3);
        }

        [Fact]
        private async void Update_MessageExists_ContentUpdated()
        {
            var messageToCreate = new Message
            {
                SenderId = 1,
                RecipientId = 3,
                Content = "Hi",
                IsRead = false,
                MessageSent = DateTime.UtcNow,
                SenderDeleted = false,
                RecipientDeleted = false
            };
            var messageToUpdate = await this.messageRepository.Add(messageToCreate);
            messageToUpdate.Value.Content = "Bye";

            var message = await this.messageRepository.Update(messageToUpdate.Value);
            message.Value.Content.Should().Be("Bye");
        }

        [Fact]
        private async void Add_NewMessage_Message()
        {
            var messageToCreate = new Message
            {
                SenderId = 1,
                RecipientId = 2,
                Content = "Hello Mike",
                IsRead = false,
                MessageSent = DateTime.UtcNow,
                SenderDeleted = false,
                RecipientDeleted = false
            };

            var createdMessage = await messageRepository.Add(messageToCreate);
            var message = await messageRepository.Get(createdMessage.Value.Id);
            message.Value.Content.Should().Be("Hello Mike");
        }

        [Fact]
        private async void Delete_MessageExists_Successful()
        {
            var messageToCreate = new Message
            {
                SenderId = 1,
                RecipientId = 2,
                Content = "Hi",
                IsRead = false,
                MessageSent = DateTime.UtcNow,
                SenderDeleted = false,
                RecipientDeleted = false
            };
            var messageToDelete = await this.messageRepository.Add(messageToCreate);

            var message = await this.messageRepository.Delete(messageToDelete.Value);
            message.IsSuccess.Should().BeTrue();
        }
    }
}
