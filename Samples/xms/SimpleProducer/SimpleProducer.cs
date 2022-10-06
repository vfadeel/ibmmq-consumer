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
/*  FILE NAME:      SimpleProducer.cs                                */
/*                                                                   */
/*  DESCRIPTION:    Basic example of simple message producer.        */
/*                                                                   */
/*                                                                   */
/* A simple application to demonstrate sending messages.             */
/*                                                                   */
/* Notes: The application can be used to send messages to both queue */
/*        and topic destinations.                                    */
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
    class SimpleProducer
    {       
        /// <summary>
        /// Sample message to send
        /// </summary>
        private const String simpleMessage = "This is a simple message from XMS.NET producer";
        /// <summary>
        /// Dictionary to store all the properties
        /// </summary>
        private IDictionary<string, object> properties = null;
        /// <summary>
        /// Expected command-line arguments
        /// </summary>
        private readonly String[] cmdArgs = { "-m", "-d", "-k", "-h", "-p", "-l", "-s", "-dn", "-kr", "-cr" };
        /// <summary>
        /// Main entry
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("===> START of Simple Producer sample for WMQ transport <===\n");
            try
            {
                SimpleProducer simpleProducer = new SimpleProducer
                {
                    properties = new Dictionary<string, object>()
                };

                if (simpleProducer.ParseCommandline(args))
                    simpleProducer.SendMessage();
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
            Console.WriteLine("===> END of Simple Producer sample for WMQ transport <===\n\n");
        }

        /// <summary>
        /// Send message
        /// </summary>
        void SendMessage()
        {           

            // destination - queue or topic
            String destinationName = (String)properties["Destination"];
            bool isTopic = false;
            // Next parameter will either be queue name or topic string
            if (destinationName.StartsWith("topic://"))
            {
                isTopic = true;
            }

            //Get the destination name
            destinationName = destinationName.Remove(0, 8);

            // Create connection.
            var connectionWMQ = CreateConnection();
            Console.WriteLine("Connection created");

            // Create session
            using (var sessionWMQ = connectionWMQ.CreateSession(false, AcknowledgeMode.AutoAcknowledge))
            {
                Console.WriteLine("Session created");
                IDestination destination = null;
                // Create destination
                if (isTopic)
                    destination = sessionWMQ.CreateTopic(destinationName);
                else
                    destination = sessionWMQ.CreateQueue(destinationName);
                Console.WriteLine("Destination created");

                // Create producer
                var producer = sessionWMQ.CreateProducer(destination);
                Console.WriteLine("Producer created");

                // Start the connection to receive messages.
                connectionWMQ.Start();
                Console.WriteLine("Connection started");

                // Create a text message and send it.
                var  textMessage = sessionWMQ.CreateTextMessage();
                textMessage.Text = simpleMessage;
                producer.Send(textMessage);
                Console.WriteLine("Message sent");
            }
            connectionWMQ.Close();
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

            return cf.CreateConnection();
        }

        /// <summary>
        /// Display help
        /// </summary>
        private void DisplayHelp()
        {
            Console.WriteLine("Usage: SimpleProducer -m queueManager -d destinationURI -k keyrespository [-h host -p port -l channel -s cipherspec -dn sslpeername -kr keyresetcount -cr sslcertificate revocation check]");
            Console.WriteLine("Ex: SimpleProducer -m QM -d QA");
            Console.WriteLine("    SimpleProducer -m QM -d topic://TopicA -h remotehost -p 1414 -l SYSTEM.DEF.SVRCONN");
            Console.WriteLine("For Ssl Connections: SimpleProducer -m QM -d QA -k *SYSTEM");
            Console.WriteLine("                     SimpleProducer -m QM -d QA -k *SYSTEM -s TLS_RSA_WITH_AES_128_CBC_SHA256 -kr 45000");
        }

        /// <summary>
        /// Parse commandline parameters
        /// Usage: SimpleProducer -m queueManager -d destinationURI [-h host -p port -l channel]
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