/*
 * Gewächshaus - Besondere Lernleistung 2019, Hessen
 * Code von Hung Truong
 * Zuletzt verändert am 06.03.19
 * 
 * 
 * Quellenverzeichnis:
 * -https://docs.microsoft.com/de-de/dotnet/framework/network-programming/asynchronous-server-socket-example
 * -https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-client-socket-example
 */
namespace Gartenhaus_2
{
    class Program
    {
        static void Main(string[] args)
        {
            new Server().StartServer();
        }
    }
}
