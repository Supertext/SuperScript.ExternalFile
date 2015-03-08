using FluentScheduler;


namespace SuperScript.ExternalFile
{
    public class Scavenger : ITask
    {
        public void Execute()
        {
            var storeProvider = Configuration.Settings.Instance.StoreProvider;
            var scavengeOlderThan = Configuration.Settings.Instance.ScavengeItemsOlderThan;

            storeProvider.Scavenge(scavengeOlderThan);
        }
    }
}