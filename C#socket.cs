using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using socketServer.Codes;

namespace socketServer
{
    //这个类用于控制sock的发送和接受信息的过程
   public  partial class theServer
    {

        private string IP = "-------";//默认IP地址，可以换

        private static int myProt = 8886;   //端口  

        static Socket serverSocket = null;//保留服务器的socket引用
        private List<Socket> clientSockets = new List<Socket>();//客户端的socket连接

        private Thread theServerThread;//服务器主线程
        private List<Thread> theClientThreads = new List<Thread>();//用于保留客户端的线程引用，用于关闭之

        private bool opened = true;//是否开启
        private int mode = 1;//1 单人使用 2 多人使用


        public bool Opened//opened状态只读，可查询
        {
            get{ return opened;}
        }
        //这个适用于单个客户端的情况
        information theInformationController = null; //信息处理控制单元，必须要有，这个才是核心
        //这个适用于多个客户端的情况
        List<information> theInformationControllers;//信息处理单元组
        private Protocol theProtocolController;//协议解析单元
        List<string> theIPS;//用字符串IP作为区分

        public theServer(string theIPUse = "", int port = 8886 )
        {
            if (SystemSave.theServrForAll != null)
                return;
            try
            {
                theInformationControllers = new List<socketServer.information>();
                theIPS = new List<string>();
                theInformationController = new socketServer.information();
                theProtocolController = new socketServer.Protocol();
                //设定IP地址的策略
                //首先看传入的IPUse是不是空，如果不是就用IPUse
                //如果IPUse为空，就是用SystemSave的IP，并且这个可以在设置面板设置
                //实际上SystemSave的IP在是真正需要使用的IP，theIPUse 只是一个扩展的功能
                if (string.IsNullOrEmpty(theIPUse) == false)
                {
                    IP = theIPUse;
                    myProt = port;
                }
                else
                {
                    IP = SystemSave.serverIP;
                    myProt = SystemSave.serverPort;
                }
                //IPAddress ip = IPAddress.Parse(IP);
                // serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口  
                // serverSocket.Listen(10);    //设定最多10个排队连接请求  
                // MessageBox.Show("启动监听" + serverSocket.LocalEndPoint.ToString() + "成功" + "\ntype: server");
                //通过Clientsoket发送数据  

                //不论哪一种模式，实际上server都是唯一的一个，至于分开，都是从内部开始的线程之间的问题
                SystemSave.theServrForAll = this;
            }
            catch(Exception E)
            {
                Log.saveLog(LogType.error, "Server 构建出错:" + E.Message);
                serverSocket = null;
                SystemSave.theServrForAll = null;
            }
        }

        //构造方法，用于建立服务器的对象，但是并不是立即开启
        public theServer( information theInformationControllerIn ,  string theIPUse = "", int port = 8886)
        {
            try
            {
                theInformationController = theInformationControllerIn;
                theProtocolController = new socketServer.Protocol();
                //设定IP地址的策略
                //首先看传入的IPUse是不是空，如果不是就用IPUse
                //如果IPUse为空，就是用SystemSave的IP，并且这个可以在设置面板设置
                //实际上SystemSave的IP在是真正需要使用的IP，theIPUse 只是一个扩展的功能
                if (string .IsNullOrEmpty (theIPUse) == false)
                { 
                    IP = theIPUse;
                    myProt = port;
                }
                else
                {
                    IP = SystemSave.serverIP;
                    myProt = SystemSave .serverPort;
                }
                //IPAddress ip = IPAddress.Parse(IP);
               // serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
               // serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口  
               // serverSocket.Listen(10);    //设定最多10个排队连接请求  
                // MessageBox.Show("启动监听" + serverSocket.LocalEndPoint.ToString() + "成功" + "\ntype: server");
                //通过Clientsoket发送数据  
            }
            catch
            {
                serverSocket = null;
            }
           
        }
        //如果使用别的方法可以修改mode但是默认是不需要的
        //修改mode需要在start之前完成
        public void setMode(int modeIn = 1)
        {
            mode = modeIn;
        }



        //开启服务器的接收内容，实际上是开启上层的消息接收内容
        //专门开一个新的线程用于管理这些信息的接收
        //此外，由于消息内容推送的速度问题，有一些灵活的存储方法似乎不能使用
        public string startTheServer(bool useSystemSave = false)
       {
           //如果socket没有建立，或者没有总控单元，那么所有工作都不会开始
           if ( theInformationController == null)
                return "服务器无法开启，可能是初始化没有做好";

            if (useSystemSave)
            {
                IP = SystemSave.serverIP;
                myProt = SystemSave.serverPort;
            }
            IPAddress ip = IPAddress.Parse(IP);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Console.WriteLine("ServerIP is " + ip);
            serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口  
            serverSocket.Listen(10);    //设定最多10个排队连接请求  
            opened = true;//标记，用与表示开启

            theServerThread = new Thread(ListenClientConnect);//新建服务器主要线程
            theServerThread .Start();//真正开启服务器
  
            return "服务器正式启动(端口:"+myProt +"  IP:"+IP+")";
        }

