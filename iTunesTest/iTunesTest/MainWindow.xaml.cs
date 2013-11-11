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
using System.Net;
using Newtonsoft.Json;

namespace iTunesTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String url = "https://itunes.apple.com/search?term=karma%20police&attribute=songTerm&entity=album";
            var json = new WebClient().DownloadString(url);

            iTunesResult deserialized = JsonConvert.DeserializeObject<iTunesResult>(json);
            
            //Filter tracks by artist
            List<iTunesTrack> resultsForArtist = getResultsForArtist(deserialized, @"Radiohead");

            //Get the oldest track (filters out all the "best of" albums, etc.)
            iTunesTrack oldestTrack = getOldestTitle(resultsForArtist);

            String imageUrl = oldestTrack.artworkUrl100;

            setImageToUrl(imageUrl);

        }

        private List<iTunesTrack> getResultsForArtist(iTunesResult data, String artist)
        {
            List<iTunesTrack> returnList = new List<iTunesTrack>();

            foreach (iTunesTrack result in data.results)
            {
                if(result.artistName.Equals(artist))
                {
                    returnList.Add(result);
                }
            }

            return returnList;
        }

        private iTunesTrack getOldestTitle(List<iTunesTrack> resultList)
        {
            iTunesTrack oldestTrack = null;
            DateTime oldestTrackDate = DateTime.Now;

            foreach(iTunesTrack result in resultList)
            {
                DateTime trackDate = Convert.ToDateTime(result.releaseDate);

                if(DateTime.Compare(trackDate, oldestTrackDate) < 0)
                {
                    oldestTrackDate = trackDate;
                    oldestTrack = result;
                }
            }

            return oldestTrack;

        }

        private void setImageToUrl(String url)
        {
            BitmapImage urlImage = new BitmapImage();

            urlImage.BeginInit();
            urlImage.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
            urlImage.EndInit();

            CoverArtImage.Source = urlImage;
        }

    }

    public class iTunesResult
    {
        public string resultCount { get; set; }
        public iTunesTrack[] results {get; set; }
    }

    public class iTunesTrack
    {
        public String artworkUrl60 { get; set; }
        public String artworkUrl100 { get; set; }
        public String artistName { get; set; }
        public String releaseDate { get; set; }
    }


}
