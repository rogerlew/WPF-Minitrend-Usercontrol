using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MiniTrendExampleProject
{
    public class SumOfSinesWaveform
    {
        public struct WaveformComponent
        {
            public float amplitude;
            public float phase;
        }

        List<WaveformComponent> waveformComponents;
        public SumOfSinesWaveform(List<WaveformComponent> w)
        {
            waveformComponents = w;
        }
    }

    public partial class MainWindow : Window
    {
        // Need a timer to update the minitrends
        DispatcherTimer timer;

        // We need to keep track of wall time for creating a fake signal
        Stopwatch sw;

        SumOfSinesDisturbance dist;

        public MainWindow()
        {
            InitializeComponent();

            // The time base of the MiniTrend is determined by the update interval.
            // Everytime you call it, it shifts the graph by 1 pixel.
            // The graph portion of the MiniTrend is 180 pixels wide, so if you
            // update it once a second it would have a time base of 180 seconds or
            // 3 minutes.
            // So if you want a time base of 5 minutes your inteval should be 3.333 (600.0 / 180.0)
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.3333333); // 0.33333 is a time base of 60 seconds
            timer.Tick += Update;
            timer.Start();

            // Setup variables for creating the signal            
            sw = Stopwatch.StartNew();

            Random random = new Random();
            List<SumOfSinesDisturbance.WaveformComponent> waveforms = new List<SumOfSinesDisturbance.WaveformComponent>();
            waveforms.Add(new SumOfSinesDisturbance.WaveformComponent(10.0, 0.0005, random.Next() * Math.PI * 2.0));
            waveforms.Add(new SumOfSinesDisturbance.WaveformComponent(5.0,  0.0007, random.Next() * Math.PI * 2.0));
            waveforms.Add(new SumOfSinesDisturbance.WaveformComponent(3.0,  0.0011, random.Next() * Math.PI * 2.0));
            waveforms.Add(new SumOfSinesDisturbance.WaveformComponent(2.0,  0.0023, random.Next() * Math.PI * 2.0));
            dist = new SumOfSinesDisturbance(waveforms, stdev:0.1, bias:100.0);
        }

        void Update(object sender, EventArgs e)
        {
            var value = dist.Sample(sw.Elapsed.TotalSeconds);
            aMiniTrend.Update(value);
        }

    }
}
