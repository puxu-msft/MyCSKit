using System;
using System.Threading;

using Garnet;

try {
    using var server = new GarnetServer(args);
    server.Start();
    Thread.Sleep(Timeout.Infinite);
}
catch (Exception ex) {
    Console.WriteLine($"Failed to init GarnetServer: {ex.Message}");
}
