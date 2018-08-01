using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Win32;
using System.Net;
using System.Net.NetworkInformation;
using System.Management;

namespace AionLogAnalyzer
{
    public partial class PingForm : Form
    {
        private MainForm main;
        private Thread thread;

        public PingForm(MainForm main)
        {
            this.main = main;
            InitializeComponent();
        }

        public void Start()
        {
            thread = new Thread(new ThreadStart(ThreadMethod));
            thread.Start();
        }

        String version = null;
        String IP = null;
        String GUID = null;
        String MAC = null;
        int TCPNoDelay = -1;
        int TcpAckFrequency = -1;


        private void GetInfo()
        {
            // 키값이 있는지 확인한다. 만약 없다면 메시지 박스를 띄운다.

            // 현재 연결되어 있는 인터넷 값으로 guid를 얻는다.
            // 모든 정보를 일단 구한다.
            StringBuilder sb = new StringBuilder();
            // OS
            OperatingSystem os = System.Environment.OSVersion;
            Version v = os.Version;
            // 5.0 2000
            // 5.1 xp
            // 6.0 비스타
            // 6.1 윈도우7
            // 6.2 윈도우8
            if (v.Major == 5 && v.Minor == 1)
            {
                sb.Append("윈도우 XP");
            }
            else if (v.Major == 6 && v.Minor == 1)
            {
                sb.Append("윈도우 7");
            }
            else if (v.Major == 6 && v.Minor == 2)
            {
                sb.Append("윈도우 8");
            }
            sb.Append(" (" + v.ToString() + ")");

            version = sb.ToString();

            //IP
            //IPAddress address = GetRealIpAddress();

            //IPAddress[] pIPAddress = Dns.GetHostAddresses(Dns.GetHostName());

            IPAddress address = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            if (address != null)
            {
                IP = address.ToString();
            }
            else
            {
                IP = "알수없음";
            }

            // GUID
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                IPInterfaceProperties properties = ni.GetIPProperties();
                UnicastIPAddressInformationCollection uniCast = properties.UnicastAddresses;
                if (uniCast != null)
                {
                    foreach (UnicastIPAddressInformation uni in uniCast)
                    {
                        if (uni.Address.ToString() == IP)
                        {
                            GUID = ni.Id;
                            //MAC
                            MAC = ni.GetPhysicalAddress().ToString();
                            break;
                        }
                    }
                }
            }
            

            // bTCPNoDelay
            try
            {
                String str = "SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters\\Interfaces\\" + GUID;
                RegistryKey reg = Registry.LocalMachine.OpenSubKey(str);
                object obj = reg.GetValue("TCPNoDelay");
                if (obj != null)
                {
                    TCPNoDelay = Int32.Parse(obj.ToString());
                }
                else
                {
                    TCPNoDelay = -1;
                }

                obj = reg.GetValue("TcpAckFrequency"); //이거 디폴드 2임.
                if (obj != null)
                {
                    TcpAckFrequency = Int32.Parse(obj.ToString());
                }
                else
                {
                    TcpAckFrequency = -1;
                }
                reg.Close();
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }

        private void SetInfo()
        {
            this.labelOS.Text = version;
            this.labelIP.Text = IP;
            this.labelGUID.Text = GUID;
            this.labelMAC.Text = MAC;
            this.labelTCPNoDelay.Text = (TCPNoDelay == 1) ? "ON" : "OFF";
            this.labelTckAckFrequency.Text = (TcpAckFrequency == 1) ? "ON" : "OFF";
            if (this.TCPNoDelay == 1 && this.TcpAckFrequency == 1)
            {
                this.textBox1.AppendText("네트워크(패핑) ON 상태입니다.\r\n");
            }
            else
            {
                this.textBox1.AppendText("네트워크(패핑) OFF 상태입니다.\r\n");
            }
        }

