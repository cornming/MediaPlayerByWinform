using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace RandomMusicByWinForm
{
    public partial class Form1 : Form
    {
        WindowsMediaPlayer _player = new WMPLib.WindowsMediaPlayer();
            
        WMPLib.IWMPMedia media;
        Timer _timer = new Timer();
        List<string> _listUrl = new List<string>();

        
        public Form1()
        {
            InitializeComponent();

            _timer.Interval = 500;
            _timer.Tick += _timer_Tick;

            this.progressBar1.Step = 1;
            this.trackBar1.TickFrequency = 1;

            this.trackBar1.MouseDown += TrackBar1_MouseDown;
            this.trackBar1.MouseCaptureChanged += TrackBar1_MouseCaptureChanged;

            string applicationStartUrl = Application.StartupPath;
            string directoryPath = Path.GetDirectoryName(applicationStartUrl);
            DirectoryHelper.DirSearch(directoryPath, "*.mp3", ref _listUrl);
            this.listBox1.DataSource = _listUrl;


            PlayMedia();
        }

        

        private void TrackBar1_MouseCaptureChanged(object sender, EventArgs e)
        {
            _player.controls.currentPosition = trackBar1.Value;
        }

        private void TrackBar1_MouseDown(object sender, MouseEventArgs e)
        {
            _player.controls.currentPosition = trackBar1.Value;
        }

      

        private void _timer_Tick(object sender, EventArgs e)
        {
            double t = Math.Floor(_player.currentMedia.duration - _player.controls.currentPosition);
            this.InfoLabel.Text = $"t=>{t.ToString()} ,duration=>{ (int)_player.currentMedia.duration} ,currentPosition=>{(int)(_player.controls.currentPosition)}";

            this.trackBar1.Maximum = this.progressBar1.Maximum = (int)_player.currentMedia.duration;
            this.trackBar1.Value = this.progressBar1.Value = (int)(_player.controls.currentPosition);

           
        }

        private void Player_PlayStateChange(int NewState)
        {
            var state = (WMPLib.WMPPlayState)NewState;
            //state.Dump();

            switch (state)
            {
                case WMPPlayState.wmppsUndefined:
                    break;
                case WMPPlayState.wmppsStopped:
                    this.progressBar1.Value = (int)(_player.controls.currentPosition);
                    _player.close();
                    _player.currentPlaylist = _player.playlistCollection.newPlaylist("myplaylist");
                    this.PlayBtn.Text = "播放";
                    _timer.Stop();
                    break;
                case WMPPlayState.wmppsPaused:
                    break;
                case WMPPlayState.wmppsPlaying:
                    break;
                case WMPPlayState.wmppsScanForward:
                    break;
                case WMPPlayState.wmppsScanReverse:
                    break;
                case WMPPlayState.wmppsBuffering:
                    break;
                case WMPPlayState.wmppsWaiting:
                    break;
                case WMPPlayState.wmppsMediaEnded:
                    break;
                case WMPPlayState.wmppsTransitioning:
                    break;
                case WMPPlayState.wmppsReady:
                    break;
                case WMPPlayState.wmppsReconnecting:
                    break;
                case WMPPlayState.wmppsLast:
                    break;
                default:
                    break;
            }
        }
        private void Player_MediaError(object pMediaObject)
        {
            Console.WriteLine("Error");
            _player.close();
        }

        private void PlayOrPause_Click(object sender, EventArgs e)
        {
            if (this.PlayBtn.Text == "停止")
            {
                Player_PlayStateChange((int)WMPPlayState.wmppsStopped);
                return;
            }

            PlayMedia();
        }

        private void PlayMedia(string pointUrl = null)
        {

            this.PlayBtn.Text = "停止";


            WMPLib.IWMPPlaylist playlist = _player.playlistCollection.newPlaylist("myplaylist");

            _player.PlayStateChange +=
             new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(Player_PlayStateChange);
            _player.MediaError +=
             new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
            _player.CurrentItemChange += new WMPLib._WMPOCXEvents_CurrentItemChangeEventHandler(player_CurrentItemChange);

            playlist.clear();

            if (string.IsNullOrWhiteSpace(pointUrl) == false)
            {
                media = _player.newMedia(pointUrl);
                playlist.appendItem(media);
            }

            var array = Shuffle<string>(_listUrl.ToArray());
            foreach (string urlStr in array)
            {
                media = _player.newMedia(urlStr);
                playlist.appendItem(media);
            }
            _player.currentPlaylist = playlist;

            _timer.Start();
            _player.controls.play();


           
        }

        private void _player_CurrentItemChange(object pdispMedia)
        {
        }

        private void player_CurrentItemChange(object sender)
        {
            this.CurrentPlayNameLabel.Text = _player.currentMedia.name;

            var index = listBox1.Items.IndexOf(_player.currentMedia.sourceURL);
            this.listBox1.SetSelected(index, true);
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if(_player != null)
            {
                _player.close();
                _timer.Stop();
            }

            PlayMedia(this.listBox1.SelectedItem.ToString());
        }

        public T[] Shuffle<T>(T[] Source)
        {
            if (Source == null) return null;
            int len = Source.Length;//用變數記會快一點點點
            Random rd = new Random();
            int r;//記下隨機產生的號碼
            T tmp;//暫存用
            for (int i = 0; i < len - 1; i++)
            {
                r = rd.Next(i, len);//取亂數，範圍從自己到最後，決定要和哪個位置交換，因此也不用跑最後一圈了
                if (i == r) continue;
                tmp = Source[i];
                Source[i] = Source[r];
                Source[r] = tmp;
            }
            return Source;
        }

        private void RandomPlayBtn_Click(object sender, EventArgs e)
        {
            PlayMedia();
        }
    }
}
