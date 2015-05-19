using System;
using System.Collections.Generic;
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

namespace DCSTrends
{
    /// <summary>
    /// Interaction logic for MiniTrendDisplay.xaml
    /// </summary>
    public partial class MiniTrendDisplay : UserControl
    {

        // n is used to keep track of the last updated index of Data
        // gets reset at N - 1  so it doesn't need to be a double
        public int n;

        private const int N = 180;

        public double Additive;

        #region DependencyProperty Declarations

        // Units <string>
        public static DependencyProperty UnitsProperty =
            DependencyProperty.Register("Units", typeof(string),
                                        typeof(MiniTrendDisplay));

        public string Units
        {
            get { return (string)GetValue(UnitsProperty); }
            set { SetValue(UnitsProperty, value); }
        }

        // Timebase <string>
        public static DependencyProperty TimebaseProperty =
            DependencyProperty.Register("Timebase", typeof(string),
                                        typeof(MiniTrendDisplay));

        public string Timebase
        {
            get { return (string)GetValue(TimebaseProperty); }
            set { SetValue(TimebaseProperty, value); }
        }

        // ValueStr <string>
        public static DependencyProperty ValueStrProperty =
            DependencyProperty.Register("ValueStr", typeof(string),
                                        typeof(MiniTrendDisplay));

        public string ValueStr
        {
            get { return (string)GetValue(ValueStrProperty); }
            set { SetValue(ValueStrProperty, value); }
        }

        // Ymin <double>
        public static DependencyProperty YminProperty =
            DependencyProperty.Register("Ymin", typeof(double),
                                        typeof(MiniTrendDisplay),
                                        new PropertyMetadata((object)57.0));

        public double Ymin
        {
            get { return (double)GetValue(YminProperty); }
            set { SetValue(YminProperty, (double)value); }
        }


        // Ymax <double>
        public static DependencyProperty YmaxProperty =
            DependencyProperty.Register("Ymax", typeof(double),
                                        typeof(MiniTrendDisplay),
                                        new PropertyMetadata((object)63.0));

        public double Ymax
        {
            get { return (double)GetValue(YmaxProperty); }
            set { SetValue(YmaxProperty, (double)value); }
        }


        // NoiseStd <double>
        public static DependencyProperty NoiseStdProperty =
            DependencyProperty.Register("NoiseStd", typeof(double),
                                        typeof(MiniTrendDisplay),
                                        new PropertyMetadata((object)0.0));

        public double NoiseStd
        {
            get { return (double)GetValue(NoiseStdProperty); }
            set { SetValue(NoiseStdProperty, (double)value); }
        }

        // NumSensors <double>
        public static DependencyProperty NumSensorsProperty =
            DependencyProperty.Register("NumSensors", typeof(double),
                                        typeof(MiniTrendDisplay),
                                        new PropertyMetadata((object)1.0));
        #endregion


        // Data stores the trend values is list of ints that get mapped 
        // directly to the Grid Container Coordinates
        public double[] Data1;
        Random random;
        
        bool initialized = false;

        public MiniTrendDisplay()
        {
            InitializeComponent();

            random = new Random();

            // Initialize Data array to container's height
            double H = this.Container.ActualHeight;
            n = 0;
            Additive = 0;
            Data1 = new double[N];
            for (int i = 0; i < N; i++)
            {
                Data1[i] = H;
            }
        }

        #region Support Functions
        private double Normal(double stdDev)
        {
            // http://stackoverflow.com/questions/218060/random-gaussian-variables

            if (stdDev == 0.0)
            {
                return 0.0;
            }
            double u1 = random.NextDouble();
            double u2 = random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                   Math.Sin(2.0 * Math.PI * u2);
            return stdDev * randStdNormal;
        }

        private double SampleStdDev(IEnumerable<double> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                double avg = values.Average();
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }
        #endregion

        private double TransformValue(double Value, double H, double rng)
        {
            // Scale to pixel units
            double NewValue = (H - H * ((Value - Ymin) / rng));
            if (NewValue > H) NewValue = H;
            if (NewValue < 0) NewValue = 0;

            return NewValue;
        }

        private double SensorNoise()
        {
            if (random.NextDouble() < 0.1)
                return Normal(NoiseStd);

            return 0.0;
        }

        public void Update(double Value1)
        {
            double H = Container.ActualHeight;

            // update the live trend with the new value
            int index = ((n % N) + 1) % N;

            // Store NewValue to List
            Data1[index] = TransformValue(Value1, H, Ymax - Ymin);

            if (!initialized)
            {
                for (int i = 0; i < N; i++)
                {
                    if (i == index)
                        continue;

                    Data1[i] = Data1[index];
                }
                initialized = true;
            }

            // update the counter
            n++;
            n %= N;

            UpdateTrendLine(Data1, Trend1);

            // Update the live text value
            ValueStr = String.Format("{0:0.00} ", Value1);

        }

        private void UpdateTrendLine(double[] Data, Path Trend)
        {
            // Rebuild the Data path
            // Might be a better way to do this without having to 
            // build the path string first
            var PathStr = new StringBuilder();
            PathStr.Append("M");
            for (int i = 1; i < N; i++)
            {
                var index = ((n % N) + 1 + i) % N;
                PathStr.Append(String.Format(" {0:0},{1:0}", i, Data[index]));
            }
            Trend.Data = Geometry.Parse(PathStr.ToString());
        }


    }
}