using Xray.Hope.Web.Client.Models.Domain;
using static Xray.Hope.Web.Client.Services.State.HubConnectionSucceeded;
using static Xray.Hope.Web.Client.Services.State.XuiInstallOutputReceived;

namespace Xray.Hope.Web.Client.Services.State
{
    public record StateEvent { }

    public record StateEvent<T>(T Value) : StateEvent { }

    public record XuiConnectionSucceeded : StateEvent<XuiConnectionOptions>
    {
        public XuiConnectionSucceeded(XuiConnectionOptions value) : base(value)
        {
        }
    }

    public record XuiConnectionFailed : StateEvent<XuiConnectionOptions>
    {
        public XuiConnectionFailed(XuiConnectionOptions value) : base(value)
        {
        }
    }

    public record ServerConnectionSucceeded : StateEvent<SshConnectionOptions>
    {
        public ServerConnectionSucceeded(SshConnectionOptions value) : base(value)
        {
        }
    }

    public record ServerConnectionFailed : StateEvent<SshConnectionOptions>
    {
        public ServerConnectionFailed(SshConnectionOptions value) : base(value)
        {
        }
    }

    public record ServerConnectionBeingChecked : StateEvent { }

    public record XuiConnectionBeingChecked : StateEvent { }

    public record XuiInstallOutputReceived : StateEvent<XuiInstallOutputReceivedArgs>
    {
        public XuiInstallOutputReceived(XuiInstallOutputReceivedArgs value) : base(value)
        {
        }

        public record XuiInstallOutputReceivedArgs(string Output);
    }

    public record XuiInstallFinished : StateEvent { }

    public record HubConnectionSucceeded : StateEvent<HubConnectionSucceededArgs>
    {
        public HubConnectionSucceeded(HubConnectionSucceededArgs value) : base(value)
        {
        }

        public record HubConnectionSucceededArgs(string HubConnectionId);
    }
    public record XuiInstallScriptsBeingExecuted : StateEvent { }

    public record XuiInstallScriptsExecuted : StateEvent { }
}