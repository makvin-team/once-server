using Hangfire;

namespace Once.Application.Jobs;

public static class JobsRegistrar
{
    public static void RegisterRecurringJobs()
    {
        //RecurringJob.AddOrUpdate<IPaymentJob>(
        //    JobConstants.ProcessRegistryPaymentStatusesJob,
        //    job => job.ProcessRegistryPaymentStatusUpdatesAsync(),
        //    Cron.Never()); // it should run after completing scoring-payment-statuses
    }
}