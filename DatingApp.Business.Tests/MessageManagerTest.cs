using System;
using System.Collections.Generic;
using System.Linq;
using DatingApp.Business;
using DatingApp.Models;
using DatingApp.Shared.ErrorTypes;
using FluentAssertions;
using Xunit;

namespace DatingApp.DataAccess.Tests
{
    public class MessageManagerTest : IDisposable
    {
        private readonly DatabaseFixture fixture;
        private readonly IMessageManager messageManager;

        public MessageManagerTest()
        {
            this.fixture = new DatabaseFixture();
            messageManager = fixture.GetService<IMessageManager>();

            var messageRepository = fixture.GetService<IMessageRepository>();
            new List<Message>
            {
                new Message
                {
                    SenderId = 1,
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
                    RecipientId = 1,
                    Content = "Hi",
                    IsRead = false,
                    MessageSent = DateTime.UtcNow,
                    SenderDeleted = false,
                    RecipientDeleted = false
                },
                new Message
                {
                    SenderId = 1,
                    RecipientId = 2,
                    Content = "Nice to meet you :)",
                    IsRead = false,
                    MessageSent = DateTime.UtcNow,
                    SenderDeleted = false,
                    RecipientDeleted = false
                }
            }.ForEach(message => messageRepository.Add(message).GetAwaiter().GetResult());
        }

        public void Dispose() =>
            this.fixture.Dispose();

        [Fact]
        private async void Get_MessageDoesNotExist_Null()
        {
            var message = await messageManager.Get(12);
            message.Value.Should().BeNull();
        }

        [Fact]
        private async void Get_UnreadMessagesFromSpecifiedUser_Message()
        {
            var messageParams = new MessageParams
            {
                UserId = 1
            };

            var message = await messageManager.Get(messageParams);
            message.Value.Count().Should().Be(1);
        }

        [Fact]
        private async void Get_UnreadMessagesFromSpecifiedUser_Messages()
        {
            var messageParams = new MessageParams
            {
                UserId = 2
            };

            var message = await messageManager.Get(messageParams);
            message.Value.Count().Should().Be(2);
        }

        [Fact]
        private async void Get_InboxMessages_Messages()
        {
            var messageParams = new MessageParams
            {
                MessageContainer = "Inbox",
                UserId = 1,
            };

            var message = await messageManager.Get(messageParams);
            message.Value.Count().Should().Be(1);
        }

        [Fact]
        private async void Get_OutboxMessages_Messages()
        {
            var messageParams = new MessageParams
            {
                MessageContainer = "Outbox",
                UserId = 1,
            };

            var message = await messageManager.Get(messageParams);
            message.Value.Count().Should().Be(2);
        }

        [Fact]
        private async void Get_MessageThreadExist_Messages()
        {
            var message = await messageManager.GetThread(1, 2);
            message.Value.Count().Should().Be(3);
        }

        [Fact]
        private async void Add_NewMessage_Message()
        {
            var messageToCreate = new Message
            {
                SenderId = 1,
                RecipientId = 2,
                Content = "Hello Mike"
            };

            var createdMessage = await messageManager.Add(1, messageToCreate);
            var message = await messageManager.Get(createdMessage.Value.Id);
            message.Value.Content.Should().Be("Hello Mike");
        }

        [Fact]
        private async void Add_SendingMessageAsAnotherUser_UnauthorizedError()
        {
            var messageToCreate = new Message
            {
                SenderId = 1,
                RecipientId = 2
            };

            var message = await messageManager.Add(3, messageToCreate);
            message.Error.GetType().Should().Be(typeof(UnauthorizedError));
        }

        [Fact]
        private async void Add_SendingMessageToSelf_Error()
        {
            var messageToCreate = new Message
            {
                SenderId = 1,
                RecipientId = 1
            };

            var message = await messageManager.Add(1, messageToCreate);
            message.Error.GetType().Should().Be(typeof(Error));
        }

        [Fact]
        private async void Delete_MessageExists_Successful()
        {
            var messageToCreate = new Message
            {
                SenderId = 1,
                RecipientId = 2
            };
            var messageToDelete = await this.messageManager.Add(1, messageToCreate);

            var message = await this.messageManager.Delete(1, messageToDelete.Value.Id);
            message.IsSuccess.Should().BeTrue();
        }

        [Fact]
        private async void MarkAsRead_NewMessage_Read()
        {
            var messageToCreate = new Message
            {
                SenderId = 1,
                RecipientId = 2
            };
            var messageToMarkAsRead = await this.messageManager.Add(1, messageToCreate);
            var originalIsReadValue = messageToMarkAsRead.Value.IsRead;

            var message = await this.messageManager.MarkAsRead(1, messageToMarkAsRead.Value.Id);
            originalIsReadValue.Should().Be(false);
            message.Value.IsRead.Should().Be(true);
        }

        [Fact]
        private async void MarkAsRead_DifferentUser_UnauthorizedError()
        {
            var messageToCreate = new Message
            {
                SenderId = 1,
                RecipientId = 2
            };
            var messageToMarkAsRead = await this.messageManager.Add(1, messageToCreate);

            var message = await this.messageManager.MarkAsRead(2, messageToMarkAsRead.Value.Id);
            message.Error.GetType().Should().Be(typeof(UnauthorizedError));
        }
    }
}
