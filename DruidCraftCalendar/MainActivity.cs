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
using DruidCraftCalendar.Touch;
using System.Collections.Generic;
using Android.Support.V4.View;

namespace DruidCraftCalendar
{
    [Activity(Label = "Druidcraft Calendar", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private TextView _gregorianLabel;
        private TextView _metonicLabel;
        private bool _playing = false;
        Stopwatch stopwatch = new Stopwatch();
        SKImageInfo _info; 
        SKSurface _surface; 
        SKCanvas _canvas;
        SkiaSharp.Views.Android.SKCanvasView canvasView;
        List<long> touchIds = new List<long>();
        CalendarControl _calendar;
        float Xtranslation;
        float Ytranslation;
        float Scale = 1f;


        public MainActivity()
        {
            _calendar = new CalendarControl();

            Xtranslation = 0;
            Ytranslation = 0;

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            _calendar.SetYearZero();

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            canvasView = FindViewById<SkiaSharp.Views.Android.SKCanvasView>(Resource.Id.canvasView);

            canvasView.PaintSurface += OnCanvasViewPaintSurface;
            canvasView.Touch += OnTouchEffectAction;

            _gregorianLabel = FindViewById<Android.Widget.TextView>(Resource.Id.textView1);
            _metonicLabel = FindViewById<Android.Widget.TextView>(Resource.Id.textView2);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Druidcraft Calendar";

            var diff = (DateTime.Now - _calendar.GetGregorianDate()).TotalDays;
            if (DateTime.Now.ToString("tt") == "PM")
            {
                diff = diff - 1;
            }

            _calendar.AddDays(Convert.ToInt32(diff));
            _calendar.SetGregorianDate(DateTime.Now);
            _gregorianLabel.Text = "Gregorian Date: " + _calendar.GetGregorianDate().ToLongDateString();
            _metonicLabel.Text = "Druidcraft Date: " + _calendar.GetMetonicDate();

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
                _calendar.AddDay();

                while (!_calendar.CheckLunarEclipse())
                {
                    _calendar.AddDay();
                }
                _gregorianLabel.Text = "Gregorian Date: " + _calendar.GetGregorianDate().ToLongDateString();
                _metonicLabel.Text = "Druidcraft Date: " + _calendar.GetMetonicDate();
                canvasView.Invalidate();
            }

            if (item.TitleFormatted.ToString() == "Next solar eclipse")
            {
                _calendar.AddDay();

                while (!_calendar.CheckSolarEclipse())
                {
                    _calendar.AddDay();
                }
                _gregorianLabel.Text = "Gregorian Date: " + _calendar.GetGregorianDate().ToLongDateString();
                _metonicLabel.Text = "Druidcraft Date: " + _calendar.GetMetonicDate();
                canvasView.Invalidate();
            }

            if (item.TitleFormatted.ToString() == "Next festival day")
            {
                _calendar.AddDay();

                while (!_calendar.CheckNextFestivalDay())
                {
                    _calendar.AddDay();
                }
                _gregorianLabel.Text = "Gregorian Date: " + _calendar.GetGregorianDate().ToLongDateString();
                _metonicLabel.Text = "Druidcraft Date: " + _calendar.GetMetonicDate();
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

            if (Xtranslation == 0 && Ytranslation == 0)
                System.Diagnostics.Debug.WriteLine("back to 0?");


            _canvas.Translate(Xtranslation, Ytranslation);
            _canvas.Scale(Scale);

      

            string resourceID = "DruidCraftCalendar.Assets.calendar2.png";
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            var cx = (_info.Width - Utility.GetWidthValueFromPercentage(_info, 99)) / 2;
            var cy = (_info.Height - Utility.GetWidthValueFromPercentage(_info, 99)) / 2;

            using (var stream = assembly.GetManifestResourceStream(resourceID))
            using (var bitmap = SKBitmap.Decode(stream))
            using (var paint = new SKPaint())
            {
                _canvas.DrawBitmap(bitmap, SKRect.Create(cx, cy, Utility.GetWidthValueFromPercentage(_info, 99), Utility.GetWidthValueFromPercentage(_info, 99)), paint);
            }

            CalendarRenderer.DrawCalendar(_info, _canvas, _calendar);

            _canvas.Save();
            _canvas.Translate(Xtranslation, Ytranslation);
            _canvas.Scale(Scale);
            _canvas.Restore();
        }

        void OnTouchEffectAction(object sender, Android.Views.View.TouchEventArgs args)
        {
            SKPoint point = new SKPoint(args.Event.GetX(), args.Event.GetY());
            var oldXtranslation = Xtranslation;
            var index = MotionEventCompat.GetActionIndex(args.Event);

            switch (args.Event.Action)
            {
                case MotionEventActions.Down:
                case MotionEventActions.Pointer2Down:
                case MotionEventActions.Pointer1Down:
                    args.Handled = true;
                    touchIds.Add(args.Event.GetPointerId(index));
                    ProcessTouchEvent(args.Event.GetPointerId(index), args.Event.Action, point);
                    break;
                case MotionEventActions.Move:

                    if (touchIds.Contains(args.Event.GetPointerId(index)))
                    {
                        ProcessTouchEvent(args.Event.GetPointerId(index), args.Event.Action, point);
                        canvasView.Invalidate();
                    }
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.Pointer1Up:
                case MotionEventActions.Pointer2Up:
                case MotionEventActions.Cancel:
                    if (touchIds.Contains(args.Event.GetPointerId(index)))
                    {
                        ProcessTouchEvent(args.Event.GetPointerId(index), args.Event.Action, point);
                        touchIds.Remove(args.Event.GetPointerId(index));
                        //canvasView.Invalidate();
                    }
                    break;
            }
        }

        async Task AnimateLoop()
        {
            stopwatch.Start();
            while (_playing)
            {
                var delay = 1000;
                _calendar.AddDay();
                _gregorianLabel.Text = "Gregorian Date: " + _calendar.GetGregorianDate().ToLongDateString();
                _metonicLabel.Text = "Druidcraft Date: " + _calendar.GetMetonicDate();
                canvasView.Invalidate();
                if (_calendar.CheckLunarEclipse())
                {
                    Toast.MakeText(this,"Lunar Eclipse",ToastLength.Long).Show();
                    delay = 2000;
                }
                if (_calendar.CheckSolarEclipse())
                {
                    Toast.MakeText(this, "Possible Solar Eclipse", ToastLength.Long).Show();
                    delay = 2000;
                }
                await Task.Delay(delay);
            }
            stopwatch.Stop();
        }

        void DateSelect_OnClick()
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                Date_Changed(time);
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }


        Dictionary<long, TouchManipulationInfo> touchDictionary = new Dictionary<long, TouchManipulationInfo>();

        private void ProcessTouchEvent(long id, MotionEventActions type, SKPoint location)
        {
            switch (type)
            {
                case MotionEventActions.Down:
                case MotionEventActions.Pointer2Down:
                case MotionEventActions.Pointer1Down:    
                    touchDictionary.Add(id, new TouchManipulationInfo
                    {
                        PreviousPoint = location,
                        NewPoint = location
                    });
                    break;
                case MotionEventActions.Move:
                    TouchManipulationInfo info = touchDictionary[id];
                    info.NewPoint = location;
                    Manipulate();
                    info.PreviousPoint = info.NewPoint;
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.Pointer2Up:
                case MotionEventActions.Pointer1Up:    
                    touchDictionary[id].NewPoint = location;
                    //Manipulate();
                    touchDictionary.Remove(id);
                    break;
                case MotionEventActions.Cancel:
                    touchDictionary.Remove(id);
                    break;
                   
            }
        }

        public SKMatrix Matrix = SKMatrix.MakeIdentity();

        private void Manipulate()
        {
            TouchManipulationInfo[] infos = new TouchManipulationInfo[touchDictionary.Count];
            touchDictionary.Values.CopyTo(infos, 0);
            SKMatrix touchMatrix = SKMatrix.MakeIdentity();

            if (infos.Length == 1)
            {
                SKPoint prevPoint = infos[0].PreviousPoint;
                SKPoint newPoint = infos[0].NewPoint;
                SKPoint pivotPoint = Matrix.MapPoint(_info.Width / 2, _info.Height /2);

                OneFingerManipulate(prevPoint, newPoint, pivotPoint);
            }
            else if (infos.Length >= 2)
            {
                int pivotIndex = infos[0].NewPoint == infos[0].PreviousPoint ? 0 : 1;
                SKPoint pivotPoint = Matrix.MapPoint(_info.Width / 2, _info.Height / 2);
                SKPoint newPoint = infos[1 - pivotIndex].NewPoint;
                SKPoint prevPoint = infos[1 - pivotIndex].PreviousPoint;

                TwoFingerManipulate(prevPoint, newPoint, pivotPoint);
            }
        }

        private void TwoFingerManipulate(SKPoint prevPoint, SKPoint newPoint, SKPoint pivotPoint)
        {
            SKPoint oldVector = prevPoint - pivotPoint;
            SKPoint newVector = newPoint - pivotPoint;
            Scale = Magnitude(newVector) / Magnitude(oldVector);
            if (Scale > 1)
            {
                var xCentre = (_info.Width * Scale) / 2;
                var yCentre = (_info.Height * Scale) / 2;

                var xshift = xCentre - (_info.Width / 2);
                var yshift = yCentre - (_info.Height / 2);

                //Xtranslation -= xshift;
                //Ytranslation -= yshift;
            }
        }

        float Magnitude(SKPoint point)
        {
            return (float)Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2));
        }

        private void OneFingerManipulate(SKPoint prevPoint, SKPoint newPoint, SKPoint pivotPoint)
        {
            Xtranslation = newPoint.X - prevPoint.X;
            Ytranslation = newPoint.Y - prevPoint.Y;
        }


        private void Date_Changed(DateTime date)
        {
            if (date == null)
            {
                this.Title = "No date";
            }
            else
            {
                _calendar.SetYearZero();
                var diff = (date - _calendar.GetGregorianDate()).TotalDays;
                _calendar.AddDays(Convert.ToInt32(diff));
                //var canvasView = FindViewById<SkiaSharp.Views.Android.SKCanvasView>(Resource.Id.canvasView);
                canvasView.Invalidate();

                _calendar.SetGregorianDate(_calendar.GetGregorianDate().AddDays(1));
                _gregorianLabel.Text = "Gregorian Date: " + date.ToLongDateString();
                _metonicLabel.Text = "Druidcraft Date: " + _calendar.GetMetonicDate();
            }
        }
    }
}

