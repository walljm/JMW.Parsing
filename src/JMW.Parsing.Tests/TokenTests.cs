using Xunit.Abstractions;

namespace JMW.Parsing.Tests;

public class TokenTests(ITestOutputHelper testOutputHelper)
{
   [Fact]
    public void TokenizeTest()
    {
        var tokens = Tokenizer.Tokenize(ShowInterfaces);
        var dict = new Dictionary<string, int>();
        foreach (var token in tokens)
        {
            var key = token.ToStringValue();
            if (!dict.TryAdd(key, 1)) dict[key]++;
        }

        foreach (var kvp in dict.OrderByDescending(o => o.Value))
        {
           testOutputHelper.WriteLine(kvp.Key + ": " + kvp.Value);
        }
    }

    [Fact]
    public void TokenizeTest2()
    {
        var reader = new StringReader(ShowInterfaces);
        var blockData = Tokenizer.GetBlockData(reader);
        foreach (var block in blockData.Blocks)
        {
            foreach (var token in block)
            {
                if (blockData.CommonTokens.Contains(token.ToLower()))
                {
                    // its a key!
                    testOutputHelper.WriteLine($"  Key: {token}");
                }
                else
                {
                    // not a key, but a value
                    testOutputHelper.WriteLine($"Value: {token}");
                }
            }
        }
    }

    #region data

