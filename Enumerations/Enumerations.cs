namespace SuperScript
{
    public enum Longevity
    {
        /// <summary>
        /// Indicates that this file should be deleted after first request.
        /// </summary>
        SingleUse,

        /// <summary>
        /// Indicates that this file should exist until further notice.
        /// </summary>
        Reuse
    }
}