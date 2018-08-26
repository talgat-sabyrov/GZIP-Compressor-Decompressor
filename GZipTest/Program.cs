using System;
using Autofac;
using GZipTest.Abstractions;
using GZipTest.Business;
using GZipTest.Data;

namespace GZipTest
{
    class Program
    {
        private static readonly IGZipProcessor GZipProcessor;
        private static readonly IGZipProgress GZipProgress;
        
        static Program()
        {
            var container = ContainerConfig.Configure();
            using (var scope = container.BeginLifetimeScope())
            {
                GZipProcessor = scope.Resolve<IGZipProcessor>();
                GZipProgress = scope.Resolve<IGZipProgress>();
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Процесс начался...\n");
            
            try
            {
                //InputCommand input = new InputCommand
                //{
                //    CommandName = Constants.CompressCommand,
                //    SourceFile = "D:\\Marvel\\1.Iron.Man.2008_HDRip_[scarabey.org].avi",
                //    DestinationFile = "D:\\abc"
                //};
                //InputCommand input = new InputCommand
                //{
                //    CommandName = Constants.DecompressCommand,
                //    SourceFile = "D:\\abc.gz",
                //    DestinationFile = "D:\\abc1.avi"
                //};

                InputCommand input = ConvertArgsToInputCommand(args);
                GZipProgress.ProcessProgressedEvent += ProgressBar;
                GZipProgress.ProcessFinished += Finished;
                GZipProcessor.Process(input);
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(e.Message);
            }
            
            Console.ReadLine();
        }

        private static void ProgressBar(long current, long total)
        {
            string res = current + " из " + total;
            Console.Write("\r{0}", res);
        }

        private static void Finished()
        {
            Console.WriteLine("\n");
            Console.WriteLine("Процесс закончен!");
        }

        static InputCommand ConvertArgsToInputCommand(string[] args)
        {
            if (args.Length != Constants.WordsCount)
            {
                throw new Exception("Введите команду по шаблону:\n compress(decompress) [путь исходного файла] [путь файла назначения].");
            }
            if (args[Constants.CommandIndex].ToLower() != Constants.CompressCommand && args[Constants.CommandIndex].ToLower() != Constants.DecompressCommand)
            {
                throw new Exception("Неизвестная команда:" + args[0] + "\n Введите команду по шаблону:\n compress(decompress) [путь исходного файла] [путь файла назначения].");
            }

            return new InputCommand
            {
                CommandName = args[Constants.CommandIndex],
                SourceFile = args[Constants.SourceIndex],
                DestinationFile = args[Constants.DestinationIndex]
            };
        }
    }
}
