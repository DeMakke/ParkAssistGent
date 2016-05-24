using ParkAssistGent.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
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
    public sealed partial class SaveParkingspace : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private Geoposition parkingspace;

        public SaveParkingspace()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            this.NavigationCacheMode = NavigationCacheMode.Required;
            ShowMyLocationOnTheMap();
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        private async void ShowMyLocationOnTheMap()
        {
            Geoposition geoposition = await getLocation();


            mapWithMyLocation.Center = geoposition.Coordinate.Point;
            MapIcon mapIcon = new MapIcon();
            mapIcon.NormalizedAnchorPoint = new Point(0.25, 0.9);
            mapIcon.Location = geoposition.Coordinate.Point;
            mapIcon.Title = "You are here";
            mapWithMyLocation.MapElements.Add(mapIcon);
            mapWithMyLocation.ZoomLevel = 14;

            

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
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            //if (e.PageState != null && e.PageState.ContainsKey("Park"))
            //{
            //    parkingspace = (Geoposition)e.PageState["Park"];

            //}
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
            //if (e.PageState != null && e.PageState.ContainsKey("Park"))
            //{
            //    e.PageState.Remove("Park");
            //}
            //e.PageState.Add("Park", parkingspace);
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

        #endregion

        private async void btnSaveParkingspace_Click(object sender, RoutedEventArgs e)
        {
            mapWithMyLocation.MapElements.Clear();
            Geoposition geoposition = await getLocation();

            mapWithMyLocation.Center = geoposition.Coordinate.Point;
            MapIcon mapIconPark = new MapIcon();
            mapIconPark.NormalizedAnchorPoint = new Point(0.25, 0.9);
            mapIconPark.Location = geoposition.Coordinate.Point;
            mapIconPark.Title = "Your saved parkingspace";
            mapWithMyLocation.MapElements.Add(mapIconPark);
            mapWithMyLocation.ZoomLevel = 16;
            parkingspace = geoposition;

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



        private async void btnFindParkingspace_Click(object sender, RoutedEventArgs e)
        {
            MapService.ServiceToken = "PP-GfLf36HgHjM8ZVPl2GA";
            mapWithMyLocation.MapElements.Clear();
            Geoposition startPosition = await getLocation();

            if (parkingspace != null)
            {
                Geoposition endPosition = parkingspace;
                MapIcon mapIconPark2 = new MapIcon();
                mapIconPark2.NormalizedAnchorPoint = new Point(0.25, 0.9);
                mapIconPark2.Location = endPosition.Coordinate.Point;
                mapIconPark2.Title = "Parkingspace";
                mapWithMyLocation.MapElements.Add(mapIconPark2);
            }
            else
            {
                var messageDialog = new MessageDialog("No parkingspace saved.");
                await messageDialog.ShowAsync();
            }

            BasicGeoposition startLocation = new BasicGeoposition();
            startLocation.Latitude = startPosition.Coordinate.Point.Position.Latitude;
            startLocation.Longitude = startPosition.Coordinate.Point.Position.Longitude;
            Geopoint startPoint = new Geopoint(startLocation);

            BasicGeoposition endLocation = new BasicGeoposition();
            endLocation.Latitude = parkingspace.Coordinate.Point.Position.Latitude;
            endLocation.Longitude = parkingspace.Coordinate.Point.Position.Longitude;
            Geopoint endPoint = new Geopoint(endLocation);

            mapWithMyLocation.Center = startPosition.Coordinate.Point;

            MapIcon mapIconStart = new MapIcon();
            mapIconStart.NormalizedAnchorPoint = new Point(0.25, 0.9);
            mapIconStart.Location = startPosition.Coordinate.Point;
            mapIconStart.Title = "You are here";
            mapWithMyLocation.MapElements.Add(mapIconStart);
            mapWithMyLocation.ZoomLevel = 12;
  
            MapRouteFinderResult routeResult = await MapRouteFinder.GetWalkingRouteAsync(startPoint, endPoint);

            if (routeResult.Status == MapRouteFinderStatus.Success)
            {
                // Use the route to initialize a MapRouteView.
                MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                viewOfRoute.RouteColor = Colors.Blue;
                viewOfRoute.OutlineColor = Colors.Blue;
                // Add the new MapRouteView to the Routes collection
                // of the MapControl.
                mapWithMyLocation.Routes.Add(viewOfRoute);
                // Fit the MapControl to the route.
                await mapWithMyLocation.TrySetViewBoundsAsync(
                  routeResult.Route.BoundingBox,
                  null,
                  Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Bow);
            }
   
        }


        private void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (mapWithMyLocation.ZoomLevel < 20)
            {
                if (mapWithMyLocation.ZoomLevel > 19)
                    mapWithMyLocation.ZoomLevel = 20;
                else
                    mapWithMyLocation.ZoomLevel++;
            }
        }

        private void btnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (mapWithMyLocation.ZoomLevel > 1)
            {
                if (mapWithMyLocation.ZoomLevel < 2)
                    mapWithMyLocation.ZoomLevel = 1;
                else
                    mapWithMyLocation.ZoomLevel--;
            }
        }
    }
}