    private const string ShowInterfaces = """

                                          show interfaces

                                          Loopback0 is up, line protocol is up
                                            Interface state transitions: 1
                                            Hardware is Loopback interface(s)
                                            Description: global-loopback
                                            Internet address is 10.0.12.12/32
                                            MTU 1500 bytes, BW 0 Kbit
                                               reliability Unknown, txload Unknown, rxload Unknown
                                            Encapsulation Loopback,  loopback not set,
                                            Last input Unknown, output Unknown
                                            Last clearing of "show interface" counters Unknown
                                            Input/output data rate is disabled.

                                          Loopback1 is up, line protocol is up
                                            Interface state transitions: 1
                                            Hardware is Loopback interface(s)
                                            Description: sdev-core-tr-north-loopback
                                            Internet address is 10.1.12.12/32
                                            MTU 1500 bytes, BW 0 Kbit
                                               reliability Unknown, txload Unknown, rxload Unknown
                                            Encapsulation Loopback,  loopback not set,
                                            Last input Unknown, output Unknown
                                            Last clearing of "show interface" counters Unknown
                                            Input/output data rate is disabled.

                                          Loopback2 is up, line protocol is up
                                            Interface state transitions: 1
                                            Hardware is Loopback interface(s)
                                            Description: sdev-dmz-tr-north-loopback
                                            Internet address is 10.2.12.12/32
                                            MTU 1500 bytes, BW 0 Kbit
                                               reliability Unknown, txload Unknown, rxload Unknown
                                            Encapsulation Loopback,  loopback not set,
                                            Last input Unknown, output Unknown
                                            Last clearing of "show interface" counters Unknown
                                            Input/output data rate is disabled.

                                          Loopback3 is up, line protocol is up
                                            Interface state transitions: 1
                                            Hardware is Loopback interface(s)
                                            Description: sdev-guest-tr-north-loopback
                                            Internet address is 10.3.12.12/32
                                            MTU 1500 bytes, BW 0 Kbit
                                               reliability Unknown, txload Unknown, rxload Unknown
                                            Encapsulation Loopback,  loopback not set,
                                            Last input Unknown, output Unknown
                                            Last clearing of "show interface" counters Unknown
                                            Input/output data rate is disabled.

                                          Loopback4 is up, line protocol is up
                                            Interface state transitions: 1
                                            Hardware is Loopback interface(s)
                                            Description: sdev-net-tr-north-loopback
                                            Internet address is 10.4.12.12/32
                                            MTU 1500 bytes, BW 0 Kbit
                                               reliability Unknown, txload Unknown, rxload Unknown
                                            Encapsulation Loopback,  loopback not set,
                                            Last input Unknown, output Unknown
                                            Last clearing of "show interface" counters Unknown
                                            Input/output data rate is disabled.

                                          Loopback5 is up, line protocol is up
                                            Interface state transitions: 1
                                            Hardware is Loopback interface(s)
                                            Description: sdev-sndbx-tr-north-loopback
                                            Internet address is 10.5.12.12/32
                                            MTU 1500 bytes, BW 0 Kbit
                                               reliability Unknown, txload Unknown, rxload Unknown
                                            Encapsulation Loopback,  loopback not set,
                                            Last input Unknown, output Unknown
                                            Last clearing of "show interface" counters Unknown
                                            Input/output data rate is disabled.

                                          Null0 is up, line protocol is up
                                            Interface state transitions: 1
                                            Hardware is Null interface
                                            Internet address is Unknown
                                            MTU 1500 bytes, BW 0 Kbit
                                               reliability 255/255, txload Unknown, rxload Unknown
                                            Encapsulation Null,  loopback not set,
                                            Last input never, output never
                                            Last clearing of "show interface" counters never
                                            5 minute input rate 0 bits/sec, 0 packets/sec
                                            5 minute output rate 0 bits/sec, 0 packets/sec
                                               0 packets input, 0 bytes, 0 total input drops
                                               0 drops for unrecognized upper-level protocol
                                               Received 0 broadcast packets, 0 multicast packets
                                               0 packets output, 0 bytes, 0 total output drops
                                               Output 0 broadcast packets, 0 multicast packets

                                          MgmtEth0/0/CPU0/0 is up, line protocol is up
                                            Interface state transitions: 1
                                            Hardware is Management Ethernet, address is 0050.569a.baa8 (bia 0050.569a.baa8)
                                            Internet address is 10.130.18.12/22
                                            MTU 1514 bytes, BW 0 Kbit
                                               reliability 255/255, txload Unknown, rxload Unknown
                                            Encapsulation ARPA,
                                            Duplex unknown, 0Kb/s, unknown, link type is autonegotiation
                                            output flow control is off, input flow control is off
                                            Carrier delay (up) is 10 msec
                                            loopback not set,
                                            ARP type ARPA, ARP timeout 04:00:00
                                            Last input 00:00:00, output 00:00:00
                                            Last clearing of "show interface" counters never
                                            5 minute input rate 6000 bits/sec, 11 packets/sec
                                            5 minute output rate 9000 bits/sec, 2 packets/sec
                                               352183057 packets input, 23341507981 bytes, 0 total input drops
                                               0 drops for unrecognized upper-level protocol
                                               Received 18632966 broadcast packets, 324622389 multicast packets
                                                        0 runts, 0 giants, 0 throttles, 0 parity
                                               0 input errors, 0 CRC, 0 frame, 0 overrun, 0 ignored, 0 abort
                                               7958904 packets output, 6047105534 bytes, 0 total output drops
                                               Output 1 broadcast packets, 0 multicast packets
                                               0 output errors, 0 underruns, 0 applique, 0 resets
                                               0 output buffer failures, 0 output buffers swapped out
                                               1 carrier transitions

                                          GigabitEthernet0/0/0/0 is up, line protocol is up
                                            Interface state transitions: 1
                                            Hardware is GigabitEthernet, address is 0050.569a.5677 (bia 0050.569a.5677)
                                            Internet address is 192.168.33.12/24
                                            MTU 4484 bytes, BW 1000000 Kbit (Max: 1000000 Kbit)
                                               reliability 255/255, txload 0/255, rxload 0/255
                                            Encapsulation ARPA,
                                            Full-duplex, 1000Mb/s, unknown, link type is force-up
                                            output flow control is off, input flow control is off
                                            Carrier delay (up) is 10 msec
                                            loopback not set,
                                            ARP type ARPA, ARP timeout 04:00:00
                                            Last input 00:00:00, output 00:00:00
                                            Last clearing of "show interface" counters never
                                            5 minute input rate 14000 bits/sec, 15 packets/sec
                                            5 minute output rate 2000 bits/sec, 3 packets/sec
                                               594017986 packets input, 84979206308 bytes, 0 total input drops
                                               0 drops for unrecognized upper-level protocol
                                               Received 1406709 broadcast packets, 490004185 multicast packets
                                                        0 runts, 0 giants, 0 throttles, 0 parity
                                               0 input errors, 0 CRC, 0 frame, 0 overrun, 0 ignored, 0 abort
                                               150196835 packets output, 34873591859 bytes, 0 total output drops
                                               Output 12 broadcast packets, 49306994 multicast packets
                                               0 output errors, 0 underruns, 0 applique, 0 resets
                                               0 output buffer failures, 0 output buffers swapped out
                                               1 carrier transitions

                                          GigabitEthernet0/0/0/1 is up, line protocol is up
                                            Interface state transitions: 1
                                            Hardware is GigabitEthernet, address is 0050.569a.2e19 (bia 0050.569a.2e19)
                                            Internet address is Unknown
                                            MTU 1514 bytes, BW 1000000 Kbit (Max: 1000000 Kbit)
                                               reliability 255/255, txload 0/255, rxload 0/255
                                            Encapsulation ARPA,
                                            Full-duplex, 1000Mb/s, unknown, link type is force-up
                                            output flow control is off, input flow control is off
                                            Carrier delay (up) is 10 msec
                                            loopback not set,
                                            Last input 00:00:00, output 1y12w
                                            Last clearing of "show interface" counters never
                                            5 minute input rate 314000 bits/sec, 172 packets/sec
                                            5 minute output rate 0 bits/sec, 0 packets/sec
                                               6715089747 packets input, 1538780344512 bytes, 0 total input drops
                                               0 drops for unrecognized upper-level protocol
                                               Received 0 broadcast packets, 6715089747 multicast packets
                                                        0 runts, 0 giants, 0 throttles, 0 parity
                                               0 input errors, 0 CRC, 0 frame, 0 overrun, 0 ignored, 0 abort
                                               2560 packets output, 117760 bytes, 0 total output drops
                                               Output 2560 broadcast packets, 0 multicast packets
                                               0 output errors, 0 underruns, 0 applique, 0 resets
                                               0 output buffer failures, 0 output buffers swapped out
                                               1 carrier transitions

                                          GigabitEthernet0/0/0/1.10 is up, line protocol is up
                                            Interface state transitions: 1
                                            Hardware is VLAN sub-interface(s), address is 0050.569a.2e19
                                            Description: v10.sdev-core-tr-north
                                            Internet address is 172.12.10.1/29
                                            MTU 1518 bytes, BW 1000000 Kbit (Max: 1000000 Kbit)
                                               reliability 255/255, txload 0/255, rxload 0/255
                                            Encapsulation 802.1Q Virtual LAN, VLAN Id 10,  loopback not set,
                                            ARP type ARPA, ARP timeout 04:00:00
                                            Last input never, output never
                                            Last clearing of "show interface" counters never
                                            5 minute input rate 0 bits/sec, 0 packets/sec
                                            5 minute output rate 0 bits/sec, 0 packets/sec
                                               0 packets input, 0 bytes, 0 total input drops
                                               0 drops for unrecognized upper-level protocol
                                               Received 0 broadcast packets, 0 multicast packets
                                               0 packets output, 0 bytes, 0 total output drops
                                               Output 1 broadcast packets, 0 multicast packets

                                          GigabitEthernet0/0/0/1.11 is up, line protocol is up
                                            Interface state transitions: 1
                                            Hardware is VLAN sub-interface(s), address is 0050.569a.2e19
                                            Description: v11.sdev-core-tr-north
                                            Internet address is 172.12.10.9/29
                                            MTU 1518 bytes, BW 1000000 Kbit (Max: 1000000 Kbit)
                                               reliability 255/255, txload 0/255, rxload 0/255
                                            Encapsulation 802.1Q Virtual LAN, VLAN Id 11,  loopback not set,
                                            ARP type ARPA, ARP timeout 04:00:00
                                            Last input never, output never
                                            Last clearing of "show interface" counters never
                                            5 minute input rate 0 bits/sec, 0 packets/sec
                                            5 minute output rate 0 bits/sec, 0 packets/sec
                                               0 packets input, 0 bytes, 0 total input drops
                                               0 drops for unrecognized upper-level protocol
                                               Received 0 broadcast packets, 0 multicast packets
                                               0 packets output, 0 bytes, 0 total output drops
                                               Output 1 broadcast packets, 0 multicast packets

                                          GigabitEthernet0/0/0/1.12 is up, line protocol is up
                                            Interface state transitions: 1
                                            Hardware is VLAN sub-interface(s), address is 0050.569a.2e19
                                            Description: v12.sdev-core-tr-north
                                            Internet address is 172.12.10.17/29
                                            MTU 1518 bytes, BW 1000000 Kbit (Max: 1000000 Kbit)
                                               reliability 255/255, txload 0/255, rxload 0/255
                                            Encapsulation 802.1Q Virtual LAN, VLAN Id 12,  loopback not set,
                                            ARP type ARPA, ARP timeout 04:00:00
                                            Last input never, output never
                                            Last clearing of "show interface" counters never
                                            5 minute input rate 0 bits/sec, 0 packets/sec
                                            5 minute output rate 0 bits/sec, 0 packets/sec
                                               0 packets input, 0 bytes, 0 total input drops
                                               0 drops for unrecognized upper-level protocol
                                               Received 0 broadcast packets, 0 multicast packets
                                               0 packets output, 0 bytes, 0 total output drops
                                               Output 1 broadcast packets, 0 multicast packets

                                          GigabitEthernet0/0/0/1.13 is up, line protocol is up
                                            Interface state transitions: 1
                                            Hardware is VLAN sub-interface(s), address is 0050.569a.2e19
                                            Description: v13.sdev-core-tr-north
                                            Internet address is 172.12.10.25/29
                                            MTU 1518 bytes, BW 1000000 Kbit (Max: 1000000 Kbit)
                                               reliability 255/255, txload 0/255, rxload 0/255
                                            Encapsulation 802.1Q Virtual LAN, VLAN Id 13,  loopback not set,
                                            ARP type ARPA, ARP timeout 04:00:00
                                            Last input never, output never
                                            Last clearing of "show interface" counters never
                                            5 minute input rate 0 bits/sec, 0 packets/sec
                                            5 minute output rate 0 bits/sec, 0 packets/sec
                                               0 packets input, 0 bytes, 0 total input drops
                                               0 drops for unrecognized upper-level protocol
                                               Received 0 broadcast packets, 0 multicast packets
                                               0 packets output, 0 bytes, 0 total output drops
                                               Output 1 broadcast packets, 0 multicast packets

                                          GigabitEthernet0/0/0/1.14 is up, line protocol is up
                                            Interface state transitions: 1
                                            Hardware is VLAN sub-interface(s), address is 0050.569a.2e19
                                            Description: v14.sdev-core-tr-north
                                            Internet address is 172.12.10.33/29
                                            MTU 1518 bytes, BW 1000000 Kbit (Max: 1000000 Kbit)
                                               reliability 255/255, txload 0/255, rxload 0/255
                                            Encapsulation 802.1Q Virtual LAN, VLAN Id 14,  loopback not set,
                                            ARP type ARPA, ARP timeout 04:00:00
                                            Last input never, output never
                                            Last clearing of "show interface" counters never
                                            5 minute input rate 0 bits/sec, 0 packets/sec
                                            5 minute output rate 0 bits/sec, 0 packets/sec
                                               0 packets input, 0 bytes, 0 total input drops
                                               0 drops for unrecognized upper-level protocol
                                               Received 0 broadcast packets, 0 multicast packets
                                               0 packets output, 0 bytes, 0 total output drops
                                               Output 1 broadcast packets, 0 multicast packets

                                          GigabitEthernet0/0/0/1.2569 is up, line protocol is up
                                            Interface state transitions: 1
                                            Hardware is VLAN sub-interface(s), address is 0050.569a.2e19
                                            Description: v2569.sdev-wifi-ut-west
                                            Internet address is 172.12.89.249/29
                                            MTU 1518 bytes, BW 1000000 Kbit (Max: 1000000 Kbit)
                                               reliability 255/255, txload 0/255, rxload 0/255
                                            Encapsulation 802.1Q Virtual LAN, VLAN Id 2569,  loopback not set,
                                            ARP type ARPA, ARP timeout 04:00:00
                                            Last input never, output never
                                            Last clearing of "show interface" counters never
                                            5 minute input rate 0 bits/sec, 0 packets/sec
                                            5 minute output rate 0 bits/sec, 0 packets/sec
                                               0 packets input, 0 bytes, 0 total input drops
                                               0 drops for unrecognized upper-level protocol
                                               Received 0 broadcast packets, 0 multicast packets
                                               0 packets output, 0 bytes, 0 total output drops
                                               Output 1 broadcast packets, 0 multicast packets

                                          GigabitEthernet0/0/0/2 is administratively down, line protocol is administratively down
                                            Interface state transitions: 0
                                            Hardware is GigabitEthernet, address is 0050.569a.9db6 (bia 0050.569a.9db6)
                                            Internet address is Unknown
                                            MTU 1514 bytes, BW 1000000 Kbit (Max: 1000000 Kbit)
                                               reliability 255/255, txload 0/255, rxload 0/255
                                            Encapsulation ARPA,
                                            Full-duplex, 1000Mb/s, unknown, link type is force-up
                                            output flow control is off, input flow control is off
                                            Carrier delay (up) is 10 msec
                                            loopback not set,
                                            Last input never, output never
                                            Last clearing of "show interface" counters never
                                            5 minute input rate 0 bits/sec, 0 packets/sec
                                            5 minute output rate 0 bits/sec, 0 packets/sec
                                               0 packets input, 0 bytes, 0 total input drops
                                               0 drops for unrecognized upper-level protocol
                                               Received 0 broadcast packets, 0 multicast packets
                                                        0 runts, 0 giants, 0 throttles, 0 parity
                                               0 input errors, 0 CRC, 0 frame, 0 overrun, 0 ignored, 0 abort
                                               0 packets output, 0 bytes, 0 total output drops
                                               Output 0 broadcast packets, 0 multicast packets
                                               0 output errors, 0 underruns, 0 applique, 0 resets
                                               0 output buffer failures, 0 output buffers swapped out
                                               0 carrier transitions

                                          GigabitEthernet0/0/0/3 is administratively down, line protocol is administratively down
                                            Interface state transitions: 0
                                            Hardware is GigabitEthernet, address is 0050.569a.01d3 (bia 0050.569a.01d3)
                                            Internet address is Unknown
                                            MTU 1514 bytes, BW 1000000 Kbit (Max: 1000000 Kbit)
                                               reliability 255/255, txload 0/255, rxload 0/255
                                            Encapsulation ARPA,
                                            Full-duplex, 1000Mb/s, unknown, link type is force-up
                                            output flow control is off, input flow control is off
                                            Carrier delay (up) is 10 msec
                                            loopback not set,
                                            Last input never, output never
                                            Last clearing of "show interface" counters never
                                            5 minute input rate 0 bits/sec, 0 packets/sec
                                            5 minute output rate 0 bits/sec, 0 packets/sec
                                               0 packets input, 0 bytes, 0 total input drops
                                               0 drops for unrecognized upper-level protocol
                                               Received 0 broadcast packets, 0 multicast packets
                                                        0 runts, 0 giants, 0 throttles, 0 parity
                                               0 input errors, 0 CRC, 0 frame, 0 overrun, 0 ignored, 0 abort
                                               0 packets output, 0 bytes, 0 total output drops
                                               Output 0 broadcast packets, 0 multicast packets
                                               0 output errors, 0 underruns, 0 applique, 0 resets
                                               0 output buffer failures, 0 output buffers swapped out
                                               0 carrier transitions

                                          GigabitEthernet0/0/0/4 is administratively down, line protocol is administratively down
                                            Interface state transitions: 0
                                            Hardware is GigabitEthernet, address is 0050.569a.caf1 (bia 0050.569a.caf1)
                                            Internet address is Unknown
                                            MTU 1514 bytes, BW 1000000 Kbit (Max: 1000000 Kbit)
                                               reliability 255/255, txload 0/255, rxload 0/255
                                            Encapsulation ARPA,
                                            Full-duplex, 1000Mb/s, unknown, link type is force-up
                                            output flow control is off, input flow control is off
                                            Carrier delay (up) is 10 msec
                                            loopback not set,
                                            Last input never, output never
                                            Last clearing of "show interface" counters never
                                            5 minute input rate 0 bits/sec, 0 packets/sec
                                            5 minute output rate 0 bits/sec, 0 packets/sec
                                               0 packets input, 0 bytes, 0 total input drops
                                               0 drops for unrecognized upper-level protocol
                                               Received 0 broadcast packets, 0 multicast packets
                                                        0 runts, 0 giants, 0 throttles, 0 parity
                                               0 input errors, 0 CRC, 0 frame, 0 overrun, 0 ignored, 0 abort
                                               0 packets output, 0 bytes, 0 total output drops
                                               Output 0 broadcast packets, 0 multicast packets
                                               0 output errors, 0 underruns, 0 applique, 0 resets
                                               0 output buffer failures, 0 output buffers swapped out
                                               0 carrier transitions

                                          """;

    #endregion
}