using Newtonsoft.Json;
using ParkAssistGent.Classes;
using ParkAssistGent.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace ParkAssistGent
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShowParkings : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        List<PushPin> pushpins = new List<PushPin>();

        public ShowParkings()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

        
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            Geolocator geolocator = new Geolocator();

            ShowMyLocationOnTheMap();

            Geoposition position = await geolocator.GetGeopositionAsync();
            BasicGeoposition basicgeo = new BasicGeoposition();
            basicgeo.Latitude = position.Coordinate.Point.Position.Latitude;
            basicgeo.Longitude = position.Coordinate.Point.Position.Longitude;
            mapWithParkings.ZoomLevel = 15;

            Geopoint positionpoint = new Geopoint(basicgeo);
            mapWithParkings.Center = positionpoint;
            getPlaces(positionpoint.Position.Latitude, positionpoint.Position.Longitude);    
        }
        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);

        }

        

        

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        public async void getPlaces(double lat, double lon)
        {
            
                string httpheader = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + lat + ","
                + lon + "&radius=10000&types=parking&key=AIzaSyDGGzD_RjFpH8HyUMjYq29j6wj4o0tSw9c";
                BasicGeoposition positiontest = new BasicGeoposition();     //test for json results

                var client = new HttpClient();
                var result = await client.GetStringAsync(httpheader);
                PlacesResponse placeresponse = (PlacesResponse)JsonConvert.DeserializeObject<PlacesResponse>(result);
                int count = placeresponse.results.Length;

                if (placeresponse.status == "OK")
                {
                    for (int i = 0; i < count; i++)
                    {
                        PushPin pushpin1 = new PushPin();
                        string name = placeresponse.results[i].name;
                        positiontest.Latitude = placeresponse.results[i].geometry.location.lat;
                        positiontest.Longitude = placeresponse.results[i].geometry.location.lng;
                        Geopoint geopoint1 = new Geopoint(positiontest);
                        AddPushPinOnMap(pushpin1, placeresponse, name, count, i, geopoint1);
                    }
                }
                                
                    Pushpins.ItemsSource = pushpins;
                
            }

        public void AddPushPinOnMap(PushPin pushpin, PlacesResponse response, string name, int max, int count, Geopoint geopoint)
        {
            MapIcon icon = new MapIcon();
            icon.Location = geopoint;
            icon.Title = name;
            mapWithParkings.MapElements.Add(icon);
        }

        
        private async void ShowMyLocationOnTheMap()
        {
            Geoposition geoposition = await getLocation();


            mapWithParkings.Center = geoposition.Coordinate.Point;
            MapIcon mapIcon = new MapIcon();
            mapIcon.NormalizedAnchorPoint = new Point(0.25, 0.9);
            mapIcon.Location = geoposition.Coordinate.Point;
            mapIcon.Title = "You are here";
            mapWithParkings.MapElements.Add(mapIcon);
           
            
        }


        
        private async Task<Geoposition> getLocation()
        {
            Geolocator geolocator = new Geolocator();
            Geoposition geoposition = null;
            try
            {
                geoposition = await geolocator.GetGeopositionAsync();
            }
            catch (Exception ex)
            {
                // Handle errors like unauthorized access to location
                // services or no Internet access.
            }
            return geoposition;
        }


        private void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (mapWithParkings.ZoomLevel < 20)
            {
                if (mapWithParkings.ZoomLevel > 19)
                    mapWithParkings.ZoomLevel = 20;
                else
                    mapWithParkings.ZoomLevel++;
            }
        }

        private void btnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (mapWithParkings.ZoomLevel > 1)
            {
                if (mapWithParkings.ZoomLevel < 2)
                    mapWithParkings.ZoomLevel = 1;
                else
                    mapWithParkings.ZoomLevel--;
            }
        }

        #endregion
    }
}
