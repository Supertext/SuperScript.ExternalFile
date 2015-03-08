using FluentScheduler;

namespace SuperScript.ExternalFile
{
    public class SchedulerRegistry : Registry
    {
        public SchedulerRegistry()
        {
            // if the TotalSeconds of the scavenger period is greater than an int can handle, reduce the number to an int.
            var period = Configuration.Settings.Instance.ScavengePeriod.TotalSeconds;
            if (period > int.MaxValue)
            {
                period = int.MaxValue;
            }

            Schedule<Scavenger>().ToRunEvery((int) period).Seconds();
        }
    }
}