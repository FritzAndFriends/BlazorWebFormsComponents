using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using WingtipToys.Models;

public class NVPAPICaller
{
  private const bool bSandbox = true;
  private const string CVV2 = "CVV2";
  private string pEndPointURL = "https://api-3t.paypal.com/nvp";
  private string host = "www.paypal.com";
  private string pEndPointURL_SB = "https://api-3t.sandbox.paypal.com/nvp";
  private string host_SB = "www.sandbox.paypal.com";
  private const string SIGNATURE = "SIGNATURE";
  private const string PWD = "PWD";
  private const string ACCT = "ACCT";
  public string APIUsername = "<Your API Username>";
  private string APIPassword = "<Your API Password>";
  private string APISignature = "<Your Signature>";
  private string Subject = "";
  private string BNCode = "PP-ECWizard";
  private const int Timeout = 15000;
  private static readonly string[] SECURED_NVPS = new string[] { ACCT, CVV2, SIGNATURE, PWD };

  public void SetCredentials(string Userid, string Pwd, string Signature)
  {
    APIUsername = Userid;
    APIPassword = Pwd;
    APISignature = Signature;
  }

  public bool ShortcutExpressCheckout(string amt, ref string token, ref string retMsg)
  {
    if (bSandbox)
    {
      pEndPointURL = pEndPointURL_SB;
      host = host_SB;
    }

    string returnURL = "https://localhost:44300/Checkout/CheckoutReview.aspx";
    string cancelURL = "https://localhost:44300/Checkout/CheckoutCancel.aspx";

    NVPCodec encoder = new NVPCodec();
    encoder["METHOD"] = "SetExpressCheckout";
    encoder["RETURNURL"] = returnURL;
    encoder["CANCELURL"] = cancelURL;
    encoder["BRANDNAME"] = "Wingtip Toys Sample Application";
    encoder["PAYMENTREQUEST_0_AMT"] = amt;
    encoder["PAYMENTREQUEST_0_ITEMAMT"] = amt;
    encoder["PAYMENTREQUEST_0_PAYMENTACTION"] = "Sale";
    encoder["PAYMENTREQUEST_0_CURRENCYCODE"] = "USD";

    using (WingtipToys.Logic.ShoppingCartActions myCartOrders = new WingtipToys.Logic.ShoppingCartActions())
    {
      List<CartItem> myOrderList = myCartOrders.GetCartItems();

      for (int i = 0; i < myOrderList.Count; i++)
      {
        encoder["L_PAYMENTREQUEST_0_NAME" + i] = myOrderList[i].Product.ProductName.ToString();
        encoder["L_PAYMENTREQUEST_0_AMT" + i] = myOrderList[i].Product.UnitPrice.ToString();
        encoder["L_PAYMENTREQUEST_0_QTY" + i] = myOrderList[i].Quantity.ToString();
      }
    }

    string pStrrequestforNvp = encoder.Encode();
    string pStresponsenvp = HttpCall(pStrrequestforNvp);

    NVPCodec decoder = new NVPCodec();
    decoder.Decode(pStresponsenvp);

    string strAck = decoder["ACK"].ToLower();
    if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
    {
      token = decoder["TOKEN"];
      string ECURL = "https://" + host + "/cgi-bin/webscr?cmd=_express-checkout" + "&token=" + token;
      retMsg = ECURL;
      return true;
    }
    else
    {
      retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
          "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
          "Desc2=" + decoder["L_LONGMESSAGE0"];
      return false;
    }
  }

  public bool GetCheckoutDetails(string token, ref string PayerID, ref NVPCodec decoder, ref string retMsg)
  {
    if (bSandbox)
    {
      pEndPointURL = pEndPointURL_SB;
    }

    NVPCodec encoder = new NVPCodec();
    encoder["METHOD"] = "GetExpressCheckoutDetails";
    encoder["TOKEN"] = token;

    string pStrrequestforNvp = encoder.Encode();
    string pStresponsenvp = HttpCall(pStrrequestforNvp);

    decoder = new NVPCodec();
    decoder.Decode(pStresponsenvp);

    string strAck = decoder["ACK"].ToLower();
    if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
    {
      PayerID = decoder["PAYERID"];
      return true;
    }
    else
    {
      retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
          "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
          "Desc2=" + decoder["L_LONGMESSAGE0"];

      return false;
    }
  }

