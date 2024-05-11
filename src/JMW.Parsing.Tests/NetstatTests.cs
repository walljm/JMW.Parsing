using Xunit.Abstractions;

namespace JMW.Parsing.Tests;

public class NetstatTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void ParseJsonMacOsTest()
    {
        const string commandOutput = @" netstat -rn
Routing tables

Internet:
Destination        Gateway            Flags               Netif Expire
default            10.120.10.1        UGScg                 en8
default            10.181.10.1        UGScIg                en0
10.120.10/23       link#23            UCS                   en8      !
10.120.10.1/32     link#23            UCS                   en8      !
10.120.10.1        0:0:c:7:ac:78      UHLWIir               en8   1151
10.120.10.13/32    link#23            UCS                   en8      !
10.181.10/23       link#15            UCS                   en0      !
10.181.10.1/32     link#15            UCS                   en0      !
10.181.10.1        0:0:c:7:ac:b5      UHLWIr                en0   1103
10.181.10.2        0:1e:7a:dd:c9:ff   UHLWI                 en0   1122
10.181.10.24/32    link#15            UCS                   en0      !
127                127.0.0.1          UCS                   lo0
127.0.0.1          127.0.0.1          UH                    lo0
169.254            link#23            UCS                   en8      !
169.254            link#15            UCSI                  en0      !
169.254.210.166    a0:29:19:c8:77:50  UHLSW                 en8      !
224.0.0/4          link#23            UmCS                  en8      !
224.0.0/4          link#15            UmCSI                 en0      !
224.0.0.251        1:0:5e:0:0:fb      UHmLWI                en0
230.230.230.230    1:0:5e:66:e6:e6    UHmLWI                en8
239.255.255.250    1:0:5e:7f:ff:fa    UHmLWI                en0
239.255.255.250    1:0:5e:7f:ff:fa    UHmLWI                en8
255.255.255.255/32 link#23            UCS                   en8      !
255.255.255.255/32 link#15            UCSI                  en0      !

Internet6:
Destination                             Gateway                                 Flags               Netif Expire
default                                 fe80::%utun0                            UGcIg               utun0
default                                 fe80::%utun1                            UGcIg               utun1
default                                 fe80::%utun2                            UGcIg               utun2
default                                 fe80::%utun3                            UGcIg               utun3
default                                 fe80::%utun4                            UGcIg               utun4
default                                 fe80::%utun5                            UGcIg               utun5
::1                                     ::1                                     UHL                   lo0
fe80::%lo0/64                           fe80::1%lo0                             UcI                   lo0
fe80::1%lo0                             link#1                                  UHLI                  lo0
fe80::%ap1/64                           link#14                                 UCI                   ap1
fe80::603e:5fff:fe88:c1a%ap1            62:3e:5f:88:c:1a                        UHLI                  lo0
fe80::%en0/64                           link#15                                 UCI                   en0
fe80::8b2:9f20:d1e3:e6b2%en0            f0:18:98:1e:a6:1e                       UHLWI                 en0
fe80::c04:a54b:381c:cd84%en0            50:de:6:a1:22:c                         UHLWI                 en0
fe80::c3f:8342:b210:f73b%en0            60:3e:5f:88:c:1a                        UHLI                  lo0
fe80::cf0:9bff:fea7:346%en0             e:f0:9b:a7:3:46                         UHLWI                 en0
fe80::102c:f38e:1417:835e%en0           4e:e1:57:ad:3a:db                       UHLWI                 en0
fe80::1c2e:13ab:47e6:63cd%en0           9c:3e:53:20:16:43                       UHLWI                 en0
fe80::70ca:16ff:feca:c98d%awdl0         72:ca:16:ca:c9:8d                       UHLI                  lo0
fe80::70ca:16ff:feca:c98d%llw0          72:ca:16:ca:c9:8d                       UHLI                  lo0
fe80::%utun0/64                         fe80::53b2:8d35:9bd7:daa2%utun0         UcI                 utun0
fe80::53b2:8d35:9bd7:daa2%utun0         link#18                                 UHLI                  lo0
fe80::%utun1/64                         fe80::feef:be1b:e135:bbeb%utun1         UcI                 utun1
fe80::feef:be1b:e135:bbeb%utun1         link#19                                 UHLI                  lo0
fe80::%utun2/64                         fe80::c34a:71cd:412e:c767%utun2         UcI                 utun2
fe80::c34a:71cd:412e:c767%utun2         link#20                                 UHLI                  lo0
fe80::%utun3/64                         fe80::ce81:b1c:bd2c:69e%utun3           UcI                 utun3
fe80::ce81:b1c:bd2c:69e%utun3           link#21                                 UHLI                  lo0
fe80::%utun4/64                         fe80::deed:f19f:2404:f43a%utun4         UcI                 utun4
fe80::deed:f19f:2404:f43a%utun4         link#22                                 UHLI                  lo0
fe80::%en8/64                           link#23                                 UCI                   en8
fe80::10d5:1230:951c:7c36%en8           9c:eb:e8:6d:23:c3                       UHLI                  lo0
fe80::%utun5/64                         fe80::563d:344c:8ed3:9147%utun5         UcI                 utun5
fe80::563d:344c:8ed3:9147%utun5         link#25                                 UHLI                  lo0
ff00::/8                                ::1                                     UmCI                  lo0
ff00::/8                                link#14                                 UmCI                  ap1
ff00::/8                                link#15                                 UmCI                  en0
ff00::/8                                link#16                                 UmCI                awdl0
ff00::/8                                link#17                                 UmCI                 llw0
ff00::/8                                fe80::53b2:8d35:9bd7:daa2%utun0         UmCI                utun0
ff00::/8                                fe80::feef:be1b:e135:bbeb%utun1         UmCI                utun1
ff00::/8                                fe80::c34a:71cd:412e:c767%utun2         UmCI                utun2
ff00::/8                                fe80::ce81:b1c:bd2c:69e%utun3           UmCI                utun3
ff00::/8                                fe80::deed:f19f:2404:f43a%utun4         UmCI                utun4
ff00::/8                                link#23                                 UmCI                  en8
ff00::/8                                fe80::563d:344c:8ed3:9147%utun5         UmCI                utun5
ff01::%lo0/32                           ::1                                     UmCI                  lo0
ff01::%ap1/32                           link#14                                 UmCI                  ap1
ff01::%en0/32                           link#15                                 UmCI                  en0
ff01::%utun0/32                         fe80::53b2:8d35:9bd7:daa2%utun0         UmCI                utun0
ff01::%utun1/32                         fe80::feef:be1b:e135:bbeb%utun1         UmCI                utun1
ff01::%utun2/32                         fe80::c34a:71cd:412e:c767%utun2         UmCI                utun2
ff01::%utun3/32                         fe80::ce81:b1c:bd2c:69e%utun3           UmCI                utun3
ff01::%utun4/32                         fe80::deed:f19f:2404:f43a%utun4         UmCI                utun4
ff01::%en8/32                           link#23                                 UmCI                  en8
ff01::%utun5/32                         fe80::563d:344c:8ed3:9147%utun5         UmCI                utun5
ff02::%lo0/32                           ::1                                     UmCI                  lo0
ff02::%ap1/32                           link#14                                 UmCI                  ap1
ff02::%en0/32                           link#15                                 UmCI                  en0
ff02::%utun0/32                         fe80::53b2:8d35:9bd7:daa2%utun0         UmCI                utun0
ff02::%utun1/32                         fe80::feef:be1b:e135:bbeb%utun1         UmCI                utun1
ff02::%utun2/32                         fe80::c34a:71cd:412e:c767%utun2         UmCI                utun2
ff02::%utun3/32                         fe80::ce81:b1c:bd2c:69e%utun3           UmCI                utun3
ff02::%utun4/32                         fe80::deed:f19f:2404:f43a%utun4         UmCI                utun4
ff02::%en8/32                           link#23                                 UmCI                  en8
ff02::%utun5/32                         fe80::563d:344c:8ed3:9147%utun5         UmCI                utun5
";

        const string expectedJson = @"";

        using var reader = new StringReader(commandOutput);
        using var writer = new StringWriter();
        Netstat.Parse(reader, writer, new ParsingOptions(OutputType.Json));

        var actualOutput = writer.ToString();
        testOutputHelper.WriteLine(actualOutput);
        Assert.Equal(expectedJson, actualOutput);
    }
}