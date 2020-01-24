using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    /// <summary>
    /// The AutoMapper profiles class.
    /// Used for defining the auto mapper configurations.
    /// </summary>
    public class AutoMapperProfiles : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfiles"/> class.
        /// Sets up the mapping to the DTO.
        /// </summary>
        public AutoMapperProfiles()
        {
            this.CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.Age()));

            this.CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.Age()));
                
            this.CreateMap<Photo, PhotosForDetailedDto>();
            this.CreateMap<Photo, PhotoForReturnDto>();
            this.CreateMap<PhotoForCreationDto, Photo>();
            this.CreateMap<UserForUpdateDto, User>();            
            this.CreateMap<UserForRegisterDto, User>();
            this.CreateMap<MessageForCreationDto, Message>();
            this.CreateMap<Message , MessageToReturnDto>()
                .ForMember(m => m.SenderPhotoUrl, opt => opt.MapFrom(m => m.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(m => m.RecipientPhotoUrl, opt => opt.MapFrom(m => m.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));
        }
    }
}