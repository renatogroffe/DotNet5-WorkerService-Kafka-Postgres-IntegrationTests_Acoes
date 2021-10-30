using System;
using Microsoft.Extensions.DependencyInjection;

namespace BaseAcoes.Migrations
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("**** BaseAcoes no PostgreSQL - Execucao de Migrations ****");

            if (args.Length != 1)
            {
                var oldForegroundColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(
                    "Informe como unico parametro a string de conexao com o PostgreSQL!");
                Console.ForegroundColor = oldForegroundColor;
                return;
            }

            var services = new ServiceCollection();
            Startup.ConfigureServices(services, args);
            services.BuildServiceProvider()
                .GetService<ConsoleApp>().Run();
        }
    }
}