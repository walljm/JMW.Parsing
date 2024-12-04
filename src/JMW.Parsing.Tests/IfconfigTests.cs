using Xunit.Abstractions;

namespace JMW.Parsing.Tests;

public class IfconfigTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void ParseJsonMacOsTest1()
    {
        const string ifconfigOutput = @"
lo0: flags=8049<UP,LOOPBACK,RUNNING,MULTICAST> mtu 16384
	options=1203<RXCSUM,TXCSUM,TXSTATUS,SW_TIMESTAMP>
	inet 127.0.0.1 netmask 0xff000000
	inet6 ::1 prefixlen 128
	inet6 fe80::1%lo0 prefixlen 64 scopeid 0x1
	nd6 options=201<PERFORMNUD,DAD>
gif0: flags=8010<POINTOPOINT,MULTICAST> mtu 1280
stf0: flags=0<> mtu 1280
anpi0: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=400<CHANNEL_IO>
	ether ae:11:a3:27:51:42
	media: none
	status: inactive
anpi1: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=400<CHANNEL_IO>
	ether ae:11:a3:27:51:43
	media: none
	status: inactive
anpi2: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=400<CHANNEL_IO>
	ether ae:11:a3:27:51:44
	media: none
	status: inactive
en4: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=400<CHANNEL_IO>
	ether ae:11:a3:27:51:22
	nd6 options=201<PERFORMNUD,DAD>
	media: none
	status: inactive
en5: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=400<CHANNEL_IO>
	ether ae:11:a3:27:51:23
	nd6 options=201<PERFORMNUD,DAD>
	media: none
	status: inactive
en6: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=400<CHANNEL_IO>
	ether ae:11:a3:27:51:24
	nd6 options=201<PERFORMNUD,DAD>
	media: none
	status: inactive
en1: flags=8963<UP,BROADCAST,SMART,RUNNING,PROMISC,SIMPLEX,MULTICAST> mtu 1500
	options=460<TSO4,TSO6,CHANNEL_IO>
	ether 36:10:0f:92:66:40
	media: autoselect <full-duplex>
	status: inactive
en2: flags=8963<UP,BROADCAST,SMART,RUNNING,PROMISC,SIMPLEX,MULTICAST> mtu 1500
	options=460<TSO4,TSO6,CHANNEL_IO>
	ether 36:10:0f:92:66:44
	media: autoselect <full-duplex>
	status: inactive
en3: flags=8963<UP,BROADCAST,SMART,RUNNING,PROMISC,SIMPLEX,MULTICAST> mtu 1500
	options=460<TSO4,TSO6,CHANNEL_IO>
	ether 36:10:0f:92:66:48
	media: autoselect <full-duplex>
	status: inactive
bridge0: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=63<RXCSUM,TXCSUM,TSO4,TSO6>
	ether 36:10:0f:92:66:40
	Configuration:
		id 0:0:0:0:0:0 priority 0 hellotime 0 fwddelay 0
		maxage 0 holdcnt 0 proto stp maxaddr 100 timeout 1200
		root id 0:0:0:0:0:0 priority 0 ifcost 0 port 0
		ipfilter disabled flags 0x0
	member: en1 flags=3<LEARNING,DISCOVER>
	        ifmaxaddr 0 port 10 priority 0 path cost 0
	member: en2 flags=3<LEARNING,DISCOVER>
	        ifmaxaddr 0 port 11 priority 0 path cost 0
	member: en3 flags=3<LEARNING,DISCOVER>
	        ifmaxaddr 0 port 12 priority 0 path cost 0
	nd6 options=201<PERFORMNUD,DAD>
	media: <unknown type>
	status: inactive
ap1: flags=8843<UP,BROADCAST,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=6460<TSO4,TSO6,CHANNEL_IO,PARTIAL_CSUM,ZEROINVERT_CSUM>
	ether 62:3e:5f:88:0c:1a
	inet6 fe80::603e:5fff:fe88:c1a%ap1 prefixlen 64 scopeid 0xe
	nd6 options=201<PERFORMNUD,DAD>
	media: autoselect (<unknown type>)
	status: inactive
en0: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=6460<TSO4,TSO6,CHANNEL_IO,PARTIAL_CSUM,ZEROINVERT_CSUM>
	ether 60:3e:5f:88:0c:1a
	inet6 fe80::c3f:8342:b210:f73b%en0 prefixlen 64 secured scopeid 0xf
	inet 192.168.1.237 netmask 0xffffff00 broadcast 192.168.1.255
	nd6 options=201<PERFORMNUD,DAD>
	media: autoselect
	status: active
awdl0: flags=8843<UP,BROADCAST,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=6460<TSO4,TSO6,CHANNEL_IO,PARTIAL_CSUM,ZEROINVERT_CSUM>
	ether 7a:3a:66:17:6f:09
	inet6 fe80::783a:66ff:fe17:6f09%awdl0 prefixlen 64 scopeid 0x10
	nd6 options=201<PERFORMNUD,DAD>
	media: autoselect
	status: active
llw0: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=400<CHANNEL_IO>
	ether 7a:3a:66:17:6f:09
	inet6 fe80::783a:66ff:fe17:6f09%llw0 prefixlen 64 scopeid 0x11
	nd6 options=201<PERFORMNUD,DAD>
	media: autoselect
	status: inactive
utun0: flags=8051<UP,POINTOPOINT,RUNNING,MULTICAST> mtu 1500
	inet6 fe80::53b2:8d35:9bd7:daa2%utun0 prefixlen 64 scopeid 0x12
	nd6 options=201<PERFORMNUD,DAD>
utun1: flags=8051<UP,POINTOPOINT,RUNNING,MULTICAST> mtu 1380
	inet6 fe80::feef:be1b:e135:bbeb%utun1 prefixlen 64 scopeid 0x13
	nd6 options=201<PERFORMNUD,DAD>
utun2: flags=8051<UP,POINTOPOINT,RUNNING,MULTICAST> mtu 2000
	inet6 fe80::c34a:71cd:412e:c767%utun2 prefixlen 64 scopeid 0x14
	nd6 options=201<PERFORMNUD,DAD>
utun3: flags=8051<UP,POINTOPOINT,RUNNING,MULTICAST> mtu 1000
	inet6 fe80::ce81:b1c:bd2c:69e%utun3 prefixlen 64 scopeid 0x15
	nd6 options=201<PERFORMNUD,DAD>
utun4: flags=8051<UP,POINTOPOINT,RUNNING,MULTICAST> mtu 1380
	inet6 fe80::deed:f19f:2404:f43a%utun4 prefixlen 64 scopeid 0x16
	nd6 options=201<PERFORMNUD,DAD>
utun5: flags=8051<UP,POINTOPOINT,RUNNING,MULTICAST> mtu 1380
	inet6 fe80::563d:344c:8ed3:9147%utun5 prefixlen 64 scopeid 0x19
	nd6 options=201<PERFORMNUD,DAD>
en7: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=6464<VLAN_MTU,TSO4,TSO6,CHANNEL_IO,PARTIAL_CSUM,ZEROINVERT_CSUM>
	ether 64:4b:f0:13:a3:eb
	inet6 fe80::cc9:384e:130:7cb4%en7 prefixlen 64 secured scopeid 0x18
	inet 192.168.1.204 netmask 0xffffff00 broadcast 192.168.1.255
	nd6 options=201<PERFORMNUD,DAD>
	media: autoselect (1000baseT <full-duplex,flow-control>)
	status: active
";

		    const string expectedJson = @"[
  {
    ""InterfaceName"": ""lo0"",
    ""Flags"": {
      ""Bits"": ""8049"",
      ""Values"": [
        ""UP"",
        ""LOOPBACK"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""16384"",
    ""Options"": {
      ""Bits"": ""1203"",
      ""Values"": [
        ""RXCSUM"",
        ""TXCSUM"",
        ""TXSTATUS"",
        ""SW_TIMESTAMP""
      ]
    },
    ""Inets"": [
      {
        ""Inet"": ""127.0.0.1"",
        ""Netmask"": ""0xff000000""
      }
    ],
    ""Inet6s"": [
      {
        ""Inet6"": ""::1"",
        ""Prefixlen"": ""128""
      },
      {
        ""Inet6"": ""fe80::1"",
        ""Interface"": ""lo0"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x1""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    }
  },
  {
    ""InterfaceName"": ""gif0"",
    ""Flags"": {
      ""Bits"": ""8010"",
      ""Values"": [
        ""POINTOPOINT"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1280""
  },
  {
    ""InterfaceName"": ""stf0"",
    ""Flags"": {
      ""Bits"": ""0"",
      ""Values"": [
        """"
      ]
    },
    ""Mtu"": ""1280""
  },
  {
    ""InterfaceName"": ""anpi0"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""400"",
      ""Values"": [
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""ae:11:a3:27:51:42"",
    ""Media"": ""none"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""anpi1"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""400"",
      ""Values"": [
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""ae:11:a3:27:51:43"",
    ""Media"": ""none"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""anpi2"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""400"",
      ""Values"": [
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""ae:11:a3:27:51:44"",
    ""Media"": ""none"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""en4"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""400"",
      ""Values"": [
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""ae:11:a3:27:51:22"",
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    },
    ""Media"": ""none"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""en5"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""400"",
      ""Values"": [
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""ae:11:a3:27:51:23"",
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    },
    ""Media"": ""none"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""en6"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""400"",
      ""Values"": [
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""ae:11:a3:27:51:24"",
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    },
    ""Media"": ""none"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""en1"",
    ""Flags"": {
      ""Bits"": ""8963"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""PROMISC"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""460"",
      ""Values"": [
        ""TSO4"",
        ""TSO6"",
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""36:10:0f:92:66:40"",
    ""Media"": ""autoselect \u003Cfull-duplex\u003E"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""en2"",
    ""Flags"": {
      ""Bits"": ""8963"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""PROMISC"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""460"",
      ""Values"": [
        ""TSO4"",
        ""TSO6"",
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""36:10:0f:92:66:44"",
    ""Media"": ""autoselect \u003Cfull-duplex\u003E"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""en3"",
    ""Flags"": {
      ""Bits"": ""8963"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""PROMISC"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""460"",
      ""Values"": [
        ""TSO4"",
        ""TSO6"",
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""36:10:0f:92:66:48"",
    ""Media"": ""autoselect \u003Cfull-duplex\u003E"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""bridge0"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""63"",
      ""Values"": [
        ""RXCSUM"",
        ""TXCSUM"",
        ""TSO4"",
        ""TSO6""
      ]
    },
    ""Ether"": ""36:10:0f:92:66:40"",
    ""Configuration"": {
      ""Id"": ""0:0:0:0:0:0"",
      ""Priority"": ""0"",
      ""Hellotime"": ""0"",
      ""Fwddelay"": ""0"",
      ""Maxage"": ""0"",
      ""Holdcnt"": ""0"",
      ""Proto"": ""stp"",
      ""Maxaddr"": ""100"",
      ""Timeout"": ""1200"",
      ""Root"": {
        ""Id"": ""0:0:0:0:0:0"",
        ""Priority"": ""0"",
        ""Ifcost"": ""0"",
        ""Port"": ""0"",
        ""Ipfilter"": ""disabled"",
        ""Flags"": {
          ""Bits"": ""0x0"",
          ""Values"": []
        },
        ""Members"": [
          {
            ""Member"": ""en1"",
            ""Flags"": {
              ""Bits"": ""3"",
              ""Values"": [
                ""LEARNING"",
                ""DISCOVER""
              ]
            },
            ""Ifmaxaddr"": ""0"",
            ""Port"": ""10"",
            ""Priority"": ""0"",
            ""PathCost"": ""0""
          },
          {
            ""Member"": ""en2"",
            ""Flags"": {
              ""Bits"": ""3"",
              ""Values"": [
                ""LEARNING"",
                ""DISCOVER""
              ]
            },
            ""Ifmaxaddr"": ""0"",
            ""Port"": ""11"",
            ""Priority"": ""0"",
            ""PathCost"": ""0""
          },
          {
            ""Member"": ""en3"",
            ""Flags"": {
              ""Bits"": ""3"",
              ""Values"": [
                ""LEARNING"",
                ""DISCOVER""
              ]
            },
            ""Ifmaxaddr"": ""0"",
            ""Port"": ""12"",
            ""Priority"": ""0"",
            ""PathCost"": ""0""
          }
        ]
      }
    },
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    },
    ""Media"": ""\u003Cunknown type\u003E"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""ap1"",
    ""Flags"": {
      ""Bits"": ""8843"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""6460"",
      ""Values"": [
        ""TSO4"",
        ""TSO6"",
        ""CHANNEL_IO"",
        ""PARTIAL_CSUM"",
        ""ZEROINVERT_CSUM""
      ]
    },
    ""Ether"": ""62:3e:5f:88:0c:1a"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::603e:5fff:fe88:c1a"",
        ""Interface"": ""ap1"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0xe""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    },
    ""Media"": ""autoselect (\u003Cunknown type\u003E)"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""en0"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""6460"",
      ""Values"": [
        ""TSO4"",
        ""TSO6"",
        ""CHANNEL_IO"",
        ""PARTIAL_CSUM"",
        ""ZEROINVERT_CSUM""
      ]
    },
    ""Ether"": ""60:3e:5f:88:0c:1a"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::c3f:8342:b210:f73b"",
        ""Interface"": ""en0"",
        ""Prefixlen"": ""64"",
        ""Secured"": ""true"",
        ""Scopeid"": ""0xf""
      }
    ],
    ""Inets"": [
      {
        ""Inet"": ""192.168.1.237"",
        ""Netmask"": ""0xffffff00"",
        ""Broadcast"": ""192.168.1.255""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    },
    ""Media"": ""autoselect"",
    ""Status"": ""active""
  },
  {
    ""InterfaceName"": ""awdl0"",
    ""Flags"": {
      ""Bits"": ""8843"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""6460"",
      ""Values"": [
        ""TSO4"",
        ""TSO6"",
        ""CHANNEL_IO"",
        ""PARTIAL_CSUM"",
        ""ZEROINVERT_CSUM""
      ]
    },
    ""Ether"": ""7a:3a:66:17:6f:09"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::783a:66ff:fe17:6f09"",
        ""Interface"": ""awdl0"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x10""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    },
    ""Media"": ""autoselect"",
    ""Status"": ""active""
  },
  {
    ""InterfaceName"": ""llw0"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""400"",
      ""Values"": [
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""7a:3a:66:17:6f:09"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::783a:66ff:fe17:6f09"",
        ""Interface"": ""llw0"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x11""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    },
    ""Media"": ""autoselect"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""utun0"",
    ""Flags"": {
      ""Bits"": ""8051"",
      ""Values"": [
        ""UP"",
        ""POINTOPOINT"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::53b2:8d35:9bd7:daa2"",
        ""Interface"": ""utun0"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x12""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    }
  },
  {
    ""InterfaceName"": ""utun1"",
    ""Flags"": {
      ""Bits"": ""8051"",
      ""Values"": [
        ""UP"",
        ""POINTOPOINT"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1380"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::feef:be1b:e135:bbeb"",
        ""Interface"": ""utun1"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x13""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    }
  },
  {
    ""InterfaceName"": ""utun2"",
    ""Flags"": {
      ""Bits"": ""8051"",
      ""Values"": [
        ""UP"",
        ""POINTOPOINT"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""2000"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::c34a:71cd:412e:c767"",
        ""Interface"": ""utun2"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x14""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    }
  },
  {
    ""InterfaceName"": ""utun3"",
    ""Flags"": {
      ""Bits"": ""8051"",
      ""Values"": [
        ""UP"",
        ""POINTOPOINT"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1000"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::ce81:b1c:bd2c:69e"",
        ""Interface"": ""utun3"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x15""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    }
  },
  {
    ""InterfaceName"": ""utun4"",
    ""Flags"": {
      ""Bits"": ""8051"",
      ""Values"": [
        ""UP"",
        ""POINTOPOINT"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1380"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::deed:f19f:2404:f43a"",
        ""Interface"": ""utun4"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x16""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    }
  },
  {
    ""InterfaceName"": ""utun5"",
    ""Flags"": {
      ""Bits"": ""8051"",
      ""Values"": [
        ""UP"",
        ""POINTOPOINT"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1380"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::563d:344c:8ed3:9147"",
        ""Interface"": ""utun5"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x19""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    }
  },
  {
    ""InterfaceName"": ""en7"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""6464"",
      ""Values"": [
        ""VLAN_MTU"",
        ""TSO4"",
        ""TSO6"",
        ""CHANNEL_IO"",
        ""PARTIAL_CSUM"",
        ""ZEROINVERT_CSUM""
      ]
    },
    ""Ether"": ""64:4b:f0:13:a3:eb"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::cc9:384e:130:7cb4"",
        ""Interface"": ""en7"",
        ""Prefixlen"": ""64"",
        ""Secured"": ""true"",
        ""Scopeid"": ""0x18""
      }
    ],
    ""Inets"": [
      {
        ""Inet"": ""192.168.1.204"",
        ""Netmask"": ""0xffffff00"",
        ""Broadcast"": ""192.168.1.255""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    },
    ""Media"": ""autoselect (1000baseT \u003Cfull-duplex,flow-control\u003E)"",
    ""Status"": ""active""
  }
]
";

        using var reader = new StringReader(ifconfigOutput);
        using var writer = new StringWriter();
        Ifconfig.Parse(reader, writer, new DisplayOptions(OutputType.Json));

        var actualOutput = writer.ToString();
        testOutputHelper.WriteLine(actualOutput);
        Assert.Equal(expectedJson.Replace("\r\n", "\n"), actualOutput.Replace("\r\n", "\n"));
    }


    [Fact]
    public void ParseJsonMacOsTest()
    {
        const string ifconfigOutput = @"
lo0: flags=8049<UP,LOOPBACK,RUNNING,MULTICAST> mtu 16384
	options=1203<RXCSUM,TXCSUM,TXSTATUS,SW_TIMESTAMP>
	inet 127.0.0.1 netmask 0xff000000
	inet6 ::1 prefixlen 128
	inet6 fe80::1%lo0 prefixlen 64 scopeid 0x1
	nd6 options=201<PERFORMNUD,DAD>
gif0: flags=8010<POINTOPOINT,MULTICAST> mtu 1280
stf0: flags=0<> mtu 1280
anpi0: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=400<CHANNEL_IO>
	ether ae:11:a3:27:51:42
	media: none
	status: inactive
anpi1: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=400<CHANNEL_IO>
	ether ae:11:a3:27:51:43
	media: none
	status: inactive
anpi2: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=400<CHANNEL_IO>
	ether ae:11:a3:27:51:44
	media: none
	status: inactive
en4: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=400<CHANNEL_IO>
	ether ae:11:a3:27:51:22
	nd6 options=201<PERFORMNUD,DAD>
	media: none
	status: inactive
en5: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=400<CHANNEL_IO>
	ether ae:11:a3:27:51:23
	nd6 options=201<PERFORMNUD,DAD>
	media: none
	status: inactive
en6: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=400<CHANNEL_IO>
	ether ae:11:a3:27:51:24
	nd6 options=201<PERFORMNUD,DAD>
	media: none
	status: inactive
en1: flags=8963<UP,BROADCAST,SMART,RUNNING,PROMISC,SIMPLEX,MULTICAST> mtu 1500
	options=460<TSO4,TSO6,CHANNEL_IO>
	ether 36:10:0f:92:66:40
	media: autoselect <full-duplex>
	status: inactive
en2: flags=8963<UP,BROADCAST,SMART,RUNNING,PROMISC,SIMPLEX,MULTICAST> mtu 1500
	options=460<TSO4,TSO6,CHANNEL_IO>
	ether 36:10:0f:92:66:44
	media: autoselect <full-duplex>
	status: inactive
en3: flags=8963<UP,BROADCAST,SMART,RUNNING,PROMISC,SIMPLEX,MULTICAST> mtu 1500
	options=460<TSO4,TSO6,CHANNEL_IO>
	ether 36:10:0f:92:66:48
	media: autoselect <full-duplex>
	status: inactive
bridge0: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=63<RXCSUM,TXCSUM,TSO4,TSO6>
	ether 36:10:0f:92:66:40
	Configuration:
		id 0:0:0:0:0:0 priority 0 hellotime 0 fwddelay 0
		maxage 0 holdcnt 0 proto stp maxaddr 100 timeout 1200
		root id 0:0:0:0:0:0 priority 0 ifcost 0 port 0
		ipfilter disabled flags 0x0
	member: en1 flags=3<LEARNING,DISCOVER>
	        ifmaxaddr 0 port 10 priority 0 path cost 0
	member: en2 flags=3<LEARNING,DISCOVER>
	        ifmaxaddr 0 port 11 priority 0 path cost 0
	member: en3 flags=3<LEARNING,DISCOVER>
	        ifmaxaddr 0 port 12 priority 0 path cost 0
	nd6 options=201<PERFORMNUD,DAD>
	media: <unknown type>
	status: inactive
ap1: flags=8843<UP,BROADCAST,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=6460<TSO4,TSO6,CHANNEL_IO,PARTIAL_CSUM,ZEROINVERT_CSUM>
	ether 62:3e:5f:88:0c:1a
	inet6 fe80::603e:5fff:fe88:c1a%ap1 prefixlen 64 scopeid 0xe
	nd6 options=201<PERFORMNUD,DAD>
	media: autoselect (<unknown type>)
	status: inactive
en0: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=6460<TSO4,TSO6,CHANNEL_IO,PARTIAL_CSUM,ZEROINVERT_CSUM>
	ether 60:3e:5f:88:0c:1a
	inet6 fe80::3c:e0f0:253d:c31f%en0 prefixlen 64 secured scopeid 0xf
	inet 192.168.2.237 netmask 0xffffff00 broadcast 192.168.2.255
	nd6 options=201<PERFORMNUD,DAD>
	media: autoselect
	status: active
awdl0: flags=8843<UP,BROADCAST,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=6460<TSO4,TSO6,CHANNEL_IO,PARTIAL_CSUM,ZEROINVERT_CSUM>
	ether 9a:36:82:02:2b:3e
	inet6 fe80::9836:82ff:fe02:2b3e%awdl0 prefixlen 64 scopeid 0x10
	nd6 options=201<PERFORMNUD,DAD>
	media: autoselect
	status: active
llw0: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=400<CHANNEL_IO>
	ether 9a:36:82:02:2b:3e
	inet6 fe80::9836:82ff:fe02:2b3e%llw0 prefixlen 64 scopeid 0x11
	nd6 options=201<PERFORMNUD,DAD>
	media: autoselect
	status: inactive
utun0: flags=8051<UP,POINTOPOINT,RUNNING,MULTICAST> mtu 1500
	inet6 fe80::a80f:53e9:473b:2414%utun0 prefixlen 64 scopeid 0x12
	nd6 options=201<PERFORMNUD,DAD>
utun1: flags=8051<UP,POINTOPOINT,RUNNING,MULTICAST> mtu 1380
	inet6 fe80::11cf:8ceb:4dc1:fb51%utun1 prefixlen 64 scopeid 0x13
	nd6 options=201<PERFORMNUD,DAD>
utun2: flags=8051<UP,POINTOPOINT,RUNNING,MULTICAST> mtu 2000
	inet6 fe80::4a8d:8857:1874:7499%utun2 prefixlen 64 scopeid 0x14
	nd6 options=201<PERFORMNUD,DAD>
utun3: flags=8051<UP,POINTOPOINT,RUNNING,MULTICAST> mtu 1000
	inet6 fe80::ce81:b1c:bd2c:69e%utun3 prefixlen 64 scopeid 0x15
	nd6 options=201<PERFORMNUD,DAD>
en7: flags=8863<UP,BROADCAST,SMART,RUNNING,SIMPLEX,MULTICAST> mtu 1500
	options=6464<VLAN_MTU,TSO4,TSO6,CHANNEL_IO,PARTIAL_CSUM,ZEROINVERT_CSUM>
	ether 64:4b:f0:13:a3:eb
	inet6 fe80::1c4c:edd5:ff4e:2a6e%en7 prefixlen 64 secured scopeid 0x19
	inet 192.168.1.204 netmask 0xffffff00 broadcast 192.168.1.255
	nd6 options=201<PERFORMNUD,DAD>
	media: autoselect (1000baseT <full-duplex,flow-control>)
	status: active
";

		    const string expectedJson = @"[
  {
    ""InterfaceName"": ""lo0"",
    ""Flags"": {
      ""Bits"": ""8049"",
      ""Values"": [
        ""UP"",
        ""LOOPBACK"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""16384"",
    ""Options"": {
      ""Bits"": ""1203"",
      ""Values"": [
        ""RXCSUM"",
        ""TXCSUM"",
        ""TXSTATUS"",
        ""SW_TIMESTAMP""
      ]
    },
    ""Inets"": [
      {
        ""Inet"": ""127.0.0.1"",
        ""Netmask"": ""0xff000000""
      }
    ],
    ""Inet6s"": [
      {
        ""Inet6"": ""::1"",
        ""Prefixlen"": ""128""
      },
      {
        ""Inet6"": ""fe80::1"",
        ""Interface"": ""lo0"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x1""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    }
  },
  {
    ""InterfaceName"": ""gif0"",
    ""Flags"": {
      ""Bits"": ""8010"",
      ""Values"": [
        ""POINTOPOINT"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1280""
  },
  {
    ""InterfaceName"": ""stf0"",
    ""Flags"": {
      ""Bits"": ""0"",
      ""Values"": [
        """"
      ]
    },
    ""Mtu"": ""1280""
  },
  {
    ""InterfaceName"": ""anpi0"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""400"",
      ""Values"": [
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""ae:11:a3:27:51:42"",
    ""Media"": ""none"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""anpi1"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""400"",
      ""Values"": [
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""ae:11:a3:27:51:43"",
    ""Media"": ""none"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""anpi2"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""400"",
      ""Values"": [
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""ae:11:a3:27:51:44"",
    ""Media"": ""none"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""en4"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""400"",
      ""Values"": [
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""ae:11:a3:27:51:22"",
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    },
    ""Media"": ""none"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""en5"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""400"",
      ""Values"": [
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""ae:11:a3:27:51:23"",
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    },
    ""Media"": ""none"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""en6"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""400"",
      ""Values"": [
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""ae:11:a3:27:51:24"",
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    },
    ""Media"": ""none"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""en1"",
    ""Flags"": {
      ""Bits"": ""8963"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""PROMISC"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""460"",
      ""Values"": [
        ""TSO4"",
        ""TSO6"",
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""36:10:0f:92:66:40"",
    ""Media"": ""autoselect \u003Cfull-duplex\u003E"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""en2"",
    ""Flags"": {
      ""Bits"": ""8963"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""PROMISC"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""460"",
      ""Values"": [
        ""TSO4"",
        ""TSO6"",
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""36:10:0f:92:66:44"",
    ""Media"": ""autoselect \u003Cfull-duplex\u003E"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""en3"",
    ""Flags"": {
      ""Bits"": ""8963"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""PROMISC"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""460"",
      ""Values"": [
        ""TSO4"",
        ""TSO6"",
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""36:10:0f:92:66:48"",
    ""Media"": ""autoselect \u003Cfull-duplex\u003E"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""bridge0"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""63"",
      ""Values"": [
        ""RXCSUM"",
        ""TXCSUM"",
        ""TSO4"",
        ""TSO6""
      ]
    },
    ""Ether"": ""36:10:0f:92:66:40"",
    ""Configuration"": {
      ""Id"": ""0:0:0:0:0:0"",
      ""Priority"": ""0"",
      ""Hellotime"": ""0"",
      ""Fwddelay"": ""0"",
      ""Maxage"": ""0"",
      ""Holdcnt"": ""0"",
      ""Proto"": ""stp"",
      ""Maxaddr"": ""100"",
      ""Timeout"": ""1200"",
      ""Root"": {
        ""Id"": ""0:0:0:0:0:0"",
        ""Priority"": ""0"",
        ""Ifcost"": ""0"",
        ""Port"": ""0"",
        ""Ipfilter"": ""disabled"",
        ""Flags"": {
          ""Bits"": ""0x0"",
          ""Values"": []
        },
        ""Members"": [
          {
            ""Member"": ""en1"",
            ""Flags"": {
              ""Bits"": ""3"",
              ""Values"": [
                ""LEARNING"",
                ""DISCOVER""
              ]
            },
            ""Ifmaxaddr"": ""0"",
            ""Port"": ""10"",
            ""Priority"": ""0"",
            ""PathCost"": ""0""
          },
          {
            ""Member"": ""en2"",
            ""Flags"": {
              ""Bits"": ""3"",
              ""Values"": [
                ""LEARNING"",
                ""DISCOVER""
              ]
            },
            ""Ifmaxaddr"": ""0"",
            ""Port"": ""11"",
            ""Priority"": ""0"",
            ""PathCost"": ""0""
          },
          {
            ""Member"": ""en3"",
            ""Flags"": {
              ""Bits"": ""3"",
              ""Values"": [
                ""LEARNING"",
                ""DISCOVER""
              ]
            },
            ""Ifmaxaddr"": ""0"",
            ""Port"": ""12"",
            ""Priority"": ""0"",
            ""PathCost"": ""0""
          }
        ]
      }
    },
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    },
    ""Media"": ""\u003Cunknown type\u003E"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""ap1"",
    ""Flags"": {
      ""Bits"": ""8843"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""6460"",
      ""Values"": [
        ""TSO4"",
        ""TSO6"",
        ""CHANNEL_IO"",
        ""PARTIAL_CSUM"",
        ""ZEROINVERT_CSUM""
      ]
    },
    ""Ether"": ""62:3e:5f:88:0c:1a"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::603e:5fff:fe88:c1a"",
        ""Interface"": ""ap1"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0xe""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    },
    ""Media"": ""autoselect (\u003Cunknown type\u003E)"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""en0"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""6460"",
      ""Values"": [
        ""TSO4"",
        ""TSO6"",
        ""CHANNEL_IO"",
        ""PARTIAL_CSUM"",
        ""ZEROINVERT_CSUM""
      ]
    },
    ""Ether"": ""60:3e:5f:88:0c:1a"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::3c:e0f0:253d:c31f"",
        ""Interface"": ""en0"",
        ""Prefixlen"": ""64"",
        ""Secured"": ""true"",
        ""Scopeid"": ""0xf""
      }
    ],
    ""Inets"": [
      {
        ""Inet"": ""192.168.2.237"",
        ""Netmask"": ""0xffffff00"",
        ""Broadcast"": ""192.168.2.255""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    },
    ""Media"": ""autoselect"",
    ""Status"": ""active""
  },
  {
    ""InterfaceName"": ""awdl0"",
    ""Flags"": {
      ""Bits"": ""8843"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""6460"",
      ""Values"": [
        ""TSO4"",
        ""TSO6"",
        ""CHANNEL_IO"",
        ""PARTIAL_CSUM"",
        ""ZEROINVERT_CSUM""
      ]
    },
    ""Ether"": ""9a:36:82:02:2b:3e"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::9836:82ff:fe02:2b3e"",
        ""Interface"": ""awdl0"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x10""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    },
    ""Media"": ""autoselect"",
    ""Status"": ""active""
  },
  {
    ""InterfaceName"": ""llw0"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""400"",
      ""Values"": [
        ""CHANNEL_IO""
      ]
    },
    ""Ether"": ""9a:36:82:02:2b:3e"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::9836:82ff:fe02:2b3e"",
        ""Interface"": ""llw0"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x11""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    },
    ""Media"": ""autoselect"",
    ""Status"": ""inactive""
  },
  {
    ""InterfaceName"": ""utun0"",
    ""Flags"": {
      ""Bits"": ""8051"",
      ""Values"": [
        ""UP"",
        ""POINTOPOINT"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::a80f:53e9:473b:2414"",
        ""Interface"": ""utun0"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x12""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    }
  },
  {
    ""InterfaceName"": ""utun1"",
    ""Flags"": {
      ""Bits"": ""8051"",
      ""Values"": [
        ""UP"",
        ""POINTOPOINT"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1380"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::11cf:8ceb:4dc1:fb51"",
        ""Interface"": ""utun1"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x13""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    }
  },
  {
    ""InterfaceName"": ""utun2"",
    ""Flags"": {
      ""Bits"": ""8051"",
      ""Values"": [
        ""UP"",
        ""POINTOPOINT"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""2000"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::4a8d:8857:1874:7499"",
        ""Interface"": ""utun2"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x14""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    }
  },
  {
    ""InterfaceName"": ""utun3"",
    ""Flags"": {
      ""Bits"": ""8051"",
      ""Values"": [
        ""UP"",
        ""POINTOPOINT"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1000"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::ce81:b1c:bd2c:69e"",
        ""Interface"": ""utun3"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x15""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    }
  },
  {
    ""InterfaceName"": ""en7"",
    ""Flags"": {
      ""Bits"": ""8863"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""SMART"",
        ""RUNNING"",
        ""SIMPLEX"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Options"": {
      ""Bits"": ""6464"",
      ""Values"": [
        ""VLAN_MTU"",
        ""TSO4"",
        ""TSO6"",
        ""CHANNEL_IO"",
        ""PARTIAL_CSUM"",
        ""ZEROINVERT_CSUM""
      ]
    },
    ""Ether"": ""64:4b:f0:13:a3:eb"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::1c4c:edd5:ff4e:2a6e"",
        ""Interface"": ""en7"",
        ""Prefixlen"": ""64"",
        ""Secured"": ""true"",
        ""Scopeid"": ""0x19""
      }
    ],
    ""Inets"": [
      {
        ""Inet"": ""192.168.1.204"",
        ""Netmask"": ""0xffffff00"",
        ""Broadcast"": ""192.168.1.255""
      }
    ],
    ""Nd6Options"": {
      ""Bits"": ""201"",
      ""Values"": [
        ""PERFORMNUD"",
        ""DAD""
      ]
    },
    ""Media"": ""autoselect (1000baseT \u003Cfull-duplex,flow-control\u003E)"",
    ""Status"": ""active""
  }
]
";

        using var reader = new StringReader(ifconfigOutput);
        using var writer = new StringWriter();
        Ifconfig.Parse(reader, writer, new DisplayOptions(OutputType.Json, null));

        var actualOutput = writer.ToString();
        testOutputHelper.WriteLine(actualOutput);
        Assert.Equal(expectedJson.Replace("\r\n", "\n"), actualOutput.Replace("\r\n", "\n"));
    }

    [Fact]
    public void ParseJsonUbuntu1Test()
    {
        const string ifconfigOutput = @"docker0: flags=4099<UP,BROADCAST,MULTICAST>  mtu 1500
        inet 172.17.0.1  netmask 255.255.0.0  broadcast 172.17.255.255
        ether 02:42:1b:bf:97:2c  txqueuelen 0  (Ethernet)
        RX packets 0  bytes 0 (0.0 B)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 0  bytes 0 (0.0 B)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

enx9cebe844d8ab: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        inet 192.168.1.210  netmask 255.255.255.0  broadcast 192.168.1.255
        inet6 fe80::c927:bb65:7a0f:4bb  prefixlen 64  scopeid 0x20<link>
        ether 9c:eb:e8:44:d8:ab  txqueuelen 1000  (Ethernet)
        RX packets 16108553  bytes 5450117374 (5.4 GB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 16102736  bytes 4625588491 (4.6 GB)
        TX errors 0  dropped 118 overruns 0  carrier 0  collisions 0

lo: flags=73<UP,LOOPBACK,RUNNING>  mtu 65536
        inet 127.0.0.1  netmask 255.0.0.0
        inet6 ::1  prefixlen 128  scopeid 0x10<host>
        loop  txqueuelen 1000  (Local Loopback)
        RX packets 396599  bytes 39772176 (39.7 MB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 396599  bytes 39772176 (39.7 MB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

tailscale0: flags=4305<UP,POINTOPOINT,RUNNING,NOARP,MULTICAST>  mtu 1280
        inet 100.93.162.26  netmask 255.255.255.255  destination 100.93.162.26
        inet6 fe80::1a81:dad8:7fc5:ef7e  prefixlen 64  scopeid 0x20<link>
        inet6 fd7a:115c:a1e0:ab12:4843:cd96:625d:a21a  prefixlen 128  scopeid 0x0<global>
        unspec 00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00  txqueuelen 500  (UNSPEC)
        RX packets 2611314  bytes 163139569 (163.1 MB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 3754187  bytes 2733369047 (2.7 GB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

wlp2s0: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        inet 192.168.2.157  netmask 255.255.255.0  broadcast 192.168.2.255
        inet6 fe80::3cdf:8918:a707:1972  prefixlen 64  scopeid 0x20<link>
        ether e0:b9:a5:d1:72:c1  txqueuelen 1000  (Ethernet)
        RX packets 22595837  bytes 8486851713 (8.4 GB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 243693  bytes 44562396 (44.5 MB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

";

		    const string expectedJson = @"[
  {
    ""InterfaceName"": ""docker0"",
    ""Flags"": {
      ""Bits"": ""4099"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Inets"": [
      {
        ""Inet"": ""172.17.0.1"",
        ""Netmask"": ""255.255.0.0"",
        ""Broadcast"": ""172.17.255.255""
      }
    ],
    ""Ether"": ""02:42:1b:bf:97:2c"",
    ""Txqueuelen"": ""0"",
    ""RX"": {
      ""Packets"": ""0"",
      ""Bytes"": ""0 (0.0 B)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Frame"": ""0""
    },
    ""TX"": {
      ""Packets"": ""0"",
      ""Bytes"": ""0 (0.0 B)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Carrier"": ""0"",
      ""Collisions"": ""0""
    }
  },
  {
    ""InterfaceName"": ""enx9cebe844d8ab"",
    ""Flags"": {
      ""Bits"": ""4163"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Inets"": [
      {
        ""Inet"": ""192.168.1.210"",
        ""Netmask"": ""255.255.255.0"",
        ""Broadcast"": ""192.168.1.255""
      }
    ],
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::c927:bb65:7a0f:4bb"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x20\u003Clink\u003E""
      }
    ],
    ""Ether"": ""9c:eb:e8:44:d8:ab"",
    ""Txqueuelen"": ""1000"",
    ""RX"": {
      ""Packets"": ""16108553"",
      ""Bytes"": ""5450117374 (5.4 GB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Frame"": ""0""
    },
    ""TX"": {
      ""Packets"": ""16102736"",
      ""Bytes"": ""4625588491 (4.6 GB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""118"",
      ""Overruns"": ""0"",
      ""Carrier"": ""0"",
      ""Collisions"": ""0""
    }
  },
  {
    ""InterfaceName"": ""lo"",
    ""Flags"": {
      ""Bits"": ""73"",
      ""Values"": [
        ""UP"",
        ""LOOPBACK"",
        ""RUNNING""
      ]
    },
    ""Mtu"": ""65536"",
    ""Inets"": [
      {
        ""Inet"": ""127.0.0.1"",
        ""Netmask"": ""255.0.0.0""
      }
    ],
    ""Inet6s"": [
      {
        ""Inet6"": ""::1"",
        ""Prefixlen"": ""128"",
        ""Scopeid"": ""0x10\u003Chost\u003E""
      }
    ],
    ""Loop"": ""true"",
    ""Txqueuelen"": ""1000"",
    ""RX"": {
      ""Packets"": ""396599"",
      ""Bytes"": ""39772176 (39.7 MB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Frame"": ""0""
    },
    ""TX"": {
      ""Packets"": ""396599"",
      ""Bytes"": ""39772176 (39.7 MB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Carrier"": ""0"",
      ""Collisions"": ""0""
    }
  },
  {
    ""InterfaceName"": ""tailscale0"",
    ""Flags"": {
      ""Bits"": ""4305"",
      ""Values"": [
        ""UP"",
        ""POINTOPOINT"",
        ""RUNNING"",
        ""NOARP"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1280"",
    ""Inets"": [
      {
        ""Inet"": ""100.93.162.26"",
        ""Netmask"": ""255.255.255.255""
      }
    ],
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::1a81:dad8:7fc5:ef7e"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x20\u003Clink\u003E""
      },
      {
        ""Inet6"": ""fd7a:115c:a1e0:ab12:4843:cd96:625d:a21a"",
        ""Prefixlen"": ""128"",
        ""Scopeid"": ""0x0\u003Cglobal\u003E""
      }
    ],
    ""Unspec"": ""00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00"",
    ""Txqueuelen"": ""500"",
    ""RX"": {
      ""Packets"": ""2611314"",
      ""Bytes"": ""163139569 (163.1 MB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Frame"": ""0""
    },
    ""TX"": {
      ""Packets"": ""3754187"",
      ""Bytes"": ""2733369047 (2.7 GB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Carrier"": ""0"",
      ""Collisions"": ""0""
    }
  },
  {
    ""InterfaceName"": ""wlp2s0"",
    ""Flags"": {
      ""Bits"": ""4163"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Inets"": [
      {
        ""Inet"": ""192.168.2.157"",
        ""Netmask"": ""255.255.255.0"",
        ""Broadcast"": ""192.168.2.255""
      }
    ],
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::3cdf:8918:a707:1972"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x20\u003Clink\u003E""
      }
    ],
    ""Ether"": ""e0:b9:a5:d1:72:c1"",
    ""Txqueuelen"": ""1000"",
    ""RX"": {
      ""Packets"": ""22595837"",
      ""Bytes"": ""8486851713 (8.4 GB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Frame"": ""0""
    },
    ""TX"": {
      ""Packets"": ""243693"",
      ""Bytes"": ""44562396 (44.5 MB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Carrier"": ""0"",
      ""Collisions"": ""0""
    }
  }
]
";

        using var reader = new StringReader(ifconfigOutput);
        using var writer = new StringWriter();
        Ifconfig.Parse(reader, writer, new DisplayOptions(OutputType.Json));

        var actualOutput = writer.ToString();
        testOutputHelper.WriteLine(actualOutput);
        Assert.Equal(expectedJson.Replace("\r\n", "\n"), actualOutput.Replace("\r\n", "\n"));
    }

    [Fact]
    public void ParseJsonUbuntu2Test()
    {
        const string ifconfigOutput = @"br-530f8ddccaf3: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        inet 172.24.0.1  netmask 255.255.0.0  broadcast 172.24.255.255
        inet6 fe80::42:7ff:fe77:643e  prefixlen 64  scopeid 0x20<link>
        ether 02:42:07:77:64:3e  txqueuelen 0  (Ethernet)
        RX packets 17070416  bytes 17418305276 (17.4 GB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 16235754  bytes 6814602232 (6.8 GB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

br-9336a9a52d58: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        inet 172.25.0.1  netmask 255.255.0.0  broadcast 172.25.255.255
        inet6 fe80::42:dcff:fe7a:6dc6  prefixlen 64  scopeid 0x20<link>
        ether 02:42:dc:7a:6d:c6  txqueuelen 0  (Ethernet)
        RX packets 1704  bytes 1720925 (1.7 MB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 305016  bytes 13471808 (13.4 MB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

docker0: flags=4099<UP,BROADCAST,MULTICAST>  mtu 1500
        inet 172.17.0.1  netmask 255.255.0.0  broadcast 172.17.255.255
        ether 02:42:dd:b2:26:88  txqueuelen 0  (Ethernet)
        RX packets 0  bytes 0 (0.0 B)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 0  bytes 0 (0.0 B)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

eth0: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 9001
        inet 172.30.0.21  netmask 255.255.255.0  broadcast 172.30.0.255
        inet6 fe80::8ff:41ff:fe40:6ef9  prefixlen 64  scopeid 0x20<link>
        ether 0a:ff:41:40:6e:f9  txqueuelen 1000  (Ethernet)
        RX packets 19928899  bytes 7661006299 (7.6 GB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 24211498  bytes 18502727774 (18.5 GB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

lo: flags=73<UP,LOOPBACK,RUNNING>  mtu 65536
        inet 127.0.0.1  netmask 255.0.0.0
        inet6 ::1  prefixlen 128  scopeid 0x10<host>
        loop  txqueuelen 1000  (Local Loopback)
        RX packets 2389363  bytes 319799604 (319.7 MB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 2389363  bytes 319799604 (319.7 MB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

tailscale0: flags=4305<UP,POINTOPOINT,RUNNING,NOARP,MULTICAST>  mtu 1280
        inet 100.88.1.107  netmask 255.255.255.255  destination 100.88.1.107
        inet6 fe80::2faf:887e:330b:8783  prefixlen 64  scopeid 0x20<link>
        inet6 fd7a:115c:a1e0:ab12:4843:cd96:6258:16b  prefixlen 128  scopeid 0x0<global>
        unspec 00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00  txqueuelen 500  (UNSPEC)
        RX packets 8097  bytes 680630 (680.6 KB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 117434  bytes 11014214 (11.0 MB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

veth0ec3231: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        inet6 fe80::581e:cfff:fe36:2ea5  prefixlen 64  scopeid 0x20<link>
        ether 5a:1e:cf:36:2e:a5  txqueuelen 0  (Ethernet)
        RX packets 5857  bytes 2945118 (2.9 MB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 311016  bytes 15949827 (15.9 MB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

veth2ef2a03: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        inet6 fe80::842a:e6ff:feef:b2df  prefixlen 64  scopeid 0x20<link>
        ether 86:2a:e6:ef:b2:df  txqueuelen 0  (Ethernet)
        RX packets 1704  bytes 1744781 (1.7 MB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 305020  bytes 13472284 (13.4 MB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

veth3dc1383: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        inet6 fe80::3403:e8ff:fecc:dbd9  prefixlen 64  scopeid 0x20<link>
        ether 36:03:e8:cc:db:d9  txqueuelen 0  (Ethernet)
        RX packets 167262683  bytes 32033232911 (32.0 GB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 164352748  bytes 1330083561074 (1.3 TB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

veth6f75e49: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        inet6 fe80::181b:a7ff:fe62:6ced  prefixlen 64  scopeid 0x20<link>
        ether 1a:1b:a7:62:6c:ed  txqueuelen 0  (Ethernet)
        RX packets 3369553  bytes 29049556393 (29.0 GB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 3519440  bytes 433375972 (433.3 MB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

veth9cb8381: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        inet6 fe80::b0d3:40ff:fe20:3e24  prefixlen 64  scopeid 0x20<link>
        ether b2:d3:40:20:3e:24  txqueuelen 0  (Ethernet)
        RX packets 19840596  bytes 18548796872 (18.5 GB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 18016840  bytes 15951114694 (15.9 GB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

vethe262982: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        inet6 fe80::90e4:e6ff:fe21:fda4  prefixlen 64  scopeid 0x20<link>
        ether 92:e4:e6:21:fd:a4  txqueuelen 0  (Ethernet)
        RX packets 2084531  bytes 1386047641 (1.3 GB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 2725578  bytes 2609723244 (2.6 GB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

ztrfyb5gbi: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 2800
        inet 192.168.10.101  netmask 255.255.255.0  broadcast 192.168.10.255
        inet6 fe80::a032:94ff:fe69:6a86  prefixlen 64  scopeid 0x20<link>
        ether a2:32:94:69:6a:86  txqueuelen 1000  (Ethernet)
        RX packets 1457  bytes 133462 (133.4 KB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 70  bytes 8600 (8.6 KB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0
";

		    const string expectedJson = @"[
  {
    ""InterfaceName"": ""br-530f8ddccaf3"",
    ""Flags"": {
      ""Bits"": ""4163"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Inets"": [
      {
        ""Inet"": ""172.24.0.1"",
        ""Netmask"": ""255.255.0.0"",
        ""Broadcast"": ""172.24.255.255""
      }
    ],
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::42:7ff:fe77:643e"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x20\u003Clink\u003E""
      }
    ],
    ""Ether"": ""02:42:07:77:64:3e"",
    ""Txqueuelen"": ""0"",
    ""RX"": {
      ""Packets"": ""17070416"",
      ""Bytes"": ""17418305276 (17.4 GB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Frame"": ""0""
    },
    ""TX"": {
      ""Packets"": ""16235754"",
      ""Bytes"": ""6814602232 (6.8 GB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Carrier"": ""0"",
      ""Collisions"": ""0""
    }
  },
  {
    ""InterfaceName"": ""br-9336a9a52d58"",
    ""Flags"": {
      ""Bits"": ""4163"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Inets"": [
      {
        ""Inet"": ""172.25.0.1"",
        ""Netmask"": ""255.255.0.0"",
        ""Broadcast"": ""172.25.255.255""
      }
    ],
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::42:dcff:fe7a:6dc6"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x20\u003Clink\u003E""
      }
    ],
    ""Ether"": ""02:42:dc:7a:6d:c6"",
    ""Txqueuelen"": ""0"",
    ""RX"": {
      ""Packets"": ""1704"",
      ""Bytes"": ""1720925 (1.7 MB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Frame"": ""0""
    },
    ""TX"": {
      ""Packets"": ""305016"",
      ""Bytes"": ""13471808 (13.4 MB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Carrier"": ""0"",
      ""Collisions"": ""0""
    }
  },
  {
    ""InterfaceName"": ""docker0"",
    ""Flags"": {
      ""Bits"": ""4099"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Inets"": [
      {
        ""Inet"": ""172.17.0.1"",
        ""Netmask"": ""255.255.0.0"",
        ""Broadcast"": ""172.17.255.255""
      }
    ],
    ""Ether"": ""02:42:dd:b2:26:88"",
    ""Txqueuelen"": ""0"",
    ""RX"": {
      ""Packets"": ""0"",
      ""Bytes"": ""0 (0.0 B)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Frame"": ""0""
    },
    ""TX"": {
      ""Packets"": ""0"",
      ""Bytes"": ""0 (0.0 B)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Carrier"": ""0"",
      ""Collisions"": ""0""
    }
  },
  {
    ""InterfaceName"": ""eth0"",
    ""Flags"": {
      ""Bits"": ""4163"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""9001"",
    ""Inets"": [
      {
        ""Inet"": ""172.30.0.21"",
        ""Netmask"": ""255.255.255.0"",
        ""Broadcast"": ""172.30.0.255""
      }
    ],
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::8ff:41ff:fe40:6ef9"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x20\u003Clink\u003E""
      }
    ],
    ""Ether"": ""0a:ff:41:40:6e:f9"",
    ""Txqueuelen"": ""1000"",
    ""RX"": {
      ""Packets"": ""19928899"",
      ""Bytes"": ""7661006299 (7.6 GB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Frame"": ""0""
    },
    ""TX"": {
      ""Packets"": ""24211498"",
      ""Bytes"": ""18502727774 (18.5 GB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Carrier"": ""0"",
      ""Collisions"": ""0""
    }
  },
  {
    ""InterfaceName"": ""lo"",
    ""Flags"": {
      ""Bits"": ""73"",
      ""Values"": [
        ""UP"",
        ""LOOPBACK"",
        ""RUNNING""
      ]
    },
    ""Mtu"": ""65536"",
    ""Inets"": [
      {
        ""Inet"": ""127.0.0.1"",
        ""Netmask"": ""255.0.0.0""
      }
    ],
    ""Inet6s"": [
      {
        ""Inet6"": ""::1"",
        ""Prefixlen"": ""128"",
        ""Scopeid"": ""0x10\u003Chost\u003E""
      }
    ],
    ""Loop"": ""true"",
    ""Txqueuelen"": ""1000"",
    ""RX"": {
      ""Packets"": ""2389363"",
      ""Bytes"": ""319799604 (319.7 MB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Frame"": ""0""
    },
    ""TX"": {
      ""Packets"": ""2389363"",
      ""Bytes"": ""319799604 (319.7 MB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Carrier"": ""0"",
      ""Collisions"": ""0""
    }
  },
  {
    ""InterfaceName"": ""tailscale0"",
    ""Flags"": {
      ""Bits"": ""4305"",
      ""Values"": [
        ""UP"",
        ""POINTOPOINT"",
        ""RUNNING"",
        ""NOARP"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1280"",
    ""Inets"": [
      {
        ""Inet"": ""100.88.1.107"",
        ""Netmask"": ""255.255.255.255""
      }
    ],
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::2faf:887e:330b:8783"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x20\u003Clink\u003E""
      },
      {
        ""Inet6"": ""fd7a:115c:a1e0:ab12:4843:cd96:6258:16b"",
        ""Prefixlen"": ""128"",
        ""Scopeid"": ""0x0\u003Cglobal\u003E""
      }
    ],
    ""Unspec"": ""00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00"",
    ""Txqueuelen"": ""500"",
    ""RX"": {
      ""Packets"": ""8097"",
      ""Bytes"": ""680630 (680.6 KB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Frame"": ""0""
    },
    ""TX"": {
      ""Packets"": ""117434"",
      ""Bytes"": ""11014214 (11.0 MB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Carrier"": ""0"",
      ""Collisions"": ""0""
    }
  },
  {
    ""InterfaceName"": ""veth0ec3231"",
    ""Flags"": {
      ""Bits"": ""4163"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::581e:cfff:fe36:2ea5"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x20\u003Clink\u003E""
      }
    ],
    ""Ether"": ""5a:1e:cf:36:2e:a5"",
    ""Txqueuelen"": ""0"",
    ""RX"": {
      ""Packets"": ""5857"",
      ""Bytes"": ""2945118 (2.9 MB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Frame"": ""0""
    },
    ""TX"": {
      ""Packets"": ""311016"",
      ""Bytes"": ""15949827 (15.9 MB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Carrier"": ""0"",
      ""Collisions"": ""0""
    }
  },
  {
    ""InterfaceName"": ""veth2ef2a03"",
    ""Flags"": {
      ""Bits"": ""4163"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::842a:e6ff:feef:b2df"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x20\u003Clink\u003E""
      }
    ],
    ""Ether"": ""86:2a:e6:ef:b2:df"",
    ""Txqueuelen"": ""0"",
    ""RX"": {
      ""Packets"": ""1704"",
      ""Bytes"": ""1744781 (1.7 MB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Frame"": ""0""
    },
    ""TX"": {
      ""Packets"": ""305020"",
      ""Bytes"": ""13472284 (13.4 MB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Carrier"": ""0"",
      ""Collisions"": ""0""
    }
  },
  {
    ""InterfaceName"": ""veth3dc1383"",
    ""Flags"": {
      ""Bits"": ""4163"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::3403:e8ff:fecc:dbd9"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x20\u003Clink\u003E""
      }
    ],
    ""Ether"": ""36:03:e8:cc:db:d9"",
    ""Txqueuelen"": ""0"",
    ""RX"": {
      ""Packets"": ""167262683"",
      ""Bytes"": ""32033232911 (32.0 GB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Frame"": ""0""
    },
    ""TX"": {
      ""Packets"": ""164352748"",
      ""Bytes"": ""1330083561074 (1.3 TB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Carrier"": ""0"",
      ""Collisions"": ""0""
    }
  },
  {
    ""InterfaceName"": ""veth6f75e49"",
    ""Flags"": {
      ""Bits"": ""4163"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::181b:a7ff:fe62:6ced"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x20\u003Clink\u003E""
      }
    ],
    ""Ether"": ""1a:1b:a7:62:6c:ed"",
    ""Txqueuelen"": ""0"",
    ""RX"": {
      ""Packets"": ""3369553"",
      ""Bytes"": ""29049556393 (29.0 GB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Frame"": ""0""
    },
    ""TX"": {
      ""Packets"": ""3519440"",
      ""Bytes"": ""433375972 (433.3 MB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Carrier"": ""0"",
      ""Collisions"": ""0""
    }
  },
  {
    ""InterfaceName"": ""veth9cb8381"",
    ""Flags"": {
      ""Bits"": ""4163"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::b0d3:40ff:fe20:3e24"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x20\u003Clink\u003E""
      }
    ],
    ""Ether"": ""b2:d3:40:20:3e:24"",
    ""Txqueuelen"": ""0"",
    ""RX"": {
      ""Packets"": ""19840596"",
      ""Bytes"": ""18548796872 (18.5 GB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Frame"": ""0""
    },
    ""TX"": {
      ""Packets"": ""18016840"",
      ""Bytes"": ""15951114694 (15.9 GB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Carrier"": ""0"",
      ""Collisions"": ""0""
    }
  },
  {
    ""InterfaceName"": ""vethe262982"",
    ""Flags"": {
      ""Bits"": ""4163"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""1500"",
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::90e4:e6ff:fe21:fda4"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x20\u003Clink\u003E""
      }
    ],
    ""Ether"": ""92:e4:e6:21:fd:a4"",
    ""Txqueuelen"": ""0"",
    ""RX"": {
      ""Packets"": ""2084531"",
      ""Bytes"": ""1386047641 (1.3 GB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Frame"": ""0""
    },
    ""TX"": {
      ""Packets"": ""2725578"",
      ""Bytes"": ""2609723244 (2.6 GB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Carrier"": ""0"",
      ""Collisions"": ""0""
    }
  },
  {
    ""InterfaceName"": ""ztrfyb5gbi"",
    ""Flags"": {
      ""Bits"": ""4163"",
      ""Values"": [
        ""UP"",
        ""BROADCAST"",
        ""RUNNING"",
        ""MULTICAST""
      ]
    },
    ""Mtu"": ""2800"",
    ""Inets"": [
      {
        ""Inet"": ""192.168.10.101"",
        ""Netmask"": ""255.255.255.0"",
        ""Broadcast"": ""192.168.10.255""
      }
    ],
    ""Inet6s"": [
      {
        ""Inet6"": ""fe80::a032:94ff:fe69:6a86"",
        ""Prefixlen"": ""64"",
        ""Scopeid"": ""0x20\u003Clink\u003E""
      }
    ],
    ""Ether"": ""a2:32:94:69:6a:86"",
    ""Txqueuelen"": ""1000"",
    ""RX"": {
      ""Packets"": ""1457"",
      ""Bytes"": ""133462 (133.4 KB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Frame"": ""0""
    },
    ""TX"": {
      ""Packets"": ""70"",
      ""Bytes"": ""8600 (8.6 KB)"",
      ""Errors"": ""0"",
      ""Dropped"": ""0"",
      ""Overruns"": ""0"",
      ""Carrier"": ""0"",
      ""Collisions"": ""0""
    }
  }
]
";

        using var reader = new StringReader(ifconfigOutput);
        using var writer = new StringWriter();
        Ifconfig.Parse(reader, writer, new DisplayOptions(OutputType.Json));

        var actualOutput = writer.ToString();
        testOutputHelper.WriteLine(actualOutput);
        Assert.Equal(expectedJson.Replace("\r\n", "\n"), actualOutput.Replace("\r\n", "\n"));
    }
}