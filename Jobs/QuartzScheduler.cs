using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

public class QuartzScheduler
{
    public static async Task IniciarScheduler()
    {
        IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        await scheduler.Start();

        // Definir el Job
        IJobDetail job = JobBuilder.Create<StockJob>().Build();

        // Crear un Trigger para ejecutar cada 2 horas
        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("StockTrigger")
            .StartNow()
            .WithSimpleSchedule(x => x.WithIntervalInHours(2).RepeatForever()) // Cada 2 horas
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }
}