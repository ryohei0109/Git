using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.Threading;
using Android.Media;
using System;

namespace KitchenTimerAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            /// 10分追加
            var add10MinButton = FindViewById<Button>(Resource.Id.Add10MinButton);
            add10MinButton.Click += Add10MinButton_Click;

            // ラムダ式で1分追加
            var add1MinButton = FindViewById<Button>(Resource.Id.Add1MinButton);
            add1MinButton.Click += (s, e) =>
            {
                _remainingMillSec += 60 * 1000;
                ShowRemainingTime();
            };

            // 10秒追加
            var add10SecButton = FindViewById<Button>(Resource.Id.Add10SecButton);
            add10SecButton.Click += Add10SecButton_Click;

            // 1秒追加
            var add1SecButton = FindViewById<Button>(Resource.Id.Add1SecButton);
            add1SecButton.Click += Add1SecButton_Click;

            // クリア
            var clearButton = FindViewById<Button>(Resource.Id.ClearButton);
            clearButton.Click += ClearButton_Click;

            // スタートボタンクリック
            _startButton = FindViewById<Button>(Resource.Id.StartButton);
            _startButton.Click += StartButton_Click;

            _timer = new Timer(Timer_OnTick, null, 0, 100);
        }

        /// <summary>
        /// 10分追加ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add10MinButton_Click(object sender, System.EventArgs e)
        {
            _remainingMillSec += 60 * 10 * 1000;
            ShowRemainingTime();
        }

        /// <summary>
        /// 10秒追加ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add10SecButton_Click(object sender, System.EventArgs e)
        {
            _remainingMillSec += 10 * 1000;
            ShowRemainingTime();
        }

        /// <summary>
        /// 1秒追加ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add1SecButton_Click(object sender, System.EventArgs e)
        {
            _remainingMillSec += 1 * 1000;
            ShowRemainingTime();
        }

        /// <summary>
        /// クリアボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_Click(object sender, System.EventArgs e)
        {
            _remainingMillSec = 0;
            ShowRemainingTime();
        }

        private void StartButton_Click(object sender, System.EventArgs e)
        {
            _isStart = !_isStart;
            if (_isStart)
            {
                _startButton.Text = "ストップ";
            }
            else
            {
                _startButton.Text = "スタート";
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void ShowRemainingTime()
        {
            var sec = _remainingMillSec / 1000;
            FindViewById<TextView>(Resource.Id.RemainingTimeTextView).Text
                = string.Format("{0:F0}:{1:d2}",
                sec / 60,
                sec % 60);
        }

        private void Timer_OnTick(object state)
        {
            if (!_isStart)
            {
                return;
            }

            RunOnUiThread(() =>
                {
                    _remainingMillSec -= 100;
                    if (_remainingMillSec <= 0)
                    {
                        _isStart = false;
                        _remainingMillSec = 0;
                        _startButton.Text = "スタート";

                        try
                        {
                            // アラートを鳴らす
                            var toneGenerator = new ToneGenerator(Stream.System, 50);
                            toneGenerator.StartTone(Tone.PropBeep);
                        }
                        catch (Exception e)
                        {

                        }
                    }

                    ShowRemainingTime();
                });
        }

        /// <summary>
        /// タイマー値
        /// </summary>
        private int _remainingMillSec = 0;

        /// <summary>
        /// 計測中フラグ
        /// </summary>
        private bool _isStart = false;

        private Button _startButton;

        private Timer _timer;
    }
}