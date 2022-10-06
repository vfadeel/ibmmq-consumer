/*********************************************************************/
/*   <copyright                                                      */
/*   notice="lm-source-program"                                      */
/*   pids="5724-M21,"                                                */
/*   years="2018,2020"                                               */
/*   crc="2787562084" >                                              */
/*   Licensed Materials - Property of IBM                            */
/*                                                                   */
/*   5724-H72,                                                       */
/*                                                                   */
/*   (C) Copyright IBM Corp.2018,2020 All Rights Reserved.           */
/*                                                                   */
/*   US Government Users Restricted Rights - Use, duplication or     */
/*   disclosure restricted by GSA ADP Schedule Contract with         */
/*   IBM Corp.                                                       */
/*   </copyright>                                                    */
/*********************************************************************/
/*                                                                   */
/*                  IBM Message Service Client for .NET              */
/*                                                                   */
/*  FILE NAME:      SimpleConsumer.cs                                */
/*                                                                   */
/*  DESCRIPTION:    Basic example of a simple message consumer       */
/*                                                                   */
/* A simple application demonstrate synchronous message consumer.    */
/*                                                                   */
/* Notes: The consumer receives messages synchronously.              */
/*                                                                   */
/* API type: IBM XMS.NET API v2.0                                    */
/*                                                                   */
/* Messaging domain: P2P and Pub/Sub                                 */
/*                                                                   */
/* Provider type: WebSphere MQ                                       */
/*                                                                   */
/* JNDI in use: No                                                   */
/*                                                                   */
/*********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using IBM.XMS;

namespace SimpleWMQSamples
{
    class SimpleConsumer
    {
        /// <summary>
        /// Dictionary to store all the properties
        /// </summary>
        private IDictionary<string, object> properties = null;
        /// <summary>
        /// Expected command-line arguments
        /// </summary>
        private readonly String[] cmdArgs = { "-m", "-d", "-k", "-h", "-p", "-l", "-s", "-dn", "-kr", "-cr" };
        /// <summary>
        /// Timeout
        /// </summary>
        private const int TIMEOUTTIME = 30000;

        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("===> START of Simple Consumer sample for WMQ transport <===\n\n");
            try
            {
                SimpleConsumer simpleConsumer = new SimpleConsumer
                {
                    properties = new Dictionary<string, object>()
                };

                if (simpleConsumer.ParseCommandline(args))
                    simpleConsumer.ReceiveMessages();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Invalid arguments!\n{0}", ex);
            }
            catch (XMSException ex)
            {
                Console.WriteLine("XMSException caught: {0}", ex);
                if (ex.LinkedException != null)
                {
                    Console.WriteLine("Stack Trace:\n {0}", ex.LinkedException.StackTrace);
                }
                Console.WriteLine("Sample execution  FAILED!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught: {0}", ex);
                Console.WriteLine("Sample execution  FAILED!");
            }
            Console.WriteLine("===> END of Simple Consumer sample for WMQ transport <===\n\n");
        }

        /// <summary>
        /// Create a connection to the Queue Manager
        /// </summary>
        /// <returns></returns>
        private IConnection CreateConnection()
        {
            // Get an instance of factory.
            var factoryFactory = XMSFactoryFactory.GetInstance(XMSC.CT_WMQ);

            // Create WMQ Connection Factory.
            var cf = factoryFactory.CreateConnectionFactory();

            // Set the properties
            cf.SetStringProperty(XMSC.WMQ_HOST_NAME, (String)properties[XMSC.WMQ_HOST_NAME]);
            cf.SetIntProperty(XMSC.WMQ_PORT, Convert.ToInt32(properties[XMSC.WMQ_PORT]));
            cf.SetStringProperty(XMSC.WMQ_CHANNEL, (String)properties[XMSC.WMQ_CHANNEL]);
            cf.SetIntProperty(XMSC.WMQ_CONNECTION_MODE, XMSC.WMQ_CM_CLIENT);
            cf.SetStringProperty(XMSC.WMQ_QUEUE_MANAGER, (String)properties[XMSC.WMQ_QUEUE_MANAGER]);
            // SSL properties
            if ((String)properties[XMSC.WMQ_SSL_KEY_REPOSITORY] != "") cf.SetStringProperty(XMSC.WMQ_SSL_KEY_REPOSITORY, (String)properties[XMSC.WMQ_SSL_KEY_REPOSITORY]);
            if ((String)properties[XMSC.WMQ_SSL_CIPHER_SPEC] != "") cf.SetStringProperty(XMSC.WMQ_SSL_CIPHER_SPEC, (String)properties[XMSC.WMQ_SSL_CIPHER_SPEC]);
            if ((String)properties[XMSC.WMQ_SSL_PEER_NAME] != "") cf.SetStringProperty(XMSC.WMQ_SSL_PEER_NAME, (String)properties[XMSC.WMQ_SSL_PEER_NAME]);
            if ((Int32)properties[XMSC.WMQ_SSL_KEY_RESETCOUNT] != -1) cf.SetIntProperty(XMSC.WMQ_SSL_KEY_RESETCOUNT, (Int32)properties[XMSC.WMQ_SSL_KEY_RESETCOUNT]);
            if ((Boolean)properties[XMSC.WMQ_SSL_CERT_REVOCATION_CHECK] != false) cf.SetBooleanProperty(XMSC.WMQ_SSL_CERT_REVOCATION_CHECK, true);

            // Create connection.
            return cf.CreateConnection();
        }

        /// <summary>
        /// Method to connect to queue manager and receive messages.
        /// </summary>
        void ReceiveMessages()
        {
            
            // destination - queue or topic
            String destinationName = (String)properties["Destination"];
            bool isTopic = false;
            // Next parameter will either be queue name or topic string
            if (destinationName.StartsWith("topic://"))
            {
                isTopic = true;
            }
            
            // Does not start with topic, then it's a queue name.
            destinationName = destinationName.Remove(0, 8);
            

            //Create Connection
            IConnection connectionWMQ = CreateConnection();
            Console.WriteLine("Connection created");

            // Create session
            using (var sessionWMQ = connectionWMQ.CreateSession(false, AcknowledgeMode.AutoAcknowledge))
            {
                Console.WriteLine("Session created.");

                IDestination destination = null;
                // Create destination
                if (isTopic)
                    destination = sessionWMQ.CreateTopic(destinationName);
                else
                    destination = sessionWMQ.CreateQueue(destinationName);

                Console.WriteLine("Destination created.");

                // Create consumer
                var consumer = sessionWMQ.CreateConsumer(destination);
                Console.WriteLine("Message Consumer created. Starting the connection now.");
                // Start the connection to receive messages.
                connectionWMQ.Start();

                Console.WriteLine("Receive message: " + TIMEOUTTIME / 1000 + " seconds wait time");
                // Wait for 30 seconds for messages. Exit if no message by then
                var textMessage = (ITextMessage)consumer.Receive(TIMEOUTTIME);
                if (textMessage != null)
                {
                    Console.WriteLine("Message received.");
                    Console.Write(textMessage);
                    Console.WriteLine("\n");
                }
                else
                    Console.WriteLine("Wait timed out.");
            }
            connectionWMQ.Close();
        }

        /// <summary>
        /// Display help
        /// </summary>
        private void DisplayHelp()
        {
            Console.WriteLine("Usage: SimpleConsumer -m queueManager -d destinationURI -k keyrespository [-h host -p port -l channel -s cipherspec -dn sslpeername -kr keyresetcount -cr sslcertificate revocation check]");
            Console.WriteLine("Ex: SimpleConsumer -m QM -d QA");
            Console.WriteLine("    SimpleConsumer -m QM -d topic://TopicA -h remotehost -p 1414 -l SYSTEM.DEF.SVRCONN");
            Console.WriteLine("For Ssl Connections: SimpleConsumer -m QM -d QA -k *SYSTEM");
            Console.WriteLine("                     SimpleConsumer -m QM -d QA -k *SYSTEM -s TLS_RSA_WITH_AES_128_CBC_SHA256 -kr 45000");
        }

        /// <summary>
        /// Parse commandline parameters
        /// Usage: SimpleConsumer -m queueManager -d destinationURI [-h host -p port -l channel]
        /// </summary>
        /// <param name="args"></param>
        bool ParseCommandline(string[] args)
        {
            if (args.Length < 4)
            {
                DisplayHelp();
                return false;
            }

            var cmdlineArguments = Enumerable.Range(0, args.Length / 2).ToDictionary(i => args[2 * i], i => args[2 * i + 1]);

            foreach (String arg in cmdlineArguments.Keys)
            {
                if (!cmdArgs.Contains(arg))
                    throw new ArgumentException("Invalid argument", arg);
            }

            // set the properties
            properties.Add(XMSC.WMQ_HOST_NAME, cmdlineArguments.ContainsKey("-h") ? cmdlineArguments["-h"] : "localhost");
            properties.Add(XMSC.WMQ_PORT, cmdlineArguments.ContainsKey("-p") ? Convert.ToInt32(cmdlineArguments["-p"]) : 1414);
            properties.Add(XMSC.WMQ_CHANNEL, cmdlineArguments.ContainsKey("-l") ? cmdlineArguments["-l"] : "SYSTEM.DEF.SVRCONN");
            properties.Add(XMSC.WMQ_QUEUE_MANAGER, cmdlineArguments["-m"]);
            properties.Add(XMSC.WMQ_SSL_KEY_REPOSITORY, cmdlineArguments.ContainsKey("-k") ? cmdlineArguments["-k"] : "");
            properties.Add(XMSC.WMQ_SSL_CIPHER_SPEC, cmdlineArguments.ContainsKey("-s") ? cmdlineArguments["-s"] : "");
            properties.Add(XMSC.WMQ_SSL_PEER_NAME, cmdlineArguments.ContainsKey("-dn") ? cmdlineArguments["-dn"] : "");
            properties.Add(XMSC.WMQ_SSL_KEY_RESETCOUNT, cmdlineArguments.ContainsKey("-kr") ? Convert.ToInt32(cmdlineArguments["-kr"]) : -1);
            properties.Add(XMSC.WMQ_SSL_CERT_REVOCATION_CHECK, cmdlineArguments.ContainsKey("-cr") ? Convert.ToBoolean(cmdlineArguments["-cr"]) : false);
            properties.Add("Destination", cmdlineArguments["-d"]);

            return true;
        }
    }
}