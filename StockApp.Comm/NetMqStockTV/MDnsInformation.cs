using Makaretu.Dns;

namespace StockApp.Comm.NetMqStockTV
{
    interface IMDnsInformation
    {
        int ControlServicePort { get; }
        int PublisherServicePort { get; }
        string FW { get; }
        string IpAddress { get; }
        string HostName { get; }
        DomainName DomainName { get; }
        HashSet<string> Informations { get; }
    }

    internal class MDnsInformation : IMDnsInformation
    {
        private readonly string _hostname;
        public MDnsInformation(DomainName domainName, string ipAddress)
        {
            DomainName = domainName;
            _hostname = DomainName.Labels[0];

            IpAddress = ipAddress;
            Informations = new HashSet<string>();
        }

        public DomainName DomainName { get;  }

        public string HostName => _hostname;

        public string IpAddress { get;  }

        public HashSet<string> Informations { get; }

        public string FW => Informations?
                        .First(i => i.StartsWith("pkg"))?
                        .Split('=')[1] ?? "n.a.";

        public int PublisherServicePort
        {
            get
            {
                var s = Informations?
                    .First(i => i.StartsWith("pubSvc"))?
                    .Split('=')[1];

                if (int.TryParse(s, out int p))
                {
                    return p;
                }
                else
                {
                    return 0;
                }

            }
        }

        public int ControlServicePort
        {
            get
            {
                var s = Informations?
                    .First(i => i.StartsWith("ctrSvc"))?
                    .Split('=')[1];

                if (int.TryParse(s, out int p))
                {
                    return p;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
