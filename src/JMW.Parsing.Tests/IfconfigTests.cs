using Xunit.Abstractions;

namespace JMW.Parsing.Tests;

public class IfconfigTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void ParseJsonTest()
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
        Ifconfig.Parse(reader, writer, new ParsingOptions(OutputType.Json));

        var actualOutput = writer.ToString();
        testOutputHelper.WriteLine(actualOutput);
        Assert.Equal(expectedJson, actualOutput);
    }
}