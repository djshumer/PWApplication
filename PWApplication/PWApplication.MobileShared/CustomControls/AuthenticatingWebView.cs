using Xamarin.Forms;

namespace PWApplication.MobileShared.CustomControls
{
    public delegate bool ShouldTrustCertificate(ICustomCertificate certificate);
    public class AuthenticatingWebView : WebView
    {
        public ShouldTrustCertificate ShouldTrustUnknownCertificate { get; set; }

    }
}
