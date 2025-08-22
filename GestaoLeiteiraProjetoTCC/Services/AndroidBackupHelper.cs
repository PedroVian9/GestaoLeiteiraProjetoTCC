// Em Services/AndroidBackupHelper.cs

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using SQLite; // Manter este using

namespace GestaoLeiteiraProjetoTCC.Services
{
    public static class AndroidBackupHelper
    {
        // O cabeçalho padrão de um backup Android sem compressão e sem criptografia.
        private const string BackupHeader = "ANDROID BACKUP\n5\n1\nnone\n";

        public static string ExtractDbFromBackup(string backupFilePath, string dbName, string packageName)
        {
            byte[] allFileData = File.ReadAllBytes(backupFilePath);

            // Um backup .ab é essencialmente um arquivo zlib (Deflate) com um cabeçalho de 24 bytes.
            // Verificamos se o cabeçalho corresponde ao esperado para um backup não criptografado.
            string headerFromFile = Encoding.UTF8.GetString(allFileData, 0, BackupHeader.Length);

            if (headerFromFile != BackupHeader)
            {
                // Se o cabeçalho for diferente, o backup pode estar criptografado com senha.
                // O ADB não oferece uma forma de fornecer essa senha via linha de comando.
                throw new NotSupportedException("O arquivo de backup parece estar criptografado com uma senha. " +
                                                "Por favor, remova a senha de backup nas Opções de Desenvolvedor do Android e tente novamente.");
            }

            // Os dados comprimidos começam exatamente após o cabeçalho de 24 bytes.
            byte[] compressedData = new byte[allFileData.Length - BackupHeader.Length];
            Array.Copy(allFileData, BackupHeader.Length, compressedData, 0, compressedData.Length);

            byte[] decompressedTarData = DecompressData(compressedData);

            return FindAndExtractFileFromTar(decompressedTarData, dbName, packageName);
        }

        private static byte[] DecompressData(byte[] compressedData)
        {
            try
            {
                using var inputStream = new MemoryStream(compressedData);
                // Usamos a DeflateStream, que é o formato de compressão zlib usado pelo backup do Android.
                using var decompressionStream = new DeflateStream(inputStream, CompressionMode.Decompress);
                using var outputStream = new MemoryStream();

                decompressionStream.CopyTo(outputStream);
                return outputStream.ToArray();
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("Falha ao descomprimir os dados do backup. O arquivo pode estar corrompido ou em um formato inesperado.", ex);
            }
        }

        // O método FindAndExtractFileFromTar permanece o mesmo da sua versão anterior,
        // pois ele já funciona bem para ler o arquivo .tar resultante.
        private static string FindAndExtractFileFromTar(byte[] tarData, string dbName, string packageName)
        {
            using (var tarStream = new MemoryStream(tarData))
            {
                var headerBuffer = new byte[512];
                var possiblePaths = new[] { $"apps/{packageName}/f/{dbName}", $"apps/{packageName}/db/{dbName}" };

                while (tarStream.Read(headerBuffer, 0, headerBuffer.Length) == 512)
                {
                    var fileName = Encoding.ASCII.GetString(headerBuffer, 0, 100).Trim('\0');
                    if (string.IsNullOrWhiteSpace(fileName)) break;

                    var sizeOctal = Encoding.ASCII.GetString(headerBuffer, 124, 11).Trim('\0', ' ');
                    var fileSize = Convert.ToInt64(sizeOctal, 8);

                    if (possiblePaths.Any(p => p.Equals(fileName, StringComparison.OrdinalIgnoreCase)))
                    {
                        var fileData = new byte[fileSize];
                        tarStream.Read(fileData, 0, fileData.Length);

                        // Alinhar ao próximo bloco de 512 bytes
                        long remainingInBlock = 512 - (fileSize % 512);
                        if (remainingInBlock < 512)
                        {
                            tarStream.Seek(remainingInBlock, SeekOrigin.Current);
                        }

                        var tempDbPath = Path.Combine(Path.GetTempPath(), $"extracted_{Guid.NewGuid():N[..8]}_{dbName}");
                        File.WriteAllBytes(tempDbPath, fileData);

                        try
                        {
                            VerifyDatabaseIntegrity(tempDbPath);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"O banco de dados foi extraído, mas parece estar inválido ou vazio. Erro: {ex.Message}", ex);
                        }

                        return tempDbPath;
                    }
                    else
                    {
                        // Pular os blocos de dados do arquivo atual
                        long blocksToSkip = (fileSize + 511) / 512;
                        tarStream.Seek(blocksToSkip * 512, SeekOrigin.Current);
                    }
                }
            }
            throw new Exception($"O arquivo de banco de dados '{dbName}' não foi encontrado dentro do backup.");
        }

        private static void VerifyDatabaseIntegrity(string dbPath)
        {
            var fileInfo = new FileInfo(dbPath);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException($"Arquivo de banco não encontrado: {dbPath}");
            }

            if (fileInfo.Length < 4096) // Um DB válido tem pelo menos 4KB
            {
                throw new Exception("Arquivo de banco de dados extraído é muito pequeno para ser válido.");
            }

            using var db = new SQLiteConnection(dbPath);

            // 1. Rodar PRAGMA integrity_check
            var integrityResult = db.ExecuteScalar<string>("PRAGMA integrity_check;");
            if (!string.Equals(integrityResult, "ok", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Falha na integridade do banco: {integrityResult}");
            }

            // 2. Verificar tabelas essenciais
            var tabelasEssenciais = new[] { "Propriedade", "Animal", "Raca" };
            foreach (var tabela in tabelasEssenciais)
            {
                var tableInfo = db.GetTableInfo(tabela);
                if (tableInfo == null || !tableInfo.Any())
                {
                    throw new Exception($"Tabela essencial '{tabela}' não encontrada no banco extraído.");
                }
            }
        }

    }
}