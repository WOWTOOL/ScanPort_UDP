using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ScanPort_UDP
{
    class Program
    {
        public enum TypeEnum {
            TCP,
            UDP,

        }
        public static bool bDebug = false;

        public static TypeEnum ScanType;
        public static string StrScanType;
        public static string UserInput;
        public static string[] UserInputArray;
        public static string sIPAddress;
        public static string sStartEnd;
        public static string[] StartEndArray;
        public static int iStart, iEnd;

        public static bool VerifyInput() {
   

            if (UserInput.Length == 0)
            {
                Console.WriteLine("Input scan type, eg: TCP:192.168.1.1:80-88 or  UDP:192.168.1.1:80-88");
                return false;
            }
            //verify user input
            UserInputArray = UserInput.Split(':');
            if (UserInputArray.Length != 3)
            {
                Console.WriteLine("format error, please Input scan type, eg: TCP:192.168.1.1:80-88 or  UDP:192.168.1.1:80-88");
                return false;
            }

            //get parameters
            StrScanType = UserInputArray[0];
            sIPAddress = UserInputArray[1];
            sStartEnd = UserInputArray[2];

            switch (StrScanType)
            {
                case "TCP": ScanType = TypeEnum.TCP; break;
                case "UDP": ScanType = TypeEnum.UDP; break;
                default:
                    Console.WriteLine("format error, only accept TCP/UDP");
                    return false;
                    break;
            }
            //start end port
            StartEndArray = sStartEnd.Split('-');
            if (StartEndArray.Length != 2)
            {
                Console.WriteLine("format error, port must use format like 80-88");
                return false;
            }
            iStart = int.Parse(StartEndArray[0]);
            iEnd = int.Parse(StartEndArray[1]);

            if (iStart < 1)
            {
                Console.WriteLine("start port must >1");
                return false;
            }
            if (iEnd > 65536)
            {
                Console.WriteLine("end port must <65535");
                return false;
            }
            if (iEnd < iStart)
            {
                Console.WriteLine("end port must > start port");
                return false;
            }
            return true;
        }
        static void Main(string[] args)
        {

            //get user input
            if (bDebug)
            {
                //UserInput = "UDP:144.0.3.171:21-33";
                UserInput = "TCP:144.0.3.171:21-33";
            }
            else
            {
                Console.WriteLine("Input scan type, eg: TCP:192.168.1.1:80-88 or  UDP:192.168.1.1:80-88");
                UserInput = Console.ReadLine();
            }

            while (!VerifyInput())
            {
                UserInput = Console.ReadLine();
            }

            for (int i = iStart; i <= iEnd; i++)
            {
                switch (ScanType)
                {
                    case TypeEnum.UDP: {
                            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(sIPAddress), i);
                            UdpClient server;
                            byte[] data;
                            try
                            {
                                server = new UdpClient();
                                server.Client.ReceiveTimeout = 10;
                                server.Connect(ipe);
                                data = new byte[1024];
                                server.Send(data, data.Length);
                                data = server.Receive(ref ipe);
                                Console.WriteLine(ipe + "  open");
                            }
                            catch
                            {
                                Console.WriteLine(ipe + "  Close");
                            }

                        }
                        break;
                    case TypeEnum.TCP:
                        {
                            IPEndPoint ipetcp = new IPEndPoint(IPAddress.Parse(sIPAddress), i);
                            Socket servertcp;
                            byte[] datatcp;
                            try
                            {
                                servertcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                servertcp.ReceiveTimeout = 200;
                                servertcp.Connect(ipetcp);
                                datatcp = new byte[1024];
                                //string message2 = textBox2.Text;
                                //tcpClient.Send(Encoding.UTF8.GetBytes(message2));
                                servertcp.Send(datatcp);
                                int length = servertcp.Receive(datatcp);
                                string message = Encoding.UTF8.GetString(datatcp, 0, length);//只将接收到的数据进行转化
                                Console.WriteLine(ipetcp + " " + message);
                            }
                            catch
                            {
                                Console.WriteLine(ipetcp + "  Close");
                            }

                        }
                        break;
                    default:
                        Console.WriteLine("format error, only accept TCP/UDP");
                        return;
                        break;
                }
            }
            Console.ReadKey();
        }
    }
}
