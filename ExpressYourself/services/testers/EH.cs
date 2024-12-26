using Microsoft.Extensions.Hosting;
namespace ExpressYourself.services;
//This class contain the automation aspect of the endpoint test
public class EH : IHostedService, IDisposable //Stands for Endpoint Hosted
{
    private Timer? timer;
    private readonly TimeSpan interval = TimeSpan.FromMinutes(15); //Period between every excecution.
    private bool isRunning = false; //Conflit controler

    public Task StartAsync(CancellationToken cancellationToken)
    {
        timer = new Timer(ExecuteTask, null, TimeSpan.Zero, interval); //Start timer. TimeSpan.Zero makes an execution at the Server Start.
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        timer?.Change(Timeout.Infinite, 0); //Stop Timer
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        timer?.Dispose(); //Release timer memory usage
    }

    private async void ExecuteTask(object? state)
    {
        if (isRunning) return; //Conflit protection

        try
        {
            isRunning = true;
            timer?.Change(Timeout.Infinite, 0); //This will prevents the timer to keep going while the task is running. It is facultative
            Console.WriteLine($"Starting EndPoint Hosted Requisition Test Service at: {DateTime.Now}");
            await CurrentTask(); //The actual function to be called
            Console.WriteLine($"Ending EndPoint Hosted Requisition Test Service at: {DateTime.Now}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"EndPoint Hosted Requisition Test Service Error: {ex.Message}"); // Execution Log
        }
        finally
        {
            isRunning = false;
            timer?.Change(interval, interval); //Start the timer again. Necessary if you have choosen to stop it while task execution
        }
    }

    private async Task CurrentTask()
    {
        await ET.ReqRandomIP(); //Calling the test
        Console.WriteLine("Executed Function!(Hosted EndPoint Service)"); //Execution Log
    }
}