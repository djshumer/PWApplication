namespace PWApplication.MobileShared.CustomControls
{
    public interface ICustomCertificate
    {
        string Host { get; }
        byte[] Hash { get; }
        string HashString { get; }
        byte[] PublicKey { get; }
        string PublicKeyString { get; }
    }
}
