using System;
using System.Collections;
using IBM.WMQ;

namespace IBMMQ
{
    public static class ProgramMQ
    {
        //Ao contrário do método que está na main, esse modelo utiliza a biblioteca IBM.WMQ que é a biblioteca 
        //nativa da IBM. 
        static void Consumidor()
        {

            Hashtable connectionProperties = new Hashtable();
            connectionProperties.Add(MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED);
            connectionProperties.Add(MQC.HOST_NAME_PROPERTY, "localhost");
            connectionProperties.Add(MQC.PORT_PROPERTY, 1414);
            connectionProperties.Add(MQC.CHANNEL_PROPERTY, "DEV.APP.SVRCONN");

            connectionProperties.Add(MQC.USE_MQCSP_AUTHENTICATION_PROPERTY, true);
            using (MQQueueManager queueManager = new MQQueueManager("QM1", connectionProperties))
            {
                var queue = queueManager.AccessQueue("TESTEDOVITOR", MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_FAIL_IF_QUIESCING);
                    
                MQMessage message = new MQMessage();

                queue.Get(message);

                string mensagem = message.ReadString(message.MessageLength);
            }

        }
    }
}
