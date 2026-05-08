using System.Collections.Specialized;

namespace WingtipToys.Logic;

public class NVPCodec : NameValueCollection
{
}

public class NVPAPICaller
{
    public bool ShortcutExpressCheckout(string amt, ref string token, ref string retMsg)
    {
        token = string.Empty;
        retMsg = "/Checkout/CheckoutReview";
        return false;
    }

    public bool GetCheckoutDetails(string token, ref string payerId, ref NVPCodec decoder, ref string retMsg)
    {
        payerId = string.Empty;
        decoder = new NVPCodec();
        retMsg = "Checkout is not configured in this benchmark run.";
        return false;
    }

    public bool DoCheckoutPayment(string finalPaymentAmount, string token, string payerId, ref string decoder, ref string retMsg)
    {
        decoder = string.Empty;
        retMsg = "Checkout is not configured in this benchmark run.";
        return false;
    }
}
