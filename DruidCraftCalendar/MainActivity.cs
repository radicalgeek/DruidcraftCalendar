using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using SkiaSharp;
using Xamarin.Forms;
using Android.Support.V7.App;
using SkiaSharp.Views.Android;
using SkiaSharp.Views.Forms;
using DruidCraftCalendar.Rings;
using System.Timers;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DruidCraftCalendar
{
    [Activity(Label = "Druidcraft Calendar", MainLauncher = true)]
    public class MainActivity : Activity
    {
        //private DispatcherTimer _dispatcherTimer;
        private readonly IDayModel _dayModel;
        private readonly IMetonicYearModel _metonicYearModel;
        private readonly IMonthModel _monthModel;
        private readonly IMoonModel _moonModel;
        private readonly INodesModel _nodesModel;
        private readonly ISunCountModel _sunCountModel;
        private readonly ISunModel _sunModel;
        private DateTime _gregorianDate;
        private TextView _gregorianLabel;
        private TextView _metonicLabel;
        private bool _playing = false;
        Stopwatch stopwatch = new Stopwatch();
        SKImageInfo _info; 
        SKSurface _surface; 
        SKCanvas _canvas;
        SkiaSharp.Views.Android.SKCanvasView canvasView;
        public MainActivity()
        {
            _metonicYearModel = new MetonicYearModel();
            _moonModel = new MoonModel();
            _nodesModel = new NodesModel();
            _sunModel = new SunModel(_nodesModel);
            _sunCountModel = new SunCountModel(_sunModel);
            _monthModel = new MonthModel(_metonicYearModel, _sunModel, _moonModel);
            _dayModel = new DayModel(_monthModel, _metonicYearModel);


        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetYearZero();

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            canvasView = FindViewById<SkiaSharp.Views.Android.SKCanvasView>(Resource.Id.canvasView);

            canvasView.PaintSurface += OnCanvasViewPaintSurface;

            _gregorianLabel = FindViewById<Android.Widget.TextView>(Resource.Id.textView1);

            _metonicLabel = FindViewById<Android.Widget.TextView>(Resource.Id.textView2);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Druidcraft Calendar";

            var diff = (DateTime.Now - _gregorianDate).TotalDays;
            if (DateTime.Now.ToString("tt") == "PM")
            {
                diff = diff - 1;
            }
            AddDays(Convert.ToInt32(diff));
            _gregorianDate = DateTime.Now;
            _gregorianLabel.Text = "Gregorian Date: " + _gregorianDate.ToLongDateString();
            _metonicLabel.Text = "Druidcraft Date: " + GetMetonicDate();

        }

        async Task AnimateLoop()
        {
            stopwatch.Start();
            while (_playing)
            {
                var delay = 1000;
                AddDay();
                _gregorianLabel.Text = "Gregorian Date: " + _gregorianDate.ToLongDateString();
                _metonicLabel.Text = "Druidcraft Date: " + GetMetonicDate();
                canvasView.Invalidate();
                if (CheckLunarEclipse())
                {
                    Toast.MakeText(this,"Lunar Eclipse",ToastLength.Long).Show();
                    delay = 2000;
                }
                if (CheckSolarEclipse())
                {
                    Toast.MakeText(this, "Possible Solar Eclipse", ToastLength.Long).Show();
                    delay = 2000;
                }
                await Task.Delay(delay);
            }
            stopwatch.Stop();
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.TitleFormatted.ToString() == "Set Date")
                DateSelect_OnClick();
            
            if (item.TitleFormatted.ToString() == "Now")
            {
                Date_Changed(DateTime.Now);
            }
            
            if (item.TitleFormatted.ToString() == "Play")
            {
                _playing = true;
                AnimateLoop();
            }

            if (item.TitleFormatted.ToString() == "Stop")
            {
                _playing = false;
            }

            if (item.TitleFormatted.ToString() == "Next lunar eclipse")
            {
                
                while(!CheckLunarEclipse())
                {
                    AddDay();
                }
                _gregorianLabel.Text = "Gregorian Date: " + _gregorianDate.ToLongDateString();
                _metonicLabel.Text = "Druidcraft Date: " + GetMetonicDate();
                canvasView.Invalidate();
            }

            if (item.TitleFormatted.ToString() == "Next solar eclipse")
            {

                while (!CheckSolarEclipse())
                {
                    AddDay();
                }
                _gregorianLabel.Text = "Gregorian Date: " + _gregorianDate.ToLongDateString();
                _metonicLabel.Text = "Druidcraft Date: " + GetMetonicDate();
                canvasView.Invalidate();
            }

            return base.OnOptionsItemSelected(item);
        }

        void OnCanvasViewPaintSurface(object sender, SkiaSharp.Views.Android.SKPaintSurfaceEventArgs args)
        {
            _info = args.Info;
            _surface = args.Surface;
            _canvas = _surface.Canvas;

            _canvas.Clear();
            var scale = _info.Width / canvasView.Width;
            _canvas.Scale(scale);                 

           
            string resourceID = "DruidCraftCalendar.Assets.calendar2.png";
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            var cx = (_info.Width - Calendar.GetWidthValueFromPercentage(_info, 99)) /2;
            var cy = (_info.Height - Calendar.GetWidthValueFromPercentage(_info, 99)) /2;

            using (var stream = assembly.GetManifestResourceStream(resourceID))
            using (var bitmap = SKBitmap.Decode(stream))
            using (var paint = new SKPaint()){
                _canvas.DrawBitmap(bitmap, SKRect.Create(cx,cy ,Calendar.GetWidthValueFromPercentage(_info, 99), Calendar.GetWidthValueFromPercentage(_info, 99)),paint);
            }

            Calendar.DrawCalendar(_info,_canvas, _nodesModel, _sunModel, _moonModel, _metonicYearModel, _monthModel, _dayModel, _sunCountModel);

        }

        void DateSelect_OnClick()
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                Date_Changed(time);
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }

        public void AddDays(int daysToAdd)
        {
            for (int i = 1; i <= daysToAdd; i++)
            {
                AddDay();
            }
        }

        public void AddDay()
        {
            _dayModel.Incriment();
            _moonModel.Incriment();
            _gregorianDate = _gregorianDate.AddDays(1);
            _sunCountModel.Incriment(_gregorianDate);

        }

        private void SetYearZero()
        {


            _dayModel.Set(1);
            _dayModel.IsShortMonth();
            _monthModel.Set(1);
            _metonicYearModel.SetMetonicYear(4);
            _moonModel.Set(1);
            _sunCountModel.Set(1);
            _sunModel.Set(1);
            _nodesModel.SetNode1Position(8);
            _nodesModel.SetNode2Position(36);
            _gregorianDate = new DateTime(1998, 12, 18);
        }

        private void Date_Changed(DateTime date)
        {

            if (date == null)
            {
                this.Title = "No date";
            }
            else
            {
                SetYearZero();
                var diff = (date - _gregorianDate).TotalDays;
                AddDays(Convert.ToInt32(diff));
                var canvasView = FindViewById<SkiaSharp.Views.Android.SKCanvasView>(Resource.Id.canvasView);
                canvasView.Invalidate();

                _gregorianDate = _gregorianDate.AddDays(1);
                _gregorianLabel.Text = "Gregorian Date: " + date.ToLongDateString();
                _metonicLabel.Text = "Druidcraft Date: " + GetMetonicDate();
            }
        }

        private string GetMetonicDate()
        {
            string date = _dayModel.Get() + " " + _monthModel.GetMonthName() + ", Year " + _metonicYearModel.GetMetonicYear();
            return date;
        }

        public string GetMetonicDate(DateTime date)
        {
            SetYearZero();
            TimeSpan diff = date - _gregorianDate;
            AddDays(diff.Days);
            return GetMetonicDate();
        }

        private bool CheckLunarEclipse()
        {
            if ((_nodesModel.GetNode1Position() == _moonModel.Get() && _nodesModel.GetNode2Position() == _sunModel.Get()) || (_nodesModel.GetNode2Position() == _moonModel.Get() && _nodesModel.GetNode1Position() == _sunModel.Get()))
            {
                return true;
            }
            return false;
        }

        private bool CheckSolarEclipse()
        {
            
            bool isEclipse = false;

            bool node1SunAligned = false;
            if (_nodesModel.GetNode1Position() == _sunModel.Get() || _nodesModel.GetNode1Position() == _sunModel.Get() - 1 || _nodesModel.GetNode1Position() == _sunModel.Get() + 1)
                node1SunAligned = true;

            bool node1MoonAligned = false;
            if (_nodesModel.GetNode1Position() == _moonModel.Get() || _nodesModel.GetNode1Position() == _moonModel.Get() - 1 || _nodesModel.GetNode1Position() == _moonModel.Get() + 1)
                node1MoonAligned = true;
            
            bool node2SunAligned = false;
            if (_nodesModel.GetNode2Position() == _sunModel.Get() || _nodesModel.GetNode2Position() == _sunModel.Get() - 1 || _nodesModel.GetNode2Position() == _sunModel.Get() + 1)
                node2SunAligned = true;

            bool node2MoonAligned = false;
            if (_nodesModel.GetNode2Position() == _moonModel.Get() || _nodesModel.GetNode2Position() == _moonModel.Get() - 1 || _nodesModel.GetNode2Position() == _moonModel.Get() + 1)
                node2MoonAligned = true;

            if ((node1SunAligned == true && node1MoonAligned == true) || (node2SunAligned == true && node2MoonAligned == true))
                isEclipse = true;
            
            return isEclipse;
        }
    }
}

