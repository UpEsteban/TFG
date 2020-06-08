using AutoMapper;
using TFG.Domain.Shared.Models;
using TFG.ServiceContracts.Models;

namespace TFG.Domain.Helpers.Mappers
{
    public class DTOMessagesMappers : Profile
    {
        public DTOMessagesMappers()
        {
            CreateMap<MessageDTO, Message>();
            CreateMap<Message, MessageDTO>();
        }
    }
}