        public string closeServer()
        {
            //如果socket没有建立，或者没有总控单元，那么所有工作都不会开始
            if (serverSocket == null || theInformationController == null )
            {
               // Log.saveLog(LogType.information, "Server 已经关闭，不必再次关闭" );
                return "socket关闭失败，丢失引用或者根本就没开启";
            }
            //关掉客户端线程
            for (int i = 0; i < theClientThreads.Count; i++)
            {
                if (theClientThreads[i] == null)
                    continue;
                theClientThreads[i].Abort();
            }
            theClientThreads.Clear();
            //关掉服务器线程和socket
            serverSocket.Close();
            serverSocket = null; //防止多次关闭
            theServerThread.Abort();

            //关闭所有的客户端socket
            for (int i = 0; i < clientSockets.Count; i++ )
            {
                if (clientSockets[i] == null)
                    continue;
                try
                {
                    clientSockets[i].Shutdown(SocketShutdown.Both);
                    clientSockets[i].Close();
                    clientSockets[i] = null;
                }
                catch(Exception E)
                {
                    Log.saveLog(LogType.error, "Server 关闭出错: " + E.Message);
                    // Console.WriteLine("srever socket 不必重复删除");
                }
            }
            clientSockets.Clear();
           
            opened = false;

            Log.saveLog(LogType.information, "Serve已经关闭");
            return "服务器socket关闭成功";
        }
         
        //用于接收信息的线程方法
        private void ListenClientConnect()
        {
            //Console.WriteLine("server Mode = " + mode);
            //Console.WriteLine("opened = "+ opened);
            while (opened)
            {
                Log.saveLog(LogType.information, "Server以"+ mode+ "形式开始侦听");
                Console.WriteLine("Server started with mode " + mode);
                try
                {
                    //如果接受了一个新的连接
                    Socket clientSocket = serverSocket.Accept();
                    //尝试发送验证信息
                    //clientSocket.Send(Encoding.ASCII.GetBytes("Server Say Hello"));
                    //开启接收这个客户端的线程的方法
                    if (mode == 1)
                    {
                        Thread receiveThread = new Thread(ReceiveMessage);
                        theClientThreads.Add(receiveThread);//保留这个客户端连接的引用
                        clientSockets.Add(clientSocket);//保留socket引用
                        receiveThread.Start(clientSocket);
                    }
                    else if (mode == 2)
                    {
                        Thread receiveThread = new Thread(ReceiveMessage2);
                        theClientThreads.Add(receiveThread);//保留这个客户端连接的引用
                        clientSockets.Add(clientSocket);//保留socket引用
                        receiveThread.Start(clientSocket);
                    }
                  
                }
                catch(Exception E)
                {
                    Log.saveLog(LogType.error, "Server 出错： " + E.Message+" 已经强制关闭");
                    Console.WriteLine("Server is closed with error");
                    //如果服务器崩了，就直接关闭
                    closeServer();
                    break;
                }
            }
        }

