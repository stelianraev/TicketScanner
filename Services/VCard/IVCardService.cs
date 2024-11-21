using CheckIN.Services.VCard.Models;

namespace CheckIN.Services.VCard
{
    public interface IVCardService
    {
        byte[] CreateVCard(Contact contact);

        byte[] GenerateQRCode(string vcardContent);
    }
}
