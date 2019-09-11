using System;
using Android.Content;
using PWApplication.Droid.Renderers;
using PWApplication.MobileShared.CustomControls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using WebView = Xamarin.Forms.WebView;

[assembly: ExportRenderer(typeof(AuthenticatingWebView), typeof(AuthenticatingWebViewRenderer))]
namespace PWApplication.Droid.Renderers
{
    
    public class AuthenticatingWebViewRenderer : WebViewRenderer
    {
        private WebNavigationEvent _lastNavigationEvent;
        private WebViewSource _lastSource;
        private string _lastUrl;

        public AuthenticatingWebViewRenderer(Context context) : base(context)
        {

        }

        private new AuthenticatingWebView Element { get { return (AuthenticatingWebView)base.Element; } }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            var proxyClient = new AuthenticatingWebViewClient(this);

            Control.SetWebViewClient(proxyClient);
            //Control.Settings.UserAgentString = "Mozilla/5.0 (X11; U; Linux i686; en-US; rv:1.9.0.4) Gecko/20100101 Firefox/4.0";

            if (e.OldElement != null)
            {
                ((WebView)e.OldElement).Navigating -= HandleElementNavigating;
            }

            if (e.NewElement != null)
            {
                ((WebView)e.NewElement).Navigating += HandleElementNavigating;
            }
        }
        internal class JavascriptCallback : Java.Lang.Object, Android.Webkit.IValueCallback
        {
            public JavascriptCallback(Action<string> callback)
            {
                _callback = callback;
            }

            private Action<string> _callback;
            public void OnReceiveValue(Java.Lang.Object value)
            {
                _callback?.Invoke(Convert.ToString(value));
            }
        }
        private async void HandleElementNavigating(object sender, WebNavigatingEventArgs e)
        {
            try
            {
                _lastNavigationEvent = e.NavigationEvent;
                _lastSource = e.Source;
                _lastUrl = e.Url;
            }
            catch (Exception ex)
            {

            }
        }

        private class AuthenticatingWebViewClient : Android.Webkit.WebViewClient
        {
            private readonly AuthenticatingWebViewRenderer _renderer;

            public AuthenticatingWebViewClient(AuthenticatingWebViewRenderer renderer)
            {
                _renderer = renderer;
            }

            public override void OnReceivedSslError(Android.Webkit.WebView view, Android.Webkit.SslErrorHandler handler, Android.Net.Http.SslError error)
            {
                if (_renderer.Element.ShouldTrustUnknownCertificate != null)
                {
                    var certificate = new CustomCertificateDroid(error.Url, error.Certificate);
                    var result = _renderer.Element.ShouldTrustUnknownCertificate(certificate);
                    if (result)
                    {
                        handler.Proceed();
                    }
                    else
                    {
                        handler.Cancel();
                    }
                }
               
                base.OnReceivedSslError(view, handler, error);
            }

            [Obsolete("deprecated")]
            public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, string url)
            {
                _renderer.Element.SendNavigating(new WebNavigatingEventArgs(WebNavigationEvent.NewPage, _renderer._lastSource, url));
                return base.ShouldOverrideUrlLoading(view, url);
            }
           
        }

    }
}
