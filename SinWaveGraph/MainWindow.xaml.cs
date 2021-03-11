using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System.Threading;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace SinWaveGraph
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private float _axisMin;
        private float _axisMax;
        public SeriesCollection SeriesCol { get; set; }
        public LineSeries LSeries { get; set; }
        public SinWave MySinWave { get; set; }
        public bool IsReading { get; set; }
        public bool IsPaused { get; set; }
        public float AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                OnPropertyChanged("AxisMin");
            }
        }
        public float AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                OnPropertyChanged("AxisMax");
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            SeriesCol = new SeriesCollection();
            LSeries = new LineSeries()
            {
                PointGeometry = null,
                Fill = Brushes.Transparent,
                Values = new ChartValues<ObservablePoint>()
            };
            SeriesCol.Add(LSeries);
            SetAxisLimitsStart();

            DataContext = this;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            float amplitude;
            float frequency;
            var _A = float.TryParse(tbAmplitude.Text, out amplitude);
            var _F = float.TryParse(tbFrequency.Text, out frequency);

            MySinWave = new SinWave(amplitude, frequency);
            LSeries.Values.Clear();
            IsReading = true;
            IsPaused = false;
            UpdateChart();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            IsPaused = !IsPaused;

            if (IsPaused)
            {
                IsReading = false;
                btnPause.Content = "Continue";
            }
            else
            {
                IsReading = true;
                btnPause.Content = "Pause";
                UpdateChart();
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        private void UpdateChart()
        {
            const int keepRecords = 600;

            Task.Run(() =>
            {
                while (IsReading)
                {
                    // Runs every 10ms
                    Thread.Sleep(10);
                    MySinWave.SetNextValue();
                    LSeries.Values.Add(new ObservablePoint(MySinWave.TimeStep, MySinWave.CurrentValue));

                    if (LSeries.Values.Count > keepRecords)
                    {
                        LSeries.Values.RemoveAt(0);
                        SetAxisLimits();
                    }
                }
            });
        }

        private void ClearControls()
        {
            tbAmplitude.Text = string.Empty;
            tbFrequency.Text = string.Empty;
            btnPause.Content = "Pause";
            IsReading = false;
            LSeries.Values.Clear();
            SetAxisLimitsStart();
        }

        private void SetAxisLimitsStart()
        {
            AxisMin = 0;
            AxisMax = 10;
        }
        private void SetAxisLimits()
        {
            AxisMin += 0.01f;
            AxisMax += 0.01f;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Only accept numbers in text box
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
