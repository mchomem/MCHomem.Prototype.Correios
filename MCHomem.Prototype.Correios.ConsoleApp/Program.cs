using MCHomem.Prototype.Correios.ConsumeWS.br.com.correios.apps;
using MCHomem.Prototype.Correios.Models.Entities;
using MCHomem.Prototype.Correios.Models.Utilities;
using System;
using System.Threading;

namespace MCHomem.Prototype.Correios.ConsoleApp
{
    class Program
    {
        #region Main Method

        static void Main(string[] args)
        {
            Console.Title = "Using Web Service Correios";
            BuildAppMenu();
        }

        #endregion

        #region Methods

        private static void BuildAppMenu()
        {
            while (true)
            {
                Console.Clear();

                String imagePath = String.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, @"Images\correios.png");
                ConsoleImageReader.DrawImage(imagePath);

                Console.SetCursorPosition(0, 11);
                Console.WriteLine("Menu");
                Console.WriteLine();
                Console.WriteLine("1 - Consult zip code");
                Console.WriteLine("0 - Exit");
                Console.Write("\nOption: ");
                String op = Console.ReadLine();

                switch (op)
                {
                    case "1":
                        Console.Clear();
                        Console.Write("Enter a zip code: ");
                        String cep = Console.ReadLine();

                        while (cep.Length != 0)
                        {
                            SearchZipCode(cep);
                            cep = String.Empty;
                        }

                        break;

                    case "0":
                        Console.WriteLine("\nShutting down...");
                        Thread.Sleep(3000);
                        Environment.Exit(0);
                        break;

                    default:
                        break;
                }
            }
        }

        private static void SearchZipCode(String cep)
        {
            try
            {
                Console.Clear();
                String title = "Zip code information consulted";
                Console.WriteLine(String.Empty.PadRight(Console.BufferWidth, '*'));
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(String.Empty.PadRight((Console.WindowWidth / 2) - (title.Length / 2), ' ') + title.ToUpper());
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(String.Empty.PadRight(Console.BufferWidth, '*'));

                Console.WriteLine("Consultation in progress...\n");

                // Mapped object from the Correios Webservice.
                AtendeClienteService correios = new AtendeClienteService();
                Address address = TransferToModelObject(correios.consultaCEP(cep));

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Address:".PadRight(10, ' '));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(address.PulicPlace);
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Neighborhood:".PadRight(10, ' '));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(address.Neighborhood);
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Zip Code:".PadRight(10, ' '));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(address.ZipCode);
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("City:".PadRight(10, ' '));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(address.City);
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Federation Unity:".PadRight(10, ' '));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(address.FederationUnity);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(String.Empty.PadRight(Console.BufferWidth, '*'));
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine
                    (
                        String.Format
                        (
                            "Error: Application failure.\nMessage: {0}\nStackTrace: {1}\nInnerException:{2}"
                            , e.Message
                            , e.StackTrace
                            , e.InnerException
                        )
                    );

                Console.ForegroundColor = ConsoleColor.Gray;
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Copy the data coming from the WS object to the application model
        /// object creating a separation between what is in WS and the model.
        /// </summary>
        /// <param name="enderecoWS">Object retrieved from the Post Office WS.</param>
        /// <returns></returns>
        private static Address TransferToModelObject(ConsumeWS.br.com.correios.apps.enderecoERP enderecoWS)
        {
            Address address = new Address();
            address.Neighborhood = enderecoWS.bairro;
            address.ZipCode = enderecoWS.cep;
            address.City = enderecoWS.cidade;
            address.PulicPlace = enderecoWS.end;
            address.FederationUnity = enderecoWS.uf;

            return address;
        }

        #endregion
    }
}