        public void ThreadMethod()
        {
            GetInfo();
            if (this.TCPNoDelay == 1 && this.TcpAckFrequency == 1)
            {
                // 패핑 ON 
                this.main.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                {
                    SetInfo();
                }));
            }
            else
            {
                this.main.BeginInvoke(new EventHandler(delegate(object s, EventArgs ee)
                {
                    SetInfo();
                    main.ShowForm(this, 1);
                    this.Show();
                }));
            }
        }


        private void buttonOn_Click(object sender, EventArgs e)
        {
            if (this.TCPNoDelay == 1 && this.TcpAckFrequency == 1)
            {
                this.textBox1.AppendText("이미 ON 상태입니다.\r\n");
            }
            else
            {
                GetInfo(); SetInfo();
                this.textBox1.AppendText("잠시만 기다려주세요\r\n");
                this.textBox1.AppendText("인터넷 연결이 끊어진 후 재연결됩니다.\r\n");
                this.textBox1.AppendText("5~10초 후 새로고침 버튼을 눌러주세요.\r\n");
                SetReg(true);
                NetworkReset();
            }
        }

        private void buttonOff_Click(object sender, EventArgs e)
        {
            if (this.TCPNoDelay == 1 && this.TcpAckFrequency == 1)
            {
                GetInfo(); SetInfo();
                this.textBox1.AppendText("잠시만 기다려주세요\r\n");
                this.textBox1.AppendText("인터넷 연결이 끊어진 후 재연결됩니다.\r\n");
                this.textBox1.AppendText("5~10초 후 새로고침 버튼을 눌러주세요.\r\n");
                SetReg(false);
                NetworkReset();
            }
            else
            {
                this.textBox1.AppendText("이미 OFF 상태입니다.\r\n");
            }
        }

        private void SetReg(bool bOn)
        {
            try
            {
                String str = "SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters\\Interfaces\\" + GUID;
                RegistryKey reg = Registry.LocalMachine.OpenSubKey(str, true);
                reg.SetValue("TCPNoDelay", ((bOn) ? 1 : 0));
                reg.SetValue("TcpAckFrequency", ((bOn) ? 1 : 2));
                reg.Close();
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }

        private void NetworkReset()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "select * from Win32_NetworkAdapter");

            ManagementObjectCollection coll = searcher.Get();
            foreach (ManagementObject obj in coll)
            {
                //Console.WriteLine(obj.ClassPath.ClassName);
                PropertyData pd = obj.Properties["GUID"];
                if ( pd  == null ) continue;
                if (pd.Value == null) continue;
                    
                if (pd.Value.ToString().CompareTo(GUID) == 0 )
                {
                    obj.InvokeMethod("Disable", null);
                    Thread.Sleep(1000);
                    obj.InvokeMethod("Enable", null);
                }
            }
            GetInfo();
            SetInfo();

            /*
            foreach (ManagementObject obj in coll)
            {
                string name = obj.Properties["Name"].Value.ToString();
                if (name.Contains("Wireless"))
                    obj.InvokeMethod("Enable", null);

            }
             */
        }

        public void View()
        {
            GetInfo();
            SetInfo();
            main.ShowForm(this, 1);
            this.Show();
        }


        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            GetInfo();
            SetInfo();
        }

        

