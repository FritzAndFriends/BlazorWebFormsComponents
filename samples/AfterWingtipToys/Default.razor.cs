namespace WingtipToys
{
    public partial class Default
    {
        private void Page_Error()
        {
            var exc = Server.GetLastError();
            if (exc is InvalidOperationException)
            {
                Server.Transfer("ErrorPage?handler=Page_Error%20-%20Default");
            }
        }
    }
}
