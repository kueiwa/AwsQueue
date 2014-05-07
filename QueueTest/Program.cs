using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QueueTest
{
    /// <summary>
    /// Simulates server usage over time, and determines if requests will be dropped
    /// </summary>
    class Program
    {
        static uint simulationTime = 60 * 60 * 8; // Length of time to be simulated in seconds
        static double avgServiceTime = 5.00d; // Average time needed to service a request in seconds
        static double avgRequestRate = 10000000d / (60 * 60 * 8 * 22); // Average requests per second

        // Assuming Servers in Round-Robin Configuration
        static uint serverCount = 1; // Number of servers used in simulation
        static uint serverCapacity = 150; // Number of simulataneous requests a server can handle

        static void Main(string[] args)
        {
            // Initialize a Matrix Representing Available Capacity
            List<double>[] serverUsage = new List<double>[serverCount];
            for (int i = 0; i < serverCount; i++) {
                serverUsage[i] = new List<double>();
            }

            // Initialize Request Distributions
            Poisson arrivalDist = new Poisson(avgRequestRate);

            // Initialize Variables for Simulating Round-Robin Load Balancing
            int nextServerToUse = 0; // Next Server to Receive a Request
            
            // Initialize Measurement Variables
            int requestsRejectedByServer = 0;
            int totalRequestsInSimulation = 0;

            // Iterate through Simulation Rounds
            for(int round = 0; round < simulationTime; round++)
            {
                // Add New Requests to the Servers
                int requestsThisRound = arrivalDist.Sample();
                totalRequestsInSimulation += requestsThisRound;
                for(int request = 0; request < requestsThisRound; request++)
                {
                    // Determine if the Next Server can handle the Request
                    double requestServiceTime = avgServiceTime;
                    if(serverUsage[nextServerToUse].Count >= serverCapacity) {
                        requestsRejectedByServer++;
                    } else {
                        serverUsage[nextServerToUse].Add(requestServiceTime);
                    }

                    // Use Round-Robin to Select Next Server
                    if(nextServerToUse == serverCount - 1) {
                        nextServerToUse = 0;
                    } else {
                        nextServerToUse++;
                    }
                }

                for (int server = 0; server < serverCount; server++) {
                    // Calculate Remaining Time needed for Requests
                    for (int request = 0; request < serverUsage[server].Count; request++)
                    {
                        serverUsage[server][request] -= 1d;
                    }

                    // Remove Completed Requests from the Servers
                    serverUsage[server].RemoveAll(requestTime => requestTime <= 0);
                }

                // Render Output
                if (round % (int)Math.Round(simulationTime / 10000d) == 0)
                {
                    Console.SetCursorPosition(0, 0);
                    Console.ResetColor();
                    Console.WriteLine("Round {0}/{1} ({2}%)", round, simulationTime, 
                        Math.Round((double)round * 100d / (double)simulationTime, 2));
                    for (int server = 0; server < serverCount; server++)
                    {
                        if (serverUsage[server].Count >= serverCapacity)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }

                        Console.WriteLine("Server {0}: [{1}]", server.ToString(), 
                            String.Empty
                            .PadRight(serverUsage[server].Count, '|')
                            .PadRight((int)serverCapacity));
                    }
                }
            }

            Console.WriteLine("Simulated Request Count: " + totalRequestsInSimulation);
            Console.WriteLine("Requests Rejected By Server: " + requestsRejectedByServer);
            Console.ReadKey(true);
        }
    }
}
