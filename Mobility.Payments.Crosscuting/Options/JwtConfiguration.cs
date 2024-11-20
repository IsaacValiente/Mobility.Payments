namespace Mobility.Payments.Crosscuting.Options
{
    public class JwtConfiguration
    {
        public const string Section = "JwtConfiguration";

        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpirationInMinutes { get; set; }
    }
}
