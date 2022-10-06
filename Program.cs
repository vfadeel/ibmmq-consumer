using System;
using IBM.XMS;

namespace IBMMQ
{
    class Program
    {
        static void Main(string[] args)
        {

            XMSFactoryFactory factory = XMSFactoryFactory.GetInstance(XMSC.CT_WMQ);

            IConnectionFactory connectionFactory = factory.CreateConnectionFactory();

            connectionFactory.SetStringProperty(XMSC.WMQ_HOST_NAME, "localhost");
            connectionFactory.SetIntProperty(XMSC.WMQ_PORT, 1414);
            connectionFactory.SetStringProperty(XMSC.WMQ_CHANNEL, "DEV.APP.SVRCONN");
            connectionFactory.SetIntProperty(XMSC.WMQ_CONNECTION_MODE, XMSC.WMQ_CM_CLIENT);
            connectionFactory.SetStringProperty(XMSC.WMQ_QUEUE_MANAGER, "QM1");

            IConnection connection = connectionFactory.CreateConnection();

            using (var session = connection.CreateSession(false, AcknowledgeMode.AutoAcknowledge))
            {
                IDestination fila = session.CreateQueue("TESTEDOVITOR");

                IMessageConsumer consumer = session.CreateConsumer(fila);

                //consumer.MessageListener = new MessageListener(OnMessageReceive);

                connection.Start();

                //while(true){}


                try
                {

                    while (true)
                    {

                        IMessage mensagem = consumer.Receive(30 * 1000);

                    }

                }
                catch (Exception e)
                {
                    connection.Close();
                    throw e;
                }

            }


        }


        public static void OnMessageReceive(IMessage msg)
        {
            string texto = msg.ToString();
        }

        
        public static void OnMessageException(Exception msg)
        {
            throw new Exception("aaaa");
        }


        // static void Main(string[] args)
        // {

        //     Hashtable connectionProperties = new Hashtable();
        //     connectionProperties.Add(MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED);
        //     connectionProperties.Add(MQC.HOST_NAME_PROPERTY, "localhost");
        //     connectionProperties.Add(MQC.PORT_PROPERTY, 1414);
        //     connectionProperties.Add(MQC.CHANNEL_PROPERTY, "DEV.APP.SVRCONN");
        //     //connectionProperties.Add(MQC.USER_ID_PROPERTY, "vitor");
        //     //connectionProperties.Add(MQC.PASSWORD_PROPERTY, "");

        //     //connectionProperties.Add("QueueName", "QM1");
        //     connectionProperties.Add(MQC.USE_MQCSP_AUTHENTICATION_PROPERTY, true);
        //     using (MQQueueManager queueManager = new MQQueueManager("QM1", connectionProperties))
        //     {
        //         var queue = queueManager.AccessQueue("TESTEDOVITOR", MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_FAIL_IF_QUIESCING);

        //         MQMessage message = new MQMessage();

        //         queue.Get(message);

        //         string mensagem = message.ReadString(message.MessageLength);
        //     }

        //     Console.WriteLine("Hello World!");
        // }
    }
}
