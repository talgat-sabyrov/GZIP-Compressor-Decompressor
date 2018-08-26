using System;
using System.IO;
using GZipTest.Abstractions;
using GZipTest.Data;

namespace GZipTest.Business
{
    public class Validator : IValidator
    {
        public void CommandIsValid(InputCommand input)
        {
            if (input.CommandName.ToLower() != Constants.CompressCommand && input.CommandName.ToLower() != Constants.DecompressCommand)
            {
                throw new Exception("Неизвестная команда:" + input.CommandName + "\n Введите команду по шаблону:\n compress(decompress) [путь исходного файла] [путь файла назначения].");
            }
            if (!File.Exists(input.SourceFile))
            {
                throw new Exception("Исходный файл не найден.");
            }

            if (input.SourceFile == input.DestinationFile)
            {
                throw new Exception("Пути исходного файла и файла назначения не могут быть одинковыми.");
            }
            var sourceFileInfo = new FileInfo(input.SourceFile);
            if (sourceFileInfo.Extension == Constants.GzipExtention && input.CommandName == Constants.CompressCommand)
            {
                throw new Exception("Файл уже был сжат.");
            }
            if (sourceFileInfo.Extension != Constants.GzipExtention && input.CommandName == Constants.DecompressCommand)
            {
                throw new Exception("Неверный формат файла для разжатия.");
            }

            var destinationFileInfo = new FileInfo(input.DestinationFile);
            if (destinationFileInfo.Exists)
            {
                throw new Exception("Файл пути назначения уже существует.");
            }
        }
    }
}
