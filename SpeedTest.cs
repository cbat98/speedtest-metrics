static class SpeedTest
{
    public static string Run()
    {
        Console.WriteLine("Running speedtest..");

        var cliOutput = "";

        using (var speedtestCli = new System.Diagnostics.Process())
        {
            speedtestCli.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"speedtest");
            speedtestCli.StartInfo.Arguments = "--format=json --progress=no --accept-license --accept-gdpr";
            speedtestCli.StartInfo.RedirectStandardOutput = true;

            speedtestCli.Start();
            speedtestCli.WaitForExit();

            cliOutput = speedtestCli.StandardOutput.ReadToEnd();

            speedtestCli.Close();
        }

        var results = System.Text.Json.JsonSerializer.Deserialize<SpeedTestResults>(cliOutput);

        var bytesToMbps = (float)8 / 1000 / 1000;

        Console.WriteLine($"Server: {results.server.name} - Download: {results.download.bandwidth * bytesToMbps}Mbps - Upload: {results.upload.bandwidth * bytesToMbps}Mbps");

        return cliOutput;
    }
}

