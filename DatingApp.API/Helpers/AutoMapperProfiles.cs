using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.Business;
using DatingApp.Business.Dtos;
using DatingApp.Models;
using DatingApp.Shared.FunctionalExtensions;

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
                .ForMember(dest => dest.IsLiked, opt => opt.Ignore())
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
            this.CreateMap<Message, MessageToReturnDto>()
                .ForMember(m => m.SenderPhotoUrl, opt => opt.MapFrom(m => m.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(m => m.RecipientPhotoUrl, opt => opt.MapFrom(m => m.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));
        }

        /// <summary>
        /// Updates is liked.
        /// </summary>
        /// <param name="source">The source paged list.</param>
        /// <param name="dest">The destination list.</param>
        /// <param name="userId">The user id.</param>
        public static void UpdateIsLiked(PagedList<User> source, IEnumerable<UserForListDto> dest, int userId) =>
            dest.ForEach(userForList => userForList.IsLiked = source.First(u => u.Id == userForList.Id).Likers.Any(liker => liker.LikerId == userId));
    }
}