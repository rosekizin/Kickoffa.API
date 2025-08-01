using Kickoffa.API.Domain.Interfaces.Models;

namespace Kickoffa.API.Application.Interfaces.Checkilists
{
    public interface IAddBriefingMediaService
    {
        void CreateAndAddBriefingMediaFromImageUrl(IBriefingSection section, string imageUrl);

        void CreateAndAddBriefingMediaFromImageUrls(IBriefingSection section, List<string> imageUrls);
    }
}