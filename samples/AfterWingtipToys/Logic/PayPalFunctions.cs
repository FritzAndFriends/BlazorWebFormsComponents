using System.Collections.Specialized;

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
        payerId = string.Empty;
        decoder ??= new NVPCodec();
        retMsg = "PayPal checkout is not configured.";
        return false;
    }

    public bool DoCheckoutPayment(string finalPaymentAmount, string token, string payerId, ref NVPCodec decoder, ref string retMsg)
    {
        decoder ??= new NVPCodec();
        retMsg = "PayPal checkout is not configured.";
        return false;
    }

    public string HttpCall(string nvpRequest) => string.Empty;
}

public class NVPCodec : NameValueCollection
{
    public string Encode() => string.Empty;

    public void Decode(string input)
    {
    }
}