#if FALSE
        #region GetRealIpAddress
        private IPAddress GetRealIpAddress()
        {
            IPAddress gateway = FindGetGatewayAddress();

            if (gateway == null)
                return null;

            IPAddress[] pIPAddress = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress address in pIPAddress)
                if (IsAddressOfGateway(address, gateway))
                    return address;

            return null;
        }

        private bool IsAddressOfGateway(IPAddress address, IPAddress gateway)
        {
            if (address != null && gateway != null)
                return IsAddressOfGateway(address.GetAddressBytes(), gateway.GetAddressBytes());

            return false;
        }

        private bool IsAddressOfGateway(byte[] address, byte[] gateway)
        {
            if (address != null && gateway != null)
            {
                int gwLen = gateway.Length;

                if (gwLen > 0)
                {
                    if (address.Length == gateway.Length)
                    {
                        --gwLen;
                        int counter = 0;

                        for (int i = 0; i < gwLen; i++)
                        {
                            if (address[i] == gateway[i])
                                ++counter;
                        }

                        return (counter == gwLen);
                    }
                }
            }

            return false;
        }

        private IPAddress FindGetGatewayAddress()
        {
            IPGlobalProperties ipGlobProps = IPGlobalProperties.GetIPGlobalProperties();

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                IPInterfaceProperties ipInfProps = ni.GetIPProperties();

                foreach (GatewayIPAddressInformation gi in ipInfProps.GatewayAddresses)
                    return gi.Address;
            }

            return null;
        }


        public String ShowNetworkInterfaces()
        {
            StringBuilder sb = new StringBuilder();
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            sb.AppendLine(String.Format("Interface information for {0}.{1}     ",
                    computerProperties.HostName, computerProperties.DomainName));
            sb.AppendLine("GetIsNetworkAvailable : " + NetworkInterface.GetIsNetworkAvailable());
            if (nics == null || nics.Length < 1)
            {
                sb.AppendLine("  No network interfaces found.");
                return sb.ToString(); ;
            }

            sb.AppendLine(String.Format("  Number of interfaces .................... : {0}", nics.Length));
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                sb.AppendLine();
                sb.AppendLine(adapter.Description);
                sb.AppendLine(String.Empty.PadLeft(adapter.Description.Length, '='));
                sb.AppendLine("ID : " + adapter.Id);
                
                sb.AppendLine(String.Format("  Interface type .......................... : {0}", adapter.NetworkInterfaceType));
                sb.AppendLine(String.Format("  Physical Address ........................ : {0}",
                           adapter.GetPhysicalAddress().ToString()));
                sb.AppendLine(String.Format("  Operational status ...................... : {0}",
                    adapter.OperationalStatus));
                string versions = "";

                // Create a display string for the supported IP versions. 
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    versions = "IPv4";
                }
                if (adapter.Supports(NetworkInterfaceComponent.IPv6))
                {
                    if (versions.Length > 0)
                    {
                        versions += " ";
                    }
                    versions += "IPv6";
                }
                sb.AppendLine(String.Format("  IP version .............................. : {0}", versions));
                sb.AppendLine(ShowIPAddresses(properties));

                // The following information is not useful for loopback adapters. 
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }
                sb.AppendLine(String.Format("  DNS suffix .............................. : {0}",
                    properties.DnsSuffix));

                string label = "";
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    IPv4InterfaceProperties ipv4 = properties.GetIPv4Properties();
                    if (ipv4 != null)
                    {
                        sb.AppendLine(String.Format("  MTU...................................... : {0}", ipv4.Mtu));
                        if (ipv4.UsesWins)
                        {

                            IPAddressCollection winsServers = properties.WinsServersAddresses;
                            if (winsServers.Count > 0)
                            {
                                label = "  WINS Servers ............................ :";
                                //ShowIPAddresses(label, winsServers);
                            }
                        }
                    }
                }

                sb.AppendLine(String.Format("  DNS enabled ............................. : {0}",
                    properties.IsDnsEnabled));
                sb.AppendLine(String.Format("  Dynamically configured DNS .............. : {0}",
                    properties.IsDynamicDnsEnabled));
                sb.AppendLine(String.Format("  Receive Only ............................ : {0}",
                    adapter.IsReceiveOnly));
                sb.AppendLine(String.Format("  Multicast ............................... : {0}",
                    adapter.SupportsMulticast));
                //ShowInterfaceStatistics(adapter);

                sb.AppendLine();
            }
            return sb.ToString();
        }

        public String ShowIPAddresses(IPInterfaceProperties adapterProperties)
        {
            StringBuilder sb = new StringBuilder();
            IPAddressCollection dnsServers = adapterProperties.DnsAddresses;
            if (dnsServers != null)
            {
                foreach (IPAddress dns in dnsServers)
                {
                    sb.AppendLine(String.Format("  DNS Servers ............................. : {0}",
                        dns.ToString()
                   ));
                }
            }
            IPAddressInformationCollection anyCast = adapterProperties.AnycastAddresses;
            if (anyCast != null)
            {
                foreach (IPAddressInformation any in anyCast)
                {
                    sb.AppendLine(String.Format("  Anycast Address .......................... : {0} {1} {2}",
                        any.Address,
                        any.IsTransient ? "Transient" : "",
                        any.IsDnsEligible ? "DNS Eligible" : ""
                    ));
                }
                sb.AppendLine();
            }

            MulticastIPAddressInformationCollection multiCast = adapterProperties.MulticastAddresses;
            if (multiCast != null)
            {
                foreach (IPAddressInformation multi in multiCast)
                {
                    sb.AppendLine(String.Format("  Multicast Address ....................... : {0} {1} {2}",
                        multi.Address,
                        multi.IsTransient ? "Transient" : "",
                        multi.IsDnsEligible ? "DNS Eligible" : ""
                    ));
                }
                sb.AppendLine();
            }
            UnicastIPAddressInformationCollection uniCast = adapterProperties.UnicastAddresses;
            if (uniCast != null)
            {
                string lifeTimeFormat = "dddd, MMMM dd, yyyy  hh:mm:ss tt";
                foreach (UnicastIPAddressInformation uni in uniCast)
                {
                    DateTime when;

                    sb.AppendLine(String.Format("  Unicast Address ......................... : {0}", uni.Address));
                    sb.AppendLine(String.Format("     Prefix Origin ........................ : {0}", uni.PrefixOrigin));
                    sb.AppendLine(String.Format("     Suffix Origin ........................ : {0}", uni.SuffixOrigin));
                    sb.AppendLine(String.Format("     Duplicate Address Detection .......... : {0}",
                        uni.DuplicateAddressDetectionState));

                    // Format the lifetimes as Sunday, February 16, 2003 11:33:44 PM 
                    // if en-us is the current culture. 

                    // Calculate the date and time at the end of the lifetimes.    
                    when = DateTime.UtcNow + TimeSpan.FromSeconds(uni.AddressValidLifetime);
                    when = when.ToLocalTime();
                    sb.AppendLine(String.Format("     Valid Life Time ...................... : {0}",
                        when.ToString(lifeTimeFormat, System.Globalization.CultureInfo.CurrentCulture)
                    ));
                    when = DateTime.UtcNow + TimeSpan.FromSeconds(uni.AddressPreferredLifetime);
                    when = when.ToLocalTime();
                    sb.AppendLine(String.Format("     Preferred life time .................. : {0}",
                        when.ToString(lifeTimeFormat, System.Globalization.CultureInfo.CurrentCulture)
                    ));

                    when = DateTime.UtcNow + TimeSpan.FromSeconds(uni.DhcpLeaseLifetime);
                    when = when.ToLocalTime();
                    sb.AppendLine(String.Format("     DHCP Leased Life Time ................ : {0}",
                        when.ToString(lifeTimeFormat, System.Globalization.CultureInfo.CurrentCulture)
                    ));
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private void NetworkDisable()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "select * from Win32_NetworkAdapter");

            ManagementObjectCollection coll = searcher.Get();
            foreach (ManagementObject obj in coll)
            {
                //Console.WriteLine(obj.ClassPath.ClassName);
                string name = obj.Properties["Name"].Value.ToString();
                if (name.Contains("Gigabit Network"))
                {
                    obj.InvokeMethod("Disable", null);

                    Thread.Sleep(1000);
                    obj.InvokeMethod("Enable", null);
                }
            }

            /*
            foreach (ManagementObject obj in coll)
            {
                string name = obj.Properties["Name"].Value.ToString();
                if (name.Contains("Wireless"))
                    obj.InvokeMethod("Enable", null);

            }
             */
        }

        #endregion
#endif
    }

}
