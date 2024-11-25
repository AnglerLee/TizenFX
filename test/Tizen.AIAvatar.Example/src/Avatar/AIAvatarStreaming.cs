using System;
using Tizen;
using Tizen.NUI;
using Tizen.NUI.Scene3D;

namespace AIAvatar
{
    public partial class AIAvatar : Model
    {


        private Timer timer;
        private int currentIndex = 0;


        private bool LipSyncStreamingTest(object source, Timer.TickEventArgs e)
        {
            if (currentIndex + 2 < testVowels.Length)
            {
                Animation lipAnimation = lipSyncer.GenerateAnimationFromVowels(new string[] { testVowels[currentIndex], testVowels[currentIndex + 1] }, 0.08f, true);
                lipSyncer.Enqueue(lipAnimation);
                currentIndex += 2;
            }
            else
            {
                return false;
            }

            return true;
        }
        public void PlayStreamingLipSync()
        {
            try
            {
                lipSyncer.Stop();

                currentIndex = 0;
                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                }
                timer = new Timer(30);
                timer.Tick += LipSyncStreamingTest;
                timer.Start();

            }
            catch (Exception ex)
            {
                Log.Error(Utils.LogTag, "An unexpected exception occurred: " + ex.Message);
            }
        }
    }
}
