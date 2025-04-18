using Microsoft.Maui.Devices;

namespace TipMate
{
    public static class HapticsHelper
    {
        public static void Vibrate(int duration = 100)
        {
            try
            {
                if (Vibration.Default.IsSupported)
                {
                    Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(duration));
                }
            }
            catch (Exception ex)
            {
                // Optional: log or handle exceptions gracefully
                System.Diagnostics.Debug.WriteLine($"Vibration error: {ex.Message}");
            }
        }
    }
}
