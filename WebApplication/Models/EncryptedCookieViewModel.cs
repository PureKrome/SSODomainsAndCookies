namespace WebApplication.Models
{
    public class EncryptedCookieViewModel
    {
        public EncryptedCookieViewModel(string name,string raw, string decrypted)
        {
            Name = name;
            Raw = raw;
            Decrypted = decrypted;
        }

        public string Name { get; private set; }
        public string Raw { get; private set; }
        public string Decrypted { get; private set; }
    }
}