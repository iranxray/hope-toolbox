namespace Xray.Hope.Web.Shared
{
    public record HopeHubMethod(string Name)
    {
        public static implicit operator string(HopeHubMethod method) => method?.Name;

        public static HopeHubMethod ReceiveScriptExecutionOutput = new HopeHubMethod("receiveScriptExecutionOutput");
        public static HopeHubMethod FinishScriptExection = new HopeHubMethod("finishScriptExection");
    }
}
