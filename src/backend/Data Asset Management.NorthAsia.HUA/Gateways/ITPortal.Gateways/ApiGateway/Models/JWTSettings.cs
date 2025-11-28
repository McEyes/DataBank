namespace ApiGateway.Models
{
    public class JWTSettings
    {
        public bool ValidateIssuerSigningKey { get; set; }
        public string SecurityKey { get; set; }
        public bool ValidateIssuer { get; set; }
        public string ValidIssuer { get; set; }
        public bool ValidateAudience { get; set; }
        public string ValidAudience { get; set; }
        public bool ValidateLifetime { get; set; }
        public long ExpiredTime { get; set; }
        public long ClockSkew { get; set; }
        public string Algorithm { get; set; }
    }
}
