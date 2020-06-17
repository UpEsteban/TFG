using AutoMapper;
using TFG.Domain.Shared.Abstractions.Domains;
using TFG.Domain.Shared.Abstractions.Repositories;
using TFG.Domain.Shared.Models;
using TFG.ServiceContracts.Models;

namespace TFG.Domain.Domains
{
    public class MessagesDomain : IMessagesDomain
    {
        private readonly IMessagesRepository repository;

        private readonly IMapper mapper;

        public MessagesDomain(IMessagesRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public Message Get(string messageId)
        {
            var res = repository.Get(messageId);

            if (res == null)
            {
                res = new MessageDTO();
            }

            return mapper.Map<MessageDTO, Message>(res);
        }
    }
}
