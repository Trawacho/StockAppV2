﻿using System.Net;
using Zeroconf;

namespace StockApp.Comm.MDns;
public interface IMDnsHost
{
    string DisplayName { get; }
    string IPAddress { get; }
    int ControlServicePort { get; }
    int PublisherServicePort { get; }
    string Version { get; }
    string HostName { get; }
}
internal class MDnsHostManual : IMDnsHost
{
    public string DisplayName { get; set; }

    public string IPAddress { get; set; }

    public int ControlServicePort { get; set; }

    public int PublisherServicePort { get; set; }

    public string Version { get; set; }

    public string HostName { get; set; }
}

public class MDnsHost : IMDnsHost, IZeroconfHost
{
    readonly IZeroconfHost _host;
    public MDnsHost(IZeroconfHost host)
    {
        _host = host;
    }
    private MDnsHost()
    {

    }

    public static IMDnsHost Create(string hostname, string ipAddress, int ctrPort, int pubPort, string version)
    {
        return new MDnsHostManual()
        {
            HostName = hostname,
            IPAddress = ipAddress,  
            ControlServicePort = ctrPort,
            PublisherServicePort = pubPort,
            Version = version
        };
    }

    public string DisplayName => _host.DisplayName;
    public string HostName => DisplayName;

    public string Id => _host.Id;

    public string IPAddress => _host.IPAddresses.First(a => IsIPv4Address(a));

    private static bool IsIPv4Address(string address)
    {
        return System.Net.IPAddress.TryParse(address, out IPAddress iP)
            && iP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;
    }

    public IReadOnlyList<string> IPAddresses => _host.IPAddresses;

    public IReadOnlyDictionary<string, IService> Services => _host.Services;

    public IService FirstService => _host.Services.FirstOrDefault().Value;
    public string FirstServiceKey => _host.Services.FirstOrDefault().Key;

    public IService StockTVServices => _host.Services.First(kvp => kvp.Key.Contains("_stockTV._tcp")).Value;

    public IReadOnlyDictionary<string, string> FirstPropertySet => StockTVServices?.Properties.FirstOrDefault();

    public int ControlServicePort => int.Parse(FirstPropertySet?.Where(a => a.Key == "ctrSvc").First().Value);
    public int PublisherServicePort => int.Parse(FirstPropertySet?.Where(a => a.Key == "pubSvc").First().Value);
    public string Version => FirstPropertySet?.Where(a => a.Key == "pkgVer").First().Value;

}

