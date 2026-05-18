// Auto-generated API-compatible stub. Original referenced legacy Web Forms APIs.
// TODO(bwfc-general): Rebuild method bodies using ASP.NET Core equivalents.

public partial class NVPAPICaller
{
    public void SetCredentials(string Userid, string Pwd, string Signature) { }
    public bool ShortcutExpressCheckout(string amt, ref string token, ref string retMsg) => throw new NotImplementedException();
    public bool GetCheckoutDetails(string token, ref string PayerID, ref NVPCodec decoder, ref string retMsg) => throw new NotImplementedException();
    public bool DoCheckoutPayment(string finalPaymentAmount, string token, string PayerID, ref NVPCodec decoder, ref string retMsg) => throw new NotImplementedException();
    public string HttpCall(string NvpRequest) => throw new NotImplementedException();
    public static bool IsEmpty(string s) => throw new NotImplementedException();
}

public partial class NVPCodec
{
    public string Encode() => throw new NotImplementedException();
    public void Decode(string nvpstring) { }
    public void Add(string name, string value, int index) { }
    public void Remove(string arrayName, int index) { }
}
