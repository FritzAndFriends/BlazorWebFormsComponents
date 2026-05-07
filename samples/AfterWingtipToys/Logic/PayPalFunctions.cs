using System.Collections.Specialized;

namespace WingtipToys.Logic;

public class NVPAPICaller
{
    public bool ShortcutExpressCheckout(string amt, ref string token, ref string retMsg)
    {
        token = Guid.NewGuid().ToString("N");
        retMsg = "/Checkout/CheckoutReview";
        return true;
    }

    public bool GetCheckoutDetails(string token, ref string payerId, ref NVPCodec decoder, ref string retMsg)
    {
        payerId = "sample-payer";
        decoder = new NVPCodec();
        retMsg = string.Empty;
        return true;
    }

    public bool DoCheckoutPayment(string finalPaymentAmount, string token, string payerId, ref NVPCodec decoder, ref string retMsg)
    {
        decoder ??= new NVPCodec();
        retMsg = string.Empty;
        return true;
    }
}

public sealed class NVPCodec : NameValueCollection
{
}