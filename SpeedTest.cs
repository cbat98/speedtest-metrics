static class SpeedTest
{
    public static string Run()
    {
        var cliOutput = "";

        using (var speedtestCli = new System.Diagnostics.Process())
        {
            speedtestCli.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"speedtest.exe");
            speedtestCli.StartInfo.Arguments = "--format=json --progress=no --accept-license --accept-gdpr";
            speedtestCli.StartInfo.RedirectStandardOutput = true;

            speedtestCli.Start();
            speedtestCli.WaitForExit();

            cliOutput = speedtestCli.StandardOutput.ReadToEnd();

            speedtestCli.Close();
        }

        return cliOutput;
    }
}

