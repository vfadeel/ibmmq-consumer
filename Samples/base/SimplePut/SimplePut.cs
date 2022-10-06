/* @(#) MQMBID sn=p930-L220607.DE su=_T1YMxuZZEeywe89ziwFDLA pn=basedotnet/samples/cs/core/base/SimplePut/SimplePut.cs                                               */
/*********************************************************************/
/*   <copyright                                                      */
/*   notice="lm-source-program"                                      */
/*   pids="5724-H72,"                                                */
/*   years="2018,2020"                                               */
/*   crc="2787562084" >                                              */
/*   Licensed Materials - Property of IBM                            */
/*                                                                   */
/*   5724-H72,                                                       */
/*                                                                   */
/*   (C) Copyright IBM Corp. 2018, 2020 All Rights Reserved.         */
/*                                                                   */
/*   US Government Users Restricted Rights - Use, duplication or     */
/*   disclosure restricted by GSA ADP Schedule Contract with         */
/*   IBM Corp.                                                       */
/*   </copyright>                                                    */
/*********************************************************************/
/*                                                                   */
/* A simple application to demonstrate putting of messages.          */
/*                                                                   */
/* Notes: The application can be used to put messages to queue       */
/*                                                                   */
/* Usage:dotnet SimplePut -q queueName -k keyRepository -s cipherSpec*/
/*         [-h host -p port -l channel -n numberOfMsgs               */
/*	   -dn sslPeerName -kr keyResetCount -cr sslCertRevocationCheck] */
/*                                                                   */
/* - queueName      : name of a queue                                */
/* - keyRepository  : can be *SYSTEM or *USER                        */
/* - cipherSpec     : CipherSpec value.                              */
/* - host           : hostname                                       */
/* - port           : port number                                    */
/* - channel        : connection channel                             */
/* - numberOfMsgs   : number of messages                             */
/* - sslPeerName    : distinguished name of the server certificate   */
/* - keyResetCount  : KeyResetCount value                            */
/* - sslCertRevocationCheck  : Enable Certificate Revocation Check   */
/*                                                                   */
/*  "keyRepository" and "cipherSpec" values are required only for    */
/*	SSL connection.                                                  */
/*                                                                   */
/* Provider type: IBM MQ                                             */
/*                                                                   */
/*                                                                   */
/*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IBM.WMQ;

namespace SimplePut
{

    /// <summary>
    /// Summary description for SimplePut.
    /// </summary>
    class SimplePut
    {
        /// <summary>
        /// Dictionary to store the properties
        /// </summary>
        private IDictionary<string, object> properties = null;
        /// <summary>
        /// Expected command-line arguments
        /// </summary>
        private String[] cmdArgs = { "-q", "-h", "-p", "-l", "-n", "-k", "-s", "-dn", "-kr", "-cr" };
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            Console.WriteLine("Start of SimplePut Application\n");
            try
            {
                SimplePut simplePut = new SimplePut
                {
                    properties = new Dictionary<string, object>()
                };

                if (simplePut.ParseCommandline(args))
                    simplePut.PutMessages();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Invalid arguments!\n{0}", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught: {0}", ex);
                Console.WriteLine("Sample execution FAILED!");
            }

            Console.WriteLine("End of SimplePut Application");
        }

        /// <summary>
        /// Parse commandline parameters
        /// Usage: SimplePut -q queueName [-h host -p port -l channel -n numberOfMsgs]
        /// </summary>
        /// <param name="args"></param>
        bool ParseCommandline(string[] args)
        {
            try
            {
                if (args.Length < 2 || args.Length % 2 == 1)
                {
                    DisplayHelp();
                    return false;
                }
                var cmdlineArguments = Enumerable.Range(0, args.Length / 2).ToDictionary(i => args[2 * i], i => args[2 * i + 1]);

                if (!cmdlineArguments.Keys.All(x => cmdArgs.Contains(x)))
                {
                    DisplayHelp();
                    return false;
                }

                // set the properties
                properties.Add(MQC.HOST_NAME_PROPERTY, cmdlineArguments.ContainsKey("-h") ? cmdlineArguments["-h"] : "localhost");
                properties.Add(MQC.PORT_PROPERTY, cmdlineArguments.ContainsKey("-p") ? Convert.ToInt32(cmdlineArguments["-p"]) : 1414);
                properties.Add(MQC.CHANNEL_PROPERTY, cmdlineArguments.ContainsKey("-l") ? (cmdlineArguments["-l"]) : "SYSTEM.DEF.SVRCONN");
                properties.Add(MQC.SSL_CERT_STORE_PROPERTY, cmdlineArguments.ContainsKey("-k") ? cmdlineArguments["-k"] : "");
                properties.Add(MQC.SSL_CIPHER_SPEC_PROPERTY, cmdlineArguments.ContainsKey("-s") ? cmdlineArguments["-s"] : "");
                properties.Add(MQC.SSL_PEER_NAME_PROPERTY, cmdlineArguments.ContainsKey("-dn") ? cmdlineArguments["-dn"] : "");
                properties.Add(MQC.SSL_RESET_COUNT_PROPERTY, cmdlineArguments.ContainsKey("-kr") ? Convert.ToInt32(cmdlineArguments["-kr"]) : 0);
                properties.Add("QueueName", cmdlineArguments["-q"]);
                properties.Add("MessageCount", cmdlineArguments.ContainsKey("-n") ? Convert.ToInt32(cmdlineArguments["-n"]) : 1);
                properties.Add("sslCertRevocationCheck", cmdlineArguments.ContainsKey("-cr") ? Convert.ToBoolean(cmdlineArguments["-cr"]) : false);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught while parsing command line arguments: " + e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Display Help
        /// </summary>
        void DisplayHelp()
        {
            Console.WriteLine("Usage: SimplePut -q queueName -k keyRepository -s cipherSpec [-h host -p port -l channel -n numberOfMsgs -dn sslPeerName -kr keyResetCount -cr sslCertRevocationCheck]");
            Console.WriteLine("- queueName    : a queue name");
            Console.WriteLine("- keyRepository  : can be *SYSTEM or *USER");
            Console.WriteLine("- cipherSpec     : cipherSpec like TLS_RSA_WITH_AES_128_CBC_SHA");
            Console.WriteLine("- host         : hostname like 192.122.178.78. Default hostname is localhost");
            Console.WriteLine("- port         : port number like 3555. Default port is 1414");
            Console.WriteLine("- channel      : connection channel. Default is SYSTEM.DEF.SVRCONN");
            Console.WriteLine("- numberOfMsgs : The number of messages. Default is 1");
            Console.WriteLine("- sslPeerName    : distinguished name of the server certificate");
            Console.WriteLine("- keyResetCount  : KeyResetCount value.");
            Console.WriteLine("- sslCertRevocationCheck    : can be true or false");
            Console.WriteLine("Ex: SimplePut -q QA -k *SYSTEM -s TLS_RSA_WITH_AES_128_CBC_SHA");
            Console.WriteLine("    SimplePut -q B -k *SYSTEM -s TLS_RSA_WITH_AES_128_CBC_SHA -h remotehost -p 1414 -l SYSTEM.DEF.SVRCONN -dn CN=IBMWEBSPHEREMQQM07 -kr 40000 -cr false");
            Console.WriteLine();
        }

        /// <summary>
        /// Display Properties
        /// </summary>
        void DisplayMQProperties()
        {
            // display all details
            Console.WriteLine("MQ Parameters");
            Console.WriteLine("1) queueName = " + properties["QueueName"]);
            Console.WriteLine("2) keyRepository = " + properties[MQC.SSL_CERT_STORE_PROPERTY]);
            Console.WriteLine("3) cipherSpec = " + properties[MQC.SSL_CIPHER_SPEC_PROPERTY]);
            Console.WriteLine("4) host = " + properties[MQC.HOST_NAME_PROPERTY]);
            Console.WriteLine("5) port = " + properties[MQC.PORT_PROPERTY]);
            Console.WriteLine("6) channel = " + properties[MQC.CHANNEL_PROPERTY]);
            Console.WriteLine("7) numberOfMsgs = " + properties["MessageCount"]);
            Console.WriteLine("8) sslPeerName = " + properties[MQC.SSL_PEER_NAME_PROPERTY]);
            Console.WriteLine("9) keyResetCount = " + properties[MQC.SSL_RESET_COUNT_PROPERTY]);
            Console.WriteLine("10) sslCertRevocationCheck = " + properties["sslCertRevocationCheck"]);
            Console.WriteLine("");
        }

        /// <summary>
        /// Create a connection to the Queue Manager
        /// </summary>
        private MQQueueManager CreateQMgrConnection()
        {
            // mq properties
            var connectionProperties = new Hashtable {
                    { MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED},
                    { MQC.HOST_NAME_PROPERTY, properties[MQC.HOST_NAME_PROPERTY] },
                    { MQC.PORT_PROPERTY, properties[MQC.PORT_PROPERTY] },
                    { MQC.CHANNEL_PROPERTY, properties[MQC.CHANNEL_PROPERTY] }  };

            if ((String)properties[MQC.SSL_CERT_STORE_PROPERTY] != "") connectionProperties.Add(MQC.SSL_CERT_STORE_PROPERTY, properties[MQC.SSL_CERT_STORE_PROPERTY]);
            if ((String)properties[MQC.SSL_CIPHER_SPEC_PROPERTY] != "") connectionProperties.Add(MQC.SSL_CIPHER_SPEC_PROPERTY, properties[MQC.SSL_CIPHER_SPEC_PROPERTY]);
            if ((String)properties[MQC.SSL_PEER_NAME_PROPERTY] != "") connectionProperties.Add(MQC.SSL_PEER_NAME_PROPERTY, properties[MQC.SSL_PEER_NAME_PROPERTY]);
            if ((Int32)properties[MQC.SSL_RESET_COUNT_PROPERTY] != 0) connectionProperties.Add(MQC.SSL_RESET_COUNT_PROPERTY, properties[MQC.SSL_RESET_COUNT_PROPERTY]);
            if ((Boolean)properties["sslCertRevocationCheck"] != false) MQEnvironment.SSLCertRevocationCheck = true;

            // Queue Manager name can be passed instead of "" in the MQQueueManager constructor
            return new MQQueueManager("", connectionProperties);
        }

        /// <summary>
        /// Put messages
        /// </summary>
        void PutMessages()
        {
            try
            {
                String queueName = Convert.ToString(properties["QueueName"]);
                int noOfMsgs = Convert.ToInt32(properties["MessageCount"]);

                DisplayMQProperties();

                // create connection
                Console.Write("Connecting to queue manager.. ");
                using (var queueManager = CreateQMgrConnection())
                {
                    Console.WriteLine("done");

                    // accessing queue
                    Console.Write("Accessing queue " + queueName + ".. ");
                    var queue = queueManager.AccessQueue(queueName, MQC.MQOO_OUTPUT + MQC.MQOO_FAIL_IF_QUIESCING);
                    Console.WriteLine("done");

                    // creating a message object
                    var message = new MQMessage();
                    String messageString = "test message";
                    message.WriteString("test message");

                    // putting messages continuously
                    for (int i = 1; i <= noOfMsgs; i++)
                    {
                        Console.Write("Message " + i + " <" + messageString + ">.. ");
                        queue.Put(message);
                        Console.WriteLine("put successfully onto the queue");
                    }

                    // closing queue
                    Console.Write("Closing queue.. ");
                    queue.Close();
                    Console.WriteLine("done");

                    // disconnecting queue manager
                    Console.Write("Disconnecting queue manager.. ");
                }
                Console.WriteLine("done");
            }
            catch (MQException mqe)
            {
                Console.WriteLine("");
                Console.WriteLine("MQException received. Details: {0} - {1}", mqe.ReasonCode, mqe.Message);
                Console.WriteLine(mqe.StackTrace);
            }
        }
    }
}