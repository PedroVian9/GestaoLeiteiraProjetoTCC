using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services
{
    public static class AutoAdbManager
    {
        private static string _adbPath;
        private static readonly object _lock = new object();
        private static readonly string PRIMARY_ADB_DIR = Path.Combine(Path.GetTempPath(), "GestaoLeiteiraADB");
        private static readonly string ALTERNATIVE_ADB_DIR = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GestaoLeiteiraADB");

        /// <summary>
        /// Garante que o ADB esteja disponível, baixando-o se necessário.
        /// </summary>
        public static async Task<string> EnsureAdbAsync(Action<string> onStatusUpdate = null)
        {
            if (!string.IsNullOrEmpty(_adbPath) && File.Exists(_adbPath))
                return _adbPath;

            lock (_lock)
            {
                if (!string.IsNullOrEmpty(_adbPath) && File.Exists(_adbPath))
                    return _adbPath;
            }

            var primaryAdbPath = Path.Combine(PRIMARY_ADB_DIR, "adb.exe");
            if (await IsAdbWorkingAsync(primaryAdbPath))
            {
                onStatusUpdate?.Invoke("ADB encontrado e funcionando.");
                return _adbPath = primaryAdbPath;
            }

            try
            {
                onStatusUpdate?.Invoke("Tentando download para a pasta principal...");
                await DownloadAndExtractAdbAsync(onStatusUpdate, PRIMARY_ADB_DIR);
                _adbPath = primaryAdbPath;
            }
            catch (Exception primaryEx)
            {
                onStatusUpdate?.Invoke($"Falha no download principal: {primaryEx.Message}. Tentando pasta alternativa...");

                var alternativeAdbPath = Path.Combine(ALTERNATIVE_ADB_DIR, "adb.exe");
                if (await IsAdbWorkingAsync(alternativeAdbPath))
                {
                    onStatusUpdate?.Invoke("ADB alternativo encontrado e funcionando.");
                    return _adbPath = alternativeAdbPath;
                }

                await DownloadAndExtractAdbAsync(onStatusUpdate, ALTERNATIVE_ADB_DIR);
                _adbPath = alternativeAdbPath;
            }

            if (!await IsAdbWorkingAsync(_adbPath))
                throw new Exception("ADB foi baixado, mas não está funcionando corretamente.");

            onStatusUpdate?.Invoke("ADB pronto para uso.");
            return _adbPath;
        }
        // Adicionar dentro da classe AutoAdbManager

        /// <summary>
        /// Executa um comando ADB e redireciona a saída binária diretamente para um arquivo.
        /// Ideal para comandos como 'cat' que transferem arquivos.
        /// </summary>
        public static async Task<(string error, int exitCode)> ExecuteAdbCommandAndStreamOutputToFileAsync(string arguments, string outputFilePath, Action<string> onStatusUpdate = null)
        {
            var adbPath = await EnsureAdbAsync(onStatusUpdate);

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = adbPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(adbPath)
                }
            };

            process.Start();

            // Lê a stream de erro como texto
            var errorTask = process.StandardError.ReadToEndAsync();

            // Copia a stream de saída (binária) diretamente para o arquivo
            await using (var fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await process.StandardOutput.BaseStream.CopyToAsync(fileStream);
            }

            await process.WaitForExitAsync();
            var error = await errorTask;

            return (error, process.ExitCode);
        }
        /// <summary>
        /// Executa um comando ADB e retorna o resultado.
        /// </summary>
        public static async Task<(string output, string error, int exitCode)> ExecuteAdbCommandAsync(string arguments, Action<string> onStatusUpdate = null)
        {
            var adbPath = await EnsureAdbAsync(onStatusUpdate);

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = adbPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(adbPath)
                }
            };

            process.Start();
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            var output = await outputTask;
            var error = await errorTask;

            return (output, error, process.ExitCode);
        }

        /// <summary>
        /// Limpa todos os arquivos ADB baixados.
        /// </summary>
        public static async Task ClearAdbCacheAsync()
        {
            _adbPath = null;
            var foldersToClean = new[] { PRIMARY_ADB_DIR, ALTERNATIVE_ADB_DIR };

            foreach (var folder in foldersToClean)
            {
                if (!Directory.Exists(folder)) continue;
                try
                {
                    var files = Directory.GetFiles(folder);
                    foreach (var file in files)
                    {
                        await SafeDeleteFileAsync(file);
                    }
                    Directory.Delete(folder, true);
                }
                catch (Exception) { /* Ignorar erros de limpeza */ }
            }
        }

        private static async Task<bool> IsAdbWorkingAsync(string adbPath)
        {
            if (!File.Exists(adbPath))
                return false;

            try
            {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = adbPath,
                        Arguments = "version",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                var output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();

                return output.Contains("Android Debug Bridge") && process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        private static async Task DownloadAndExtractAdbAsync(Action<string> onStatusUpdate, string targetDir)
        {
            Directory.CreateDirectory(targetDir);
            using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
            var zipPath = Path.Combine(targetDir, $"platform-tools_{Guid.NewGuid():N[..8]}.zip");
            var maxRetries = 3;
            var url = "https://dl.google.com/android/repository/platform-tools-latest-windows.zip";
            Exception lastException = null;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    onStatusUpdate?.Invoke($"Baixando ADB... (tentativa {attempt}/{maxRetries})");
                    await SafeDeleteFileAsync(zipPath);

                    var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength ?? 0;
                    await using var contentStream = await response.Content.ReadAsStreamAsync();
                    await using var fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);

                    var buffer = new byte[8192];
                    long downloadedBytes = 0;
                    int bytesRead;
                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        downloadedBytes += bytesRead;
                        if (totalBytes > 0)
                        {
                            var progress = (int)((downloadedBytes * 100) / totalBytes);
                            onStatusUpdate?.Invoke($"Baixando... {progress}% ({downloadedBytes / (1024 * 1024)} MB)");
                        }
                    }
                    await fileStream.FlushAsync();

                    if (new FileInfo(zipPath).Length < 1024 * 1024) // Menos de 1MB é inválido
                        throw new Exception("Arquivo baixado está corrompido (muito pequeno)");

                    lastException = null;
                    break;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    await SafeDeleteFileAsync(zipPath);
                    if (attempt < maxRetries)
                    {
                        onStatusUpdate?.Invoke($"Erro no download. Tentando novamente em 2s...");
                        await Task.Delay(2000);
                    }
                }
            }

            if (lastException != null) throw lastException;

            await ExtractZipWithRetryAsync(zipPath, onStatusUpdate, targetDir);
        }

        private static async Task ExtractZipWithRetryAsync(string zipPath, Action<string> onStatusUpdate, string targetDir)
        {
            var maxRetries = 3;
            Exception lastException = null;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    onStatusUpdate?.Invoke($"Extraindo arquivos... (tentativa {attempt})");
                    if (attempt > 1) await Task.Delay(1000 * attempt);

                    await CleanOldAdbFilesAsync(targetDir);
                    var requiredFiles = new[] { "adb.exe", "AdbWinApi.dll", "AdbWinUsbApi.dll" };
                    var extractedCount = 0;

                    using (var zip = ZipFile.OpenRead(zipPath))
                    {
                        foreach (var entry in zip.Entries.Where(e => e.FullName.StartsWith("platform-tools/")))
                        {
                            var fileName = Path.GetFileName(entry.FullName);
                            if (requiredFiles.Contains(fileName))
                            {
                                var destinationPath = Path.Combine(targetDir, fileName);
                                await SafeDeleteFileAsync(destinationPath);
                                entry.ExtractToFile(destinationPath, true);
                                extractedCount++;
                                onStatusUpdate?.Invoke($"Extraído: {fileName}");
                            }
                        }
                    }

                    if (extractedCount < requiredFiles.Length)
                        throw new Exception("Nem todos os arquivos ADB necessários foram encontrados no ZIP.");

                    onStatusUpdate?.Invoke("Extração concluída.");
                    await SafeDeleteFileAsync(zipPath);
                    return; // Sucesso
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    onStatusUpdate?.Invoke($"Erro na extração. Tentando novamente...");
                }
            }
            throw lastException ?? new Exception("Falha na extração após múltiplas tentativas.");
        }

        private static async Task SafeDeleteFileAsync(string filePath)
        {
            if (!File.Exists(filePath)) return;
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    File.Delete(filePath);
                    return;
                }
                catch (IOException) when (i < 4)
                {
                    await Task.Delay(500);
                }
            }
        }

        private static async Task CleanOldAdbFilesAsync(string targetDir)
        {
            if (!Directory.Exists(targetDir)) return;

            var filesToClean = Directory.GetFiles(targetDir, "*.exe")
                .Concat(Directory.GetFiles(targetDir, "*.dll"))
                .Concat(Directory.GetFiles(targetDir, "*.zip"));

            foreach (var file in filesToClean)
            {
                await SafeDeleteFileAsync(file);
            }
        }
    }
}