  public bool DoCheckoutPayment(string finalPaymentAmount, string token, string PayerID, ref NVPCodec decoder, ref string retMsg)
  {
    if (bSandbox)
    {
      pEndPointURL = pEndPointURL_SB;
    }

    NVPCodec encoder = new NVPCodec();
    encoder["METHOD"] = "DoExpressCheckoutPayment";
    encoder["TOKEN"] = token;
    encoder["PAYERID"] = PayerID;
    encoder["PAYMENTREQUEST_0_AMT"] = finalPaymentAmount;
    encoder["PAYMENTREQUEST_0_CURRENCYCODE"] = "USD";
    encoder["PAYMENTREQUEST_0_PAYMENTACTION"] = "Sale";

    string pStrrequestforNvp = encoder.Encode();
    string pStresponsenvp = HttpCall(pStrrequestforNvp);

    decoder = new NVPCodec();
    decoder.Decode(pStresponsenvp);

    string strAck = decoder["ACK"].ToLower();
    if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
    {
      return true;
    }
    else
    {
      retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
          "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
          "Desc2=" + decoder["L_LONGMESSAGE0"];

      return false;
    }
  }

  public string HttpCall(string NvpRequest)
  {
    string url = pEndPointURL;
    string strPost = NvpRequest + "&" + buildCredentialsNVPString();
    strPost = strPost + "&BUTTONSOURCE=" + Uri.EscapeDataString(BNCode);

    HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
    objRequest.Timeout = Timeout;
    objRequest.Method = "POST";
    objRequest.ContentLength = strPost.Length;

    try
    {
      using (StreamWriter myWriter = new StreamWriter(objRequest.GetRequestStream()))
      {
        myWriter.Write(strPost);
      }
    }
    catch (Exception e)
    {
      WingtipToys.Logic.ExceptionUtility.LogException(e, "HttpCall in PayPalFunction.cs");
    }

    HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
    string result;
    using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
    {
      result = sr.ReadToEnd();
    }

    return result;
  }

  private string buildCredentialsNVPString()
  {
    NVPCodec codec = new NVPCodec();

    if (!IsEmpty(APIUsername))
      codec["USER"] = APIUsername;

    if (!IsEmpty(APIPassword))
      codec[PWD] = APIPassword;

    if (!IsEmpty(APISignature))
      codec[SIGNATURE] = APISignature;

    if (!IsEmpty(Subject))
      codec["SUBJECT"] = Subject;

    codec["VERSION"] = "88.0";

    return codec.Encode();
  }

  public static bool IsEmpty(string s)
  {
    return s == null || s.Trim() == string.Empty;
  }
}

public sealed class NVPCodec : NameValueCollection
{
  private const string AMPERSAND = "&";
  private const string EQUALS = "=";
  private static readonly char[] AMPERSAND_CHAR_ARRAY = AMPERSAND.ToCharArray();
  private static readonly char[] EQUALS_CHAR_ARRAY = EQUALS.ToCharArray();

  public string Encode()
  {
    StringBuilder sb = new StringBuilder();
    bool firstPair = true;
    foreach (string kv in AllKeys)
    {
      string name = Uri.EscapeDataString(kv);
      string value = Uri.EscapeDataString(this[kv]);
      if (!firstPair)
      {
        sb.Append(AMPERSAND);
      }
      sb.Append(name).Append(EQUALS).Append(value);
      firstPair = false;
    }
    return sb.ToString();
  }

  public void Decode(string nvpstring)
  {
    Clear();
    foreach (string nvp in nvpstring.Split(AMPERSAND_CHAR_ARRAY))
    {
      string[] tokens = nvp.Split(EQUALS_CHAR_ARRAY);
      if (tokens.Length >= 2)
      {
        string name = Uri.UnescapeDataString(tokens[0]);
        string value = Uri.UnescapeDataString(tokens[1]);
        Add(name, value);
      }
    }
  }

  public void Add(string name, string value, int index)
  {
    this.Add(GetArrayName(index, name), value);
  }

  public void Remove(string arrayName, int index)
  {
    this.Remove(GetArrayName(index, arrayName));
  }

  public string this[string name, int index]
  {
    get
    {
      return this[GetArrayName(index, name)];
    }
    set
    {
      this[GetArrayName(index, name)] = value;
    }
  }

  private static string GetArrayName(int index, string name)
  {
    if (index < 0)
    {
      throw new ArgumentOutOfRangeException(nameof(index), "index cannot be negative : " + index);
    }

    return name + index;
  }
}
