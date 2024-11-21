using CheckIN.Services.VCard.Models;
using QRCoder;
using System.Text;

namespace CheckIN.Services.VCard
{
    public class VCardService : IVCardService
    {
        const string Separator = ";";
        const string Header = "BEGIN:VCARD\r\nVERSION:2.1";
        const string Name = "N:";
        const string FormattedName = "FN:";
        const string OrganizationName = "ORG:";
        const string TitlePrefix = "TITLE:";
        //const string PhotoPrefix = "PHOTO;ENCODING=BASE64;JPEG:";
        //const string PhonePrefix = "TEL;TYPE=";        
        const string EmailPrefix = "EMAIL:";
        const string Footer = "END:VCARD";

        private string TicketType = "TicketType:";

        public byte[] CreateVCard(Contact contact)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Header);

            //Full Name
            if (!string.IsNullOrEmpty(contact.FirstName) || !string.IsNullOrEmpty(contact.LastName))
            {
                sb.AppendLine(Name + $"{contact.LastName};{contact.FirstName}");
                sb.AppendLine(FormattedName + $"{contact.FirstName} {contact.LastName}");
            }

            //Organization name
            if (!string.IsNullOrEmpty(contact.Organization))
            {
                sb.Append(OrganizationName + contact.Organization);
            }

            //Title
            if (!string.IsNullOrEmpty(contact.Title))
            {
                sb.AppendLine(TitlePrefix + contact.Title);
            }

            //Email
            if (!string.IsNullOrEmpty(contact.Email))
            {
                sb.AppendLine(EmailPrefix + contact.Email);
            }

            sb.AppendLine(TicketType + contact.TicketType);

            sb.AppendLine(Footer);

            return GenerateQRCode(sb.ToString());

        }

        public byte[] GenerateQRCode(string vcardContent)
        {
            byte[] qrCodeImage = null;
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(vcardContent, QRCodeGenerator.ECCLevel.Q);

                using (BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData))
                {
                    qrCodeImage = qrCode.GetGraphic(20);
                }
            }

            return qrCodeImage;
        }
    }
}
