static class SpeedTest
{
    public static int LatestDownloadBandwidth { get; internal set; } = 0;
    public static int LatestUploadBandwidth { get; internal set; } = 0;

    public static void Run()
    {
        var cliOutput = "";

        using (var speedtestCli = new System.Diagnostics.Process())
        {
            speedtestCli.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"speedtest");
            speedtestCli.StartInfo.Arguments = "--format=json --progress=no --accept-license --accept-gdpr";
            speedtestCli.StartInfo.RedirectStandardOutput = true;
            speedtestCli.StartInfo.RedirectStandardError = true;

            speedtestCli.Start();
            speedtestCli.WaitForExit();

            cliOutput = speedtestCli.StandardOutput.ReadToEnd();

            speedtestCli.Close();
        }

        var results = new SpeedTestResults();

        try
        {
            results = System.Text.Json.JsonSerializer.Deserialize<SpeedTestResults>(cliOutput);
        }
        catch
        {
            LatestDownloadBandwidth = 0;
            LatestUploadBandwidth = 0;
        }

        LatestDownloadBandwidth = results?.download?.bandwidth ?? 0;
        LatestUploadBandwidth = results?.upload?.bandwidth ?? 0;
    }
}

