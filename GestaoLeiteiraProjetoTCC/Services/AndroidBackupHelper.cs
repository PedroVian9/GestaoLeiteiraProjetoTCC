// Em Services/AndroidBackupHelper.cs

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using SQLite; // Adicionar este using

namespace GestaoLeiteiraProjetoTCC.Services
{
    public static class AndroidBackupHelper
    {
        public static string ExtractDbFromBackup(string backupFilePath, string dbName, string packageName)
        {
            // ... (o código para ler, descriptografar e descomprimir permanece o mesmo) ...

            byte[] allFileData = File.ReadAllBytes(backupFilePath);
            ParseHeader(allFileData, out var encryptionAlgorithm, out var isCompressed, out var payloadStartIndex);

            int payloadLength = allFileData.Length - payloadStartIndex;
            byte[] payloadData = new byte[payloadLength];
            Array.Copy(allFileData, payloadStartIndex, payloadData, 0, payloadLength);

            byte[] decompressedTarData;

            if (encryptionAlgorithm.Equals("AES-256", StringComparison.OrdinalIgnoreCase))
            {
                var decryptedData = DecryptBackupData(payloadData);
                decompressedTarData = isCompressed ? DecompressData(decryptedData) : decryptedData;
            }
            else if (encryptionAlgorithm.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                decompressedTarData = isCompressed ? DecompressData(payloadData) : payloadData;
            }
            else
            {
                throw new NotSupportedException($"O algoritmo de criptografia '{encryptionAlgorithm}' não é suportado.");
            }

            return FindAndExtractFileFromTar(decompressedTarData, dbName, packageName);
        }

        private static void ParseHeader(byte[] fileData, out string encryptionAlgorithm, out bool isCompressed, out int payloadStartIndex)
        {
            // ... (o código para ParseHeader permanece o mesmo) ...
            encryptionAlgorithm = "none";
            isCompressed = true;
            payloadStartIndex = 0;
            var headerLines = new List<string>();
            var currentLine = new StringBuilder();
            int currentIndex = 0;
            while (headerLines.Count < 4 && currentIndex < fileData.Length)
            {
                char currentChar = (char)fileData[currentIndex];
                if (currentChar == '\n')
                {
                    headerLines.Add(currentLine.ToString().TrimEnd('\r'));
                    currentLine.Clear();
                    if (headerLines.Count == 4)
                    {
                        payloadStartIndex = currentIndex + 1;
                        break;
                    }
                }
                else
                {
                    currentLine.Append(currentChar);
                }
                currentIndex++;
            }
            if (headerLines.Count < 4) throw new InvalidDataException("Formato de backup inválido: cabeçalho incompleto.");
            if (!headerLines[0].Contains("ANDROID BACKUP")) throw new InvalidDataException("Arquivo não é um backup Android válido.");
            isCompressed = (headerLines[2]?.Trim() ?? "1") == "1";
            encryptionAlgorithm = headerLines[3]?.Trim() ?? "none";
            if (payloadStartIndex == 0) payloadStartIndex = currentIndex;
        }

        private static byte[] DecompressData(byte[] compressedData)
        {
            // ... (o código para DecompressData permanece o mesmo) ...
            try
            {
                using var inputStream = new MemoryStream(compressedData);
                using var outputStream = new MemoryStream();
                using var compressionStream = new DeflateStream(inputStream, CompressionMode.Decompress);
                compressionStream.CopyTo(outputStream);
                return outputStream.ToArray();
            }
            catch (InvalidDataException)
            {
                using var inputStream = new MemoryStream(compressedData);
                using var outputStream = new MemoryStream();
                using var compressionStream = new BrotliStream(inputStream, CompressionMode.Decompress);
                compressionStream.CopyTo(outputStream);
                return outputStream.ToArray();
            }
        }