        //socket获取的字符串统一处理方法
        //非常关键的复用方法
        //这个方法里面包含最基本的协议头部分析和处理
        //这是对获得的数据的第一次解析，用来分析用什么模块来处理这些数据并做简单的处理\
        private void getInformationWithOperate(byte[] result, Socket myClientSocket, information theInformationController , MainWindow theMainWindowForOperate = null)
        {
            //通过clientSocket接收数据  
            int receiveNumber = myClientSocket.Receive(result);
            // MessageBox.Show("接收客户端" + myClientSocket.RemoteEndPoint.ToString() + "\n消息" + Encoding.ASCII.GetString(result, 0, receiveNumber) + "\ntype: server");
            //手机和PC的编码方法需要一样，否则诡异的乱码可能会出现
            string information = Encoding.UTF8.GetString(result, 0, receiveNumber).ToString();
            //第一个标记为title题头，象征着操作标记
            //这是一个多段标记，标记头以“+”
            //初步解析，解析出来标记用以分类，看看用哪一种具体的操作
            string theInformationTitle = theProtocolController.getInformaitonTitle(information);
            Console.WriteLine("switch title => "+ theInformationTitle);
            switch (theInformationTitle)
            {
                //正常数据以smartPhoneData作为报文头部
                case "clientData":
                    {
                        //接纳和处理信息
                        theProtocolController.getandMakeInformation(information, theInformationController);
                        string sendString = theProtocolController.makeSendToClients();
                        myClientSocket.Send(Encoding.UTF8.GetBytes(sendString));//发送一个步数信息
                    }
                    break;
                //离开的数据以bye作为报文头部
                case "bye":
                    {
                        myClientSocket.Shutdown(SocketShutdown.Both);
                        myClientSocket.Close();
                        return;//，这层死循环可以结束了
                    }
                //    break; //这个break永远不会被调用
                //获取数据用get作为报头
                case "get":
                    {
                        //3D显示客户端的需要
                        string sendString = theProtocolController.makeSendToClients();
                        myClientSocket.Send(Encoding.UTF8.GetBytes(sendString));//发送一个步数信息
                    }
                    
                    break;
                //如果是客户端对服务端的操作
                case "operate":
                    {
                        if (theMainWindowForOperate == null)
                        {
                            Console.WriteLine("There is no main window to operate ,so operate fail");
                            return;
                        }
                        Console.WriteLine("make operate for server with client's command");
                        Console.WriteLine("the command is " + information);
                        myClientSocket.Send(Encoding.UTF8.GetBytes("the server has got the command"));//发送一个步数信息
                        theProtocolController.clientOperateServer(information, theMainWindowForOperate);
                    }
                    break;
                //默认操作其实也就是强制性关闭
                //其他各种情况，暂时都认为是不合法的
                default:
                    {
                        Console.WriteLine("receive wrong title " + theInformationTitle);
                        Console.WriteLine("the whole information is " + information);
                        myClientSocket.Shutdown(SocketShutdown.Both);
                        myClientSocket.Close();
                    }
                    break;
            }
        }


        private void ReceiveMessage(object clientSocket)
        {
            //获取到发送消息的客户端socket引用
            //专门用来接收这个客户端的信息的线程
            Socket myClientSocket = (Socket)clientSocket;
            //用于接收的内容的缓冲
            //每个线程分配一个缓冲区，主要是怕冲突问题
             byte[] result = new byte[SystemSave.lengthForBuffer];
            while (true)
            {
                try
                {
                    getInformationWithOperate(result , myClientSocket , theInformationController , SystemSave.theMainWindowForSingle); /////////////////
                }
                catch(Exception E) //如果发送信息居然失败了，就关掉这个客户端连接
                 {
                    Log.saveLog(LogType.error, "传送信息失败 socket已经关闭");
                    Console.WriteLine("传送信息失败\n这个socket已经关闭");
                    Console.WriteLine("错误信息：\n" + E.Message);
                    //myClientSocket.Shutdown(SocketShutdown.Receive);
                    myClientSocket.Close();
                return;
                }
           }
        }


      

        public delegate MainWindow showNewMainWindow(information theInformationController);
        public MainWindow showMainWindow(information theInformationController)
        {
            MainWindow aNewMainWindow = new MainWindow();
            aNewMainWindow.makeStart(theInformationController, false);
            //在这里对应不同客户端窗口的信息title(可以扩展一下)
            aNewMainWindow.Title = "Dead Reckoning System Main Window (No. " + theInformationControllers.Count+")";
            aNewMainWindow.Show();
            return aNewMainWindow;
        }
        public delegate void closeMainWindow(MainWindow theMainWindow);
        public void closetheMainWindow(MainWindow theMainWindow)
        {
            theMainWindow.Close();
        }


        //分割线------------------------------------------------------------------
        //多客户端的情况
        private void ReceiveMessage2(object clientSocket)
        {
            information theInformationController = new socketServer.information() ;
            theInformationControllers.Add(theInformationController);
            //有关窗口UI等等的内容需要在一些特殊的线程里面处理
            MainWindow theMainWindowForthisClient =  (MainWindow)System.Windows.Application.Current.Dispatcher.Invoke(System.Windows .Threading.DispatcherPriority.Normal , new showNewMainWindow(showMainWindow),theInformationController);
            Console.WriteLine("a new client");
            Log.saveLog(LogType.information, "一个新的实验用户到来，开启新窗口");
            //获取到发送消息的客户端socket引用
            //专门用来接收这个客户端的信息的线程
            Socket myClientSocket = (Socket)clientSocket;
            //用于接收的内容的缓冲
            //每个线程分配一个缓冲区，主要是怕冲突问题
            byte[] result = new byte[SystemSave.lengthForBuffer];
            while (true)
            {
                try
                {
                    getInformationWithOperate(result, myClientSocket, theInformationController , theMainWindowForthisClient);
                }
                catch(Exception E) //如果发送信息居然失败了，就关掉这个客户端连接
                {
                    Log.saveLog(LogType.error, "传送信息失败: "+ E.Message);
                    Console.WriteLine("传送信息失败");
                    Console.WriteLine("错误信息：\n" + E.Message);
                    //myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                return;
                }
            }
        }

    }
}