        private static byte[] DecryptBackupData(byte[] encryptedData)
        {
            // ... (o código para DecryptBackupData permanece o mesmo) ...
            var salt = encryptedData.Take(16).ToArray();
            var iv = encryptedData.Skip(16).Take(16).ToArray();
            var encryptedPayload = encryptedData.Skip(32).ToArray();
            var keyGenerator = new Pkcs5S2ParametersGenerator(new Sha1Digest());
            keyGenerator.Init(PbeParametersGenerator.Pkcs5PasswordToBytes("".ToCharArray()), salt, 10000);
            var key = (KeyParameter)keyGenerator.GenerateDerivedMacParameters(256);
            var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
            cipher.Init(false, new ParametersWithIV(key, iv));
            return cipher.DoFinal(encryptedPayload);
        }

        private static string FindAndExtractFileFromTar(byte[] tarData, string dbName, string packageName)
        {
            using (var tarStream = new MemoryStream(tarData))
            {
                var headerBuffer = new byte[512];
                string expectedFilePath = $"apps/{packageName}/f/{dbName}";
                var possiblePaths = new[] { $"apps/{packageName}/f/{dbName}", $"apps/{packageName}/db/{dbName}" };

                while (tarStream.Read(headerBuffer, 0, headerBuffer.Length) == 512)
                {
                    var fileName = Encoding.ASCII.GetString(headerBuffer, 0, 100).Trim('\0');
                    if (string.IsNullOrWhiteSpace(fileName)) break;

                    var sizeOctal = Encoding.ASCII.GetString(headerBuffer, 124, 11).Trim('\0', ' ');
                    var fileSize = Convert.ToInt64(sizeOctal, 8);
                    var dataBlockCount = (fileSize + 511) / 512;

                    if (possiblePaths.Any(p => p.Equals(fileName, StringComparison.OrdinalIgnoreCase)))
                    {
                        var fileData = new byte[fileSize];
                        var totalBytesRead = 0;
                        for (var i = 0; i < dataBlockCount; i++)
                        {
                            var bytesToReadInBlock = (int)Math.Min(512, fileSize - totalBytesRead);
                            var bytesRead = tarStream.Read(fileData, totalBytesRead, bytesToReadInBlock);
                            if (bytesRead == 0) break;
                            totalBytesRead += bytesRead;
                            if (bytesRead < 512) tarStream.Seek(512 - bytesRead, SeekOrigin.Current); // Pula o resto do bloco
                        }

                        var tempDbPath = Path.Combine(Path.GetTempPath(), $"extracted_{Guid.NewGuid():N[..8]}_{dbName}");
                        File.WriteAllBytes(tempDbPath, fileData);

                        // ===== NOVA VERIFICAÇÃO DE INTEGRIDADE =====
                        try
                        {
                            VerifyDatabaseIntegrity(tempDbPath);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"O banco de dados foi extraído, mas parece estar inválido ou vazio. Erro: {ex.Message}", ex);
                        }
                        // ===== FIM DA VERIFICAÇÃO =====

                        return tempDbPath;
                    }
                    else
                    {
                        if (dataBlockCount > 0)
                        {
                            tarStream.Seek(dataBlockCount * 512, SeekOrigin.Current);
                        }
                    }
                }
            }
            throw new Exception($"O arquivo de banco de dados '{dbName}' não foi encontrado dentro do backup.");
        }

        // ===== NOVO MÉTODO DE VERIFICAÇÃO =====
        private static void VerifyDatabaseIntegrity(string dbPath)
        {
            SQLiteConnection db = null;
            try
            {
                db = new SQLiteConnection(dbPath);
                // Executa uma consulta simples para verificar se a tabela essencial 'Propriedade' existe.
                var tableInfo = db.GetTableInfo("Propriedade");
                if (tableInfo == null || !tableInfo.Any())
                {
                    throw new Exception("Tabela 'Propriedade' não encontrada.");
                }
                // Você pode adicionar mais verificações para outras tabelas se desejar.
                // ex: db.GetTableInfo("Animal");
            }
            finally
            {
                db?.Close();
            }
        }
    }
}